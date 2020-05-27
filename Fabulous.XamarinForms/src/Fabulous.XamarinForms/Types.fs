// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous

type IViewElementBuilder =
    abstract member AsViewElement: unit -> IViewElement

type IPage<'msg> = inherit IViewElementBuilder
type IView<'msg> = inherit IViewElementBuilder
type ICell<'msg> = inherit IViewElementBuilder
type IMenuItem<'msg> = inherit IViewElementBuilder