// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

open System

type IViewElement =
    abstract member TargetType: Type
    abstract member Create: unit -> obj
    abstract member Update: IViewElement voption * obj -> unit

type ViewElement
    (targetType: Type,
     create: unit -> obj,
     update: IViewElement voption * IViewElement * obj -> unit) =
    
    interface IViewElement with
        member x.TargetType = targetType
        member x.Create() = create()
        member x.Update(prev, target) = update (prev, x, target)