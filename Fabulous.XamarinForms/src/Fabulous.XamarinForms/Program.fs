// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous
open System
open Fabulous.XamarinForms.DynamicViews
open Xamarin.Forms

type ContentViewHost(contentView: ContentView) =
    interface IHost with
        member __.GetRootView() =
            match contentView.Content with
            | null -> failwith "No root view"
            | rootView -> rootView :> obj 

        member __.SetRootView(rootView) =
            match rootView with
            | :? View as view -> contentView.Content <- view
            | _ -> failwithf "Incorrect model type: expected a View but got a %O" (rootView.GetType())

type ApplicationHost(app: Application) =
    interface IHost with
        member __.GetRootView() =
            match app.MainPage with
            | null -> failwith "No root view"
            | rootView -> rootView :> obj 

        member __.SetRootView(rootView) =
            match rootView with
            | :? Page as page -> app.MainPage <- page
            | _ -> failwithf "Incorrect model type: expected a page but got a %O" (rootView.GetType())

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
        
    module AsApplication =
        /// Typical component, new commands are produced by `init` and `update` along with the new state.
        let useCmd (init : 'arg -> 'model * Cmd<'msg>) (update : 'msg -> 'model -> 'model * Cmd<'msg>) (view : 'model -> #IPage<'msg>) =
            { init = init
              update = update
              view = (fun model -> unbox<IViewElement> (view model))
              canReuseView = ViewHelpers.canReuseView
              syncDispatch = syncDispatch
              syncAction = syncAction
              subscribe = fun _ -> Cmd.none
              debug = false
              onError = onError }

        /// Simple component that produces only new state with `init` and `update`.
        let simple (init : 'arg -> 'model) (update : 'msg -> 'model -> 'model) (view : 'model -> #IPage<'msg>) = 
            useCmd (fun arg -> init arg, Cmd.none) (fun msg model -> update msg model, Cmd.none) view

        /// Typical component, new commands are produced discriminated unions returned by `init` and `update` along with the new state.
        let useCmdMsg (init: 'arg -> 'model * 'cmdMsg list) (update: 'msg -> 'model -> 'model * 'cmdMsg list) (view: 'model -> #IPage<'msg>) (mapToCmd: 'cmdMsg -> Cmd<'msg>) =
            let convert = fun (model, cmdMsgs) -> model, (cmdMsgs |> List.map mapToCmd |> Cmd.batch)
            useCmd (fun arg -> init arg |> convert) (fun msg model -> update msg model |> convert) view
            
        /// Run the app with Fabulous.XamarinForms
        let runWith (app: Application) (arg: 'arg) (definition: RunnerDefinition<'arg, 'model, 'msg>) = 
            Runner(ApplicationHost(app), definition, arg)

        /// Run the app with Fabulous.XamarinForms
        let run (app: Application) (definition: RunnerDefinition<unit, 'model, 'msg>) = 
            runWith app () definition
                
    module AsComponent =
        /// Typical component, new commands are produced by `init` and `update` along with the new state.
        let useCmd (init : 'arg -> 'model * Cmd<'msg>) (update : 'msg -> 'model -> 'model * Cmd<'msg>) (view : 'model -> #IView<'msg>) =
            { init = init
              update = update
              view = (fun model -> unbox<IViewElement> (view model))
              canReuseView = ViewHelpers.canReuseView
              syncDispatch = syncDispatch
              syncAction = syncAction
              subscribe = fun _ -> Cmd.none
              debug = false
              onError = onError }

        /// Simple component that produces only new state with `init` and `update`.
        let simple (init : 'arg -> 'model) (update : 'msg -> 'model -> 'model) (view : 'model -> #IView<'msg>) = 
            useCmd (fun arg -> init arg, Cmd.none) (fun msg model -> update msg model, Cmd.none) view

        /// Typical component, new commands are produced discriminated unions returned by `init` and `update` along with the new state.
        let useCmdMsg (init: 'arg -> 'model * 'cmdMsg list) (update: 'msg -> 'model -> 'model * 'cmdMsg list) (view: 'model -> #IView<'msg>) (mapToCmd: 'cmdMsg -> Cmd<'msg>) =
            let convert = fun (model, cmdMsgs) -> model, (cmdMsgs |> List.map mapToCmd |> Cmd.batch)
            useCmd (fun arg -> init arg |> convert) (fun msg model -> update msg model |> convert) view
        
        /// Run the component with Fabulous.XamarinForms
        let runWith (contentView: ContentView) (arg: 'arg) (definition: RunnerDefinition<'arg, 'model, 'msg>) = 
            Runner(ContentViewHost(contentView), definition, arg)

        /// Run the component with Fabulous.XamarinForms
        let run (contentView: ContentView) (definition: RunnerDefinition<unit, 'model, 'msg>) = 
            runWith contentView () definition
            
        let withModelChanged (fn: 'model -> unit) (definition: RunnerDefinition<'arg, 'model, 'msg>) =
            { definition with
                update = (fun msg model ->
                    let newModel, newCmd = definition.update msg model
                    fn newModel
                    newModel, newCmd
                ) }