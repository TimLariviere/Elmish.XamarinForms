namespace Fabulous.XamarinForms

open Xamarin.Forms

type CustomContentPage() as this =
    inherit ContentPage()
    do Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, true)

