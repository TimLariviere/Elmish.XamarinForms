// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

open System
open System.Collections.Generic

type Attribute =
    | Property of set: (obj voption * obj * obj -> unit) * unset: (obj -> unit)
    | Event of subscribe: (unit -> unit) * unsubscribe: (unit -> unit)

type ViewElement(targetType: Type, create: unit -> obj, attributes: KeyValuePair<Attribute, obj> array) =
    member x.TargetType = targetType
    member x.Create() = create()
    member x.Attributes = attributes
    
module DynamicViews =
    type DynamicViewElement<'T>(create: unit -> 'T, attributes) =
        inherit ViewElement(typeof<'T>, (create >> box), attributes)
        
module StaticViews =
    type StaticViewElement<'T>(create: unit -> 'T, setState: (obj voption * obj * obj -> unit), unsetState: (obj -> unit), state) =
        inherit ViewElement(typeof<'T>, (create >> box), [| KeyValuePair(Property (setState, unsetState), state) |])
        member x.State = state