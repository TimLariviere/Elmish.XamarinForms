// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.DynamicViews

open Fabulous
open System
open System.Collections.Generic
open Xamarin.Forms

module Attributes =
    module Scalar =
        let property<'TTarget, 'TValue> (defaultValue: 'TValue) (setter: 'TValue * 'TTarget -> unit) =
            {
                Update = (fun (_, prevOpt, currOpt, target) ->
                    match prevOpt, currOpt with
                    | ValueNone, ValueNone -> ()
                    | ValueSome prev, ValueSome curr when prev = curr -> ()
                    | ValueSome _, ValueNone -> setter(defaultValue, target :?> 'TTarget)
                    | _, ValueSome curr -> setter(curr :?> 'TValue, target :?> 'TTarget)
                )
            }
            
        let collection<'TTarget, 'TItem when 'TItem: equality> (get: ('TTarget -> IList<'TItem>)) =
            {
                Update = (fun (_, prevOpt, currOpt, target) ->
                    let prevOpt = prevOpt |> ValueOption.map (fun p -> p :?> 'TItem[])
                    let currOpt = currOpt |> ValueOption.map (fun p -> p :?> 'TItem[])
                    let coll = get(target :?> 'TTarget)
                    ChildrenUpdaters.updateChildrenInternal
                        prevOpt currOpt
                        (fun _ -> ValueNone)
                        (fun _ _ -> false)
                        (fun () -> coll.Clear())
                        (fun idx value -> coll.Insert(idx, value))
                        (fun idx _ curr -> coll.[idx] <- curr)
                        (fun oldIndex newIndex -> coll.[newIndex] <- coll.[oldIndex])
                        (fun idx -> coll.RemoveAt(idx))
                )
            }
        
    module Bindable =
        let property (bindableProperty: BindableProperty) =
            {
                Update = (fun (_, prevOpt, currOpt, target) ->
                    match prevOpt, currOpt with
                    | ValueNone, ValueNone -> ()
                    | ValueSome prev, ValueSome curr when prev = curr -> ()
                    | ValueSome _, ValueNone -> (target :?> BindableObject).ClearValue(bindableProperty)
                    | _, ValueSome curr -> (target :?> BindableObject).SetValue(bindableProperty, curr)
                )
            }
            
        let collection (bindableProperty: BindableProperty) =
            { Update = ignore }
    
    module ViewElement =
        let scalarProperty (defaultValue: 'TValue) (getter: 'TTarget -> 'TValue) (setter: 'TValue * 'TTarget -> unit) =
            {
                Update = (fun (programDefinition, prevOpt, currOpt, target) ->
                    match prevOpt, currOpt with
                    | ValueNone, ValueNone -> ()
                    | ValueSome prev, ValueSome curr when prev = curr -> ()
                    | ValueSome _, ValueNone -> setter(defaultValue, target :?> 'TTarget)
                    | ValueSome prev, ValueSome curr when programDefinition.CanReuseView (prev :?> IViewElement) (curr :?> IViewElement) ->
                        let realTarget = getter(target :?> 'TTarget)
                        (curr :?> IViewElement).Update(programDefinition, ValueSome (prev :?> IViewElement), realTarget)
                    | _, ValueSome curr ->
                        let value = (curr :?> IViewElement).Create(programDefinition)
                        setter(value :?> 'TValue, target :?> 'TTarget)
                )
            }
            
        let bindableProperty (bindableProperty: BindableProperty) =
            {
                Update = (fun (programDefinition, prevOpt, currOpt, target) ->
                    match prevOpt, currOpt with
                    | ValueNone, ValueNone -> ()
                    | ValueSome prev, ValueSome curr when prev = curr -> ()
                    | ValueSome _, ValueNone -> (target :?> BindableObject).ClearValue(bindableProperty)
                    | ValueSome prev, ValueSome curr when programDefinition.CanReuseView (prev :?> IViewElement) (curr :?> IViewElement) ->
                        let realTarget = (target :?> BindableObject).GetValue(bindableProperty)
                        (curr :?> IViewElement).Update(programDefinition, ValueSome (prev :?> IViewElement), realTarget)
                    | _, ValueSome curr -> (target :?> BindableObject).SetValue(bindableProperty, (curr :?> IViewElement).Create(programDefinition))
                )
            }
            
        let bindableTemplate (bindableProperty: BindableProperty) : DynamicProperty =
            { Update = ignore }
            
        let collection<'TTarget, 'TItem> (get: ('TTarget -> IList<'TItem>)) =
            {
                Update = (fun (programDefinition, prevOpt, currOpt, target) ->
                    let prevOpt = prevOpt |> ValueOption.map (fun p -> p :?> IViewElement[])
                    let currOpt = currOpt |> ValueOption.map (fun p -> p :?> IViewElement[])
                    let t = (target :?> 'TTarget)
                    ChildrenUpdaters.updateChildren
                        prevOpt currOpt (get t)
                        programDefinition.CanReuseView
                        (fun x -> x.Create(programDefinition) :?> 'TItem)
                        (fun prev curr target -> curr.Update(programDefinition, ValueSome prev, target))
                )
            }
        
    module Event =
        let handler<'T when 'T :> BindableObject> (get: ('T -> IEvent<EventHandler, EventArgs>)) =
            let subscribe (eventHandler: obj, target: obj) = (get (target :?> 'T)).AddHandler(eventHandler :?> EventHandler)
            let unsubscribe (eventHandler: obj, target: obj) = (get (target :?> 'T)).RemoveHandler(eventHandler :?> EventHandler)
            { Subscribe = subscribe; Unsubscribe = unsubscribe }
        
        let handlerOf<'T, 'U when 'T :> BindableObject> (get: ('T -> IEvent<EventHandler<'U>, 'U>)) =
            let subscribe (eventHandler: obj, target: obj) =
                try
                    (get (target :?> 'T)).AddHandler(eventHandler :?> EventHandler<'U>)
                with
                | exn -> System.Diagnostics.Debugger.Break()
            let unsubscribe (eventHandler: obj, target: obj) = (get (target :?> 'T)).RemoveHandler(eventHandler :?> EventHandler<'U>)
            { Subscribe = subscribe; Unsubscribe = unsubscribe }
  
    type DynamicProperty with
        member x.Value(value: obj) = (x, value)
        
    type DynamicEvent with
        member x.Value(fn: (obj -> unit) -> obj) = (x, fn)