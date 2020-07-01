namespace Fabulous.XamarinForms.DynamicViews

open System.Collections
open System.Collections.Generic
open System.Collections.ObjectModel
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.Helpers

/// This module contains the update logic for the controls with children
module ChildrenUpdaters =    
    /// Incremental list maintenance: given a collection, and a previous version of that collection, perform
    /// a reduced number of clear/move/create/update/remove operations
    ///
    /// The algorithm will try in priority to update elements sharing the same instance (usually achieved with dependsOn)
    /// or sharing the same key. All other elements will try to reuse previous elements where possible.
    /// If no reuse is possible, the element will create a new control.
    let updateChildrenInternal
            (prevCollOpt: 'T[] voption)
            (collOpt: 'T[] voption)
            (keyOf: 'T -> string voption)
            (canReuse: 'T -> 'T -> bool)
            (clear: unit -> unit)
            (create: int -> 'T -> unit)
            (update: int -> 'T -> 'T -> unit)
            (move: int -> int -> unit)
            (remove: int -> unit)
        =
        
        match prevCollOpt, collOpt with
        | ValueNone, ValueNone -> ()
        | ValueSome prevColl, ValueSome newColl when identical prevColl newColl -> ()
        | ValueSome prevColl, ValueSome newColl when prevColl <> null && newColl <> null && prevColl.Length = 0 && newColl.Length = 0 -> ()
        | ValueSome _, ValueNone -> clear ()
        | ValueSome _, ValueSome coll when (coll = null || coll.Length = 0) -> clear ()
        | _, ValueSome coll ->
            let currentState = match prevCollOpt with ValueSome x -> List(x) | _ -> List()
                
            let create newIndex newChild =
                currentState.Insert(newIndex, newChild)
                create newIndex newChild
                
            let move prevIndex newIndex =
                let child = currentState.[prevIndex]
                currentState.RemoveAt(prevIndex)
                currentState.Insert(newIndex, child)
                move prevIndex newIndex
                
            let remove index =
                currentState.RemoveAt(index)
                remove index
            
            // Separate the previous elements into 3 lists
            // The ones whose instances have been reused (dependsOn)
            // The ones whose keys have been reused
            // The rest which can be reused by any other element
            let identicalElements = HashSet<'T>()
            let keyedElements = Dictionary<string, 'T>()
            let reusableElements = ResizeArray<'T>()
            if prevCollOpt.IsSome then
                for prevChild in prevCollOpt.Value do
                    if coll |> Array.exists (identical prevChild) then
                        identicalElements.Add(prevChild) |> ignore
                    else
                        let canReuseChildOf key =
                            coll
                            |> Array.exists (fun newChild ->
                                keyOf newChild = ValueSome key
                                && canReuse prevChild newChild
                            )
                        
                        match keyOf prevChild with
                        | ValueSome key when canReuseChildOf key ->
                            keyedElements.Add(key, prevChild)
                        | _ ->
                            reusableElements.Add(prevChild)
            
            // Reuse the first element from reusableElements that returns true to canReuse
            // Otherwise create a new element
            let reuseOrCreate newIndex newChild =
                match reusableElements |> Seq.tryFind(fun c -> canReuse c newChild) with
                | Some prevChild ->
                    reusableElements.Remove prevChild |> ignore
                    
                    let prevIndex = currentState.IndexOf(prevChild)
                    update prevIndex prevChild newChild
                            
                    if prevIndex <> newIndex then
                        move prevIndex newIndex
                        
                | None ->
                    create newIndex newChild
            
            for i in 0 .. coll.Length - 1 do
                let newChild = coll.[i]
                
                // Check if the same instance was reused (dependsOn), if so just move the element to the correct index
                if identicalElements.Contains(newChild) then
                    let prevIndex = currentState.IndexOf(newChild)
                    if prevIndex <> i then
                        move prevIndex i
                else
                    // If the key existed previously, reuse the previous element
                    match keyOf newChild with
                    | ValueSome key when keyedElements.ContainsKey(key) -> 
                        let prevChild = keyedElements.[key]
                        let prevIndex = currentState.IndexOf(prevChild)
                        update prevIndex prevChild newChild
                        
                        if prevIndex <> i then
                            move prevIndex i
                    
                    // Otherwise, reuse an old element if possible or create a new one
                    | _ ->
                        reuseOrCreate i newChild
            
            // If we still have old elements that were not reused, delete them
            if reusableElements.Count > 0 then
                for remainingElement in reusableElements do
                    let prevIndex = currentState.IndexOf(remainingElement)
                    remove prevIndex

    /// Incremental list maintenance: given a collection, and a previous version of that collection, perform
    /// a reduced number of clear/add/remove/insert operations
    let updateChildren
           (prevCollOpt: IViewElement[] voption) 
           (collOpt: IViewElement[] voption) 
           (targetColl: IList<'TargetT>)
           (canReuseView: IViewElement -> IViewElement -> bool)
           (create: IViewElement -> 'TargetT)
           (update: IViewElement -> IViewElement -> 'TargetT -> unit) // Incremental element-wise update, only if element reuse is allowed
        =
        
        let create index child =
            let targetChild = create child
            targetColl.Insert(index, targetChild)
            
        let update index prevChild newChild =
            let targetChild = targetColl.[index]
            update prevChild newChild targetChild
            
        let move prevIndex newIndex =
            let targetChild = targetColl.[prevIndex]
            targetColl.RemoveAt(prevIndex)
            targetColl.Insert(newIndex, targetChild)
        
        updateChildrenInternal
            prevCollOpt collOpt (fun v -> v.TryKey) canReuseView
            (fun () -> targetColl.Clear())
            create update move
            (fun index -> targetColl.RemoveAt(index))

module DictionaryUpdaters =
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
                if not (coll |> Array.exists (fun (k, _) -> k = kvp.Key)) then
                    target.Remove(kvp.Key) |> ignore

/// This module contains the update logic for the controls with a virtualized ItemsSource
module ItemsUpdaters =    
    /// Incremental list maintenance: given a collection, and a previous version of that collection, perform
    /// a reduced number of clear/move/create/update/remove operations
    ///
    /// The algorithm will try in priority to update elements sharing the same instance (usually achieved with dependsOn)
    /// or sharing the same key. All other elements will be reused only if they are still at the same index.
    /// If no reuse is possible, the element will create a new control.
    let updateItemsInternal
            (prevCollOpt: 'T[] voption)
            (collOpt: 'T[] voption)
            (keyOf: 'T -> string voption)
            (canReuse: 'T -> 'T -> bool)
            (clear: unit -> unit)
            (create: int -> 'T -> unit)
            (update: int -> 'T -> 'T -> unit)
            (move: int -> int -> unit)
            (remove: int -> unit)
        =
        
        match prevCollOpt, collOpt with
        | ValueNone, ValueNone -> ()
        | ValueSome prevColl, ValueSome newColl when identical prevColl newColl -> ()
        | ValueSome prevColl, ValueSome newColl when prevColl <> null && newColl <> null && prevColl.Length = 0 && newColl.Length = 0 -> ()
        | ValueSome _, ValueNone -> clear ()
        | ValueSome _, ValueSome coll when (coll = null || coll.Length = 0) -> clear ()
        | _, ValueSome coll ->
            let currentState = match prevCollOpt with ValueSome x -> List(x) | _ -> List()
                
            let create newIndex newChild =
                currentState.Insert(newIndex, newChild)
                create newIndex newChild
                
            let move prevIndex newIndex =
                let child = currentState.[prevIndex]
                currentState.RemoveAt(prevIndex)
                currentState.Insert(newIndex, child)
                move prevIndex newIndex
                
            let remove index =
                currentState.RemoveAt(index)
                remove index
            
            // Separate the previous elements into 2 lists
            // The ones whose instances have been reused (dependsOn)
            // The ones whose keys have been reused
            let identicalElements = HashSet<'T>()
            let keyedElements = Dictionary<string, 'T>()
            if prevCollOpt.IsSome then
                for prevChild in prevCollOpt.Value do
                    if coll |> Array.exists (identical prevChild) then
                        identicalElements.Add(prevChild) |> ignore
                    else
                        let canReuseChildOf key =
                            coll
                            |> Array.exists (fun newChild ->
                                keyOf newChild = ValueSome key
                                && canReuse prevChild newChild
                            )
                        
                        match keyOf prevChild with
                        | ValueSome key when canReuseChildOf key ->
                            keyedElements.Add(key, prevChild)
                        | _ -> ()
            
            // Reuse the element from the same index if possible (not already reused and reusable)
            // Otherwise create a new element
            let reuseOrCreate index newChild =
                if prevCollOpt.IsSome && prevCollOpt.Value.Length > index then
                    let prevChild = prevCollOpt.Value.[index]
                    let key = keyOf prevChild
                    
                    // Reuse the previous element at the same index only if it was not reused elsewhere
                    if not (identicalElements.Contains(prevChild))
                       && (key.IsNone || not (keyedElements.ContainsKey(key.Value)))
                       && (canReuse prevChild newChild) then
                           update index prevChild newChild
                        
                    else
                        create index newChild
                else
                    create index newChild
            
            for i in 0 .. coll.Length - 1 do
                let newChild = coll.[i]
                
                // Check if the same instance was reused (dependsOn), if so just move the element to the correct index
                if identicalElements.Contains(newChild) then
                    let prevIndex = currentState.IndexOf(newChild)
                    if prevIndex <> i then
                        move prevIndex i
                else
                    // If the key existed previously, reuse the previous element
                    match keyOf newChild with
                    | ValueSome key when keyedElements.ContainsKey(key) ->
                        let prevChild = keyedElements.[key]
                        let prevIndex = currentState.IndexOf(prevChild)
                        update prevIndex prevChild newChild
                        
                        if prevIndex <> i then
                            move prevIndex i
                    
                    // Otherwise, reuse an old element if possible or create a new one
                    | _ ->
                        reuseOrCreate i newChild
            
            // Remove all the excess elements
            if prevCollOpt.IsSome && prevCollOpt.Value.Length > coll.Length then
                while currentState.Count > coll.Length do
                    remove (currentState.Count - 1)
                    
    let updateItems
           (prevCollOpt: 'T[] voption)
           (collOpt: 'T[] voption)
           (targetColl: IList<'TargetT>)
           (keyOf: 'T -> string voption)
           (canReuse: 'T -> 'T -> bool)
           (create: 'T -> 'TargetT)
           (update: 'T -> 'T -> 'TargetT -> unit)
        =
        
        let create index child =
            let targetChild = create child
            if targetColl.Count > index then
                targetColl.[index] <- targetChild
            else
                targetColl.Insert(index, targetChild)
            
        let update index prevChild newChild =
            let targetChild = targetColl.[index]
            update prevChild newChild targetChild
            
        let move prevIndex newIndex =
            let targetChild = targetColl.[prevIndex]
            targetColl.RemoveAt(prevIndex)
            targetColl.Insert(newIndex, targetChild)
        
        updateItemsInternal
            prevCollOpt collOpt keyOf canReuse
            (fun () -> targetColl.Clear())
            create update move
            (fun index -> targetColl.RemoveAt(index))
    
    let inline updateViewElementHolderItems canReuseView (prevCollOpt: IViewElement[] voption) (collOpt: IViewElement[] voption) (targetColl: IList<ViewElementHolder>) =
        updateItems prevCollOpt collOpt targetColl
            (fun v -> v.TryKey) canReuseView
            ViewElementHolder (fun _ curr holder -> holder.ViewElement <- curr)
    
    let inline getCollection<'T> (coll: IEnumerable) (set: ObservableCollection<'T> -> unit) =
        match coll with 
        | :? ObservableCollection<'T> as oc -> oc
        | _ -> let oc = ObservableCollection<'T>() in set oc; oc
    
    /// Update the items in a ItemsView control, given previous and current view elements
    let inline updateItemsViewItems canReuseView prevCollOpt collOpt (target: Xamarin.Forms.ItemsView) =
        let targetColl = getCollection<ViewElementHolder> target.ItemsSource (fun oc -> target.ItemsSource <- oc)
        updateViewElementHolderItems canReuseView prevCollOpt collOpt targetColl