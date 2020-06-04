namespace Fabulous.Maui

open Fabulous
open System.Collections.Generic
open System.Maui
open System.Linq

type FabulousFrameworkElement(data: IReadOnlyDictionary<string, obj>, programDefinition: ProgramDefinition) =
    let mutable _data = data
    let mutable _programDefinition = programDefinition
    
    member x.GetValueOrDefault(key: string, defaultValue: 'T) =
        match data.TryGetValue(key) with
        | false, _ -> defaultValue
        | true, value -> value :?> 'T
        
    member x.TryExecute(key: string, args: 'T) =
        match data.TryGetValue(key) with
        | false, _ -> ()
        | true, value -> (value :?> 'T -> obj) args |> programDefinition.Dispatch
        
    member x.Update(newData: IReadOnlyDictionary<string, obj>, programDefinition: ProgramDefinition) =
        _programDefinition <- programDefinition
        let prevData = _data
        _data <- newData
        
        match (x :> IFrameworkElement).Renderer with
        | null -> ()
        | renderer ->
            for key in prevData.Keys.Union(newData.Keys) do
                match prevData.TryGetValue(key), newData.TryGetValue(key) with
                | (true, prev), (true, curr) when prev = curr -> ()
                | _ -> renderer.UpdateValue(key)
    
    interface IFrameworkElement with
        member val Renderer: IViewRenderer = null with get, set
        member x.Arrange(bounds) = failwithf "Not implemented"
        member x.BackgroundColor = x.GetValueOrDefault("BackgroundColor", Color.Default)
        member x.DesiredSize = failwithf "Not implemented"
        member x.Frame = failwithf "Not implemented"
        member x.InvalidateArrange() = failwithf "Not implemented"
        member x.InvalidateMeasure() = failwithf "Not implemented"
        member x.IsArrangeValid = failwithf "Not implemented"
        member x.IsEnabled = x.GetValueOrDefault("IsEnabled", true)
        member x.IsMeasureValid = failwithf "Not implemented"
        member x.Measure(widthConstraint, heightConstraint) = failwithf "Not implemented"
        member x.Parent = failwithf "Not implemented"

type FabulousView(data, dispatch) =
    inherit FabulousFrameworkElement(data, dispatch)
    interface IView with
        member x.GetHorizontalAlignment(layout) = failwithf "Not implemented"
        member x.GetVerticalAlignment(layout) = failwithf "Not implemented"

type FabulousButton(data, dispatch) =
    inherit FabulousView(data, dispatch)
    interface IButton with
        member x.Clicked() = x.TryExecute("Clicked", ())
        member x.Color = x.GetValueOrDefault("Color", Color.Default)
        member x.Text = x.GetValueOrDefault("Text", "")
        member x.TextType = x.GetValueOrDefault("TextType", TextType.Text)
