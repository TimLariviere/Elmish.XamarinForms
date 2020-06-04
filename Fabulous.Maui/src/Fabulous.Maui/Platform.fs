namespace Fabulous.Maui

open Fabulous.Maui
open System.Maui

module Platform =
    let init() =
        Registrar.Handlers.Register<Fabulous.Maui.FabulousButton, System.Maui.Platform.ButtonRenderer>();
        

