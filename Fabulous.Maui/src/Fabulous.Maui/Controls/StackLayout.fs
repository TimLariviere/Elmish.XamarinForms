namespace Fabulous.Maui.Controls

open Microsoft.Maui

type FabulousStackLayout() =
    inherit FabulousLayout()
    
    interface IStackLayout with
        member this.Spacing = this.State.GetValue("Spacing", 0)

