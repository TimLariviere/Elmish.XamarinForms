namespace Fabulous.XamarinForms.DynamicViews

open System.Collections.Generic
open Fabulous.DynamicViews

type IAttachedProperty<'T> =
    inherit IProperty

type IEvent<'T> =
    inherit IEvent

type IProperty<'T> =
    inherit IProperty
    abstract member Value: 'T
    abstract member Apply: IProperty<'T> voption * obj -> unit
    
type CollectionProperty<'T, 'U when 'T :> IEnumerable<'U>> =
    inherit IProperty<'T>
    abstract member AttachedProperties: IAttachedProperty<'U> array
    
type Property<'T>(set, unset, value: 'T) =
    interface IProperty<'T> with
        member x.Value = box value
        member x.Value = value
        
        member x.Apply(prevOpt: IProperty voption, target: obj) =
            let prevOpt = prevOpt |> ValueOption.map (fun p -> p :?> IProperty<'T>)
            (x :> IProperty<'T>).Apply(prevOpt, target)
            
        member x.Apply(prevOpt: IProperty<'T> voption, target: obj) =
            let prevValueOpt = prevOpt |> ValueOption.map (fun a -> a.Value)
            match prevValueOpt, (x :> IProperty<'T>).Value with
            | ValueSome prev, curr when System.Object.ReferenceEquals(prev, curr) -> ()
            | _, curr -> set curr target
            
        member x.Unapply(target: obj) =
            unset target
            
            