// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.DynamicViews

open System
open System.Collections.Generic
open Fabulous
open Fabulous.DynamicViews
open Fabulous.XamarinForms
open Fabulous.XamarinForms.DynamicViews.Attributes
open Xamarin.Forms

module ViewAttributes =
    let ContentPageContent = Attributes.ViewElementProperty.bindable ContentPage.ContentProperty
    let ViewHorizontalOptions = Attributes.Bindable.property View.HorizontalOptionsProperty
    let ViewVerticalOptions = Attributes.Bindable.property View.VerticalOptionsProperty
    let LayoutOfTChildren = Attributes.ViewElementCollection.instance<Layout<_>, _> (fun t -> t.Children)
    let GridColumnDefinitions = Attributes.Collection.instance<Grid, _> (fun t -> t.ColumnDefinitions :> IList<_>)
    let GridRowDefinitions = Attributes.Collection.instance<Grid, _> (fun t -> t.RowDefinitions :> IList<_>)
    let GridColumn = Attributes.Bindable.property Grid.ColumnProperty
    let GridRow = Attributes.Bindable.property Grid.RowProperty
    let StackLayoutSpacing = Attributes.Bindable.property StackLayout.SpacingProperty
    let ButtonText = Attributes.Bindable.property Button.TextProperty
    let ButtonClicked = Attributes.Event.handler<Button> (fun t -> t.Clicked)
    let LabelFontSize = Attributes.Bindable.property Label.FontSizeProperty
    let LabelText = Attributes.Bindable.property Label.TextProperty
    let LabelTextColor = Attributes.Bindable.property Label.TextColorProperty
    let InputViewText = Attributes.Bindable.property InputView.TextProperty
    let InputViewTextChanged = Attributes.Event.handlerOf<InputView, _> (fun t -> t.TextChanged)
    let ItemsViewOfTItemsSource = Attributes.Bindable.collection ItemsView.ItemsSourceProperty
    let ItemsViewOfTItemTemplate = Attributes.ViewElementProperty.bindableTemplate ItemsView.ItemTemplateProperty
    let CellContextActions = Attributes.Collection.instance<Cell, _> (fun t -> t.ContextActions)
    let TextCellText = Attributes.Bindable.property TextCell.TextProperty
    let MenuItemText = Attributes.Bindable.property MenuItem.TextProperty
    let MenuItemClicked = Attributes.Event.handler<MenuItem> (fun t -> t.Clicked)
    
    
[<Sealed>]
type ContentPage<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.ContentPage>, Xamarin.Forms.ContentPage >> box, attributes)
    interface IPage<'msg>
    static member inline init(content: IView<'msg>) =
        let attributes = [PropertyNode (ViewAttributes.ContentPageContent.Value(content))]
        ContentPage<'msg>(attributes)
        
[<Sealed>]
type Grid<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.Grid>, Xamarin.Forms.Grid >> box, attributes)
    interface IView<'msg>
    static member inline init(?coldefs: GridDefinition list, ?rowdefs: GridDefinition list) =
        let attributes = []
        let attributes = match coldefs with None -> attributes | Some v -> PropertyNode (ViewAttributes.GridColumnDefinitions.Value(v))::attributes
        let attributes = match rowdefs with None -> attributes | Some v -> PropertyNode (ViewAttributes.GridRowDefinitions.Value(v))::attributes
        fun (children: IView<'msg> list) ->
            let attributes = match children with [] -> attributes | v -> PropertyNode (ViewAttributes.LayoutOfTChildren.Value(v))::attributes
            Grid<'msg>(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = PropertyNode (ViewAttributes.GridColumn.Value(column))::x.Attributes
        Grid<'msg>(attributes)
        
[<Sealed>]
type StackLayout<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.StackLayout>, Xamarin.Forms.StackLayout >> box, attributes)
    interface IView<'msg>
    static member inline init(children: IView<'msg> list, ?spacing: double) =
        let attributes = match children with [] -> [] | v -> [PropertyNode (ViewAttributes.LayoutOfTChildren.Value(v |> List.map (fun x -> x :?> DynamicViewElement) |> Array.ofList))]
        let attributes = match spacing with None -> attributes | Some v -> PropertyNode (ViewAttributes.StackLayoutSpacing.Value(v))::attributes
        StackLayout<'msg>(attributes)
    
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewHorizontalOptions.Value(options))::x.Attributes
        StackLayout<'msg>(attributes)
    
    member inline x.verticalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewVerticalOptions.Value(options))::x.Attributes
        StackLayout<'msg>(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = PropertyNode (ViewAttributes.GridColumn.Value(column))::x.Attributes
        StackLayout<'msg>(attributes)
        
