namespace Fabulous.XamarinForms.DynamicViews

open System.Collections.Generic
open Fabulous

type Attribute<'T> =
    | Property of set: ('T * obj -> unit) * unset: (obj -> unit)
    | BindableProperty of property: Xamarin.Forms.BindableProperty
    | CollectionProperty of set: ('T * obj -> unit) * unset: (obj -> unit)
    | AttachedProperty of property: Xamarin.Forms.BindableProperty
    
type EventAttribute<'T> =
    | EventHandler of (obj -> IEvent<System.EventHandler<'T>, 'T>)

module AttributeHelpers =
    let inline unboxValue<'T> func (value: obj, target: obj) =
        func ((value :?> 'T), target)
    
    let inline setBindableProperty (property: Xamarin.Forms.BindableProperty) (value, target: obj) =
        (target :?> Xamarin.Forms.BindableObject).SetValue(property, value)
        
    let inline unsetBindableProperty (property: Xamarin.Forms.BindableProperty) (target: obj) =
        (target :?> Xamarin.Forms.BindableObject).ClearValue(property)
        
    let inline subscribeEventHandler (getEvent: obj -> IEvent<System.EventHandler<'T>, 'T>) (value: obj, target: obj) =
        (getEvent target).AddHandler (value :?> System.EventHandler<'T>)
        
    let inline unsubscribeEventHandler (getEvent: obj -> IEvent<System.EventHandler<'T>, 'T>) (value: obj, target: obj) =
        (getEvent target).RemoveHandler (value :?> System.EventHandler<'T>)
    
    let ofAttribute (attribute: Attribute<'T>) : Attribute =
        match attribute with
        | Property (set, unset) -> Attribute.Property (unboxValue<'T> set, unset)
        | BindableProperty prop -> Attribute.Property (setBindableProperty prop, unsetBindableProperty prop)
        | CollectionProperty (set, unset) -> Attribute.Property (unboxValue<'T> set, unset)
        | AttachedProperty prop -> Attribute.Property (setBindableProperty prop, unsetBindableProperty prop)
        
    let ofEventAttribute (attribute: EventAttribute<'T>) : Attribute =
        match attribute with
        | EventHandler event -> Attribute.Event (subscribeEventHandler event, unsubscribeEventHandler event)
    
type AttributesBuilder(attribCount) =
    let values = ResizeArray<KeyValuePair<Attribute, obj>>(capacity = attribCount)
    
    member x.Add(attribute: Attribute<'T>, value: 'T) =
        values.Add(KeyValuePair(AttributeHelpers.ofAttribute attribute, box value))
        
    member x.Add(attribute: EventAttribute<'T>, value: 'T -> unit) =
        values.Add(KeyValuePair(AttributeHelpers.ofEventAttribute attribute, box value))
        
    member x.Close() =
        values.ToArray()