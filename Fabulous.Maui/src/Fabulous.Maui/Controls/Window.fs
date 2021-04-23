namespace Fabulous.Maui.Controls

open Microsoft.Maui

type FabulousWindow() =
    inherit FabulousBaseElement()
    
    interface IWindow with
        member val MauiContext = null with get, set
        member this.Page
            with get () = this.State.GetValue("Page", null)
            and set _value = failwith "Invalid setter usage"
            