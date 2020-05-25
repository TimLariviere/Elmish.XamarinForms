// Copyright Fabulous contributors. See LICENSE.md for license.
namespace CSharpCounterApp.Components

open Xamarin.Forms

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.DynamicViews.View

module Counter = 
    type Model = 
      { Count: int }

    type Msg = 
        | Increment
        | Decrement

    let init () = { Count = 0 }, []

    let update msg model =
        match msg with
        | Increment -> { model with Count = model.Count + 1 }, []
        | Decrement -> { model with Count = model.Count - 1 }, []
        
    let view (model: Model) =
        StackLayout(spacing = 10., children = [
            Label("Fabulous Counter")
                .font(NamedSize.Header)
                .horizontalOptions(LayoutOptions.Center)
                
            Label(sprintf "Count = %i" model.Count)
                .horizontalOptions(LayoutOptions.Center)
                .verticalOptions(LayoutOptions.CenterAndExpand)
                
            Button("Increment", Increment)
            Button("Decrement", Decrement)
        ])
             
    let runnerDefinition = Program.AsComponent.useCmd init update view

[<AbstractClass>]
type FabulousCounterView () as view =
    inherit ContentView ()

    let runner =
        Counter.runnerDefinition
        |> Program.withConsoleTrace
        |> Program.AsComponent.withModelChanged(fun model -> view.Value <- model.Count)
        |> Program.AsComponent.run view
        
    abstract member Value : int with get, set