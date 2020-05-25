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
            {
                Update = (fun (dispatch, prevOpt, currOpt, target) ->
                    match prevOpt, currOpt with
                    | ValueNone, ValueNone -> ()
                    | ValueSome prev, ValueSome curr when prev = curr -> ()
                    | ValueSome prev, ValueNone -> setter(defaultValue, target :?> 'TTarget)
                    | _, ValueSome curr -> setter(curr :?> 'TValue, target :?> 'TTarget)
                )
            }
            
        let collection<'TTarget, 'TItem when 'TTarget :> BindableObject> (get: ('TTarget -> IList<'TItem>)) =
            { Update = ignore }
        
    module Bindable =
        let property (bindableProperty: BindableProperty) =
            {
                Update = (fun (dispatch, prevOpt, currOpt, target) ->
                    match prevOpt, currOpt with
                    | ValueNone, ValueNone -> ()
                    | ValueSome prev, ValueSome curr when prev = curr -> ()
                    | ValueSome prev, ValueNone -> (target :?> BindableObject).ClearValue(bindableProperty)
                    | _, ValueSome curr -> (target :?> BindableObject).SetValue(bindableProperty, curr)
                )
            }
            
        let collection (bindableProperty: BindableProperty) =
            { Update = ignore }
    
    module ViewElement =
        let bindableProperty (bindableProperty: BindableProperty) =
            {
                Update = (fun (dispatch, prevOpt, currOpt, target) ->
                    match prevOpt, currOpt with
                    | ValueNone, ValueNone -> ()
                    | ValueSome prev, ValueSome curr when prev = curr -> ()
                    | ValueSome prev, ValueNone -> (target :?> BindableObject).ClearValue(bindableProperty)
                    | ValueNone, ValueSome curr -> (target :?> BindableObject).SetValue(bindableProperty, (curr :?> IViewElement).Create(dispatch))
                    | ValueSome prev, ValueSome curr -> (curr :?> IViewElement).Update(dispatch, ValueSome (prev :?> IViewElement), target)
                )
            }
            
        let bindableTemplate (bindableProperty: BindableProperty) : DynamicProperty =
            { Update = ignore }
            
        let collection<'TTarget, 'TItem when 'TTarget :> BindableObject> (get: ('TTarget -> IList<'TItem>)) =
            {
                Update = (fun (dispatch, prevOpt, currOpt, target) ->
                    let prevOpt = prevOpt |> ValueOption.map (fun p -> p :?> IViewElement[])
                    let currOpt = currOpt |> ValueOption.map (fun p -> p :?> IViewElement[])
                    ChildrenUpdaters.updateChildren
                        prevOpt currOpt (get (target :?> 'TTarget))
                        (fun x -> x.Create(dispatch) :?> 'TItem)
                        (fun prev curr target -> curr.Update(dispatch, ValueSome prev, target))
                )
            }
        
    module Event =
        let handler<'T when 'T :> BindableObject> (get: ('T -> IEvent<EventHandler, EventArgs>)) =
            let subscribe (eventHandler: obj, target: obj) = (get (target :?> 'T)).AddHandler(eventHandler :?> EventHandler)
            let unsubscribe (eventHandler: obj, target: obj) = (get (target :?> 'T)).RemoveHandler(eventHandler :?> EventHandler)
            { Subscribe = subscribe; Unsubscribe = unsubscribe }
        
        let handlerOf<'T, 'U when 'T :> BindableObject> (get: ('T -> IEvent<EventHandler<'U>, 'U>)) =
            let subscribe (eventHandler: obj, target: obj) = (get (target :?> 'T)).AddHandler(eventHandler :?> EventHandler<'U>)
            let unsubscribe (eventHandler: obj, target: obj) = (get (target :?> 'T)).RemoveHandler(eventHandler :?> EventHandler<'U>)
            { Subscribe = subscribe; Unsubscribe = unsubscribe }
  
    type DynamicProperty with
        member x.Value(value: obj) = KeyValuePair(x, value)
        
    type DynamicEvent with
        member x.Value(fn: (obj -> unit) -> obj) = KeyValuePair(x, DynamicEventValue(fn))