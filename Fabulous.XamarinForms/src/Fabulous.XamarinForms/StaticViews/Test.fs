namespace Fabulous.XamarinForms.StaticViews

open Xamarin.Forms

module Test = 
    let noState =
        View.StatelessView(Xamarin.Forms.Button)
    let withState dispatch =
        View.StatefulView(
            (fun () ->
                 let button = Xamarin.Forms.Button()
                 button.SetBinding(Xamarin.Forms.Button.TextProperty, "Foo")
                 button.SetBinding(Xamarin.Forms.Button.CommandProperty, "Click")
                 button),
            {| Foo = "Bar"; Click = fun() -> dispatch 10 |}
        )
