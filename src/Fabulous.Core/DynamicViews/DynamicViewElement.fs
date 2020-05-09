// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.DynamicViews

open Fabulous
    
type DynamicViewElement<'T>(create: unit -> 'T, attributes) =
    inherit ViewElement(typeof<'T>, (create >> box), attributes)