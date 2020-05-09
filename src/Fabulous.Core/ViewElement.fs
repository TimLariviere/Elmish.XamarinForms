// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

open System
open System.Collections.Generic

type Attribute =
    | Property of set: (obj * obj -> unit) * unset: (obj -> unit)
    | Event of subscribe: (obj * obj -> unit) * unsubscribe: (obj * obj -> unit)

type ViewElement(targetType: Type, create: unit -> obj, attributes: KeyValuePair<Attribute, obj> array) =
    member x.TargetType = targetType
    member x.Attributes = attributes
    member x.Create() = create()
    
module DynamicViews =
    type DynamicViewElement<'T>(create: unit -> 'T, attributes) =
        inherit ViewElement(typeof<'T>, (create >> box), attributes)
        
module StaticViews =
    type StaticViewElement<'T>(create: unit -> 'T, setState: (obj * obj -> unit), unsetState: (obj -> unit), state) =
        inherit ViewElement(typeof<'T>, (create >> box), [| KeyValuePair(Property (setState, unsetState), state) |])
        member x.State = state