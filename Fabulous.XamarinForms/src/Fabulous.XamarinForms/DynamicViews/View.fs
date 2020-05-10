namespace Fabulous.XamarinForms.DynamicViews

open Fabulous.XamarinForms
open Xamarin.Forms

type Label =
    { Attribs: string list }
    interface IView
    
    static member init(text: string) = { Attribs = [] }
    
    member x.automationId (value: string) = x
    member x.textColor (value: Xamarin.Forms.Color) = x
    member x.font (value: double) = x
    member x.font (value: NamedSize) = x
    member x.horizontalOptions (value: LayoutOptions) = x
    member x.padding(?value: Thickness) = x

type Button =
    { Attribs: string list }
    interface IView
    
    static member init(text, onClick) = { Attribs = [] }
    
    member x.automationId (value: string) = x
    member x.textColor (value: Xamarin.Forms.Color) = x
    member x.gridColumn(value: int) = x
    
type Entry =
    { Attribs: string list }
    interface IView
    
    static member init(text, onTextChanged) = { Attribs = [] }
    
    member x.horizontalOptions (value: LayoutOptions) = x
    
    member x.padding (value: Thickness) = x
    member x.padding (?left: int, ?top: int, ?right: int, ?bottom: int) = x
    
type StackLayout =
    { Attribs: string list }
    interface IView
    
    static member init (children: IView list) = { Attribs = [] }
    static member initWithParams _ (children: IView list) = { Attribs = [] }
    
    member x.padding (value: Thickness) = x
    member x.padding (?left: int, ?top: int, ?right: int, ?bottom: int) = x
    
type ContentPage =
    { Attribs: string list }
    interface IPage
    
    static member init (content: IView) = { Attribs = [] }
    
type ListView =
    { Attribs: string list }
    interface IView
    
    static member init (items: 'T list) = { Attribs = [] }
    static member initWithTemplate<'T, 'U when 'U :> ICell>(itemsSource: 'T list) (template: 'T -> 'U) = { Attribs = [] }

type TextCell =
    { Attribs: string list }
    interface ICell
    
    static member init (text: string) = { Attribs = [] }
    
    member x.contextActions (items: IMenuItem list) = { Attribs = [] }
    
type SwitchCell =
    { Attribs: string list }
    interface ICell
    
    static member init (text: string, isOn: bool) = { Attribs = [] }
    
type ImageCell =
    { Attribs: string list }
    interface ICell
    
    static member init (image: string, title: string) = { Attribs = [] }
    
type Grid =
    { Attribs: string list }
    interface IView
    
    static member init (children: IView list) = { Attribs = [] }
    
    member x.padding (value: Thickness) = x
    member x.padding (?left: int, ?top: int, ?right: int, ?bottom: int) = x
    
type MenuItem =
    { Attribs: string list }
    interface IMenuItem
    
    static member init (text: string, onInvoked: unit -> unit) = { Attribs = [] }
    

[<AbstractClass; Sealed>]
type View private () =
    static member Label(text: string) = Label.init text
    static member Button(?text: string, ?onClicked: unit -> unit) = Button.init(text, onClicked)
    static member Entry(?text: string, ?textChanged: TextChangedEventArgs -> unit) = Entry.init(text, textChanged)
    static member StackLayout(?orientation: StackOrientation, ?spacing: double) = StackLayout.initWithParams (spacing)
    static member Grid(?coldefs: GridDefinition list, ?GridDefinition: string list) = Grid.init
    static member ContentPage (test: IView) = ContentPage.init test
    
    static member TemplatedListView (itemsSource: 'T list) = ListView.initWithTemplate itemsSource
    static member ListView(?spacing:int) = ListView.init
    static member TextCell(text: string) = TextCell.init text
    static member SwitchCell(text: string, isOn: bool) = SwitchCell.init (text, isOn)
    static member ImageCell(image: string, title: string) = ImageCell.init (image, title)
    
    static member MenuItem(text: string, onInvoked: unit -> unit) = MenuItem.init (text, onInvoked)
        
open View

module Test =
    type Model = { Items: int list }
    
    let test =
        StackLayout() [
           Label("Test")
        ]
    
    let view model =
        ContentPage(
            (StackLayout(orientation = StackOrientation.Horizontal, spacing = 10.) [
                Label("Hello world")
                    .automationId("MyLabel")
                    .textColor(Color.Red)
                    
                Button("Click me")
                    .automationId("MyButton")
                    
                TemplatedListView(itemsSource = model.Items) (fun item ->
                    if item % 2 = 0 then
                        TextCell(item.ToString()) :> ICell
                    else
                        ImageCell(image = "logo.png", title = item.ToString()) :> ICell
                )
                
                ListView() [
                    for item in model.Items ->
                        TextCell(item.ToString())
                ]
            ]).padding(top = 10)
        )