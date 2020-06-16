namespace Fabulous.Maui

type IBuilder =
    abstract Build: unit -> obj

type IView = inherit IBuilder
type IPage = inherit IBuilder
