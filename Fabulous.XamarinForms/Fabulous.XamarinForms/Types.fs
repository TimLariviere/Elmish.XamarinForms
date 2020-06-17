// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms

open Fabulous

type IViewElementBuilder<'msg> =
    abstract member AsViewElement: unit -> IViewElement

type IApplication<'msg> = inherit IViewElementBuilder<'msg>
type IPage<'msg> = inherit IViewElementBuilder<'msg>
type IView<'msg> = inherit IViewElementBuilder<'msg>
type ICell<'msg> = inherit IViewElementBuilder<'msg>
type IMenu<'msg> = inherit IViewElementBuilder<'msg>
type IMenuItem<'msg> = inherit IViewElementBuilder<'msg>

type IStyle =
    abstract member AsViewElement: unit -> IViewElement