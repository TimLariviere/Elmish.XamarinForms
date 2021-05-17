// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

open System

/// Program module - functions to manipulate program instances
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Program =
    /// Subscribe to external source of events.
    /// The subscription is called once - with the initial (or resumed) model, but can dispatch new messages at any time.
    let withSubscription (subscribe : 'model -> Dispatch<'msg> -> IDisposable) (definition: RunnerDefinition<'arg, 'msg, 'model, 'externalMsg>) =
        let sub model dispatch =
            let x = definition.subscribe model dispatch
            let y = subscribe model dispatch
            { new IDisposable with
                member _.Dispose() =
                    if x <> null then x.Dispose()
                    if y <> null then y.Dispose() }
            
        { definition with subscribe = sub }

    /// Trace all the updates to the console
    let traceMvu (definition: RunnerDefinition<'arg, 'msg, 'model, 'externalMsg>) =
        let traceInit arg =
            try
                let initModel,cmd,externalMsgs = definition.init arg
                definition.trace TraceLevel.Info (String.Format("Initial model: {0}", initModel))
                initModel,cmd,externalMsgs
            with ex ->
                definition.trace TraceLevel.Error (String.Format("Error in init function: {0}", ex))
                reraise ()

        let traceUpdate (msg: 'msg) (model: 'model) =
            definition.trace TraceLevel.Info (String.Format("Message: {0}", msg))
            try
                let newModel,cmd,externalMsgs = definition.update msg model
                definition.trace TraceLevel.Info (String.Format("Updated model: {0}", newModel))
                newModel,cmd,externalMsgs
            with e ->
                definition.trace TraceLevel.Error (String.Format("Error in model function: {0}", e))
                reraise ()

        let traceView (model: 'model) =
            definition.trace TraceLevel.Info (String.Format("View, model = {0}", model))
            try
                let info = definition.view model
                definition.trace TraceLevel.Info (String.Format("View result: {0}", info))
                info
            with e ->
                definition.trace TraceLevel.Error (String.Format("Error in view function: {0}", e))
                reraise ()

        { definition with
            init = traceInit
            update = traceUpdate
            view = traceView }

    /// Set the trace level
    let withTraceLevel traceLevel definition : RunnerDefinition<_, _, _, _> =
        { definition with traceLevel = traceLevel }

    /// Set the trace function
    let withTrace trace definition : RunnerDefinition<_, _, _, _> =
        { definition with trace = trace }

    // Trace all logs to the console
    let withConsoleTrace definition : RunnerDefinition<_, _, _, _> =
        let traceFn traceLevel str =
            let currentColor = Console.ForegroundColor
            let newColor =
                match traceLevel with
                | TraceLevel.Debug -> ConsoleColor.Green
                | TraceLevel.Error -> ConsoleColor.Red
                | _ -> currentColor

            Console.ForegroundColor <- newColor
            Console.WriteLine("[{0}] - Fabulous - {1}", traceLevel, str)
            Console.ForegroundColor <- currentColor

        { definition with trace = traceFn }

    /// Set the canReuseView function
    let withCanReuseView canReuseView definition : RunnerDefinition<_, _, _, _> =
        { definition with canReuseView = canReuseView }

    /// Set the syncDispatch function
    let withSyncDispatch syncDispatch definition =
        { definition with syncDispatch = syncDispatch }

    /// Set the syncAction function
    let withSyncAction syncAction definition =
        { definition with syncAction = syncAction }