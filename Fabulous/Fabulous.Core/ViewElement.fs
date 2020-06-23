// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

open System
open System.Collections.Generic
open System.Diagnostics
open System.Runtime.CompilerServices

type ProgramDefinition =
    { CanReuseView: IViewElement -> IViewElement -> bool
      Dispatch: obj -> unit }

and IViewElement =
    abstract Create: ProgramDefinition -> obj
    abstract Update: ProgramDefinition * IViewElement voption * obj -> unit
    abstract TryKey: string voption with get

[<ReferenceEquality; DebuggerDisplay("{DebugName,nq}")>] type DynamicEvent = { DebugName: string; Subscribe: obj * obj -> unit; Unsubscribe: obj * obj -> unit }
[<ReferenceEquality; DebuggerDisplay("{DebugName,nq}")>] type DynamicProperty = { DebugName: string; Update: ProgramDefinition * obj voption * obj voption * obj -> unit }
type DynamicEventFunc = (obj -> unit) -> obj

/// To avoid capturing Dispatch when building ViewElements,
/// Dispatch is only passed when applying the changes to the actual control.
/// But for event subscription, it is required to keep a reference of the EventHandler
/// so we can unsubscribe it later.
///
/// To avoid memory leak, we only keep a weak reference to the actual control.
module internal EventHandlerCaching =
    let private _cache = ConditionalWeakTable<obj, Dictionary<DynamicEvent, obj>>()
    
    let tryGet (target: obj) (evt: DynamicEvent) =
        match _cache.TryGetValue(target) with
        | false, _ -> None
        | true, events ->
            match events.TryGetValue(evt) with
            | false, _ -> None
            | true, eventHandler -> Some eventHandler
        
    let add (target: obj) (evt: DynamicEvent) (value: obj) =
        let events = _cache.GetOrCreateValue(target)
        events.[evt] <- value

type DynamicViewElement
    (
        targetType: Type,
        create: unit -> obj,
        events: IReadOnlyDictionary<DynamicEvent, DynamicEventFunc>,
        properties: IReadOnlyDictionary<DynamicProperty, obj>
    ) =
    
    member x.TargetType = targetType
    member x.Events = events
    member x.Properties = properties
    
    member x.TryGetPropertyValue<'T>(propDefinition: DynamicProperty) =
        match x.Properties.TryGetValue(propDefinition) with
        | false, _ -> ValueNone
        | true, value -> ValueSome (value :?> 'T)
        
    member x.TryGetEventFunction(evtDefinition: DynamicEvent) =
        match x.Events.TryGetValue(evtDefinition) with
        | false, _ -> ValueNone
        | true, value -> ValueSome value
        
    member x.Create(dispatch) =
        let target = create()
        x.Update(dispatch, ValueNone, target)
        target
    
    member x.Update(programDefinition: ProgramDefinition, prevOpt: DynamicViewElement voption, target: obj) =
        // Unsubscribe events
        match prevOpt with
        | ValueNone -> ()
        | ValueSome prev ->
            for evt in prev.Events do
                match EventHandlerCaching.tryGet target evt.Key with
                | None -> ()
                | Some evtHandler -> evt.Key.Unsubscribe(evtHandler, target)
                
        // Update properties
        let allProps = Seq.distinct (seq {
            match prevOpt with
            | ValueNone -> ()
            | ValueSome prev ->
                for prop in prev.Properties do
                    yield prop.Key
                    
            for prop in x.Properties do
                yield prop.Key
        })
        for prop in allProps do
            let prevPropOpt = match prevOpt with ValueNone -> ValueNone | ValueSome prev -> prev.TryGetPropertyValue(prop)
            let currPropOpt = x.TryGetPropertyValue(prop)
            prop.Update(programDefinition, prevPropOpt, currPropOpt, target)
            
        // Subscribe events
        for evt in x.Events do
            let evtHandler = evt.Value programDefinition.Dispatch
            EventHandlerCaching.add target evt.Key evtHandler
            evt.Key.Subscribe(evtHandler, target)
        
    interface IViewElement with
        member x.Create(dispatch) = x.Create(dispatch)
        member x.Update(programDefinition, prevOpt, target) = x.Update(programDefinition, (prevOpt |> ValueOption.map(fun p -> p :?> DynamicViewElement)), target)
        member x.TryKey with get() = ValueNone
        