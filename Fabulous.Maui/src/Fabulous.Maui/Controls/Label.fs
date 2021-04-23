namespace Fabulous.Maui.Controls

open Microsoft.Maui

type FabulousLabel() =
    inherit FabulousView()
    
    interface ILabel with
        member this.CharacterSpacing = this.State.GetValue("CharacterSpacing", 0.)
        member this.Font = this.State.GetValue("Font", Font.Default)
        member this.HorizontalTextAlignment = this.State.GetValue("HorizontalTextAlignment", TextAlignment.Start)
        member this.LineBreakMode = this.State.GetValue("LineBreakMode", LineBreakMode.NoWrap)
        member this.LineHeight = this.State.GetValue("LineHeight", 0.)
        member this.MaxLines = this.State.GetValue("MaxLines", 0)
        member this.Padding = this.State.GetValue("Padding", Thickness.Zero)
        member this.Text = this.State.GetValue("Text", null)
        member this.TextColor = this.State.GetValue("TextColor", Color.Default)
        member this.TextDecorations = this.State.GetValue("TextDecorations", TextDecorations.None)

