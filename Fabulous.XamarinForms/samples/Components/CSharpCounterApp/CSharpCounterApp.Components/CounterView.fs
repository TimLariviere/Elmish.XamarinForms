// Copyright Fabulous contributors. See LICENSE.md for license.
namespace CSharpCounterApp.Components

open Xamarin.Forms

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.DynamicViews.View
open Fabulous.XamarinForms.StaticViews

type DeleteButton<'msg>(text: string, clicked: 'msg) =
    inherit StaticView<Xamarin.Forms.Button, 'msg>(fun dispatch -> box {| Text = text; Clicked = Helpers.makeCommand (fun () -> dispatch clicked) |})
    override x.Create() =
        let target = Xamarin.Forms.Button(BackgroundColor = Color.Red)
        target.SetBinding(Xamarin.Forms.Button.TextProperty, Binding("Text"))
        target.SetBinding(Xamarin.Forms.Button.CommandProperty, Binding("Clicked"))
        target

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
                .horizontalOptions(LayoutOptions.Center)
                
            Label(sprintf "Count = %i" model.Count)
                .horizontalOptions(LayoutOptions.Center)
                .verticalOptions(LayoutOptions.CenterAndExpand)
                
            Button("Increment", Increment)
            Button("Decrement", Decrement)
            DeleteButton("Reset", Reset)
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