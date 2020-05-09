namespace Fabulous.XamarinForms.DynamicViews

open System.Collections.Generic
open Fabulous

type Attribute<'T> =
    | Property of set: ('T * obj -> unit) * unset: (obj -> unit)
    | BindableProperty of property: Xamarin.Forms.BindableProperty
    | CollectionProperty of set: ('T * obj -> unit) * unset: (obj -> unit) * attachedProperties: Attribute array
    
type EventAttribute<'T> =
    | EventHandler of (obj -> IEvent<System.EventHandler<'T>, 'T>)

module AttributeHelpers =
    let ofAttribute (attribute: Attribute<'T>) : Attribute =
        Attribute.Event (ignore, ignore)
        
    let ofEventAttribute (attribute: EventAttribute<'T>) : Attribute =
        Attribute.Event (ignore, ignore)
    
type AttributesBuilder(attribCount) =
    let values = ResizeArray<KeyValuePair<Attribute, obj>>(capacity = attribCount)
    
    member x.Add(attribute: Attribute<'T>, value: 'T) =
        values.Add(KeyValuePair(AttributeHelpers.ofAttribute attribute, box value))
        
    member x.Add(attribute: EventAttribute<'T>, value: 'T -> unit) =
        values.Add(KeyValuePair(AttributeHelpers.ofEventAttribute attribute, box value))
        
    
    member x.Close() =
        [||]