namespace Fabulous.XamarinForms

type IBindableObject = interface end
type IPage = inherit IBindableObject
type IView = inherit IBindableObject
type ICell = inherit IBindableObject
type IMenuItem = inherit IBindableObject

type GridDefinition = Auto | Star | Stars of double | Absolute of int