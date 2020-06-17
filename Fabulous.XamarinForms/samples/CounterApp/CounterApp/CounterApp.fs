﻿// Copyright Fabulous contributors. See LICENSE.md for license.
namespace CounterApp

open Xamarin.Forms

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.DynamicViews
open Fabulous.XamarinForms.DynamicViews.View

type IApplicationService =
    abstract member CloseApplication: unit -> unit

module App =
    type Model =
      { Count: int
        Step: int
        TimerOn: bool }

    type Msg =
        | Increment
        | Decrement
        | Reset
        | SetStep of int
        | TimerToggled of bool
        | TimedTick
        | CloseApp

    type CmdMsg =
        | TickTimer

    let timerCmd () =
        async { do! Async.Sleep 200
                return TimedTick }
        |> Cmd.ofAsyncMsg

    let mapCmdMsgToCmd cmdMsg =
        match cmdMsg with
        | TickTimer -> timerCmd()

    let initModel () = { Count = 0; Step = 1; TimerOn=false }

    let init () = initModel () , []

    let update msg model =
        match msg with
        | Increment -> { model with Count = model.Count + model.Step }, []
        | Decrement -> { model with Count = model.Count - model.Step }, []
        | Reset -> init ()
        | SetStep n -> { model with Step = n }, []
        | TimerToggled on -> { model with TimerOn = on }, (if on then [ TickTimer ] else [])
        | TimedTick -> if model.TimerOn then { model with Count = model.Count + model.Step }, [ TickTimer ] else model, []
        | CloseApp ->
            DependencyService.Get<IApplicationService>().CloseApplication()
            model, []
    
    let view (model: Model) =
        Application(
            ContentPage(
                StackLayout([
                    Label(sprintf "%d" model.Count)
                        .automationId("CountLabel")
                        .size(width = 200.)
                        .alignment(horizontal = LayoutOptions.Center)
                        .textAlignment(horizontal = TextAlignment.Center)
                        
                    Button("Increment", Increment)
                        .automationId("IncrementButton")
                    
                    Button("Decrement", Decrement)
                        .automationId("DecrementButton")
                    
                    StackLayout(orientation = StackOrientation.Horizontal, children = [
                        Label("Timer")
                            .style(
                                StyleFor.Label()
                                    .textColor(Color.Blue)
                                    .padding(15.)
                            )
                        
                        Switch(model.TimerOn, fun args -> TimerToggled args.Value)
                            .automationId("TimerSwitch")
                    ]).alignment(horizontal = LayoutOptions.Center)
                    
                    Slider(double model.Step, (fun args -> SetStep (int (args.NewValue + 0.5))), range = (0., 10.))
                        .automationId("StepSlider")
                        
                    Label(sprintf "Step size: %d" model.Step)
                        .automationId("StepSizeLabel")
                        .alignment(horizontal = LayoutOptions.Center)
                        
                    Button("Reset", Reset)
                        .isEnabled(model <> initModel())
                        .alignment(horizontal = LayoutOptions.Center)
                ]).padding(30.)
                  .alignment(vertical = LayoutOptions.Center)
            )
        ).menu(
            MainMenu([
                Menu([
                    MenuItem("Quit CounterApp", CloseApp)
                        .accelerator("cmd+q")
                ])
                Menu("Actions", [
                    MenuItem("Increment", Increment)
                    MenuItem("Decrement", Decrement)
                ])
            ])
        )
        
    let runnerDefinition = Program.AsApplication.useCmdMsg init update view mapCmdMsgToCmd

type CounterApp () as app = 
    inherit Application ()

    let _ =
        App.runnerDefinition
        |> Program.withConsoleTrace
        |> Program.AsApplication.run app