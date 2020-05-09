// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.DynamicViews

open Fabulous

type IProperty =
    inherit IAttribute
    abstract member Value: obj
    abstract member Set: IProperty voption * obj -> unit
    abstract member Unset: obj -> unit
    
type IEvent =
    inherit IAttribute
    abstract member Handler: System.EventHandler
    abstract member Unsubscribe: unit -> unit
    abstract member Subscribe: unit -> unit
    
type AttributesBuilder(attribCount) =
    let values = ResizeArray<IAttribute>(capacity = attribCount)
    
    member x.Add(value: IAttribute) =
        values.Add(value)
    
    member x.Close() =
        [||]