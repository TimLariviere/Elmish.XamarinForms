namespace Fabulous.XamarinForms.StaticViews

open System
open Xamarin.Forms

module Test =
    type StatelessButton() =
        inherit StaticView<Button>()
        override x.Create() =
            Xamarin.Forms.Button()
            
    type StatefulButtonState =
        { Foo: string
          Click: unit -> unit }
            
    type StatefulButton(state: StatefulButtonState) =
        inherit StaticView<Button>(state)
        override x.Create() =
            let button = Xamarin.Forms.Button()
            button.SetBinding(Xamarin.Forms.Button.TextProperty, nameof state.Foo)
            button.SetBinding(Xamarin.Forms.Button.CommandProperty, nameof state.Click)
            button
    
    let noState =
        StatelessButton()
    let withState dispatch =
        StatefulButton({ Foo = "Bar"; Click = fun() -> dispatch 10 })
