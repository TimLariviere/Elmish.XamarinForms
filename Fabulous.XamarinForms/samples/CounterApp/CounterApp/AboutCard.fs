namespace CounterApp

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.Component.View
open Fabulous.XamarinForms.DynamicViews.View

module AboutCard =
    type Model = { State: int; Text: string }
    
    type Msg =
        | StateChanged of int
        | Toggle
        
    type ExternalMsg =
        | TextChanged of string
    
    type CmdMsg = Nothing
    
    let mapCmdMsg cmdMsg =
        match cmdMsg with
        | Nothing -> Cmd.none
    
    let init() =
        { State = 0; Text = "Hello World!" }, [], []
    
    let update msg model =
        match msg with
        | StateChanged state ->
            { model with State = state }, [], []
        | Toggle ->
            let newModel = { model with Text = model.Text +  " It worked!" }
            newModel, [], [ TextChanged newModel.Text ]
        
    let view model =
        StackLayout([
            Button("Toggle", Toggle)
            Label(sprintf "State is %i" model.State)
            Label(model.Text)
        ])

    let program = Program.forComponentWithCmdMsg init update view mapCmdMsg
    
    let asComponent<'msg>(state, onExternalMsg: ExternalMsg -> 'msg) =
        ComponentView(program)
            //.withState(StateChanged, state)
            .withExternalMessages(onExternalMsg)
