// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.DynamicViews

open System
open System.Collections.Generic
open Fabulous.DynamicViews
open Fabulous.XamarinForms
open Xamarin.Forms

module ViewAttributes =
    let ContentPageContent = "ContentPageContent", ViewElementProperty.Bindable ContentPage.ContentProperty
    let ViewHorizontalOptions = "ViewHorizontalOptions", Bindable.Property View.HorizontalOptionsProperty
    let LayoutOfTChildren = "LayoutOfTChildren", ViewElementCollection<_>.Instance (fun t -> (t :?> Layout<'T>).Children)
    let GridColumnDefinitions = "GridColumnDefinitions", Collection.Instance (fun t -> (t :?> Grid).ColumnDefinitions :> IList<_>)
    let GridRowDefinitions = "GridRowDefinitions", Collection.Instance (fun t -> (t :?> Grid).RowDefinitions :> IList<_>)
    let GridColumn = "GridColumn", Bindable.Property Grid.ColumnProperty
    let GridRow = "GridRow", Bindable.Property Grid.RowProperty
    let StackLayoutSpacing = "StackLayoutSpacing", Bindable.Property StackLayout.SpacingProperty
    let ButtonText = "ButtonText", Bindable.Property Button.TextProperty
    let ButtonClicked = "ButtonClicked", Event'.Handler (fun t -> (t :?> Button).Clicked)
    let LabelFontSize = "LabelFontSize", Bindable.Property Label.FontSizeProperty
    let LabelText = "LabelText", Bindable.Property Label.TextProperty
    let LabelTextColor = "LabelTextColor", Bindable.Property Label.TextColorProperty
    let InputViewText = "InputViewText", Bindable.Property InputView.TextProperty
    let InputViewTextChanged = "InputViewTextChanged", EventOf<_>.Handler (fun t -> (t :?> InputView).TextChanged)
    let ItemsViewOfTItemsSource = "ItemsViewOfTItemsSource", Bindable.Collection ItemsView.ItemsSourceProperty
    let ItemsViewOfTItemTemplate = "ItemsViewOfTItemTemplate", ViewElementProperty.BindableTemplate ItemsView.ItemTemplateProperty
    let CellContextActions = "CellContextActions", Collection.Instance (fun t -> (t :?> Cell).ContextActions)
    let TextCellText = "TextCellText", Bindable.Property TextCell.TextProperty
    let MenuItemText = "MenuItemText", Bindable.Property MenuItem.TextProperty
    let MenuItemClicked = "MenuItemClicked", Event'.Handler (fun t -> (t :?> MenuItem).Clicked)
    
        
type ContentPage (attributes: AttributesList) =
    inherit DynamicViewElement<Xamarin.Forms.ContentPage>(Xamarin.Forms.ContentPage, attributes)
    interface IPage
    static member inline init (content: IView) =
        let attributes = Value (Attribute.ofViewElementProperty ViewAttributes.ContentPageContent content, Empty)
        ContentPage(attributes)
        
type Grid (attributes: AttributesList) =
    inherit DynamicViewElement<Xamarin.Forms.Grid>(Xamarin.Forms.Grid, attributes)
    interface IView
    static member inline init(?coldefs: GridDefinition list, ?rowdefs: GridDefinition list) =
        let attributes = Empty
        let attributes = match coldefs with None -> attributes | Some v -> Value (Attribute.ofCollectionOfT ViewAttributes.GridColumnDefinitions v, attributes)
        let attributes = match rowdefs with None -> attributes | Some v -> Value (Attribute.ofCollectionOfT ViewAttributes.GridColumnDefinitions v, attributes)
        fun (children: IView list) ->
            let attributes = match children with [] -> attributes | v -> Value (Attribute.ofViewElementCollection ViewAttributes.LayoutOfTChildren v, attributes)
            Grid(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.GridColumn column, x.Attributes)
        Grid(attributes)
        
type StackLayout (attributes: AttributesList) =
    inherit DynamicViewElement<Xamarin.Forms.StackLayout>(Xamarin.Forms.StackLayout, attributes)
    interface IView
    static member inline init(?spacing: double) =
        let attributes = Empty
        let attributes = match spacing with None -> attributes | Some v -> Value (Attribute.ofBindable ViewAttributes.StackLayoutSpacing v, attributes)
        fun (children: IView list) ->
            let attributes = match children with [] -> attributes | v -> Value (Attribute.ofViewElementCollection ViewAttributes.LayoutOfTChildren v, attributes)
            StackLayout(attributes)
    
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.ViewHorizontalOptions options, x.Attributes)
        StackLayout(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.GridColumn column, x.Attributes)
        StackLayout(attributes)
        
type Button (attributes: AttributesList) =
    inherit DynamicViewElement<Xamarin.Forms.Button>(Xamarin.Forms.Button, attributes)
    interface IView
    static member inline init(?text: string, ?clicked: unit -> unit) =
        let attributes = Empty
        let attributes = match text with None -> attributes | Some v -> Value (Attribute.ofBindable ViewAttributes.ButtonText v, attributes)
        let attributes = match clicked with None -> attributes | Some v -> Value (Attribute.ofEvent ViewAttributes.ButtonClicked v, attributes)
        Button(attributes)
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.ViewHorizontalOptions options, x.Attributes)
        Button(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.GridColumn column, x.Attributes)
        Button(attributes)
       
