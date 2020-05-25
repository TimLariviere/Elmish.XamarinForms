// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.StaticViews

open Fabulous.StaticViews
open Fabulous.XamarinForms
open Xamarin.Forms

module StaticHelpers =
    let setBindingContext (_, newState, target: obj) =
        (target :?> BindableObject).BindingContext <- newState

[<AbstractClass>]  
type StaticPage<'T, 'msg when 'T :> Page>(?stateFn: (obj -> unit) -> obj) as this =
    inherit StaticViewElement(typeof<'T>, (fun () -> this.Create() |> box), StaticHelpers.setBindingContext, StateValue(fun dispatch -> match stateFn with None -> null | Some fn -> fn dispatch))
    interface IPage<'msg>
    abstract Create: unit -> 'TTarget
  
[<AbstractClass>]  
type StaticView<'T, 'msg when 'T :> View>(?stateFn: (obj -> unit) -> obj) as this =
    inherit StaticViewElement(typeof<'T>, (fun () -> this.Create() |> box), StaticHelpers.setBindingContext, StateValue(fun dispatch -> match stateFn with None -> null | Some fn -> fn dispatch))
    interface IView<'msg>
    abstract Create: unit -> 'T
    
[<AbstractClass>]
type StaticCell<'T, 'msg when 'T :> Cell>(?stateFn: (obj -> unit) -> obj) as this =
    inherit StaticViewElement(typeof<'T>, (fun () -> this.Create() |> box), StaticHelpers.setBindingContext, StateValue(fun dispatch -> match stateFn with None -> null | Some fn -> fn dispatch))
    interface ICell<'msg>
    abstract Create: unit -> 'T
    
[<AbstractClass>]
type StaticMenuItem<'T, 'msg when 'T :> MenuItem>(?stateFn: (obj -> unit) -> obj) as this =
    inherit StaticViewElement(typeof<'T>, (fun () -> this.Create() |> box), StaticHelpers.setBindingContext, StateValue(fun dispatch -> match stateFn with None -> null | Some fn -> fn dispatch))
    interface IMenuItem<'msg>
    abstract Create: unit -> 'T