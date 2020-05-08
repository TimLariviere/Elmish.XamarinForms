namespace Fabulous.XamarinForms.DynamicViews

open Fabulous.DynamicViews
open Fabulous.XamarinForms
open Xamarin.Forms
    
type Dynamic<'T when 'T :> BindableObject>(create, update, attribs) =
    inherit DynamicViewElement(typeof<'T>, create, update, attribs)
    interface IBindableObject<'T>
    
type DynamicView<'T when 'T :> View>(create, update, attribs) =
    inherit Dynamic<'T>(create, update, attribs)
    interface IView<'T>
    
type DynamicCell<'T when 'T :> Cell>(create, update, attribs) =
    inherit Dynamic<'T>(create, update, attribs)
    interface ICell<'T>

