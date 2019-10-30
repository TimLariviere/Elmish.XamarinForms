// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace AllControls.Droid

open Android.App
open Android.Content.PM
open Android.OS
open Xamarin.Forms.Platform.Android

[<Activity (Label = "AllControls.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = (ConfigChanges.ScreenSize ||| ConfigChanges.Orientation))>]
type MainActivity() =
    inherit FormsApplicationActivity()
    
    override this.OnCreate (bundle: Bundle) =
        base.OnCreate (bundle)
        Xamarin.Forms.Forms.Init (this, bundle)

        let app = new AllControls.App()
        this.LoadApplication(app)