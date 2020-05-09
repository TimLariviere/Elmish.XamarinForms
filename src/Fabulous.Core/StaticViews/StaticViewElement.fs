// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.StaticViews

open System.Collections.Generic
open Fabulous
    
type StaticViewElement<'T>(create: unit -> 'T, setState: (obj voption * obj * obj -> unit), unsetState: (obj -> unit), state) =
    inherit ViewElement(
        typeof<'T>,
        (create >> box),
        [| KeyValuePair(Property (setState, unsetState), state) |]
    )
    member x.State = state