namespace Fabulous.Maui.Controls

open Microsoft.Maui
open Microsoft.Maui.Primitives

[<AbstractClass>]
type FabulousFrameworkElement() as this =
    inherit FabulousElement()
    
    override __.Handler = (this :> IFrameworkElement).Handler
    
    interface IFrameworkElement with
        member val Handler = null with get, set
        
        member this.AutomationId = this.State.GetValue("AutomationId", null)
        member this.BackgroundColor = this.State.GetValue("BackgroundColor", Color.Default)
        member this.DesiredSize = this.State.GetValue("DesiredSize", Size.Zero)
        member this.FlowDirection = this.State.GetValue("FlowDirection", FlowDirection.LeftToRight)
        member this.Frame = this.State.GetValue("Frame", Rectangle.Zero)
        member this.Height = this.State.GetValue("Height", 0.)
        member this.HorizontalLayoutAlignment = this.State.GetValue("HorizontalLayoutAlignment", LayoutAlignment.Start)
        member this.IsArrangeValid = this.State.GetValue("IsArrangeValid", true)
        member this.IsEnabled = this.State.GetValue("IsEnabled", true)
        member this.IsMeasureValid = this.State.GetValue("IsMeasureValid", true)
        member this.Parent = this.State.GetValue("Parent", null)
        member this.Semantics = this.State.GetValue("Semantics", null)
        member this.VerticalLayoutAlignment = this.State.GetValue("VerticalLayoutAlignment", LayoutAlignment.Start)
        member this.Width = this.State.GetValue("Width", 0.)
        member this.Arrange(bounds) = ()
        member this.Measure(widthConstraint, heightConstraint) = (this :> IFrameworkElement).Handler.GetDesiredSize(widthConstraint, heightConstraint)
        member this.InvalidateArrange() = ()
        member this.InvalidateMeasure() = ()

