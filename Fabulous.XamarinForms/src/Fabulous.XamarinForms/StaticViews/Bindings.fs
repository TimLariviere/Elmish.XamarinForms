namespace Fabulous.XamarinForms.StaticViews

open Fabulous.XamarinForms

module Command =
    let msg dispatch (msg: 'msg) =
        Helpers.makeCommand (fun () -> dispatch msg)
