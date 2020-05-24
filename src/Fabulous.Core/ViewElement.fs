// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

open System
open System.Collections.Generic
open System.Linq

type IViewElement =
    abstract Create: unit -> obj
    abstract Update: IViewElement voption * obj -> unit
    
module DynamicViews =
    [<ReferenceEquality>] type DynamicEvent = { Subscribe: obj * obj -> unit; Unsubscribe: obj * obj -> unit }
    [<ReferenceEquality>] type DynamicProperty = { Set: obj * obj -> unit; Unset: obj -> unit }
    
    type Attribute =
        | EventNode of DynamicEvent * obj
        | PropertyNode of DynamicProperty * obj
    
    type DynamicViewElement
        (
            targetType: Type,
            create: unit -> obj,
            attributes: Attribute list
        ) =
        
        member x.TargetType = targetType
        member x.Attributes = attributes
        
        member internal x.BuildAttributes() =
            let events = Dictionary<DynamicEvent, obj>()
            let properties = Dictionary<DynamicProperty, obj>()
            attributes |> List.iter (fun attr ->
                match attr with
                | EventNode (evt, value) -> events.Add(evt, value)
                | PropertyNode (prop, value) -> properties.Add(prop, value)
            )
            events, properties
        
        interface IViewElement with
            member x.Create() = create()
                
            member x.Update(prevOpt, target) =
                let prevEvents, prevProperties =
                    match prevOpt with
                    | ValueNone -> Dictionary(), Dictionary()
                    | ValueSome prev -> (prev :?> DynamicViewElement).BuildAttributes()
                let currEvents, currProperties = x.BuildAttributes()
                
                // Unsubscribe events
                for evt in prevEvents do
                    evt.Key.Unsubscribe(evt.Value, target)
                        
                // Update properties
                let allProps = currProperties.Keys.Union(prevProperties.Keys).Distinct().ToList()
                for prop in allProps do
                    match prevProperties.TryGetValue(prop), currProperties.TryGetValue(prop) with
                    | (false, _), (false, _) -> ()
                    | (true, prevValue), (true, currValue) when prevValue = currValue -> ()
                    | (true, _), (false, _) -> prop.Unset(target)
                    | _, (true, currValue) -> prop.Set(currValue, target)
                    
                // Subscribe events
                for evt in currEvents do
                    evt.Key.Subscribe(evt.Value, target)
        
module StaticViews =
    type StaticViewElement<'T>
        (
            create: unit -> 'T,
            setState: obj voption * obj * 'T -> unit,
            state: obj
        ) =
        
        member x.State = state
        
        interface IViewElement with
            member x.Create() = create() |> box
                
            member x.Update(prevOpt, target) =
                let prevStateOpt = prevOpt |> ValueOption.map (fun p -> (p :?> StaticViewElement<'T>).State)
                setState(prevStateOpt, state, target :?> 'T)