namespace Fabulous.XamarinForms

open Xamarin.Forms
open Fabulous.XamarinForms.DynamicViews.View
open Fabulous.XamarinForms.StaticViews

module Test =
    type MyButton() =
        inherit StaticView<Button>()
        override x.Create() = Xamarin.Forms.Button(Text = "Click me")
        
    type MyCell(text: string) =
        inherit StaticCell<SwitchCell>(text)
        override x.Create() =
            let switchCell = Xamarin.Forms.SwitchCell()
            switchCell.SetBinding(Xamarin.Forms.SwitchCell.TextProperty, "")
            switchCell
        
    type MyStatefulButtonState =
        { Foo: string
          Click: unit -> unit }
        
    type MyStatefulButton(foo: string, click: unit -> unit) =
        inherit StaticView<Button, MyStatefulButtonState>({ Foo = foo; Click = click })
        override x.Create() =
            let button = Xamarin.Forms.Button(TextColor = Color.Blue)
            button.SetBinding(Xamarin.Forms.Button.TextProperty, nameof x.State.Foo)
            button.SetBinding(Xamarin.Forms.Button.CommandProperty, nameof x.State.Click)
            button
        
        type Model = { Foo: string; Items: int list }
        type Msg = Bar
        
        let view model dispatch =
            ContentPage(
                StackLayout() [
                    Label(sprintf "Hello %s" model.Foo)
                        .textColor(Color.Red)
                        .font(10.)
                    
                    TemplatedListView(itemsSource = model.Items) (fun item ->
                        if item % 2 = 0 then
                            TextCell(item.ToString()) :> ICell
                        else
                            ImageCell(image = "logo.png", title = item.ToString()) :> ICell
                    )
                    
                    ListView() [
                        for i in 0 .. 10 ->
                            MyCell(i.ToString())
                    ]
                    
                    MyButton()
                    MyStatefulButton(foo = model.Foo, click = (fun () -> dispatch Bar))
                ]
            )

