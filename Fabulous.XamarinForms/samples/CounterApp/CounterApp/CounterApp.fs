// Copyright Fabulous contributors. See LICENSE.md for license.
namespace CounterApp

open Xamarin.Forms

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.DynamicViews
open Fabulous.XamarinForms.DynamicViews.View
open Fabulous.XamarinForms.StaticViews

type DeleteButton<'msg>(text: string, clicked: 'msg) =
    inherit StaticView<Xamarin.Forms.Button, 'msg>(fun dispatch -> box {| Text = text; Clicked = Helpers.makeCommand (fun () -> dispatch clicked) |})
    override x.Create() =
        let target = Xamarin.Forms.Button(BackgroundColor = Color.Red)
        target.SetBinding(Xamarin.Forms.Button.TextProperty, Binding("Text"))
        target.SetBinding(Xamarin.Forms.Button.CommandProperty, Binding("Clicked"))
        target

module App = 
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
        ContentPage(
            StackLayout(spacing = 10., children = [
                Label("Fabulous CounterApp")
                    .font(NamedSize.Title)
                    .horizontalOptions(LayoutOptions.Center)
                    
                Label(sprintf "Count = %i" model.Count)
                    .horizontalOptions(LayoutOptions.Center)
                    .verticalOptions(LayoutOptions.CenterAndExpand)
                    
                Button("Increment", Increment)
                Button("Decrement", Decrement)
                DeleteButton("Reset", Reset)
            ])
        ).useSafeAreaOniOS()
             
    let runnerDefinition = Program.AsApplication.simple init update view

type CounterApp () as app = 
    inherit Application ()

    let runner =
        App.runnerDefinition
        |> Program.withConsoleTrace
        |> Program.AsApplication.run app