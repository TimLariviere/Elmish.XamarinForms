namespace Fabulous.XamarinForms

open Fabulous.XamarinForms.DynamicViews
open Fabulous.XamarinForms.StaticViews

module Test =
    type Model = { Items: int list }
    
    let button: IView<Xamarin.Forms.Button> =
        View.StatelessView(fun () -> Xamarin.Forms.Button(Text = "Lol"))
    
    let view model dispatch =
        View.StackLayout([
            View.Label("Hello")
            View.ListView([
                View.StatelessCell(Xamarin.Forms.SwitchCell)
                View.TextCell()
            ])
            View.StatelessView(Xamarin.Forms.Button)
            View.StatefulView(Xamarin.Forms.CollectionView, {| Items = model.Items |})
        ])

