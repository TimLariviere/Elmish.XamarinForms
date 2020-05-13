// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

open System
open System.Collections.Generic

[<AbstractClass>]
type ViewElement(targetType: Type, create: unit -> obj) =
    member x.TargetType = targetType
    abstract Create: unit -> obj
    abstract Update: ViewElement voption * obj -> unit
    default x.Create() =
        let target = create()
        x.Update(ValueNone, target)
        target
    
module DynamicViews =
    type Attribute =
        | Property of set: (obj * obj -> unit) * unset: (obj -> unit)
        | Event of subscribe: (obj * obj -> unit) * unsubscribe: (obj * obj -> unit)
        
    type AttributeSet =
        { Attribute: Attribute
          Value: obj }
        
    type AttributesList =
        | Empty
        | Value of head: KeyValuePair<string, AttributeSet> * tail: AttributesList
        
    let toDictionaries (attributes: AttributesList) =
        let evtDict = Dictionary<string, (obj * obj -> unit) * (obj * obj -> unit) * obj>()
        let propDict = Dictionary<string, (obj * obj -> unit) * (obj -> unit) * obj>()
        let rec iterateRec attr =
            match attr with
            | Empty -> ()
            | Value (head, tail) ->
                match head.Value.Attribute with
                | Property (set,unset) -> propDict.[head.Key] <- (set, unset, head.Value.Value)
                | Event (sub, unsub) -> evtDict.[head.Key] <- (sub, unsub, head.Value.Value)
                iterateRec tail
        iterateRec attributes
        evtDict, propDict
        
    type DynamicViewElement<'T>(create: unit -> 'T, attributes: AttributesList) =
        inherit ViewElement(typeof<'T>, (create >> box))
        member x.Attributes = attributes
        override x.Update(prevOpt: ViewElement voption, target: obj) =
            let prevEvts, prevProps =
                match prevOpt with
                | ValueNone -> Dictionary(), Dictionary()
                | ValueSome prev -> toDictionaries (prev :?> DynamicViewElement<'T>).Attributes
                
            let currEvts, currProps = toDictionaries x.Attributes
            
            let allEvtKeys = Seq.append prevEvts.Keys currEvts.Keys |> Seq.distinct |> List.ofSeq
            let allPropsKeys = Seq.append prevEvts.Keys currEvts.Keys |> Seq.distinct |> List.ofSeq
                
            for key in allEvtKeys do
                match prevEvts.TryGetValue(key) with
                | true, (_, unsub, prev) -> unsub (prev, target)
                | _ -> ()
            
            for key in allPropsKeys do
                match prevProps.TryGetValue(key), currProps.TryGetValue(key) with
                | (false, _), (false, _) -> ()
                | (true, (_, _, prev)), (true, (_, _, curr)) when System.Object.ReferenceEquals(prev, curr) -> ()
                | (true, (_, unset, _)), (false, _) -> unset target
                | _, (true, (set, _, curr)) -> set (curr, target)
                
            for key in allEvtKeys do
                match currEvts.TryGetValue(key) with
                | true, (sub, _, curr) -> sub (curr, target)
                | _ -> ()
                    
                
        
module StaticViews =
    type StaticViewElement<'T>(create: unit -> 'T, setState: (obj * obj -> unit), unsetState: (obj -> unit), state) =
        inherit ViewElement(typeof<'T>, (create >> box))
        member x.State = state
        override x.Update(prevOpt: ViewElement voption, target: obj) = ()