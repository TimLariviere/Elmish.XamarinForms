namespace Fabulous.XamarinForms.DynamicViews

open System.Collections.Generic
open Fabulous.XamarinForms.Helpers

module DictionaryHelpers =
    let updateEntries
            (prevCollOpt: ('TKey * 'TData)[] voption)
            (collOpt: ('TKey * 'TData)[] voption)
            (target: IDictionary<'TKey, 'TValue>)
            (create: 'TData -> 'TValue)
            (update: 'TData -> 'TData -> 'TValue -> 'TValue) =
        
        match prevCollOpt, collOpt with
        | ValueNone, ValueNone -> ()
        | ValueSome prevColl, ValueSome newColl when identical prevColl newColl -> ()
        | ValueSome prevColl, ValueSome newColl when prevColl <> null && newColl <> null && prevColl.Length = 0 && newColl.Length = 0 -> ()
        | ValueSome _, ValueNone -> target.Clear()
        | ValueSome _, ValueSome coll when (coll = null || coll.Length = 0) -> target.Clear()
        | _, ValueSome coll ->
            for (key, value) in coll do
                let prevOpt = match prevCollOpt with ValueNone -> None | ValueSome x -> Array.tryFind (fun (k, _) -> k = key) x
                match prevOpt with
                | None -> target.[key] <- create value
                | Some (_, prevValue) -> target.[key] <- update prevValue value target.[key]
                
            for kvp in target do
                if not (coll |> Array.exists (fun (k, v) -> k = kvp.Key)) then
                    target.Remove(kvp.Key) |> ignore

