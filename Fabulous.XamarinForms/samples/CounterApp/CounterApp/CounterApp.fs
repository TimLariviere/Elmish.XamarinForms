// Copyright Fabulous contributors. See LICENSE.md for license.
namespace CounterApp

open Xamarin.Forms

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.DynamicViews
open Fabulous.XamarinForms.DynamicViews.View

module App = 
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
            ])
        ).UseSafeArea()
             
    let runnerDefinition = Program.AsApplication.useCmd init update view

type CounterApp () as app = 
    inherit Application ()

    let runner =
        App.runnerDefinition
        |> Program.withConsoleTrace
        |> Program.AsApplication.run app