namespace Fabulous.XamarinForms.DynamicViews

open Xamarin.Forms.PlatformConfiguration.iOSSpecific
open Fabulous.XamarinForms.DynamicViews
open Fabulous.XamarinForms.DynamicViews.Attributes

[<AutoOpen>]
module iOSSpecific =
    module ViewAttributes =
        let iOSIgnoreSafeArea = Attributes.Scalar.property<Xamarin.Forms.Page, _> "iOSIgnoreSafeArea" false (fun (v, t) -> Page.SetUseSafeArea(t, not v))
    
    type ContentPage<'msg> with
        member inline x.ignoreSafeArea(?value: bool) =
            let properties = ViewAttributes.iOSIgnoreSafeArea.Value(Option.defaultValue true value)::x.Properties
            ContentPage<'msg>(x.Events, properties)