namespace CounterApp.Fabulous

open Microsoft.Maui
open Fabulous.Maui
open type View

module App =
    type Model = { Count: int }
    type Msg = Increment
    
    let init () =
        { Count = 0 }
        
    let update msg model =
        match msg with
        | Increment -> { model with Count = model.Count + 1 }
        
    let view model dispatch =
        Window(
            Page(
                Label($"Count = {model.Count}")
                    .horizontalTextAlignment(TextAlignment.Center)
                    .Build()
            ).Build()
        ).Build()
        
    let program =
        MauiProgram.mkSimple init update view


