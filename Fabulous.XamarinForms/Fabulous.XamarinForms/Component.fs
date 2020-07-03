namespace Fabulous.XamarinForms

open Fabulous
open System

module Component =
    let ComponentRunnerProperty = Xamarin.Forms.BindableProperty.CreateAttached("ComponentRunner", typeof<obj>, typeof<Xamarin.Forms.Element>, null)
    
    type IComponentViewElement =
        inherit IViewElement
        abstract TargetType : Type
    
    type ComponentViewElement<'arg, 'msg, 'model>(runnerDefinition: RunnerDefinition<'arg, 'msg, 'model>, arg: 'arg) =
        interface IComponentViewElement with
            member x.Create(_) =
                let runner = Runner<'arg, 'msg, 'model>()
                let target = runner.Start(runnerDefinition, arg, None)
                (target :?> Xamarin.Forms.BindableObject).SetValue(ComponentRunnerProperty, runner)
                target
                
            member x.Update(_, _, target) =
                match (target :?> Xamarin.Forms.BindableObject).GetValue(ComponentRunnerProperty) with
                | null -> failwithf "Can't reuse a control without an associated runner"
                | runner -> (runner :?> IRunner<'arg, 'msg, 'model>).ChangeDefinition(runnerDefinition)
                
            member x.TryKey = ValueNone
            
            member x.TargetType = runnerDefinition.GetType()

    [<Struct>]
    type ComponentView<'parentMsg, 'arg, 'msg, 'model>(runnerDefinition: RunnerDefinition<'arg, 'msg, 'model>, arg: 'arg) =
        interface IView<'parentMsg> with
            member x.AsViewElement() =
                ComponentViewElement(runnerDefinition, arg) :> IViewElement
                
        static member inline init(definition: RunnerDefinition<'arg, 'msg, 'model>, arg: 'arg) =
            ComponentView<'parentMsg, 'arg, 'msg, 'model>(definition, arg)
            
    [<AbstractClass; Sealed>]
    type View private () =
        static member inline ComponentView(definition, arg) = ComponentView.init(definition, arg)