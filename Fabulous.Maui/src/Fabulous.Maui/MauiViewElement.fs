namespace Fabulous.Maui

open System.Collections.ObjectModel
open Fabulous

type MauiViewElement(targetType: System.Type, create: unit -> obj, properties: ReadOnlyDictionary<string, obj>) =
    member x.TryGetPropertyValue<'T>(propertyName, defaultValue: 'T) =
        match properties.TryGetValue(propertyName) with
        | false, _ -> defaultValue
        | true, value -> value :?> 'T
    
    interface IViewElement with
        member x.Create(dispatch) = failwithf "Not implemented"
        member x.Update(programDefinition, prevOpt, target) = failwithf "Not implemented"
        member x.TryKey with get() = failwithf "Not implemented"