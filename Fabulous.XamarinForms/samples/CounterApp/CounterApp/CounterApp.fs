// Copyright Fabulous contributors. See LICENSE.md for license.
namespace CounterApp

open Xamarin.Forms

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.DynamicViews
open Fabulous.XamarinForms.DynamicViews.View

module App = 
    type Model = 
      { EntryValue: string
        Todos: string list }

    type Msg = 
        | EntryTextChanged of string
        | AddTodo
        | RemoveTodo of string

    let init () = { EntryValue = ""; Todos = [] } , []

    let update msg model =
        match msg with
        | EntryTextChanged newValue -> { model with EntryValue = newValue }, []
        | AddTodo -> { model with EntryValue = ""; Todos = model.EntryValue::model.Todos }, []
        | RemoveTodo todo -> { model with Todos = model.Todos |> List.except [todo] }, []
        
    let view (model: Model) =
        ContentPage(
            StackLayout(spacing = 10.) [
                Label("Fabulous Todo List")
                    .font(NamedSize.Title)
                    .textColor(Color.Blue)
                    .horizontalOptions(LayoutOptions.Center)
                    
                Grid (coldefs = [ Star; Absolute 50. ]) [
                    Entry(
                        text = model.EntryValue,
                        textChanged = fun args -> EntryTextChanged args.NewTextValue
                    )
                        
                    Button("Add", AddTodo)
                        .gridColumn(1)
                ]
                
                ListView() [
                    for todo in model.Todos ->
                        TextCell(todo)
                            .contextActions([
                                MenuItem("Delete", RemoveTodo todo)
                            ])
                ]
            ]
        )
             
    let runnerDefinition = Program.AsApplication.useCmd init update view

type CounterApp () as app = 
    inherit Application ()

    let runner =
        App.runnerDefinition
        |> Program.withConsoleTrace
        |> Program.AsApplication.run app