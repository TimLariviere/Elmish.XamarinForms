// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.DynamicViews

open System
open System.Collections.Generic
open Fabulous
open Xamarin.Forms
open Fabulous.DynamicViews

type ViewElementProperty =
    | Bindable of BindableProperty
    | BindableTemplate of BindableProperty
    
type ViewElementCollection<'T> =
    | Instance of get: (obj -> IList<'T>)
    
type Bindable =
    | Property of BindableProperty
    | Collection of BindableProperty

type Collection<'T> =
    | Instance of get: (obj -> IList<'T>)
    
type Event' =
    | Handler of get: (obj -> IEvent<EventHandler, EventArgs>)
    
type EventOf<'T> =
    | Handler of get: (obj -> IEvent<EventHandler<'T>, 'T>)
    
module Attribute =
    let ofViewElementProperty (key: string, property: ViewElementProperty) (value: obj) =
        match property with
        | ViewElementProperty.Bindable bindableProperty ->
            let set (value: obj, target: obj) = (target :?> BindableObject).SetValue(bindableProperty, (value :?> ViewElement).Create())
            let unset (target: obj) = (target :?> BindableObject).ClearValue(bindableProperty)
            KeyValuePair(key, { Attribute = Attribute.Property (set, unset); Value = value })
        | ViewElementProperty.BindableTemplate bindableProperty ->
            let set (value: obj, target: obj) = (target :?> BindableObject).SetValue(bindableProperty, DataTemplate(value :?> Func<obj>))
            let unset (target: obj) = (target :?> BindableObject).ClearValue(bindableProperty)
            KeyValuePair(key, { Attribute = Attribute.Property (set, unset); Value = value })
        
    let ofViewElementCollection (key: string, collection: ViewElementCollection<'T>) (value: obj) =
        KeyValuePair(key, { Attribute = Attribute.Property (ignore, ignore); Value = value })
        
    let ofBindable (key: string, bindable: Bindable) (value: obj) =
        match bindable with
        | Bindable.Property bindableProperty ->
            let set (value: obj, target: obj) = (target :?> BindableObject).SetValue(bindableProperty, value)
            let unset (target: obj) = (target :?> BindableObject).ClearValue(bindableProperty)
            KeyValuePair(key, { Attribute = Attribute.Property (set, unset); Value = value })
        | Bindable.Collection bindableProperty ->
            KeyValuePair(key, { Attribute = Attribute.Property (ignore, ignore); Value = value })
        
    let ofCollectionOfT (key: string, collection: Collection<'T>) (value: obj) =
        KeyValuePair(key, { Attribute = Attribute.Property (ignore, ignore); Value = value })
        
    let ofEvent (key: string, evt: Event') (value: obj) =
        match evt with
        | Event'.Handler get ->
            let subscribe (handler: obj, target: obj) = (get target).AddHandler (handler :?> EventHandler)
            let unsubscribe (handler: obj, target: obj) = (get target).RemoveHandler (handler :?> EventHandler)
            KeyValuePair(key, { Attribute = Attribute.Event (subscribe, unsubscribe); Value = value })
        
    let ofEventOfT (key: string, evt: EventOf<'T>) (value: obj) =
        match evt with
        | EventOf.Handler get ->
            let subscribe (handler: obj, target: obj) = (get target).AddHandler (handler :?> EventHandler<'T>)
            let unsubscribe (handler: obj, target: obj) = (get target).RemoveHandler (handler :?> EventHandler<'T>)
            KeyValuePair(key, { Attribute = Attribute.Event (subscribe, unsubscribe); Value = value })
        