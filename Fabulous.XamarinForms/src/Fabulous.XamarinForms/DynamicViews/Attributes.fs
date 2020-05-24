// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.DynamicViews

open Fabulous.DynamicViews
open System
open System.Collections.Generic
open Xamarin.Forms

type ViewElementProperty =
    | Bindable of BindableProperty
    | BindableTemplate of BindableProperty
    member x.WithValue(value: obj) =
        match x with
        | Bindable bindableProperty -> { Set = (fun (_, _) -> ()); Unset = fun _ -> () }, value
        | BindableTemplate bindableProperty -> { Set = (fun (_, _) -> ()); Unset = fun _ -> () }, value
        
type ViewElementCollection<'T> =
    | Instance of get: (obj -> IList<'T>)
    member x.WithValue(value: obj) =
        match x with
        | Instance get -> { Set = (fun (_, _) -> ()); Unset = fun _ -> () }, value
    
type Bindable =
    | Property of BindableProperty
    | Collection of BindableProperty
    member x.WithValue(value: obj) =
        match x with
        | Property bindableProperty -> { Set = (fun (_, _) -> ()); Unset = fun _ -> () }, value
        | Collection bindableProperty -> { Set = (fun (_, _) -> ()); Unset = fun _ -> () }, value

type Collection<'T> =
    | Instance of get: (obj -> IList<'T>)
    member x.WithValue(value: obj) =
        match x with
        | Instance get -> { Set = (fun (_, _) -> ()); Unset = fun _ -> () }, value
    
type Event' =
    | Handler of get: (obj -> IEvent<EventHandler, EventArgs>)
    member x.WithValue(value: obj) =
        match x with
        | Handler get -> { Subscribe = (fun (_, _) -> ()); Unsubscribe = fun _ -> () }, value
    
type EventOf<'T> =
    | Handler of get: (obj -> IEvent<EventHandler<'T>, 'T>)
    member x.WithValue(value: obj) =
        match x with
        | Handler get -> { Subscribe = (fun (_, _) -> ()); Unsubscribe = fun _ -> () }, value