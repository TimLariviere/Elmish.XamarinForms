// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.StaticViews

open System.Collections.Generic
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
            Update = (fun (programDefinition, prevOpt, currOpt, target) ->
                match currOpt with
                | ValueNone -> (target :?> BindableObject).BindingContext <- null
                | ValueSome curr -> (target :?> BindableObject).BindingContext <- (curr :?> StateValue).GetState(programDefinition.Dispatch)
            )
        }
        
    let createBindingContextKvp (stateFn: (('msg -> unit) -> obj) option) =
        KeyValuePair<DynamicProperty, obj>(
            BindingContext,
            StateValue(match stateFn with None -> None | Some fn -> Some (fun dispatch -> fn dispatch))
        )

[<AbstractClass>]  
type StaticPage<'T, 'msg when 'T :> Page and 'T : (new: unit -> 'T)>(events, properties) =
    inherit DynamicViewElement(typeof<'T>, (fun () -> new 'T() |> box), events, properties)
    interface IPage<'msg>
  
type StaticView<'T, 'msg when 'T :> View>(createFn, events, properties) =
    inherit DynamicViewElement(typeof<'T>, (fun () -> createFn() |> box), events, properties)
    interface IView<'msg>
    
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    member x.CreateFn: unit -> 'T = createFn
            
    static member inline init(createFn, ?state: ('msg -> unit) -> obj) =
        StaticView<'T, 'msg>(createFn, [], [ StaticHelpers.createBindingContextKvp state ])
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewHorizontalOptions.Value(options)::x.Properties
        StaticView<'T, 'msg>(x.CreateFn, x.Events, properties)
    
    member inline x.verticalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewVerticalOptions.Value(options)::x.Properties
        StaticView<'T, 'msg>(x.CreateFn, x.Events, properties)
            
    member inline x.gridColumn(column: int) =
        let properties = ViewAttributes.GridColumn.Value(column)::x.Properties
        StaticView<'T, 'msg>(x.CreateFn, x.Events, properties)
    
[<AbstractClass>]
type StaticCell<'T, 'msg when 'T :> Cell and 'T : (new: unit -> 'T)>(events, properties) =
    inherit DynamicViewElement(typeof<'T>, (fun () -> new 'T() |> box), events, properties)
    interface ICell<'msg>
    
[<AbstractClass>]
type StaticMenuItem<'T, 'msg when 'T :> MenuItem and 'T : (new: unit -> 'T)>(events, properties) =
    inherit DynamicViewElement(typeof<'T>, (fun () -> new 'T() |> box), events, properties)
    interface IMenuItem<'msg>
    
[<AbstractClass; Sealed>]
type View private () =
    static member inline StaticView(create: unit -> 'T, ?state: ('msg -> unit) -> 'state) =
        let state = match state with None -> None | Some fn -> Some (fun dispatch -> box (fn dispatch))
        StaticView<'T, 'msg>.init(create, ?state=state)
    
    
    