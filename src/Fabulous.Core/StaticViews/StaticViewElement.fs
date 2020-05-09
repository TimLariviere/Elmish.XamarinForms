// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.StaticViews

open Fabulous

type StateProperty<'T>(setState: obj voption * obj * 'T -> unit, unsetState: 'T -> unit) =
    interface IAttribute
    
type StaticViewElement<'T>(create: unit -> 'T, setState: obj voption * obj * 'T -> unit, unsetState: 'T -> unit, state) =
    inherit ViewElement(typeof<'T>, (create >> box), [| StateProperty(setState, unsetState) |])
    interface IViewElement<'T>
    member x.State = state