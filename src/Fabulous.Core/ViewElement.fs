// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

open System

type IAttribute = interface end

type IViewElement =
    abstract member TargetType: Type
    abstract member Create: unit -> obj
    abstract member Attributes: IAttribute array
    
type IViewElement<'T> =
    inherit IViewElement

type ViewElement(targetType: Type, create: unit -> obj, attributes: IAttribute array) =
    interface IViewElement with
        member x.TargetType = targetType
        member x.Create() = create()
        member x.Attributes = attributes