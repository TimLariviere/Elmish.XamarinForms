namespace Fabulous.XamarinForms

open Fabulous
open System

module Component =
    let ComponentRunnerProperty = Xamarin.Forms.BindableProperty.CreateAttached("ComponentRunner", typeof<obj>, typeof<Xamarin.Forms.Element>, null)
    
    type IComponentViewElement =
        inherit IViewElement
        abstract TargetType : Type
            
    
    /// Trace all the updates to the console
    let withExternalMsgs (definition: RunnerDefinition<'arg, 'msg, 'model, 'externalMsg>, onExternalMsg: 'externalMsg -> 'parentMsg, dispatch: 'parentMsg -> unit) =
        let init arg =
            let initModel,cmd,externalMsgs = definition.init arg
            externalMsgs |> List.iter (onExternalMsg >> dispatch)
            initModel,cmd,externalMsgs

        let update msg model =
            let newModel,cmd,externalMsgs = definition.update msg model
            externalMsgs |> List.iter (onExternalMsg >> dispatch)
            newModel,cmd,externalMsgs
                
        { definition with
            init = init 
            update = update }
    
    type ComponentViewElement<'arg, 'msg, 'model, 'state, 'externalMsg, 'parentMsg>
        (
            runnerDefinition: RunnerDefinition<'arg, 'msg, 'model, 'externalMsg>,
            arg: 'arg,
            state: (('state -> 'msg) * 'state) option,
            externalMsg: ('externalMsg -> 'parentMsg) option
        ) =
        
        interface IComponentViewElement with
            member x.Create(programDefinition) =
                let definition =
                    match externalMsg with
                    | None -> runnerDefinition
                    | Some onExternalMsg -> withExternalMsgs(runnerDefinition, onExternalMsg, (fun parentMsg -> programDefinition.Dispatch(box parentMsg)))
                
                let runner = Runner<'arg, 'msg, 'model, 'externalMsg>()
                let target = runner.Start(definition, arg, None)
                
                match state with
                | None -> ()
                | Some (onStateChanged, state) -> runner.Dispatch(onStateChanged state)
                    
                (target :?> Xamarin.Forms.BindableObject).SetValue(ComponentRunnerProperty, runner)
                target
                
            member x.Update(programDefinition, _, target) =
                match (target :?> Xamarin.Forms.BindableObject).GetValue(ComponentRunnerProperty) with
                | null -> failwithf "Can't reuse a control without an associated runner"
                | runner ->
                    let definition =
                        match externalMsg with
                        | None -> runnerDefinition
                        | Some onExternalMsg -> withExternalMsgs(runnerDefinition, onExternalMsg, (fun parentMsg -> programDefinition.Dispatch(box parentMsg)))
                    
                    let r = runner :?> IRunner<'arg, 'msg, 'model, 'externalMsg>
                    r.ChangeDefinition(definition)
                    
                    match state with
                    | None -> ()
                    | Some (onStateChanged, state) -> r.Dispatch(onStateChanged state)
                
            member x.TryKey = ValueNone
            
            member x.TargetType = runnerDefinition.GetType()

    [<Struct>]
    type ComponentView<'parentMsg, 'arg, 'msg, 'model, 'state, 'externalMsg>
        (
            runnerDefinition: RunnerDefinition<'arg, 'msg, 'model, 'externalMsg>,
            arg: 'arg,
            withStateOpt: (('state -> 'msg) * 'state) option,
            withExternalMsgOpt: ('externalMsg -> 'parentMsg) option
        ) =
        
        interface IView<'parentMsg> with
            member x.AsViewElement() =
                ComponentViewElement(runnerDefinition, arg, withStateOpt, withExternalMsgOpt) :> IViewElement
                
        member x.Definition = runnerDefinition
        member x.Argument = arg
        member x.WithStateOpt = withStateOpt
        member x.WithExternalMsgOpt = withExternalMsgOpt
                
        static member inline init(definition: RunnerDefinition<'arg, 'msg, 'model, 'externalMsg>, arg: 'arg) =
            ComponentView<'parentMsg, 'arg, 'msg, 'model, 'state, 'externalMsg>(definition, arg, None, None)
            
        member inline x.withState(onStateChanged: 'state -> 'msg, state: 'state) =
            let withState = Some (onStateChanged, state)
            ComponentView<'parentMsg, 'arg, 'msg, 'model, 'state, 'externalMsg>(x.Definition, x.Argument, withState, x.WithExternalMsgOpt)
            
        member inline x.withExternalMessages(onExternalMsg: 'externalMsg -> 'parentMsg) =
            ComponentView<'parentMsg, 'arg, 'msg, 'model, 'state, 'externalMsg>(x.Definition, x.Argument, x.WithStateOpt, Some onExternalMsg)
            
    [<AbstractClass; Sealed>]
    type View private () =
        static member inline ComponentView(definition) = ComponentView.init(definition, ())
        static member inline ComponentView(definition, arg) = ComponentView.init(definition, arg)