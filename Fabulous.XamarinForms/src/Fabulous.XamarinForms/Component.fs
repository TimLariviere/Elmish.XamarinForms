namespace Fabulous.XamarinForms

open Fabulous
open Xamarin.Forms
open System

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

/// Component module - functions to manipulate component instances
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Component =
    let internal onError (text: string, ex: exn) = 
        Console.WriteLine (sprintf "%s: %A" text ex)

    /// Typical component, new commands are produced by `init` and `update` along with the new state.
    let useCmd (init : 'arg -> 'model * Cmd<'msg>) (update : 'msg -> 'model -> 'model * Cmd<'msg>) (view : 'model -> Dispatch<'msg> -> #IBindableObject) =
        { init = init
          update = update
          view = (fun model dispatch -> unbox<ViewElement> (view model dispatch))
          canReuseView = fun _ _ -> false
          syncDispatch = id
          syncAction = id
          subscribe = fun _ -> Cmd.none
          debug = false
          onError = onError }

    /// Simple component that produces only new state with `init` and `update`.
    let simple (init : 'arg -> 'model) (update : 'msg -> 'model -> 'model) (view : 'model -> Dispatch<'msg> -> #IBindableObject) = 
        useCmd (fun arg -> init arg, Cmd.none) (fun msg model -> update msg model, Cmd.none) view

    /// Typical component, new commands are produced discriminated unions returned by `init` and `update` along with the new state.
    let useCmdMsg (init: 'arg -> 'model * 'cmdMsg list) (update: 'msg -> 'model -> 'model * 'cmdMsg list) (view: 'model -> Dispatch<'msg> -> #IBindableObject) (mapToCmd: 'cmdMsg -> Cmd<'msg>) =
        let convert = fun (model, cmdMsgs) -> model, (cmdMsgs |> List.map mapToCmd |> Cmd.batch)
        useCmd (fun arg -> init arg |> convert) (fun msg model -> update msg model |> convert) view
        
    /// Run the app with Fabulous.XamarinForms
    let runAsApplicationWith (app: Application) (arg: 'arg) (definition: RunnerDefinition<'arg, 'model, 'msg>) = 
        Runner(ApplicationHost(app), definition, arg)

    /// Run the app with Fabulous.XamarinForms
    let runAsApplication (app: Application) (definition: RunnerDefinition<unit, 'model, 'msg>) = 
        runAsApplicationWith app () definition
        
    /// Run the component with Fabulous.XamarinForms
    let runWith (contentView: ContentView) (arg: 'arg) (definition: RunnerDefinition<'arg, 'model, 'msg>) = 
        Runner(ContentViewHost(contentView), definition, arg)

    /// Run the component with Fabulous.XamarinForms
    let run (contentView: ContentView) (definition: RunnerDefinition<unit, 'model, 'msg>) = 
        runWith contentView () definition