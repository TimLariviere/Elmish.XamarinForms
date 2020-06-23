namespace Fabulous.XamarinForms

open System.ComponentModel
open Fabulous
open Xamarin.Forms

type CustomContentPage() as this =
    inherit ContentPage()
    do Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, true)

[<AllowNullLiteral>]
type IViewElementHolder =
    inherit INotifyPropertyChanged
    abstract ViewElement : IViewElement

[<AllowNullLiteral>]
type ViewElementHolder(viewElement: IViewElement) =
    let ev = new Event<_,_>()
    let mutable data = viewElement
    
    interface IViewElementHolder with
        member x.ViewElement = data
        [<CLIEvent>] member x.PropertyChanged = ev.Publish
        
    member x.ViewElement
        with get() = data
        and set(value) =
            data <- value
            ev.Trigger(x, PropertyChangedEventArgs "ViewElement")