[<Sealed>]
type Button<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.Button>, Xamarin.Forms.Button >> box, attributes)
    interface IView<'msg>
    static member inline init(?text: string, ?clicked: 'msg) =
        let attributes = []
        let attributes = match text with None -> attributes | Some v -> PropertyNode (ViewAttributes.ButtonText.Value(v))::attributes
        let attributes = match clicked with None -> attributes | Some v -> EventNode (ViewAttributes.ButtonClicked.Value(EventHandler(fun _ _ -> RunnerDispatch<'msg>.DispatchViaThunk v)))::attributes
        Button<'msg>(attributes)
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewHorizontalOptions.Value(options))::x.Attributes
        Button<'msg>(attributes)
    
    member inline x.verticalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewVerticalOptions.Value(options))::x.Attributes
        Button<'msg>(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = PropertyNode (ViewAttributes.GridColumn.Value(column))::x.Attributes
        Button<'msg>(attributes)
       
[<Sealed>]
type Entry<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.Entry>, Xamarin.Forms.Entry >> box, attributes)
    interface IView<'msg>
    static member inline init(?text: string, ?textChanged: TextChangedEventArgs -> 'msg) =
        let attributes = []
        let attributes = match text with None -> attributes | Some v -> PropertyNode (ViewAttributes.InputViewText.Value(v))::attributes
        let attributes = match textChanged with None -> attributes | Some v -> EventNode (ViewAttributes.InputViewTextChanged.Value(EventHandler<TextChangedEventArgs>(fun _ args -> v args |> RunnerDispatch<'msg>.DispatchViaThunk)))::attributes
        Entry<'msg>(attributes)
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewHorizontalOptions.Value(options))::x.Attributes
        Entry<'msg>(attributes)
    
    member inline x.verticalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewVerticalOptions.Value(options))::x.Attributes
        Entry<'msg>(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = PropertyNode (ViewAttributes.GridColumn.Value(column))::x.Attributes
        Entry<'msg>(attributes)
          
[<Sealed>]  
type Label<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.Label>, Xamarin.Forms.Label >> box, attributes)
    interface IView<'msg>
    static member inline init(text: string) =
        let attributes = [PropertyNode (ViewAttributes.LabelText.Value(text))]
        Label<'msg>(attributes)
        
    member inline x.font(fontSize: NamedSize) =
        let attributes = PropertyNode (ViewAttributes.LabelFontSize.Value(Device.GetNamedSize(fontSize, typeof<Label>)))::x.Attributes
        Label<'msg>(attributes)
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewHorizontalOptions.Value(options))::x.Attributes
        Label<'msg>(attributes)
    
    member inline x.verticalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewVerticalOptions.Value(options))::x.Attributes
        Label<'msg>(attributes)
        
    member inline x.textColor(color: Color) =
        let attributes = PropertyNode (ViewAttributes.LabelTextColor.Value(color))::x.Attributes
        Label<'msg>(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = PropertyNode (ViewAttributes.GridColumn.Value(column))::x.Attributes
        Label<'msg>(attributes)
        
[<Sealed>]
type ListView<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.Label>, Xamarin.Forms.Label >> box, attributes)
    interface IView<'msg>
    static member inline init(items: #ICell<'msg> list) =
        let attributes = [PropertyNode (ViewAttributes.ItemsViewOfTItemsSource.Value(items))]
        ListView<'msg>(attributes)
    
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewHorizontalOptions.Value(options))::x.Attributes
        ListView<'msg>(attributes)
    
    member inline x.verticalOptions(options: LayoutOptions) =
        let attributes = PropertyNode (ViewAttributes.ViewVerticalOptions.Value(options))::x.Attributes
        ListView<'msg>(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = PropertyNode (ViewAttributes.GridColumn.Value(column))::x.Attributes
        ListView<'msg>(attributes)
        
[<Sealed>]    
type TextCell<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.TextCell>, Xamarin.Forms.TextCell >> box, attributes)
    interface ICell<'msg>
    static member inline init(text: string) =
        let attributes = [PropertyNode (ViewAttributes.TextCellText.Value(text))]
        TextCell<'msg>(attributes)
        
    member inline x.contextActions(actions: IMenuItem<'msg> list) =
        let attributes = PropertyNode (ViewAttributes.CellContextActions.Value(actions))::x.Attributes
        TextCell<'msg>(attributes)        
        
[<Sealed>]
type MenuItem<'msg>(attributes: Attribute list) =
    inherit DynamicViewElement(typeof<Xamarin.Forms.MenuItem>, Xamarin.Forms.MenuItem >> box, attributes)
    interface IMenuItem<'msg>
    static member inline init(?text: string, ?clicked: 'msg) =
        let attributes = []
        let attributes = match text with None -> attributes | Some v -> PropertyNode (ViewAttributes.MenuItemText.Value(v))::attributes
        let attributes = match clicked with None -> attributes | Some v -> EventNode (ViewAttributes.MenuItemClicked.Value(v))::attributes
        MenuItem<'msg>(attributes)
        
        
[<AbstractClass; Sealed>]
type View private () =
    static member inline ContentPage(content) = ContentPage.init(content)
    static member inline Grid(?coldefs,?rowdefs) = Grid.init (?coldefs=coldefs,?rowdefs=rowdefs)
    static member inline StackLayout(children,?spacing) = StackLayout.init(children,?spacing=spacing)
    static member inline Button(?text,?clicked) = Button.init(?text=text,?clicked=clicked)
    static member inline Entry(?text,?textChanged) = Entry.init(?text=text,?textChanged=textChanged)
    static member inline Label(text) = Label.init text
    static member inline ListView(items) = ListView.init(items)
    static member inline TextCell(text) = TextCell.init text
    static member inline MenuItem(?text,?clicked) = MenuItem.init(?text=text,?clicked=clicked)