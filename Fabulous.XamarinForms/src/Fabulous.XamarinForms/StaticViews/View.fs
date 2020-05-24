// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.StaticViews

open Fabulous.StaticViews
open Fabulous.XamarinForms
open Xamarin.Forms

module StaticHelpers =
    let setBindingContext (prevState, newState, target: obj) =
        (target :?> BindableObject).BindingContext <- newState

[<AbstractClass>]  
type StaticPage<'T, 'msg when 'T :> Page>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, bindingContext |> Option.defaultValue null)
    interface IPage<'msg>
    abstract Create: unit -> 'TTarget
  
[<AbstractClass>]  
type StaticView<'T, 'msg when 'T :> View>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, bindingContext |> Option.defaultValue null)
    interface IView<'msg>
    abstract Create: unit -> 'T
    
[<AbstractClass>]
type StaticCell<'T, 'msg when 'T :> Cell>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, bindingContext |> Option.defaultValue null)
    interface ICell<'msg>
    abstract Create: unit -> 'T
    
[<AbstractClass>]
type StaticMenuItem<'T, 'msg when 'T :> MenuItem>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, bindingContext |> Option.defaultValue null)
    interface IMenuItem<'msg>
    abstract Create: unit -> 'T