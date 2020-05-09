namespace Fabulous.XamarinForms

type IBindableObject = interface end
type IPage = inherit IBindableObject
type IView = inherit IBindableObject
type ICell = inherit IBindableObject

type IBindableObject<'T  when 'T :> Xamarin.Forms.BindableObject> = inherit IBindableObject
type IPage<'T  when 'T :> Xamarin.Forms.Page> = inherit IPage; inherit IBindableObject<'T>
type IView<'T  when 'T :> Xamarin.Forms.View> = inherit IView; inherit IBindableObject<'T>
type ICell<'T  when 'T :> Xamarin.Forms.Cell> = inherit ICell; inherit IBindableObject<'T>