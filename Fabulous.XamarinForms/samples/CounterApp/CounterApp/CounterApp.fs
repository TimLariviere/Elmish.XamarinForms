// Copyright Fabulous contributors. See LICENSE.md for license.
namespace CounterApp

open Xamarin.Forms

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.DynamicViews
open Fabulous.XamarinForms.DynamicViews.View
open Fabulous.XamarinForms.StaticViews

type MyDeleteButton() as target =
    inherit Xamarin.Forms.Button()
    do
        target.Visual <- VisualMarker.Material
        target.SetBinding(Xamarin.Forms.Button.TextProperty, Binding("Text"))
        target.SetBinding(Xamarin.Forms.Button.CommandProperty, Binding("Clicked"))
        
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
                
                Static.View(MyDeleteButton, fun dispatch -> {| Text = "Reset"; Clicked = Command.msg dispatch Reset |})
                    .horizontalOptions(LayoutOptions.Center)
                    
                Static.View(MyDeleteButton)
                    .horizontalOptions(LayoutOptions.Center)
            ])
        ).useSafeAreaOniOS()
             
    let runnerDefinition = Program.AsApplication.simple init update view

type CounterApp () as app = 
    inherit Application ()

    let runner =
        App.runnerDefinition
        |> Program.withConsoleTrace
        |> Program.AsApplication.run app