// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.DynamicViews

open System.Collections.Generic
open Fabulous

type DynamicViewElement(targetType, create, update, values: KeyValuePair<string, IAttribute> array) =
    inherit ViewElement(targetType, create, update)
    member x.Values = values