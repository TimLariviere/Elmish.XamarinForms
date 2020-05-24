// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.DynamicViews

open System
open System.Collections.Generic
open Fabulous
open Fabulous.DynamicViews
open Fabulous.XamarinForms
open Xamarin.Forms

module ViewAttributes =
    let ContentPageContent = ViewElementProperty.Bindable ContentPage.ContentProperty
    let ViewHorizontalOptions = Bindable.Property View.HorizontalOptionsProperty
    let LayoutOfTChildren = ViewElementCollection<_>.Instance (fun t -> (t :?> Layout<'T>).Children)
    let GridColumnDefinitions = Collection.Instance (fun t -> (t :?> Grid).ColumnDefinitions :> IList<_>)
    let GridRowDefinitions = Collection.Instance (fun t -> (t :?> Grid).RowDefinitions :> IList<_>)
    let GridColumn = Bindable.Property Grid.ColumnProperty
    let GridRow = Bindable.Property Grid.RowProperty
    let StackLayoutSpacing = Bindable.Property StackLayout.SpacingProperty
    let ButtonText = Bindable.Property Button.TextProperty
    let ButtonClicked = Event'.Handler (fun t -> (t :?> Button).Clicked)
    let LabelFontSize = Bindable.Property Label.FontSizeProperty
    let LabelText = Bindable.Property Label.TextProperty
    let LabelTextColor = Bindable.Property Label.TextColorProperty
    let InputViewText = Bindable.Property InputView.TextProperty
    let InputViewTextChanged = EventOf<_>.Handler (fun t -> (t :?> InputView).TextChanged)
    let ItemsViewOfTItemsSource = Bindable.Collection ItemsView.ItemsSourceProperty
    let ItemsViewOfTItemTemplate = ViewElementProperty.BindableTemplate ItemsView.ItemTemplateProperty
    let CellContextActions = Collection.Instance (fun t -> (t :?> Cell).ContextActions)
    let TextCellText = Bindable.Property TextCell.TextProperty
    let MenuItemText = Bindable.Property MenuItem.TextProperty
    let MenuItemClicked = Event'.Handler (fun t -> (t :?> MenuItem).Clicked)
    
[<Sealed>]
type ContentPage<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.ContentPage>, Xamarin.Forms.ContentPage >> box, attributes)
    interface IPage<'msg>
    static member inline init(content: IView<'msg>) =
        let attributes = [PropertyNode (ViewAttributes.ContentPageContent.WithValue(content))]
        ContentPage<'msg>(attributes)
        
[<Sealed>]
type Grid<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.Grid>, Xamarin.Forms.Grid >> box, attributes)
    interface IView<'msg>
    static member inline init(?coldefs: GridDefinition list, ?rowdefs: GridDefinition list) =
        let attributes = []
        let attributes = match coldefs with None -> attributes | Some v -> PropertyNode (ViewAttributes.GridColumnDefinitions.WithValue(v))::attributes
        let attributes = match rowdefs with None -> attributes | Some v -> PropertyNode (ViewAttributes.GridRowDefinitions.WithValue(v))::attributes
        fun (children: IView<'msg> list) ->
            let attributes = match children with [] -> attributes | v -> PropertyNode (ViewAttributes.LayoutOfTChildren.WithValue(v))::attributes
            Grid<'msg>(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = PropertyNode (ViewAttributes.GridColumn.WithValue(column))::x.Attributes
        Grid<'msg>(attributes)
        
[<Sealed>]
type StackLayout<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.StackLayout>, Xamarin.Forms.StackLayout >> box, attributes)
    interface IView<'msg>
    static member inline init(?spacing: double) =
        let attributes = []
        let attributes = match spacing with None -> attributes | Some v -> PropertyNode (ViewAttributes.StackLayoutSpacing.WithValue(v))::attributes
        fun (children: IView<'msg> list) ->
            let attributes = match children with [] -> attributes | v -> PropertyNode (ViewAttributes.LayoutOfTChildren.WithValue(v))::attributes
            StackLayout<'msg>(attributes)
    
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewHorizontalOptions.WithValue(options))::x.Attributes
        StackLayout<'msg>(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = PropertyNode (ViewAttributes.GridColumn.WithValue(column))::x.Attributes
        StackLayout<'msg>(attributes)
        
[<Sealed>]
type Button<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.Button>, Xamarin.Forms.Button >> box, attributes)
    interface IView<'msg>
    static member inline init(?text: string, ?clicked: 'msg) =
        let attributes = []
        let attributes = match text with None -> attributes | Some v -> PropertyNode (ViewAttributes.ButtonText.WithValue(v))::attributes
        let attributes = match clicked with None -> attributes | Some v -> EventNode (ViewAttributes.ButtonClicked.WithValue(EventHandler(fun _ _ -> RunnerDispatch<'msg>.DispatchViaThunk v)))::attributes
        Button<'msg>(attributes)
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewHorizontalOptions.WithValue(options))::x.Attributes
        Button<'msg>(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = PropertyNode (ViewAttributes.GridColumn.WithValue(column))::x.Attributes
        Button<'msg>(attributes)
       
[<Sealed>]
type Entry<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.Entry>, Xamarin.Forms.Entry >> box, attributes)
    interface IView<'msg>
    static member inline init(?text: string, ?textChanged: TextChangedEventArgs -> 'msg) =
        let attributes = []
        let attributes = match text with None -> attributes | Some v -> PropertyNode (ViewAttributes.InputViewText.WithValue(v))::attributes
        let attributes = match textChanged with None -> attributes | Some v -> EventNode (ViewAttributes.InputViewTextChanged.WithValue(EventHandler<TextChangedEventArgs>(fun _ args -> v args |> RunnerDispatch<'msg>.DispatchViaThunk)))::attributes
        Entry<'msg>(attributes)
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewHorizontalOptions.WithValue(options))::x.Attributes
        Entry<'msg>(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = PropertyNode (ViewAttributes.GridColumn.WithValue(column))::x.Attributes
        Entry<'msg>(attributes)
          
[<Sealed>]  
type Label<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.Label>, Xamarin.Forms.Label >> box, attributes)
    interface IView<'msg>
    static member inline init(text: string) =
        let attributes = [PropertyNode (ViewAttributes.LabelText.WithValue(text))]
        Label<'msg>(attributes)
        
    member inline x.font(fontSize: NamedSize) =
        let attributes = PropertyNode (ViewAttributes.LabelFontSize.WithValue(Device.GetNamedSize(fontSize, typeof<Label>)))::x.Attributes
        Label<'msg>(attributes)
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewHorizontalOptions.WithValue(options))::x.Attributes
        Label<'msg>(attributes)
        
    member inline x.textColor(color: Color) =
        let attributes = PropertyNode (ViewAttributes.LabelTextColor.WithValue(color))::x.Attributes
        Label<'msg>(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = PropertyNode (ViewAttributes.GridColumn.WithValue(column))::x.Attributes
        Label<'msg>(attributes)
        
[<Sealed>]
type ListView<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.Label>, Xamarin.Forms.Label >> box, attributes)
    interface IView<'msg>
    static member inline init() =
        fun (items: #ICell<'msg> list) ->
            let attributes = [PropertyNode (ViewAttributes.ItemsViewOfTItemsSource.WithValue(items))]
            ListView<'msg>(attributes)
        
[<Sealed>]    
type TextCell<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.TextCell>, Xamarin.Forms.TextCell >> box, attributes)
    interface ICell<'msg>
    static member inline init(text: string) =
        let attributes = [PropertyNode (ViewAttributes.TextCellText.WithValue(text))]
        TextCell<'msg>(attributes)
        
    member inline x.contextActions(actions: IMenuItem<'msg> list) =
        let attributes = PropertyNode (ViewAttributes.CellContextActions.WithValue(actions))::x.Attributes
        TextCell<'msg>(attributes)        
        
[<Sealed>]
type MenuItem<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.MenuItem>, Xamarin.Forms.MenuItem >> box, attributes)
    interface IMenuItem<'msg>
    static member inline init(?text: string, ?clicked: 'msg) =
        let attributes = []
        let attributes = match text with None -> attributes | Some v -> PropertyNode (ViewAttributes.MenuItemText.WithValue(v))::attributes
        let attributes = match clicked with None -> attributes | Some v -> EventNode (ViewAttributes.MenuItemClicked.WithValue(v))::attributes
        MenuItem<'msg>(attributes)
        
        
[<AbstractClass; Sealed>]
type View private () =
    static member inline ContentPage(content) = ContentPage.init content
    static member inline Grid(?coldefs,?rowdefs) = Grid.init (?coldefs=coldefs,?rowdefs=rowdefs)
    static member inline StackLayout(?spacing: double) = StackLayout.init(?spacing=spacing)
    static member inline Button(?text,?clicked) = Button.init(?text=text,?clicked=clicked)
    static member inline Entry(?text,?textChanged) = Entry.init(?text=text,?textChanged=textChanged)
    static member inline Label(text) = Label.init text
    static member inline ListView() = ListView.init()
    static member inline TextCell(text) = TextCell.init text
    static member inline MenuItem(?text,?clicked) = MenuItem.init(?text=text,?clicked=clicked)