namespace Fabulous.Maui

open Microsoft.Maui

module Compat =
    type FabContentPage() =
        inherit Microsoft.Maui.Controls.ContentPage()
        member __.State = State()
        interface IPage with
            member this.View
                with get () = this.State.GetValue("View", null)
                and set _value = failwith "Invalid setter usage"
                
    type FabStackLayout() =
        inherit Microsoft.Maui.Controls.StackLayout()
        member __.State = State()