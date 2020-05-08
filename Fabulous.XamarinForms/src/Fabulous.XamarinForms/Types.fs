namespace Fabulous.XamarinForms

type IBindableObject = interface end
type IView = inherit IBindableObject
type ICell = inherit IBindableObject

type IBindableObject<'T  when 'T :> Xamarin.Forms.BindableObject> = inherit IBindableObject
type IView<'T  when 'T :> Xamarin.Forms.View> = inherit IView; inherit IBindableObject<'T>
type ICell<'T  when 'T :> Xamarin.Forms.Cell> = inherit ICell; inherit IBindableObject<'T>