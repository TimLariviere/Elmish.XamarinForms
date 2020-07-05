// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous
    
/// We store the current dispatch function for the running Elmish program as a 
/// static-global thunk because we want old view elements stored in the `dependsOn` global table
/// to be recyclable on resumption (when a new ProgramRunner gets created).
type RunnerDispatch<'msg>()  = 
    let mutable dispatchImpl = (fun (_msg: 'msg) -> failwith "do not call dispatch during initialization" : unit)

    member x.DispatchViaThunk =
        id (fun msg -> dispatchImpl msg)
 
    member x.SetDispatchThunk v = dispatchImpl <- v

/// Program type captures various aspects of program behavior
type RunnerDefinition<'arg, 'msg, 'model, 'externalMsg> = 
    { init : 'arg -> 'model * Cmd<'msg> * ('externalMsg list)
      update : 'msg -> 'model -> 'model * Cmd<'msg> * ('externalMsg list)
      subscribe : 'model -> Cmd<'msg>
      view : 'model -> IViewElement
      canReuseView: IViewElement -> IViewElement -> bool
      syncDispatch: Dispatch<'msg> -> Dispatch<'msg>
      syncAction: (unit -> unit) -> (unit -> unit)
      debug : bool
      onError : (string * exn) -> unit }
    
type IRunner<'arg, 'msg, 'model, 'externalMsg> =
    abstract Start : RunnerDefinition<'arg, 'msg, 'model, 'externalMsg> * 'arg * obj option -> obj
    abstract ChangeDefinition : RunnerDefinition<'arg, 'msg, 'model, 'externalMsg> -> unit
    abstract Dispatch: 'msg -> unit

/// Starts the Elmish dispatch loop for the page with the given Elmish program
type Runner<'arg, 'msg, 'model, 'externalMsg>() =
    let mutable runnerDefinition = Unchecked.defaultof<RunnerDefinition<'arg, 'msg, 'model, 'externalMsg>>
    let mutable programDefinition = Unchecked.defaultof<ProgramDefinition>
    let mutable lastModel = Unchecked.defaultof<'model>
    let mutable lastViewData = Unchecked.defaultof<IViewElement>
    let mutable rootView = null
    let dispatch = RunnerDispatch<'msg>()
    
    // Start Elmish dispatch loop
    let rec processMsg msg = 
        try
            let updatedModel, cmd, _ = runnerDefinition.update msg lastModel
            lastModel <- updatedModel
            try 
                updateView updatedModel 
            with ex ->
                runnerDefinition.onError ("Unable to update view:", ex)
            for sub in cmd do
                try 
                    sub dispatch.DispatchViaThunk
                with ex ->
                    runnerDefinition.onError ("Error executing commands:", ex)
        with ex ->
            runnerDefinition.onError ("Unable to process a message:", ex)

    and updateView updatedModel =
        try
            let newPageElement = runnerDefinition.view updatedModel

            if runnerDefinition.canReuseView lastViewData newPageElement then
                newPageElement.Update(programDefinition, ValueSome lastViewData, rootView)
            else
                newPageElement.Update(programDefinition, ValueNone, rootView)

            lastViewData <- newPageElement
            
        with ex -> 
            runnerDefinition.onError ("Unable to evaluate view:", ex)
            
    member x.Start(definition, arg, rootOpt) =
        (x :> IRunner<'arg, 'msg, 'model, 'externalMsg>).ChangeDefinition(definition)
        
        let initialModel, cmd, _ = definition.init arg
        let initialView = definition.view initialModel
        lastModel <- initialModel
        lastViewData <- initialView
        
        // Update the given root control, or create a new one
        let target =
            match rootOpt with
            | None -> initialView.Create(programDefinition)
            | Some root -> initialView.Update(programDefinition, ValueNone, root); root
        
        rootView <- target
        
        for sub in (definition.subscribe initialModel @ cmd) do
            try 
                sub dispatch.DispatchViaThunk
            with ex ->
                definition.onError ("Error executing commands:", ex)
                
        target
        
    member x.ChangeDefinition(definition: RunnerDefinition<'arg, 'msg, 'model, 'externalMsg>) =
        dispatch.SetDispatchThunk(processMsg |> definition.syncDispatch)
        programDefinition <- 
            { CanReuseView = definition.canReuseView
              Dispatch = (fun msg -> dispatch.DispatchViaThunk(unbox msg)) }
        runnerDefinition <- definition
        
    member x.Dispatch(msg) = dispatch.DispatchViaThunk(msg)
    
    interface IRunner<'arg, 'msg, 'model, 'externalMsg> with
        member x.Start(definition, arg, rootOpt) = x.Start(definition, arg, rootOpt)
        member x.ChangeDefinition(definition) = x.ChangeDefinition(definition)
        member x.Dispatch(msg) = x.Dispatch(msg)