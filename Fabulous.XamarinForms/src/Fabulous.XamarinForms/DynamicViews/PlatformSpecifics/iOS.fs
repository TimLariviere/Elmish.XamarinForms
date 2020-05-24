namespace Fabulous.XamarinForms.DynamicViews

open Xamarin.Forms.PlatformConfiguration.iOSSpecific
open Fabulous.DynamicViews
open Fabulous.XamarinForms.DynamicViews
open Fabulous.XamarinForms.DynamicViews.Attributes

[<AutoOpen>]
module iOSSpecifics =
    module ViewAttributes =
        let iOSUseSafeArea = Attributes.Scalar.property<Xamarin.Forms.Page, _> false (fun (v, t) -> Page.SetUseSafeArea(t, v))
    
    type ContentPage<'msg> with
        member inline x.UseSafeArea(?value: bool) =
            let attributes = PropertyNode (ViewAttributes.iOSUseSafeArea.Value(Option.defaultValue true value))::x.Attributes
            ContentPage<'msg>(attributes)