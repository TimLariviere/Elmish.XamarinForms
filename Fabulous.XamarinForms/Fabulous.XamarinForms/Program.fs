// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous
open System
open Xamarin.Forms

type ContentViewHost(contentView: ContentView) =
    interface IHost with
        member __.GetRoot() =
            match contentView.Content with
            | null -> failwith "No root view"
            | rootView -> rootView :> obj 

        member __.InitRoot(rootElement, programDefinition) =
            contentView.Content <- rootElement.Create(programDefinition) :?> View

type ApplicationHost(app: Application) =
    interface IHost with
        member __.GetRoot() = app :> obj
        member __.InitRoot(rootElement, programDefinition) =
            rootElement.Update(programDefinition, ValueNone, app)

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
        | _ -> false
        
    module AsApplication =
        /// Typical component, new commands are produced by `init` and `update` along with the new state.
        let useCmd (init : 'arg -> 'model * Cmd<'msg>) (update : 'msg -> 'model -> 'model * Cmd<'msg>) (view : 'model -> #IApplication<'msg>) =
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
        let simple (init : 'arg -> 'model) (update : 'msg -> 'model -> 'model) (view : 'model -> #IApplication<'msg>) = 
            useCmd (fun arg -> init arg, Cmd.none) (fun msg model -> update msg model, Cmd.none) view

        /// Typical component, new commands are produced discriminated unions returned by `init` and `update` along with the new state.
        let useCmdMsg (init: 'arg -> 'model * 'cmdMsg list) (update: 'msg -> 'model -> 'model * 'cmdMsg list) (view: 'model -> #IApplication<'msg>) (mapToCmd: 'cmdMsg -> Cmd<'msg>) =
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
              view = (fun model -> unbox<IViewElement> ((view model).AsViewElement()))
              canReuseView = canReuseView
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