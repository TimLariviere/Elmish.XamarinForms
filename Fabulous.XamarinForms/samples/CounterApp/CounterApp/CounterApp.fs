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
        
    let view (model: Model) dispatch =
        ContentPage(
            StackLayout(spacing = 10.) [
                Label("Fabulous Todo List")
                    .font(NamedSize.Title)
                    .textColor(Color.Blue)
                    .horizontalOptions(LayoutOptions.Center)
                    
                Grid (coldefs = [ Star; Absolute 50. ]) [
                    Entry(
                        text = model.EntryValue,
                        textChanged = fun args -> dispatch (EntryTextChanged args.NewTextValue)
                    )
                        
                    Button("Add", fun () -> dispatch AddTodo)
                        .gridColumn(1)
                ]
                
                ListView(model.Todos) (fun item ->
                    TextCell(item)
                        .contextActions([
                            MenuItem("Delete", fun() -> dispatch (RemoveTodo item))
                        ])
                )
            ]
        )
             
    let runnerDefinition = Component.useCmd init update view

type CounterApp () as app = 
    inherit Application ()

    let runner =
        App.runnerDefinition
        |> Component.withConsoleTrace
        |> Component.runAsApplication app