// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.DynamicViews

type IAttribute = interface end

type IProperty =
    inherit IAttribute
    abstract member Value: obj
    abstract member Apply: IProperty voption * obj -> unit
    abstract member Unapply: obj -> unit
    
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