type Entry (attributes: AttributesList) =
    inherit DynamicViewElement<Xamarin.Forms.Entry>(Xamarin.Forms.Entry, attributes)
    interface IView
    static member inline init(?text: string, ?textChanged: TextChangedEventArgs -> unit) =
        let attributes = Empty
        let attributes = match text with None -> attributes | Some v -> Value (Attribute.ofBindable ViewAttributes.InputViewText v, attributes)
        let attributes = match textChanged with None -> attributes | Some v -> Value (Attribute.ofEventOfT ViewAttributes.InputViewTextChanged v, attributes)
        Entry(attributes)
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.ViewHorizontalOptions options, x.Attributes)
        Entry(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.GridColumn column, x.Attributes)
        Entry(attributes)
            
type Label (attributes: AttributesList) =
    inherit DynamicViewElement<Xamarin.Forms.Label>(Xamarin.Forms.Label, attributes)
    interface IView
    static member inline init(text: string) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.LabelText text, Empty)
        Label(attributes)
        
    member inline x.font(?fontSize: NamedSize) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.LabelFontSize fontSize, x.Attributes)
        Label(attributes)
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.ViewHorizontalOptions options, x.Attributes)
        Label(attributes)
        
    member inline x.textColor(?color: Color) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.LabelTextColor color, x.Attributes)
        Label(attributes)
            
    member inline x.gridColumn(column: int) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.GridColumn column, x.Attributes)
        Label(attributes)
        
type ListView (attributes: AttributesList) =
    inherit DynamicViewElement<Xamarin.Forms.ListView>(Xamarin.Forms.ListView, attributes)
    interface IView
    static member inline init(items: 'T seq) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.ItemsViewOfTItemsSource items, Empty)
        fun (template: 'T -> #ICell) ->
            let attributes = Value (Attribute.ofViewElementProperty ViewAttributes.ItemsViewOfTItemTemplate template, attributes)
            ListView(attributes)
            
type TextCell (attributes: AttributesList) =
    inherit DynamicViewElement<Xamarin.Forms.TextCell>(Xamarin.Forms.TextCell, attributes)
    interface ICell
    static member inline init(text: string) =
        let attributes = Value (Attribute.ofBindable ViewAttributes.TextCellText text, Empty)
        TextCell(attributes)
        
    member inline x.contextActions(actions: IMenuItem list) =
        let attributes = Value (Attribute.ofCollectionOfT ViewAttributes.CellContextActions actions, x.Attributes)
        TextCell(attributes)        
        
type MenuItem (attributes: AttributesList) =
    inherit DynamicViewElement<Xamarin.Forms.MenuItem>(Xamarin.Forms.MenuItem, attributes)
    interface IMenuItem
    static member inline init(?text: string, ?clicked: unit -> unit) =
        let attributes = Empty
        let attributes = match text with None -> attributes | Some v -> Value (Attribute.ofBindable ViewAttributes.MenuItemText v, attributes)
        let attributes = match clicked with None -> attributes | Some v -> Value (Attribute.ofEvent ViewAttributes.MenuItemClicked v, attributes)
        MenuItem(attributes)
        
        
[<AbstractClass; Sealed>]
type View private () =
    static member inline ContentPage(content) = ContentPage.init content
    static member inline Grid(?coldefs,?rowdefs) = Grid.init (?coldefs=coldefs,?rowdefs=rowdefs)
    static member inline StackLayout(?spacing: double) = StackLayout.init(?spacing=spacing)
    static member inline Button(?text,?clicked) = Button.init(?text=text,?clicked=clicked)
    static member inline Entry(?text,?textChanged) = Entry.init(?text=text,?textChanged=textChanged)
    static member inline Label(text) = Label.init text
    static member inline ListView(items) = ListView.init items
    static member inline TextCell(text) = TextCell.init text
    static member inline MenuItem(?text,?clicked) = MenuItem.init(?text=text,?clicked=clicked)