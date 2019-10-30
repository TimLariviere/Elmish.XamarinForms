// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace iOS

open UIKit
open Foundation
open Xamarin.Forms
open Xamarin.Forms.Platform.iOS

[<Register ("AppDelegate")>]
type AppDelegate () =
    inherit FormsApplicationDelegate ()

    let mutable _app: AllControls.App option = None

    override this.FinishedLaunching (uiApp, options) =
        Xamarin.Forms.Forms.SetFlags([|"Shell_Experimental"; "CollectionView_Experimental"; "Visual_Experimental"|]);
        Forms.Init()
        let app = new AllControls.App()
        this.LoadApplication (app)
        _app <- Some app

        base.FinishedLaunching(uiApp, options)

module Main =
    [<EntryPoint>]
    let main args =
        UIApplication.Main(args, null, "AppDelegate")
        0

