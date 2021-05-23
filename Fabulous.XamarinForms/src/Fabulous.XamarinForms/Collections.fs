namespace Fabulous.XamarinForms

open Fabulous
open System.Collections
open System.Collections.Generic
open System.Collections.ObjectModel
open Xamarin.Forms
open Xamarin.Forms.Shapes
open System.Buffers

/// This module contains the update logic for the controls with children
module Collections =
    [<Struct>]
    type Operation<'T> =
        | Insert of insertIndex: int *  element: 'T
        | Move of moveOldIndex: int * moveNewIndex: int
        | Update of updateIndex: int * updatePrev: 'T * updateCurr: 'T
        | MoveAndUpdate of moveAndUpdateOldIndex: int * moveAndUpdateprev: 'T * moveAndUpdatenewIndex: int * moveAndUpdatecurr: 'T
        | Delete of deleteOldIndex: int * deletePrev: 'T

    let isMatch canReuse aggressiveReuseMode currIndex newChild (struct (index, reusableChild)) =
        canReuse reusableChild newChild
        && (aggressiveReuseMode || index = currIndex)

    let rec tryFindRec canReuse aggressiveReuseMode currIndex newChild (reusableElements: struct (int * 'T)[]) (reusableElementsCount: int) index =
        if index >= reusableElementsCount then
            ValueNone
        elif isMatch canReuse aggressiveReuseMode currIndex newChild reusableElements.[index] then
            let struct (prevIndex, prevChild) = reusableElements.[index]
            ValueSome (struct (index, prevIndex, prevChild))
        else
            tryFindRec canReuse aggressiveReuseMode currIndex newChild reusableElements reusableElementsCount (index + 1)

    let tryFindReusableElement canReuse aggressiveReuseMode currIndex newChild reusableElements reusableElementsCount =
        tryFindRec canReuse aggressiveReuseMode currIndex newChild reusableElements reusableElementsCount 0

    let deleteAt index (arr: 'T[]) arrCount =
        if index + 1 >= arrCount then
            ()
        else
            for i = index to arrCount - 2 do
                arr.[i] <- arr.[i + 1]

    let rec canReuseChildOfRec keyOf canReuse prevChild (coll: 'T[]) key i =
        if i >= coll.Length then
            false
        elif keyOf coll.[i] = ValueSome key && canReuse prevChild coll.[i] then
            true
        else
            canReuseChildOfRec keyOf canReuse prevChild coll key (i + 1)

    let canReuseChildOf keyOf canReuse prevChild (coll: 'T[]) key =
        canReuseChildOfRec keyOf canReuse prevChild coll key 0

    let rec isIdenticalRec identical prevChild (coll: 'T[]) i =
        if i >= coll.Length then
            false
        elif identical prevChild coll.[i] then
            true
        else
            isIdenticalRec identical prevChild coll (i + 1)

    let isIdentical identical prevChild (coll: 'T[]) =
        isIdenticalRec identical prevChild coll 0

    /// Returns a list of operations to apply to go from the initial list to the new list
    ///
    /// The algorithm will try in priority to update elements sharing the same instance (usually achieved with dependsOn)
    /// or sharing the same key. All other elements will try to reuse previous elements where possible.
    /// If no reuse is possible, the element will create a new control.
    ///
    /// In aggressive reuse mode, the algorithm will try to reuse any reusable element.
    /// In the non-aggressive reuse mode, the algorithm will try to reuse a reusable element only if it is at the same index
    let diff<'T when 'T : equality>
            (aggressiveReuseMode: bool)
            (prevCollLength: int)
            (prevCollOpt: 'T[] voption)
            (coll: 'T[])
            (keyOf: 'T -> string voption)
            (canReuse: 'T -> 'T -> bool)

            (workingSet: Operation<'T>[])
        =
        let mutable workingSetIndex = 0

        // Separate the previous elements into 4 lists
        // The ones whose instances have been reused (dependsOn)
        // The ones whose keys have been reused and should be updated
        // The ones whose keys have not been reused and should be discarded
        // The rest which can be reused by any other element
        let identicalElements = Dictionary<'T, int>()
        let keyedElements = Dictionary<string, struct (int * 'T)>()
        let reusableElements = ArrayPool<struct (int * 'T)>.Shared.Rent(prevCollLength)
        let discardedElements = ArrayPool<struct (int * 'T)>.Shared.Rent(prevCollLength)

        let mutable reusableElementsCount = 0
        let mutable discardedElementsCount = 0

        if prevCollOpt.IsSome && prevCollOpt.Value.Length > 0 then
            for prevIndex in 0 .. prevCollOpt.Value.Length - 1 do
                let prevChild = prevCollOpt.Value.[prevIndex]
                if isIdentical identical prevChild coll then
                    identicalElements.Add(prevChild, prevIndex) |> ignore
                else
                    match keyOf prevChild with
                    | ValueSome key when canReuseChildOf keyOf canReuse prevChild coll key ->
                        if keyedElements.ContainsKey(key) then
                            failwithf "A same key '%s' can't be used multiple times inside a same collection. Please use different keys." key
                        keyedElements.Add(key, struct (prevIndex, prevChild))
                    | ValueNone ->
                        reusableElements.[reusableElementsCount] <- struct (prevIndex, prevChild)
                        reusableElementsCount <- reusableElementsCount + 1
                    | ValueSome _ ->
                        discardedElements.[discardedElementsCount] <- struct (prevIndex, prevChild)
                        discardedElementsCount <- discardedElementsCount + 1

        for i in 0 .. coll.Length - 1 do
            let newChild = coll.[i]

            // Check if the same instance was reused (dependsOn), if so just move the element to the correct index
            match identicalElements.TryGetValue(newChild) with
            | (true, prevIndex) ->
                if prevIndex <> i then
                    workingSet.[workingSetIndex] <- Move (prevIndex, i)
                    workingSetIndex <- workingSetIndex + 1
            | _ ->
                // If the key existed previously, reuse the previous element
                match keyOf newChild with
                | ValueSome key when keyedElements.ContainsKey(key) ->
                    let struct (prevIndex, prevChild) = keyedElements.[key]
                    if prevIndex <> i then
                        workingSet.[workingSetIndex] <- MoveAndUpdate (prevIndex, prevChild, i, newChild)
                        workingSetIndex <- workingSetIndex + 1
                    else
                        workingSet.[workingSetIndex] <- Update (i, prevChild, newChild)
                        workingSetIndex <- workingSetIndex + 1

                // Otherwise, reuse an old element if possible or create a new one
                | _ ->
                    match tryFindReusableElement canReuse aggressiveReuseMode i newChild reusableElements reusableElementsCount with
                    | ValueSome (struct (reusableIndex, prevIndex, prevChild)) ->
                        deleteAt reusableIndex reusableElements reusableElementsCount
                        reusableElementsCount <- reusableElementsCount - 1
                        if prevIndex <> i then
                            workingSet.[workingSetIndex] <- MoveAndUpdate (prevIndex, prevChild, i, newChild)
                            workingSetIndex <- workingSetIndex + 1
                        else
                            workingSet.[workingSetIndex] <- Update (i, prevChild, newChild)
                            workingSetIndex <- workingSetIndex + 1

                    | ValueNone ->
                        workingSet.[workingSetIndex] <- Insert (i, newChild)
                        workingSetIndex <- workingSetIndex + 1

        // If we have discarded elements, delete them
        if discardedElementsCount > 0 then
            for i = 0 to discardedElementsCount - 1 do
                let struct (discardedIndex, discardedChild) = discardedElements.[i]
                workingSet.[workingSetIndex] <- Delete (discardedIndex, discardedChild)
                workingSetIndex <- workingSetIndex + 1

        // If we still have old elements that were not reused, delete them
        if reusableElementsCount > 0 then
            for i = 0 to reusableElementsCount - 1 do
                let struct (prevIndex, prevChild) = reusableElements.[i]
                workingSet.[workingSetIndex] <- Delete (prevIndex, prevChild)
                workingSetIndex <- workingSetIndex + 1

        ArrayPool<struct (int * 'T)>.Shared.Return(reusableElements)
        ArrayPool<struct (int * 'T)>.Shared.Return(discardedElements)

        workingSetIndex

    // Shift all old indices by 1 (down the list) on insert after the inserted position
    let shiftForInsert (prevIndices: int[]) index =
        for i in 0 .. prevIndices.Length - 1 do
            if prevIndices.[i] >= index then
                prevIndices.[i] <- prevIndices.[i] + 1

    // Shift all old indices by -1 (up the list) on delete after the deleted position
    let shiftForDelete (prevIndices: int[]) originalIndexInPrevColl prevIndex =
        for i in 0 .. prevIndices.Length - 1 do
            if prevIndices.[i] > prevIndex then
                prevIndices.[i] <- prevIndices.[i] - 1
        prevIndices.[originalIndexInPrevColl] <- -1

    // Shift all old indices between the previous and new position on move
    let shiftForMove (prevIndices: int[]) originalIndexInPrevColl prevIndex newIndex =
        for i in 0 .. prevIndices.Length - 1 do
            if prevIndex < prevIndices.[i] && prevIndices.[i] <= newIndex then
                prevIndices.[i] <- prevIndices.[i] - 1
            else if newIndex <= prevIndices.[i] && prevIndices.[i] < prevIndex then
                prevIndices.[i] <- prevIndices.[i] + 1
        prevIndices.[originalIndexInPrevColl] <- newIndex

    // Return an update operation preceded by a move only if actual indices don't match
    let moveAndUpdate (prevIndices: int[]) oldIndex prev newIndex curr =
        let prevIndex = prevIndices.[oldIndex]
        if prevIndex = newIndex then
            Update (newIndex, prev, curr)
        else
            shiftForMove prevIndices oldIndex prevIndex newIndex
            MoveAndUpdate (prevIndex, prev, newIndex, curr)

    /// Reduces the operations of the DiffResult to be applicable to an ObservableCollection.
    ///
    /// diff returns all the operations to move from List A to List B.
    /// Except with ObservableCollection, we're forced to apply the changes one after the other, changing the indices
    /// So this algorithm compensates this offsetting
    let adaptDiffForObservableCollection (prevCollLength: int) (workingSet: Operation<'T>[]) (workingSetIndex: int) =
        let prevIndices = Array.init prevCollLength id

        let mutable position = 0

        for i = 0 to workingSetIndex - 1 do
            match workingSet.[i] with
            | Insert (index, element) ->
                workingSet.[position] <- Insert (index, element)
                position <- position + 1
                shiftForInsert prevIndices index

            | Move (oldIndex, newIndex) ->
                // Prevent a move if the actual indices match
                let prevIndex = prevIndices.[oldIndex]
                if prevIndex <> newIndex then
                    workingSet.[position] <- (Move (prevIndex, newIndex))
                    position <- position + 1
                    shiftForMove prevIndices oldIndex prevIndex newIndex

            | Update (index, prev, curr) ->
                workingSet.[position] <- moveAndUpdate prevIndices index prev index curr
                position <- position + 1

            | MoveAndUpdate (oldIndex, prev, newIndex, curr) ->
                workingSet.[position] <- moveAndUpdate prevIndices oldIndex prev newIndex curr
                position <- position + 1

            | Delete (oldIndex, prev) ->
                let prevIndex = prevIndices.[oldIndex]
                workingSet.[position] <- Delete (prevIndex, prev)
                position <- position + 1
                shiftForDelete prevIndices oldIndex prevIndex

        position

    /// Incremental list maintenance: given a collection, and a previous version of that collection, perform
    /// a reduced number of clear/add/remove/insert operations
    let updateCollection
           (aggressiveReuseMode: bool)
           (prevCollOpt: 'T[] voption)
           (collOpt: 'T[] voption)
           (targetColl: IList<'TargetT>)
           (keyOf: 'T -> string voption)
           (canReuse: 'T -> 'T -> bool)
           (create: 'T -> 'TargetT)
           (update: 'T -> 'T -> 'TargetT -> unit) // Incremental element-wise update, only if element reuse is allowed
           (attach: 'T voption -> 'T -> obj -> unit) // adjust attached properties
           (unmount: 'T -> 'TargetT -> unit)
        =

        match struct (prevCollOpt, collOpt) with
        | struct (ValueNone, ValueNone) -> ()
        | struct (ValueSome prevColl, ValueSome newColl) when identical prevColl newColl -> ()
        | struct (ValueSome prevColl, ValueSome newColl) when prevColl <> null && newColl <> null && prevColl.Length = 0 && newColl.Length = 0 -> ()
        | struct (ValueSome _, ValueNone) -> targetColl.Clear()
        | struct (ValueSome _, ValueSome coll) when (coll = null || coll.Length = 0) -> targetColl.Clear()
        | struct (_, ValueSome coll) ->
            let prevCollLength = (match prevCollOpt with ValueNone -> 0 | ValueSome c -> c.Length)
            let workingSet = ArrayPool<Operation<'T>>.Shared.Rent(prevCollLength + coll.Length)

            let operationsCount =
                diff aggressiveReuseMode prevCollLength prevCollOpt coll keyOf canReuse workingSet
                |> adaptDiffForObservableCollection prevCollLength workingSet

            for i = 0 to operationsCount - 1 do
                match workingSet.[i] with
                | Insert (index, element) ->
                    let child = create element
                    attach ValueNone element child
                    targetColl.Insert(index, child)

                | Move (prevIndex, newIndex) ->
                    let child = targetColl.[prevIndex]
                    targetColl.RemoveAt(prevIndex)
                    targetColl.Insert(newIndex, child)

                | Update (index, prev, curr) ->
                    let child = targetColl.[index]
                    update prev curr child
                    attach (ValueSome prev) curr child

                | MoveAndUpdate (prevIndex, prev, newIndex, curr) ->
                    let child = targetColl.[prevIndex]
                    targetColl.RemoveAt(prevIndex)
                    targetColl.Insert(newIndex, child)
                    update prev curr child
                    attach (ValueSome prev) curr child

                | Delete (index, prev) ->
                    unmount prev targetColl.[index]
                    targetColl.RemoveAt(index)

            ArrayPool<Operation<'T>>.Shared.Return(workingSet)

    let updateChildren (definition: ProgramDefinition) prevCollOpt collOpt target create update attach =
        updateCollection true prevCollOpt collOpt target ViewHelpers.tryGetKey definition.canReuseView
            (fun c -> create definition (ValueSome (box target)) c)
            (fun prev curr target -> update definition prev curr target)
            (fun prevOpt curr target -> attach definition prevOpt curr target)
            (fun prev target -> prev.Unmount(target, true))

    let updateItems definition prevCollOpt collOpt target keyOf canReuse create update attach unmount =
        updateCollection false prevCollOpt collOpt target keyOf canReuse
            (fun c -> create definition (ValueSome (box target)) c)
            (fun prev curr target -> update definition prev curr target)
            (fun prevOpt curr target -> attach definition prevOpt curr target)
            unmount

    /// Update a control given the previous and new view elements
    let inline updateChild (definition: ProgramDefinition) (prevChild: IViewElement) (newChild: IViewElement) targetChild =
        newChild.Update(definition, ValueSome prevChild, targetChild)

    /// Update the items of a TableSectionBase<'T> control, given previous and current view elements
    let inline updateTableSectionBaseOfTItems<'T when 'T :> BindableObject> definition prevCollOpt collOpt (target: TableSectionBase<'T>) attach =
        updateChildren definition prevCollOpt collOpt target (fun def parentOpt c -> c.Create(def, parentOpt) :?> 'T) updateChild attach

    /// Update the items of a Shell, given previous and current view elements
    let inline updateShellItems definition prevCollOpt collOpt (target: Shell) attach =
        let createChild definition parentOpt (desc: IViewElement) =
            match desc.Create(definition, parentOpt) with
            | :? ShellContent as shellContent -> ShellItem.op_Implicit shellContent
            | :? TemplatedPage as templatedPage -> ShellItem.op_Implicit templatedPage
            | :? ShellSection as shellSection -> ShellItem.op_Implicit shellSection
            | :? MenuItem as menuItem -> ShellItem.op_Implicit menuItem
            | :? ShellItem as shellItem -> shellItem
            | child -> failwithf "%s is not compatible with the type ShellItem" (child.GetType().Name)

        let updateChild (definition: ProgramDefinition) prevViewElement (currViewElement: IViewElement) (target: ShellItem) =
            let realTarget =
                match currViewElement.TargetType with
                | t when t = typeof<ShellContent> -> target.Items.[0].Items.[0] :> Element
                | t when t = typeof<TemplatedPage> -> target.Items.[0].Items.[0] :> Element
                | t when t = typeof<ShellSection> -> target.Items.[0] :> Element
                | t when t = typeof<MenuItem> -> target.GetType().GetProperty("MenuItem").GetValue(target) :?> Element // MenuShellItem is marked as internal
                | _ -> target :> Element
            updateChild definition prevViewElement currViewElement realTarget

        updateChildren definition prevCollOpt collOpt target.Items createChild updateChild attach

    /// Update the menu items of a ShellContent, given previous and current view elements
    let inline updateShellContentMenuItems definition prevCollOpt collOpt (target: ShellContent) =
        updateChildren definition prevCollOpt collOpt target.MenuItems (fun def parentOpt c -> c.Create(def, parentOpt) :?> MenuItem) updateChild (fun _ _ _ _ -> ())

    /// Update the items of a ShellItem, given previous and current view elements
    let inline updateShellItemItems definition prevCollOpt collOpt (target: ShellItem) attach =
        let createChild definition parentOpt (desc: IViewElement) =
            match desc.Create(definition, parentOpt) with
            | :? ShellContent as shellContent -> ShellSection.op_Implicit shellContent
            | :? TemplatedPage as templatedPage -> ShellSection.op_Implicit templatedPage
            | :? ShellSection as shellSection -> shellSection
            | child -> failwithf "%s is not compatible with the type ShellSection" (child.GetType().Name)

        let updateChild definition prevViewElement (currViewElement: IViewElement) (target: ShellSection) =
            let realTarget =
                match currViewElement.TargetType with
                | t when t = typeof<ShellContent> -> target.Items.[0] :> BaseShellItem
                | t when t = typeof<TemplatedPage> -> target.Items.[0] :> BaseShellItem
                | _ -> target :> BaseShellItem
            updateChild definition prevViewElement currViewElement realTarget

        updateChildren definition prevCollOpt collOpt target.Items createChild updateChild attach

    /// Update the items of a ShellSection, given previous and current view elements
    let inline updateShellSectionItems definition prevCollOpt collOpt (target: ShellSection) attach =
        updateChildren definition prevCollOpt collOpt target.Items (fun def parentOpt c -> c.Create(def, parentOpt) :?> ShellContent) updateChild attach

    /// Update the items of a SwipeItems, given previous and current view elements
    let inline updateSwipeItems definition prevCollOpt collOpt (target: SwipeItems) =
        updateChildren definition prevCollOpt collOpt target (fun def parentOpt c -> c.Create(def, parentOpt) :?> ISwipeItem) updateChild (fun _ _ _ _ -> ())

    /// Update the children of a Menu, given previous and current view elements
    let inline updateMenuChildren definition prevCollOpt collOpt (target: Menu) attach =
        updateChildren definition prevCollOpt collOpt target (fun def parentOpt c -> c.Create(def, parentOpt) :?> Menu) updateChild attach

    /// Update the effects of an Element, given previous and current view elements
    let inline updateElementEffects definition prevCollOpt collOpt (target: Element) attach =
        let createChild definition parentOpt (desc: IViewElement) =
            match desc.Create(definition, parentOpt) with
            | :? CustomEffect as customEffect -> Effect.Resolve(customEffect.Name)
            | effect -> effect :?> Effect

        updateChildren definition prevCollOpt collOpt target.Effects createChild updateChild attach

    /// Update the toolbar items of a Page, given previous and current view elements
    let inline updatePageToolbarItems definition prevCollOpt collOpt (target: Page) attach =
        updateChildren definition prevCollOpt collOpt target.ToolbarItems (fun def parentOpt c -> c.Create(def, parentOpt) :?> ToolbarItem) updateChild attach

    /// Update the children of a TransformGroup, given previous and current view elements
    let inline updateTransformGroupChildren definition prevCollOpt collOpt (target: TransformGroup) attach =
        let targetColl =
            match target.Children with
            | null -> let oc = TransformCollection() in target.Children <- oc; oc
            | oc -> oc
        updateChildren definition prevCollOpt collOpt targetColl (fun def parentOpt c -> c.Create(def, parentOpt) :?> Transform) updateChild attach

    /// Update the children of a GeometryGroup, given previous and current view elements
    let inline updateGeometryGroupChildren definition prevCollOpt collOpt (target: GeometryGroup) attach =
        let targetColl =
            match target.Children with
            | null -> let oc = GeometryCollection() in target.Children <- oc; oc
            | oc -> oc
        updateChildren definition prevCollOpt collOpt targetColl (fun def parentOpt c -> c.Create(def, parentOpt) :?> Geometry) updateChild attach

    /// Update the segments of a PathFigure, given previous and current view elements
    let inline updatePathFigureSegments definition prevCollOpt collOpt (target: PathFigure) attach =
        let targetColl =
            match target.Segments with
            | null -> let oc = PathSegmentCollection() in target.Segments <- oc; oc
            | oc -> oc
        updateChildren definition prevCollOpt collOpt targetColl (fun def parentOpt c -> c.Create(def, parentOpt) :?> PathSegment) updateChild attach

    /// Update the stroke dash values of a Shape, given previous and current float list
    let inline updateShapeStrokeDashArray _ prevCollOpt collOpt (target: Xamarin.Forms.Shapes.Shape) =
        let targetColl =
            match target.StrokeDashArray with
            | null -> let oc = DoubleCollection() in target.StrokeDashArray <- oc; oc
            | oc -> oc
        updateCollection true prevCollOpt collOpt targetColl (fun _ -> ValueNone) (fun _ _ -> false) (fun c -> c) (fun _ _ _ -> ()) (fun _ _ _ -> ()) (fun _ _ -> ())

    let inline updateViewElementHolderItems definition (prevCollOpt: IViewElement[] voption) (collOpt: IViewElement[] voption) (targetColl: IList<ViewElementHolder>) attach =
        updateItems definition prevCollOpt collOpt targetColl
            ViewHelpers.tryGetKey ViewHelpers.canReuseView
            (fun def parentOpt c -> ViewElementHolder(c, def)) (fun _ _ curr holder -> holder.ViewElement <- curr)
            attach
            (fun prev target -> prev.Unmount(target, true))

    let inline getCollection<'T> (coll: IEnumerable) (set: ObservableCollection<'T> -> unit) =
        match coll with
        | :? ObservableCollection<'T> as oc -> oc
        | _ -> let oc = ObservableCollection<'T>() in set oc; oc

    /// Update the items in a ItemsView control, given previous and current view elements
    let inline updateItemsViewItems definition prevCollOpt collOpt (target: ItemsView) =
        let targetColl = getCollection<ViewElementHolder> target.ItemsSource (fun oc -> target.ItemsSource <- oc)
        updateViewElementHolderItems definition prevCollOpt collOpt targetColl (fun _ _ _ _ -> ())

    /// Update the items in a ItemsView<'T> control, given previous and current view elements
    let inline updateItemsViewOfTItems<'T when 'T :> BindableObject> definition prevCollOpt collOpt (target: ItemsView<'T>) =
        let targetColl = getCollection<ViewElementHolder> target.ItemsSource (fun oc -> target.ItemsSource <- oc)
        updateViewElementHolderItems definition prevCollOpt collOpt targetColl (fun _ _ _ _ -> ())

    /// Update the items in a SearchHandler control, given previous and current view elements
    let inline updateSearchHandlerItems definition _ collOpt (target: SearchHandler) =
        let targetColl = List<ViewElementHolder>()
        updateViewElementHolderItems definition ValueNone collOpt targetColl (fun _ _ _ _ -> ())
        target.ItemsSource <- targetColl

    /// Update the items in a GroupedListView control, given previous and current view elements
    let inline updateListViewGroupedItems definition (prevCollOpt: (string * IViewElement * IViewElement[])[] voption) (collOpt: (string * IViewElement * IViewElement[])[] voption) (target: ListView) =
        let updateViewElementHolderGroup definition (_prevShortName: string, _prevKey, prevColl: IViewElement[]) (currShortName: string, currKey, currColl: IViewElement[]) (target: ViewElementHolderGroup) =
            target.ShortName <- currShortName
            target.ViewElement <- currKey
            updateViewElementHolderItems definition (ValueSome prevColl) (ValueSome currColl) target (fun _ _ _ _ -> ())

        let targetColl = getCollection<ViewElementHolderGroup> target.ItemsSource (fun oc -> target.ItemsSource <- oc)
        updateItems definition prevCollOpt collOpt targetColl
            (fun (key, _, _) -> ValueSome key)
            (fun (_, prevHeader, _) (_, currHeader, _) -> ViewHelpers.canReuseView prevHeader currHeader)
            (fun def parentOpt (k, h, v) -> ViewElementHolderGroup(k, h, v, def)) updateViewElementHolderGroup
            (fun _ _ _ _ -> ())
            (fun _ _ -> ())

    /// Update the selected items in a SelectableItemsView control, given previous and current indexes
    let inline updateSelectableItemsViewSelectedItems definition (prevCollOptOpt: int[] option voption) (collOptOpt: int[] option voption) (target: SelectableItemsView) =
        let prevCollOpt = match prevCollOptOpt with ValueNone | ValueSome None -> ValueNone | ValueSome (Some x) -> ValueSome x
        let collOpt = match collOptOpt with ValueNone | ValueSome None -> ValueNone | ValueSome (Some x) -> ValueSome x
        let targetColl = getCollection<obj> target.SelectedItems (fun oc -> target.SelectedItems <- oc)

        let findItem _ _ idx =
            let itemsSource = target.ItemsSource :?> IList<ViewElementHolder>
            itemsSource.[idx] :> obj

        updateItems definition prevCollOpt collOpt targetColl (fun _ -> ValueNone) (fun x y -> x = y) findItem (fun _ _ _ _ -> ()) (fun _ _ _ _ -> ()) (fun _ _ -> ())


    let inline updatePathGeometryFigures definition (prevOpt: InputTypes.Figures.Value voption) (currOpt: InputTypes.Figures.Value voption) (target: PathGeometry) =
        match prevOpt, currOpt with
        | ValueNone, ValueNone -> ()
        | ValueSome prev, ValueSome curr when prev = curr -> ()

        | _, ValueNone ->
            target.Figures <- PathFigureCollection()

        | ValueNone, ValueSome (Figures.String curr)
        | ValueSome _, ValueSome (Figures.String curr) ->
            target.Figures <- PathFigureCollectionConverter().ConvertFromInvariantString(curr) :?> PathFigureCollection

        | ValueNone, ValueSome (Figures.FiguresList curr) ->
            let targetColl =
                match target.Figures with
                | oc when oc.GetType() = typeof<PathFigureCollection> -> oc
                | _ -> let oc = PathFigureCollection() in target.Figures <- oc; oc
            updateChildren definition ValueNone (ValueSome curr) targetColl (fun def parentOpt c -> c.Create(def, parentOpt) :?> PathFigure) updateChild (fun _ _ _ _ -> ())

        | ValueSome (Figures.FiguresList prev), ValueSome (Figures.FiguresList curr) ->
            let targetColl =
                match target.Figures with
                | oc when oc.GetType() = typeof<PathFigureCollection> -> oc
                | _ -> let oc = PathFigureCollection() in target.Figures <- oc; oc
            updateChildren definition (ValueSome prev) (ValueSome curr) targetColl (fun def parentOpt c -> c.Create(def, parentOpt) :?> PathFigure) updateChild (fun _ _ _ _ -> ())

        | ValueSome (Figures.String _), ValueSome (Figures.FiguresList curr) ->
            let targetColl = PathFigureCollection()
            updateChildren definition ValueNone (ValueSome curr) targetColl (fun def parentOpt c -> c.Create(def, parentOpt) :?> PathFigure) updateChild (fun _ _ _ _ -> ())
            target.Figures <- targetColl

    // Update the collection of gradient stops of a GradientBrush, given previous and current values
    let inline updateGradientBrushGradientStops definition (prevCollOpt: IViewElement[] voption) (collOpt: IViewElement[] voption) (target: GradientBrush) =
        let targetColl =
            match target.GradientStops with
            | null -> let oc = GradientStopCollection() in target.GradientStops <- oc; oc
            | oc -> oc
        updateChildren definition prevCollOpt collOpt targetColl (fun def parentOpt c -> c.Create(def, parentOpt) :?> GradientStop) updateChild (fun _ _ _ _ -> ())
        
    let unmountChildren (collOpt: IViewElement[] voption) (targetColl: IList<'a>) =
        match struct (collOpt, targetColl) with
        | struct (ValueNone, _) -> ()
        | struct (_, null) -> ()
        | struct (ValueSome coll, target) ->
            for i = 0 to coll.Length - 1 do
                let targetChild = target.[i]
                coll.[i].Unmount(targetChild, true)