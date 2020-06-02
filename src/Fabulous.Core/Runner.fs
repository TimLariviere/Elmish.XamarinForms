// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

open System.Diagnostics

/// Representation of the host framework with access to the root view to update (e.g. Xamarin.Forms.Application)
type IHost =
    /// Gets a reference to the root view item (e.g. Xamarin.Forms.Application.MainPage)
    abstract member GetRootView : unit -> obj
    /// Sets a new instance of the root view item (e.g. Xamarin.Forms.Application.MainPage)
    abstract member SetRootView : obj -> unit
    
/// We store the current dispatch function for the running Elmish program as a 
/// static-global thunk because we want old view elements stored in the `dependsOn` global table
/// to be recyclable on resumption (when a new ProgramRunner gets created).
type RunnerDispatch<'msg>()  = 
    let mutable dispatchImpl = (fun (_msg: 'msg) -> failwith "do not call dispatch during initialization" : unit)

    member x.DispatchViaThunk =
        id (fun msg -> dispatchImpl msg)
 
    member x.SetDispatchThunk v = dispatchImpl <- v

/// Program type captures various aspects of program behavior
type RunnerDefinition<'arg, 'model, 'msg> = 
    { init : 'arg -> 'model * Cmd<'msg>
      update : 'msg -> 'model -> 'model * Cmd<'msg>
      subscribe : 'model -> Cmd<'msg>
      view : 'model -> IViewElement
      canReuseView: IViewElement -> IViewElement -> bool
      syncDispatch: Dispatch<'msg> -> Dispatch<'msg>
      syncAction: (unit -> unit) -> (unit -> unit)
      debug : bool
      onError : (string * exn) -> unit }

/// Starts the Elmish dispatch loop for the page with the given Elmish program
type Runner<'arg, 'model, 'msg>(host: IHost, definition: RunnerDefinition<'arg, 'model, 'msg>, arg: 'arg) = 

    do Debug.WriteLine "run: computing initial model"

    // Get the initial model
    let (initialModel,cmd) = definition.init arg
    let mutable alternativeRunner : Runner<obj,obj,obj> option = None

    let mutable lastModel = initialModel
    let mutable lastViewDataOpt = None
    let mutable dispatch = RunnerDispatch<'msg>()
    let programDefinition = { CanReuseView = definition.canReuseView; Dispatch = (fun msg -> dispatch.DispatchViaThunk (unbox msg)) }
    let mutable reset = (fun () -> ())

    // Start Elmish dispatch loop
    let rec processMsg msg = 
        try
            let (updatedModel,newCommands) = definition.update msg lastModel
            lastModel <- updatedModel
            try 
                updateView updatedModel 
            with ex ->
                definition.onError ("Unable to update view:", ex)
            for sub in newCommands do
                try 
                    sub dispatch.DispatchViaThunk
                with ex ->
                    definition.onError ("Error executing commands:", ex)
        with ex ->
            definition.onError ("Unable to process a message:", ex)

    and updateView updatedModel = 
        match lastViewDataOpt with
        | None -> ()
        | Some prevPageElement ->
            let newPageElement = 
                try definition.view updatedModel
                with ex -> 
                    definition.onError ("Unable to evaluate view:", ex)
                    prevPageElement

            if definition.canReuseView prevPageElement newPageElement then
                let rootView = host.GetRootView()
                newPageElement.Update(programDefinition, ValueSome prevPageElement, rootView)
            else
                let pageObj = newPageElement.Create(programDefinition)
                host.SetRootView(pageObj)

            lastViewDataOpt <- Some newPageElement
                      
    do 
        // Set up the global dispatch function
        dispatch.SetDispatchThunk(processMsg |> definition.syncDispatch)
        reset <- (fun () -> updateView lastModel) |> definition.syncAction
        
        Debug.WriteLine "updating the initial view"
        
        // If the view is dynamic, create the initial page
        lastViewDataOpt <-
            let newRootElement = definition.view initialModel
            let rootView = newRootElement.Create(programDefinition)
            host.SetRootView(rootView)
            Some newRootElement

        Debug.WriteLine "dispatching initial commands"
        for sub in (definition.subscribe initialModel @ cmd) do
            try 
                sub dispatch.DispatchViaThunk
            with ex ->
                definition.onError ("Error executing commands:", ex)

    member __.Argument = arg
    
    member __.CurrentModel = lastModel 

    member __.Dispatch(msg) = dispatch.DispatchViaThunk msg

    member runner.ChangeDefinition(newDefinition: RunnerDefinition<obj,obj, obj>) : unit =
        let action = definition.syncAction (fun () -> 
            // TODO: transmogrify the model
            try
                alternativeRunner <- Some (Runner<obj,obj, obj>(host, newDefinition, runner.Argument))
            with ex ->
                definition.onError ("Error changing the program:", ex)
        )
        action()

    member __.ResetView() : unit  =
        let action = definition.syncAction (fun () -> 
            match alternativeRunner with 
            | Some r -> r.ResetView()
            | None -> reset()
        )
        action()

    /// Set the current model, e.g. on resume
    member __.SetCurrentModel(model, cmd: Cmd<_>) =
        let action = definition.syncAction (fun () -> 
            match alternativeRunner with 
            | Some _ -> 
                // TODO: transmogrify the resurrected model
                printfn "SetCurrentModel: ignoring (can't the model after ChangeProgram has been called)"
            | None -> 
                Debug.WriteLine "updating the view after setting the model"
                lastModel <- model
                updateView model
                for sub in definition.subscribe model @ cmd do
                    sub dispatch.DispatchViaThunk
        )
        action()