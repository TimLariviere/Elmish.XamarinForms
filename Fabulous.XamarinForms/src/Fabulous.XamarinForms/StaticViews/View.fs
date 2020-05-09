namespace Fabulous.XamarinForms.StaticViews

open Fabulous.StaticViews
open Fabulous.XamarinForms
open Xamarin.Forms

module StaticHelpers =
    let setBindingContext (_, newBindingContext, target: obj) =
        (target :?> BindableObject).BindingContext <- newBindingContext
        
    let unsetBindingContext (target: obj) =
        (target :?> BindableObject).BindingContext <- null

  
[<AbstractClass>]  
type StaticPage<'T when 'T :> Page>(?bindingContext) as this =
    inherit StaticViewElement<'T>(this.Create, StaticHelpers.setBindingContext, StaticHelpers.unsetBindingContext, bindingContext |> Option.defaultValue null)
    interface IPage<'T>
    abstract Create: unit -> 'T

  
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