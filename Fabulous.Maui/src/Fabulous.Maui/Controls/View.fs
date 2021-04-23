namespace Fabulous.Maui.Controls

open Microsoft.Maui

[<AbstractClass>]
type FabulousView() =
    inherit FabulousFrameworkElement()
    
    interface IView with
        member this.Margin = this.State.GetValue("Margin", Thickness.Zero)
    

