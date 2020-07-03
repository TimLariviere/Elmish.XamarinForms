namespace CounterApp

open Fabulous.XamarinForms
open Fabulous.XamarinForms.Component.View
open Fabulous.XamarinForms.DynamicViews.View

module AboutCard =
    type Model = { Text: string }
    
    type Msg = Toggle
    
    let init() = { Text = "Hello World!" }
    
    let update msg model =
        match msg with
        | Toggle -> { model with Text = model.Text +  " It worked!" }
        
    let view model =
        StackLayout([
            Button("Toggle", Toggle)
            Label(model.Text)
        ])

    let program = Program.simple init update view
    
    let asComponent() =
        ComponentView(program, ())
