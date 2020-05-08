// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.StaticViews

open Fabulous

type StaticViewElement(targetType, create, setState: obj voption -> obj -> obj -> unit, state: obj) =
    inherit ViewElement(
        targetType,
        create,
        (fun (prevOpt, _, target) ->
            let prevOpt = prevOpt |> ValueOption.map (fun p -> (p :?> StaticViewElement).State)
            setState prevOpt state target)
    )
    member x.State = state