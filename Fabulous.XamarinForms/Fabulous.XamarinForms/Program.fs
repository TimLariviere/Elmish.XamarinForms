// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Component
open Fabulous
open System
open Xamarin.Forms

/// Component module - functions to manipulate component instances
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Program =
    let internal onError (text: string, ex: exn) = 
        Console.WriteLine (sprintf "%s: %A" text ex)
        
    let private syncDispatch (dispatch: 'msg -> unit) =
        fun msg ->
            Device.BeginInvokeOnMainThread(fun () -> dispatch msg)
            
    let private syncAction (fn: unit -> unit) =
        fun () ->
            Device.BeginInvokeOnMainThread (fun () -> fn())
            
    let rec canReuseView (prevChild: IViewElement) (newChild: IViewElement) =
        match prevChild, newChild with
        | (:? DynamicViewElement as prevChild), (:? DynamicViewElement as newChild) -> DynamicViews.ViewHelpers.canReuseDynamicView prevChild newChild
        | (:? IComponentViewElement as prevChild), (:? IComponentViewElement as newChild) -> prevChild.TargetType = newChild.TargetType
        | _ -> false
    
    /// Typical component, new commands are produced by `init` and `update` along with the new state.
    let useCmd (init : 'arg -> 'model * Cmd<'msg>) (update : 'msg -> 'model -> 'model * Cmd<'msg>) (view : 'model -> #IViewElementBuilder<'msg>) =
        { init = init
          update = update
          view = (fun model -> (view model).AsViewElement())
          canReuseView = canReuseView
          syncDispatch = syncDispatch
          syncAction = syncAction
          subscribe = fun _ -> Cmd.none
          debug = false
          onError = onError }

    /// Simple component that produces only new state with `init` and `update`.
    let simple (init : 'arg -> 'model) (update : 'msg -> 'model -> 'model) (view : 'model -> #IViewElementBuilder<'msg>) = 
        useCmd (fun arg -> init arg, Cmd.none) (fun msg model -> update msg model, Cmd.none) view

    /// Typical component, new commands are produced discriminated unions returned by `init` and `update` along with the new state.
    let useCmdMsg (init: 'arg -> 'model * 'cmdMsg list) (update: 'msg -> 'model -> 'model * 'cmdMsg list) (view: 'model -> #IViewElementBuilder<'msg>) (mapToCmd: 'cmdMsg -> Cmd<'msg>) =
        let convert = fun (model, cmdMsgs) -> model, (cmdMsgs |> List.map mapToCmd |> Cmd.batch)
        useCmd (fun arg -> init arg |> convert) (fun msg model -> update msg model |> convert) view
        
    /// Run the app with Fabulous.XamarinForms
    let runWith (element: Element) (arg: 'arg) (definition: RunnerDefinition<'arg, 'msg, 'model>) =
        let runner = Runner()
        let _ = runner.Start(definition, arg, Some (box element))
        runner

    /// Run the app with Fabulous.XamarinForms
    let run (element: Element) (definition: RunnerDefinition<unit, 'msg, 'model>) = 
        runWith element () definition