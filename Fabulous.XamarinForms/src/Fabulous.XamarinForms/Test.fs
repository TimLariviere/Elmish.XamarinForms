namespace Fabulous.XamarinForms

open Xamarin.Forms
open Fabulous.XamarinForms.DynamicViews.View
open Fabulous.XamarinForms.StaticViews

module Test =
    type MyButton() =
        inherit StaticView<Button>()
        override x.Create() = Xamarin.Forms.Button(Text = "Click me")
        
    type MyCell() =
        inherit StaticCell<SwitchCell>()
        override x.Create() = Xamarin.Forms.SwitchCell()
        
    type StatefulButtonState =
        { Foo: string
          Click: unit -> unit }
        
    type StatefulButton(state: StatefulButtonState) =
        inherit StaticView<Button>(state)
        override x.Create() =
            let button = Xamarin.Forms.Button(TextColor = Color.Blue)
            button.SetBinding(Xamarin.Forms.Button.TextProperty, nameof state.Foo)
            button.SetBinding(Xamarin.Forms.Button.CommandProperty, nameof state.Click)
            button
    
    type Model = { Foo: string }
    type Msg = Bar
    
    let button =
        MyButton() :> IView<Button>
    
    let view model dispatch =
        ContentPage(
            StackLayout(children = [
                Label(text = "Hello")
                ListView([
                    MyCell()
                    TextCell()
                ])
                button
                StatefulButton({ Foo = model.Foo; Click = (fun () -> dispatch Bar) })
            ]) 
        )

