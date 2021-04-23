namespace Fabulous.Maui

open Fabulous

module ViewHelpers =
    let rec canReuseView (prevChild: IViewElement) (newChild: IViewElement) =
        false