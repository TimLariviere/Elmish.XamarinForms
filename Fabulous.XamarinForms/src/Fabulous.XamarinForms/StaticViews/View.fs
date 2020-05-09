namespace Fabulous.XamarinForms.StaticViews

open Fabulous.StaticViews
open Fabulous.XamarinForms
open Xamarin.Forms

module StaticHelpers =
    let setBindingContext (_, newBindingContext, target: #BindableObject) =
        target.BindingContext <- newBindingContext
        
    let unsetBindingContext (target: #BindableObject) =
        target.BindingContext <- null

  
[<AbstractClass>]  
type StaticView<'T when 'T :> View>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, StaticHelpers.unsetBindingContext, bindingContext |> Option.defaultValue null)
    interface IView<'T>
    abstract Create: unit -> 'T
    
[<AbstractClass>]
type StaticCell<'T when 'T :> Cell>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, StaticHelpers.unsetBindingContext, bindingContext |> Option.defaultValue null)
    interface ICell<'T>
    abstract Create: unit -> 'T