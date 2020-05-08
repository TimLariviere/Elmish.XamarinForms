namespace Fabulous.XamarinForms.StaticViews

open Fabulous.StaticViews
open Fabulous.XamarinForms
open Xamarin.Forms

module StatefulView =
    let setBindingContext _ newBindingContext target =
        (unbox<BindableObject> target).BindingContext <- newBindingContext


type Stateful<'T when 'T :> BindableObject>(create: unit -> 'T, bindingContext: obj) =
    inherit StaticViewElement(typeof<'T>, (create >> box), StatefulView.setBindingContext, bindingContext)
    interface IBindableObject<'T>
    
type StatefulView<'T when 'T :> View>(create, bindingContext) =
    inherit Stateful<'T>(create, bindingContext)
    interface IView<'T>
    
type StatefulCell<'T when 'T :> Cell>(create, bindingContext) =
    inherit Stateful<'T>(create, bindingContext)
    interface ICell<'T>
    
    
type Stateless<'T when 'T :> BindableObject>(create: unit -> 'T) =
    inherit StaticViewElement(typeof<'T>, (create >> box), (fun _ _ _ -> ()), null)
    interface IBindableObject<'T>
    
type StatelessView<'T when 'T :> View>(create) =
    inherit Stateless<'T>(create)
    interface IView<'T>
    
type StatelessCell<'T when 'T :> Cell>(create) =
    inherit Stateless<'T>(create)
    interface ICell<'T>
        
[<AbstractClass; Sealed>]
type View private () =
    static member StatefulObject<'T when 'T :> BindableObject>(create, bindingContext) =
        Stateful<'T>(create, bindingContext) :> IBindableObject<'T>
        
    static member StatefulView<'T when 'T :> Xamarin.Forms.View>(create, bindingContext) =
        StatefulView<'T>(create, bindingContext) :> IView<'T>
        
    static member StatefulCell<'T when 'T :> Xamarin.Forms.Cell>(create, bindingContext) =
        StatefulCell<'T>(create, bindingContext) :> ICell<'T>
        
    static member StatelessObject<'T when 'T :> BindableObject>(create) =
        Stateless<'T>(create) :> IBindableObject<'T>
        
    static member StatelessView<'T when 'T :> Xamarin.Forms.View>(create) =
        StatelessView<'T>(create) :> IView<'T>
        
    static member StatelessCell<'T when 'T :> Xamarin.Forms.Cell>(create) =
        StatelessCell<'T>(create) :> ICell<'T>