namespace Fabulous.XamarinForms.DynamicViews

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

type ComponentViewElement(init, update, view, mapCmdMsg) =
    interface IViewElement with
        member x.Create(dispatch) =
            Program.AsComponent.useCmdMsg init update view mapCmdMsg
            |> Program.AsComponent.run null
            
        member x.Update(programDefinition, prevOpt, target) =
            ()
            
        member x.TryKey with get() = ValueNone

