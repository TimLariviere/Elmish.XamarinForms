namespace Fabulous.XamarinForms.StaticViews

open Fabulous.StaticViews

module ViewHelpers =
    let rec canReuseStaticView (prevChild: StaticViewElement) (newChild: StaticViewElement) =
        prevChild.TargetType = newChild.TargetType
