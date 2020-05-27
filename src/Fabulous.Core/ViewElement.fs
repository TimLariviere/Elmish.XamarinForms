// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

open System
open System.Collections.Generic

type ProgramDefinition =
    { CanReuseView: IViewElement -> IViewElement -> bool
      Dispatch: obj -> unit }

and IViewElement =
    abstract Create: ProgramDefinition -> obj
    abstract Update: ProgramDefinition * IViewElement voption * obj -> unit
    abstract TryKey: string voption with get

[<ReferenceEquality>] type DynamicEvent = { Subscribe: obj * obj -> unit; Unsubscribe: obj * obj -> unit }
[<ReferenceEquality>] type DynamicProperty = { Update: ProgramDefinition * obj voption * obj voption * obj -> unit }

type DynamicEventValue(fn: (obj -> unit) -> obj) =
    let mutable handlerOpt = ValueNone
    member x.GetHandler(dispatch) =
        match handlerOpt with
        | ValueNone ->
            let handler = (fn dispatch)
            handlerOpt <- ValueSome handler
            handler
        | ValueSome handler ->
            handler

type DynamicViewElement
    (
        targetType: Type,
        create: unit -> obj,
        events: KeyValuePair<DynamicEvent, DynamicEventValue> list,
        properties: KeyValuePair<DynamicProperty, obj> list
    ) =
    
    member x.TargetType = targetType
    member x.Events = events
    member x.Properties = properties
    
    member x.TryGetPropertyValue<'T>(propDefinition: DynamicProperty) =
        match properties |> List.tryFind (fun kvp -> kvp.Key = propDefinition) with
        | None -> ValueNone
        | Some kvp -> ValueSome (kvp.Value :?> 'T)
        
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
                evt.Key.Unsubscribe(evt.Value.GetHandler(programDefinition.Dispatch), target)
                
        // Update properties
        let allProps = List.distinct [
            match prevOpt with
            | ValueNone -> ()
            | ValueSome prev ->
                for prop in prev.Properties do
                    yield prop.Key
                    
            for prop in x.Properties do
                yield prop.Key
        ]
        for prop in allProps do
            let prevPropOpt = match prevOpt with ValueNone -> ValueNone | ValueSome prev -> prev.TryGetPropertyValue(prop)
            let currPropOpt = x.TryGetPropertyValue(prop)
            prop.Update(programDefinition, prevPropOpt, currPropOpt, target)
            
        // Subscribe events
        for evt in x.Events do
            evt.Key.Subscribe(evt.Value.GetHandler(programDefinition.Dispatch), target)
        
    interface IViewElement with
        member x.Create(dispatch) = x.Create(dispatch)
        member x.Update(programDefinition, prevOpt, target) = x.Update(programDefinition, (prevOpt |> ValueOption.map(fun p -> p :?> DynamicViewElement)), target)
        member x.TryKey with get() = ValueNone
        