namespace Fabulous.Maui.Controls

open Microsoft.Maui

type FabulousButton() =
    inherit FabulousView()
    
    interface IButton with
        member this.CharacterSpacing = this.State.GetValue("CharacterSpacing", 0.)
        member this.Font = this.State.GetValue("Font", Font.Default)
        member this.Padding = this.State.GetValue("Padding", Thickness.Zero)
        member this.Text = this.State.GetValue("Text", null)
        member this.TextColor = this.State.GetValue("TextColor", Color.Default)
        
        
        member this.Clicked() = this.State.GetValue("Clicked", ignore)()
        member this.Pressed() = this.State.GetValue("Pressed", ignore)()
        member this.Released() = this.State.GetValue("Released", ignore)()

