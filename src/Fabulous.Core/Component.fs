namespace Fabulous

open System

/// Component module - functions to manipulate program instances
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Component =
    /// Subscribe to external source of events.
    /// The subscription is called once - with the initial (or resumed) model, but can dispatch new messages at any time.
    let withSubscription (subscribe : 'model -> Cmd<'msg>) (definition: RunnerDefinition<'arg, 'model, 'msg>) =
        let sub model =
            Cmd.batch [ definition.subscribe model
                        subscribe model ]
        { definition with subscribe = sub }

    /// Trace all the updates to the console
    let withConsoleTrace (definition: RunnerDefinition<'arg, 'model, 'msg>) =
        let traceInit arg =
            try 
                let initModel,cmd = definition.init arg
                Console.WriteLine (sprintf "Initial model: %0A" initModel)
                initModel,cmd
            with e -> 
                Console.WriteLine (sprintf "Error in init function: %0A" e)
                reraise ()

        let traceUpdate msg model =
            Console.WriteLine (sprintf "Message: %0A" msg)
            try 
                let newModel,cmd = definition.update msg model
                Console.WriteLine (sprintf "Updated model: %0A" newModel)
                newModel,cmd
            with e -> 
                Console.WriteLine (sprintf "Error in model function: %0A" e)
                reraise ()

        let traceView model dispatch =
            Console.WriteLine (sprintf "View, model = %0A" model)
            try 
                let info = definition.view model dispatch
                Console.WriteLine (sprintf "View result: %0A" info)
                info
            with e -> 
                Console.WriteLine (sprintf "Error in view function: %0A" e)
                reraise ()
                
        { definition with
            init = traceInit 
            update = traceUpdate
            view = traceView }

    /// Trace all the messages as they update the model
    let withTrace trace (definition: RunnerDefinition<'arg, 'model, 'msg>) =
        { definition
            with update = fun msg model -> trace msg model; definition.update msg model}

    /// Handle dispatch loop exceptions
    let withErrorHandler onError (definition: RunnerDefinition<'arg, 'model, 'msg>) =
        { definition
            with onError = onError }

    /// Set debugging to true
    let withDebug definition = 
        { definition with debug = true }

    /// Set the canReuseView function
    let withCanReuseView canReuseView definition = 
        { definition with canReuseView = canReuseView }

    /// Set the syncDispatch function
    let withSyncDispatch syncDispatch definition = 
        { definition with syncDispatch = syncDispatch }

    /// Set the syncAction function
    let withSyncAction syncAction definition = 
        { definition with syncAction = syncAction }

