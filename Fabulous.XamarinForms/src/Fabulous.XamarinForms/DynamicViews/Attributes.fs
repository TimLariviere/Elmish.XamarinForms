// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.DynamicViews

open Fabulous
open Fabulous.DynamicViews
open System
open System.Collections.Generic
open Xamarin.Forms

module Attributes =
    module Scalar =
        let property<'TTarget, 'TValue> (defaultValue: 'TValue) (setter: 'TValue * 'TTarget -> unit) =
            let set (_, value: obj, target: obj) = setter(value :?> 'TValue, target :?> 'TTarget)
            let unset (_, target: obj) = setter(defaultValue, target :?> 'TTarget)
            { Set = set; Unset = unset }
    
    module ViewElementProperty =
        let internal asDynamicProperty (set: DynamicViewElement voption * DynamicViewElement * BindableObject -> unit) (unset: BindableObject -> unit) =
            let set (prevValue: obj voption, value: obj, target: obj) =
                set((prevValue |> ValueOption.map (fun p -> p :?> DynamicViewElement)), (value :?> DynamicViewElement), (target :?> BindableObject))
            let unset (_, target: obj) =
                unset (target :?> BindableObject)
            { Set = set; Unset = unset }
        
        let bindable (bindableProperty: BindableProperty) =
            let set (prevValue: DynamicViewElement voption, value: DynamicViewElement, target: BindableObject) =
                match prevValue with
                | ValueNone -> target.SetValue(bindableProperty, value.Create())
                | ValueSome prev -> value.Update(ValueSome prev, target)
                
            let unset (target: BindableObject) =
                target.ClearValue(bindableProperty)
            
            asDynamicProperty set unset
            
        let bindableTemplate (bindableProperty: BindableProperty) : DynamicProperty =
            asDynamicProperty ignore ignore
            
    module ViewElementCollection =
        let instance<'TTarget, 'TItem when 'TTarget :> BindableObject> (get: ('TTarget -> IList<'TItem>)) =
            let update (prevValue: obj voption) (value: obj voption) (target: obj) =
                let prevValue = prevValue |> ValueOption.map (fun p -> p :?> DynamicViewElement[])
                let value = value |> ValueOption.map (fun p -> p :?> DynamicViewElement[])
                let target = target :?> 'TTarget
                ChildrenUpdaters.updateChildren
                    prevValue value (get target)
                    (fun x -> x.Create() :?> 'TItem)
                    (fun prev curr target -> curr.Update(ValueSome prev, target))
                    (fun _ _ _ -> ())
                
            let set (prevValue, value, target) = update prevValue (ValueSome value) target
            let unset (prevValue, target) = update (ValueSome prevValue) ValueNone target
            { Set = set; Unset = unset }
        
    module Bindable =
        let internal dynamicProperty (set: obj voption * obj * BindableObject -> unit) (unset: BindableObject -> unit) =
            let set (prevValue: obj voption, value: obj, target: obj) =
                set(prevValue, value, (target :?> BindableObject))
            let unset (_, target: obj) =
                unset (target :?> BindableObject)
            { Set = set; Unset = unset }
            
        let property (bindableProperty: BindableProperty) =
            let set (_, value: obj, target: BindableObject) = target.SetValue(bindableProperty, value)
            let unset (target: BindableObject) = target.ClearValue(bindableProperty)
            dynamicProperty set unset
            
        let collection (bindableProperty: BindableProperty) =
            { Set = (fun (_, _, _) -> ()); Unset = fun _ -> () }

    module Collection =
        let instance<'TTarget, 'TItem when 'TTarget :> BindableObject> (get: ('TTarget -> IList<'TItem>)) =
            { Set = (fun (_, _, _) -> ()); Unset = fun _ -> () }
        
    module Event =
        let handler<'T when 'T :> BindableObject> (get: ('T -> IEvent<EventHandler, EventArgs>)) =
            let subscribe (eventHandler: obj, target: obj) = (get (target :?> 'T)).AddHandler(eventHandler :?> EventHandler)
            let unsubscribe (eventHandler: obj, target: obj) = (get (target :?> 'T)).RemoveHandler(eventHandler :?> EventHandler)
            { Subscribe = subscribe; Unsubscribe = unsubscribe }
        
        let handlerOf<'T, 'U when 'T :> BindableObject> (get: ('T -> IEvent<EventHandler<'U>, 'U>)) =
            { Subscribe = (fun (_, _) -> ()); Unsubscribe = fun _ -> () }
  
    type DynamicProperty with
        member x.Value(value: obj) = x, value
        
    type DynamicEvent with
        member x.Value(value: obj) = x, value