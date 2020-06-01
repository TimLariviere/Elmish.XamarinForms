// Copyright Fabulous contributors. See LICENSE.md for license.
namespace CSharpCounterApp.Components

open Xamarin.Forms

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.DynamicViews.View
open Fabulous.XamarinForms.StaticViews
open Fabulous.XamarinForms.StaticViews.View

type DeleteButton<'msg>() as target =
    inherit Xamarin.Forms.Button(BackgroundColor = Color.Red)
    do
        target.SetBinding(Xamarin.Forms.Button.TextProperty, Binding("Text"))
        target.SetBinding(Xamarin.Forms.Button.CommandProperty, Binding("Clicked"))

module Counter = 
    type Model = 
      { Count: int }

    type Msg = 
        | Increment
        | Decrement
        | Reset

    let init () = { Count = 0 }

    let update msg model =
        match msg with
        | Increment -> { model with Count = model.Count + 1 }
        | Decrement -> { model with Count = model.Count - 1 }
        | Reset -> { model with Count = 0 }
        
    let view (model: Model) =
        StackLayout(spacing = 10., children = [
            Label("Fabulous Counter")
                .font(NamedSize.Header)
                .alignment(horizontal = LayoutOptions.Center)
                
            Label(sprintf "Count = %i" model.Count)
                .alignment(horizontal = LayoutOptions.Center, vertical = LayoutOptions.CenterAndExpand)
                
            Button("Increment", Increment)
            Button("Decrement", Decrement)
            StaticView(DeleteButton, fun dispatch -> {| Text = "Reset"; Clicked = Command.msg dispatch Reset |})
        ])
             
    let runnerDefinition = Program.AsComponent.simple init update view

[<AbstractClass>]
type FabulousCounterView () as view =
    inherit ContentView ()

    let runner =
        Counter.runnerDefinition
        |> Program.withConsoleTrace
        |> Program.AsComponent.withModelChanged(fun model -> view.Value <- model.Count)
        |> Program.AsComponent.run view
        
    abstract member Value : int with get, set
    
