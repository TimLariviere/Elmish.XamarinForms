namespace Fabulous.XamarinForms.DynamicViews

open Fabulous.DynamicViews
open Fabulous.XamarinForms
open Xamarin.Forms
    
type DynamicBindableObject<'T when 'T :> BindableObject>(create, attribs) =
    inherit DynamicViewElement<'T>(create, attribs)
    interface IBindableObject<'T>
    
type DynamicPage<'T when 'T :> Page>(create, attribs) =
    inherit DynamicBindableObject<'T>(create, attribs)
    interface IPage<'T>
    
type DynamicView<'T when 'T :> View>(create, attribs) =
    inherit DynamicBindableObject<'T>(create, attribs)
    interface IView<'T>
    
type DynamicCell<'T when 'T :> Cell>(create, attribs) =
    inherit DynamicBindableObject<'T>(create, attribs)
    interface ICell<'T>

