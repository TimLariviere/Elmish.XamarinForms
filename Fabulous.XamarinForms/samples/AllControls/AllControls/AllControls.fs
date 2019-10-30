// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace AllControls

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

type Model = { Test: int }

type Msg = MsgNone

module App = 
    let init () = 
        { Test = 0 }

    let update msg model =
        model

    let view (model: Model) dispatch =
        View.ContentPage(
            View.StackLayout([
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                View.Label()
                //View.Label() // Uncomment this line to trigger invalid IL
            ])
        )

    
type App () as app = 
    inherit Application ()
    
    let runner = 
        Program.mkSimple App.init App.update App.view
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app

    member __.Program = runner
