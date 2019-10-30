// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace iOS

open UIKit
open Foundation
open Xamarin.Forms
open Xamarin.Forms.Platform.iOS

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit FormsApplicationDelegate ()

    override this.FinishedLaunching (uiApp, options) =
        Forms.Init()
        let app = new AllControls.App()
        this.LoadApplication (app)

        base.FinishedLaunching(uiApp, options)

module Main =
    [<EntryPoint>]
    let main args =
        UIApplication.Main(args, null, "AppDelegate")
        0

