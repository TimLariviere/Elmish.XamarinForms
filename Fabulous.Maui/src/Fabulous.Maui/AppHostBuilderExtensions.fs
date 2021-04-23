namespace Fabulous.Maui

open System.Collections.Generic
open System.Runtime.CompilerServices
open Fabulous
open Microsoft.Maui
open Microsoft.Maui.Handlers
open Microsoft.Maui.Hosting
open Fabulous.Maui.Controls
open System
open Microsoft.Extensions.DependencyInjection

type FabulousApp<'model, 'msg>(program: RunnerDefinition<unit, 'model, 'msg, unit>) =
    let mutable _runner = Unchecked.defaultof<_>
    interface IApplication with
        member __.CreateWindow(activationState) =
            let runner, target = MauiProgram.run program
            _runner <- runner
            target :?> IWindow

[<Extension>]
type AppHostBuilderExtensions () =
    static let DefaultFabulousHandlers =
        let dict = Dictionary<Type,Type>()
        dict.Add(typeof<FabulousLabel>, typeof<LabelHandler>)
        dict.Add(typeof<FabulousButton>, typeof<ButtonHandler>)
        dict
    
    [<Extension>]
    static member UseFabulousApp(appHostBuilder: IAppHostBuilder, program: RunnerDefinition<unit, 'model, 'msg, unit>) =
        appHostBuilder
            .ConfigureMauiHandlers(fun handlersCollection ->
                handlersCollection.AddHandlers(DefaultFabulousHandlers) |> ignore
            )
            .ConfigureServices(fun (collection: IServiceCollection) ->
                collection.AddSingleton<IApplication>(fun _ -> FabulousApp(program) :> IApplication) |> ignore
            )

