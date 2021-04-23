namespace Fabulous.Maui

open System.Collections.Generic
open Microsoft.Maui
open Fabulous

type State() =
    let state = Dictionary<string, obj>()
    
    abstract ApplyChange: ProgramDefinition * DiffChange -> unit
    default this.ApplyChange(programDefinition, change: DiffChange) =
        match change.Value with
        | Clear -> state.Remove(change.Key) |> ignore
        | Create value ->
            let v =
                match value with
                | :? IViewElement as ve -> ve.Create(programDefinition, ValueNone)
                | _ -> value
            state.Add(change.Key, v)
        | Update newValue -> state.[change.Key] <- newValue
    
    member this.ApplyChanges(programDefinition, changes: DiffChange[]) =
        for change in changes do this.ApplyChange(programDefinition, change)
    
    member this.GetValue<'T>(key: string, defaultValue: 'T) : 'T =
        match state.TryGetValue(key) with
        | false, _ -> defaultValue
        | true, value -> value :?> 'T
    
    member this.GetValueFn<'T>(key: string, defaultValueFn: unit -> 'T) : 'T =
        match state.TryGetValue(key) with
        | false, _ -> defaultValueFn ()
        | true, value -> value :?> 'T

type IElementWithHandler =
    abstract Handler: IViewHandler with get
    
type StateWithHandler(element: IElementWithHandler) =
    inherit State()
    
    override this.ApplyChange(programDefinition, change) =
        base.ApplyChange(programDefinition, change)
        element.Handler.UpdateValue(change.Key)