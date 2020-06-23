// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.StaticViews

open System.ComponentModel
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.DynamicViews
open Fabulous.XamarinForms.DynamicViews.Attributes
open Xamarin.Forms

module StaticHelpers =
    type StateValue(stateFn: ((obj -> unit) -> obj) option) =
        member x.GetState(dispatch: obj -> unit) =
            match stateFn with
            | None -> null
            | Some fn -> fn dispatch
            
    let BindingContext =
        {
            DebugName = "BindingContext"
            Update = (fun (programDefinition, prevOpt, currOpt, target) ->
                match currOpt with
                | ValueNone -> (target :?> BindableObject).BindingContext <- null
                | ValueSome curr -> (target :?> BindableObject).BindingContext <- (curr :?> StateValue).GetState(programDefinition.Dispatch)
            )
        }
        
    let createBindingContextKvp (stateFn: (('msg -> unit) -> obj) option) =
        (BindingContext,
         StateValue(match stateFn with None -> None | Some fn -> Some (fun dispatch -> fn dispatch)) :> obj)

[<Struct>]
type StaticPage<'T, 'msg when 'T :> Page>(create: unit -> 'T, events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IPage<'msg> with
        member x.AsViewElement() =
            let createFn = create
            DynamicViewElement(typeof<'T>, (fun () -> createFn() |> box), readOnlyDict events, readOnlyDict properties) :> IViewElement
            
    static member inline init(create, ?state: ('msg -> unit) -> obj) =
        StaticPage<'T, 'msg>(create, [], [ StaticHelpers.createBindingContextKvp state ])
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Create = create
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
 
[<Struct>]
type StaticView<'T, 'msg when 'T :> View>(create: unit -> 'T, events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            let createFn = create
            DynamicViewElement(typeof<'T>, (fun () -> createFn() |> box), readOnlyDict events, readOnlyDict properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Create = create
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
            
    static member inline init(create, ?state: ('msg -> unit) -> obj) =
        StaticView<'T, 'msg>(create, [], [ StaticHelpers.createBindingContextKvp state ])
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewHorizontalOptions.Value(options)::x.Properties
        StaticView<'T, 'msg>(x.Create, x.Events, properties)
    
    member inline x.verticalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewVerticalOptions.Value(options)::x.Properties
        StaticView<'T, 'msg>(x.Create, x.Events, properties)
            
    member inline x.gridColumn(column: int) =
        let properties = ViewAttributes.GridColumn.Value(column)::x.Properties
        StaticView<'T, 'msg>(x.Create, x.Events, properties)
    
[<Struct>]
type StaticCell<'T, 'msg when 'T :> Cell>(create: unit -> 'T, events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface ICell<'msg> with
        member x.AsViewElement() =
            let createFn = create
            DynamicViewElement(typeof<'T>, (fun () -> createFn() |> box), readOnlyDict events, readOnlyDict properties) :> IViewElement
            
    static member inline init(create, ?state: ('msg -> unit) -> obj) =
        StaticCell<'T, 'msg>(create, [], [ StaticHelpers.createBindingContextKvp state ])
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Create = create
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
[<Struct>]
type StaticMenuItem<'T, 'msg when 'T :> MenuItem>(create: unit -> 'T, events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IMenuItem<'msg> with
        member x.AsViewElement() =
            let createFn = create
            DynamicViewElement(typeof<'T>, (fun () -> createFn() |> box), readOnlyDict events, readOnlyDict properties) :> IViewElement
            
    static member inline init(create, ?state: ('msg -> unit) -> obj) =
        StaticMenuItem<'T, 'msg>(create, [], [ StaticHelpers.createBindingContextKvp state ])
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Create = create
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
[<AbstractClass; Sealed>]
type View private () =
    static member inline StaticPage(create: unit -> 'T, ?state: ('msg -> unit) -> 'state) =
        let state = match state with None -> None | Some fn -> Some (fun dispatch -> box (fn dispatch))
        StaticPage<'T, 'msg>.init(create, ?state=state)
        
    static member inline StaticView(create: unit -> 'T, ?state: ('msg -> unit) -> 'state) =
        let state = match state with None -> None | Some fn -> Some (fun dispatch -> box (fn dispatch))
        StaticView<'T, 'msg>.init(create, ?state=state)
        
    static member inline StaticCell(create: unit -> 'T, ?state: ('msg -> unit) -> 'state) =
        let state = match state with None -> None | Some fn -> Some (fun dispatch -> box (fn dispatch))
        StaticCell<'T, 'msg>.init(create, ?state=state)
        
    static member inline StaticMenuItem(create: unit -> 'T, ?state: ('msg -> unit) -> 'state) =
        let state = match state with None -> None | Some fn -> Some (fun dispatch -> box (fn dispatch))
        StaticMenuItem<'T, 'msg>.init(create, ?state=state)
    