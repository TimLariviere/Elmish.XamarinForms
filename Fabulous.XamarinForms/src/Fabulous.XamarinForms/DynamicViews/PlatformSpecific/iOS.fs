namespace Fabulous.XamarinForms.DynamicViews

open Xamarin.Forms.PlatformConfiguration.iOSSpecific
open Fabulous.XamarinForms.DynamicViews
open Fabulous.XamarinForms.DynamicViews.Attributes

[<AutoOpen>]
module iOSSpecific =
    module ViewAttributes =
        let iOSUseSafeArea = Attributes.Scalar.property<Xamarin.Forms.Page, _> false (fun (v, t) -> Page.SetUseSafeArea(t, v))
    
    type ContentPage<'msg> with
        member inline x.useSafeAreaOniOS(?value: bool) =
            let properties = ViewAttributes.iOSUseSafeArea.Value(Option.defaultValue true value)::x.Properties
            ContentPage<'msg>(x.Events, properties)