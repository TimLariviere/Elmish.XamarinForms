// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.StaticViews

open Fabulous.StaticViews
open Fabulous.XamarinForms
open Xamarin.Forms

module StaticHelpers =
    let setBindingContext (prevState, newState, target: obj) =
        (target :?> BindableObject).BindingContext <- newState

[<AbstractClass>]  
type StaticPage<'T when 'T :> Page>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, bindingContext |> Option.defaultValue null)
    interface IPage
    abstract Create: unit -> 'TTarget
  
[<AbstractClass>]  
type StaticView<'T when 'T :> View>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, bindingContext |> Option.defaultValue null)
    interface IView
    abstract Create: unit -> 'T
    
[<AbstractClass>]
type StaticCell<'T when 'T :> Cell>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, bindingContext |> Option.defaultValue null)
    interface ICell
    abstract Create: unit -> 'T
    
[<AbstractClass>]
type StaticMenuItem<'T when 'T :> MenuItem>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, bindingContext |> Option.defaultValue null)
    interface IMenuItem
    abstract Create: unit -> 'T