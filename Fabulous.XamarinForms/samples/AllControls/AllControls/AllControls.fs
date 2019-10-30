// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace AllControls

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module App = 
    let view model dispatch =
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
        Program.mkSimple ignore (fun _ m -> m) App.view
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app

    member __.Program = runner
