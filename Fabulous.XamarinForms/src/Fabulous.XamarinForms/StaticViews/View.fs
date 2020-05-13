// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.StaticViews

open Fabulous.StaticViews
open Fabulous.XamarinForms
open Xamarin.Forms

module StaticHelpers =
    let setBindingContext (newBindingContext, target: obj) =
        (target :?> BindableObject).BindingContext <- newBindingContext
        
    let unsetBindingContext (target: obj) =
        (target :?> BindableObject).BindingContext <- null

  
[<AbstractClass>]  
type StaticPage<'T when 'T :> Page>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, StaticHelpers.unsetBindingContext, bindingContext |> Option.defaultValue null)
    interface IPage
    abstract Create: unit -> 'T

  
[<AbstractClass>]  
type StaticView<'T when 'T :> View>() as this =
    inherit StaticViewElement<'T>(this.Create, ignore, ignore, null)
    interface IView
    abstract Create: unit -> 'T
    
[<AbstractClass>]  
type StaticView<'TView, 'TState when 'TView :> View>(state: 'TState) as this =
    inherit StaticViewElement<'TView>(this.Create, StaticHelpers.setBindingContext, StaticHelpers.unsetBindingContext, state)
    interface IView
    abstract Create: unit -> 'TView
    member x.State = state
    
[<AbstractClass>]
type StaticCell<'T when 'T :> Cell>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, StaticHelpers.unsetBindingContext, bindingContext |> Option.defaultValue null)
    interface ICell
    abstract Create: unit -> 'T