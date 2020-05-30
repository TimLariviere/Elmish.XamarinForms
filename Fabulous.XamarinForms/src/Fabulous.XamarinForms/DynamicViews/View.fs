// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.DynamicViews

open System
open System.Collections.Generic
open System.ComponentModel
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.DynamicViews.Attributes
open Xamarin.Forms

module ViewAttributes =
    let ContentPageContent = Attributes.ViewElement.bindableProperty ContentPage.ContentProperty
    let ViewHorizontalOptions = Attributes.Bindable.property View.HorizontalOptionsProperty
    let ViewVerticalOptions = Attributes.Bindable.property View.VerticalOptionsProperty
    let LayoutOfTChildren = Attributes.ViewElement.collection<Layout<_>, _> (fun t -> t.Children)
    let GridColumnDefinitions = Attributes.Scalar.collection<Grid, _> (fun t -> t.ColumnDefinitions :> IList<_>)
    let GridRowDefinitions = Attributes.Scalar.collection<Grid, _> (fun t -> t.RowDefinitions :> IList<_>)
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
    let ItemsViewOfTItemTemplate = Attributes.ViewElement.bindableTemplate ItemsView.ItemTemplateProperty
    let CellContextActions = Attributes.Scalar.collection<Cell, _> (fun t -> t.ContextActions)
    let TextCellText = Attributes.Bindable.property TextCell.TextProperty
    let MenuItemText = Attributes.Bindable.property MenuItem.TextProperty
    let MenuItemClicked = Attributes.Event.handler<MenuItem> (fun t -> t.Clicked)
    
[<Struct>]
type ContentPage<'msg>(events: (DynamicEvent * DynamicEventValue) list, properties: (DynamicProperty * obj) list) =
    interface IPage<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.ContentPage>, Xamarin.Forms.ContentPage >> box, readOnlyDict events, readOnlyDict properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(content: IView<'msg>) =
        let properties = [ ViewAttributes.ContentPageContent.Value(content.AsViewElement()) ]
        ContentPage<'msg>([], properties)
        
[<Struct>]
type Grid<'msg>(events: (DynamicEvent * DynamicEventValue) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with        
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Grid>, Xamarin.Forms.Grid >> box, readOnlyDict events, readOnlyDict properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(children: seq<IView<'msg>>, ?coldefs: GridDefinition list, ?rowdefs: GridDefinition list) =
        let properties = [ ViewAttributes.LayoutOfTChildren.Value(children |> Seq.map (fun c -> c.AsViewElement()) |> Seq.toArray) ]
        let properties = match coldefs with None -> properties | Some v -> ViewAttributes.GridColumnDefinitions.Value(v)::properties
        let properties = match rowdefs with None -> properties | Some v -> ViewAttributes.GridRowDefinitions.Value(v)::properties
        Grid<'msg>([], properties)
            
    member inline x.gridColumn(column: int) =
        let properties = (ViewAttributes.GridColumn.Value(column))::x.Properties
        Grid<'msg>(x.Events, properties)
        
[<Struct>]
type StackLayout<'msg>(events: (DynamicEvent * DynamicEventValue) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.StackLayout>, Xamarin.Forms.StackLayout >> box, readOnlyDict events, readOnlyDict properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(children: IView<'msg> list, ?spacing: double) =
        let properties = [ ViewAttributes.LayoutOfTChildren.Value(children |> List.map (fun x -> x.AsViewElement()) |> Array.ofList) ]
        let properties = match spacing with None -> properties | Some v -> ViewAttributes.StackLayoutSpacing.Value(v)::properties
        StackLayout<'msg>([], properties)
    
    member inline x.horizontalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewHorizontalOptions.Value(options)::x.Properties
        StackLayout<'msg>(x.Events, properties)
    
    member inline x.verticalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewVerticalOptions.Value(options)::x.Properties
        StackLayout<'msg>(x.Events, properties)
            
    member inline x.gridColumn(column: int) =
        let properties = ViewAttributes.GridColumn.Value(column)::x.Properties
        StackLayout<'msg>(x.Events, properties)
        
[<Struct>]
type Button<'msg>(events: (DynamicEvent * DynamicEventValue) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Button>, Xamarin.Forms.Button >> box, readOnlyDict events, readOnlyDict properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(?text: string, ?clicked: 'msg) =
        let properties = match text with None -> [] | Some v -> [ ViewAttributes.ButtonText.Value(v) ]
        let events = match clicked with None -> [] | Some v -> [ ViewAttributes.ButtonClicked.Value(fun dispatch -> EventHandler(fun _ _ -> dispatch v) :> obj) ]
        Button<'msg>(events, properties)
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewHorizontalOptions.Value(options)::x.Properties
        Button<'msg>(x.Events, properties)
    
    member inline x.verticalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewVerticalOptions.Value(options)::x.Properties
        Button<'msg>(x.Events, properties)
            
    member inline x.gridColumn(column: int) =
        let properties = ViewAttributes.GridColumn.Value(column)::x.Properties
        Button<'msg>(x.Events, properties)
       
[<Struct>]
type Entry<'msg>(events: (DynamicEvent * DynamicEventValue) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with        
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Entry>, Xamarin.Forms.Entry >> box, readOnlyDict events, readOnlyDict properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(?text: string, ?textChanged: TextChangedEventArgs -> 'msg) =
        let properties = match text with None -> [] | Some v -> [ ViewAttributes.InputViewText.Value(v) ]
        let events = match textChanged with None -> [] | Some v -> [ ViewAttributes.InputViewTextChanged.Value(fun dispatch -> EventHandler<TextChangedEventArgs>(fun _ args -> v args |> dispatch) :> obj) ]
        Entry<'msg>(events, properties)
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewHorizontalOptions.Value(options)::x.Properties
        Entry<'msg>(x.Events, properties)
    
    member inline x.verticalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewVerticalOptions.Value(options)::x.Properties
        Entry<'msg>(x.Events, properties)
            
    member inline x.gridColumn(column: int) =
        let properties = ViewAttributes.GridColumn.Value(column)::x.Properties
        Entry<'msg>(x.Events, properties)
          
[<Struct>]  
type Label<'msg>(events: (DynamicEvent * DynamicEventValue) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Label>, Xamarin.Forms.Label >> box, readOnlyDict events, readOnlyDict properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(text: string) =
        let properties = [ ViewAttributes.LabelText.Value(text) ]
        Label<'msg>([], properties)
        
    member inline x.font(fontSize: NamedSize) =
        let properties = ViewAttributes.LabelFontSize.Value(Device.GetNamedSize(fontSize, typeof<Label>))::x.Properties
        Label<'msg>(x.Events, properties)
            
    member inline x.horizontalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewHorizontalOptions.Value(options)::x.Properties
        Label<'msg>(x.Events, properties)
    
    member inline x.verticalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewVerticalOptions.Value(options)::x.Properties
        Label<'msg>(x.Events, properties)
        
    member inline x.textColor(color: Color) =
        let properties = ViewAttributes.LabelTextColor.Value(color)::x.Properties
        Label<'msg>(x.Events, properties)
            
    member inline x.gridColumn(column: int) =
        let properties = ViewAttributes.GridColumn.Value(column)::x.Properties
        Label<'msg>(x.Events, properties)
        
[<Struct>]
type ListView<'msg>(events: (DynamicEvent * DynamicEventValue) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.ListView>, Xamarin.Forms.ListView >> box, readOnlyDict events, readOnlyDict properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(items: seq<#ICell<'msg>>) =
        let properties = [ ViewAttributes.ItemsViewOfTItemsSource.Value(items |> Seq.map (fun c -> c.AsViewElement()) |> Seq.toArray) ]
        ListView<'msg>([], properties)
    
    member inline x.horizontalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewHorizontalOptions.Value(options)::x.Properties
        ListView<'msg>(x.Events, properties)
    
    member inline x.verticalOptions(options: LayoutOptions) =
        let properties = ViewAttributes.ViewVerticalOptions.Value(options)::x.Properties
        ListView<'msg>(x.Events, properties)
            
    member inline x.gridColumn(column: int) =
        let properties = ViewAttributes.GridColumn.Value(column)::x.Properties
        ListView<'msg>(x.Events, properties)
        
[<Struct>]    
type TextCell<'msg>(events: (DynamicEvent * DynamicEventValue) list, properties: (DynamicProperty * obj) list) =
    interface ICell<'msg> with
        member x.AsViewElement() =
                DynamicViewElement(typeof<Xamarin.Forms.TextCell>, Xamarin.Forms.TextCell >> box, readOnlyDict events, readOnlyDict properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(text: string) =
        let properties = [ ViewAttributes.TextCellText.Value(text) ]
        TextCell<'msg>([], properties)
        
    member inline x.contextActions(actions: seq<IMenuItem<'msg>>) =
        let properties = ViewAttributes.CellContextActions.Value(actions |> Seq.map (fun a -> a.AsViewElement()) |> Seq.toArray)::x.Properties
        TextCell<'msg>([], properties)
        
[<Struct>]
type MenuItem<'msg>(events: (DynamicEvent * DynamicEventValue) list, properties: (DynamicProperty * obj) list) =
    interface IMenuItem<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.MenuItem>, Xamarin.Forms.MenuItem >> box, readOnlyDict events, readOnlyDict properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(?text: string, ?clicked: 'msg) =
        let properties = match text with None -> [] | Some v -> [ ViewAttributes.MenuItemText.Value(v) ]
        let events = match clicked with None -> [] | Some v -> [ ViewAttributes.MenuItemClicked.Value(fun dispatch -> EventHandler(fun _ _ -> dispatch v) :> obj) ]
        MenuItem<'msg>(events, properties)
        
[<AbstractClass; Sealed>]
type View private () =
    static member inline ContentPage(content) = ContentPage.init(content)
    static member inline Grid(children, ?coldefs,?rowdefs) = Grid.init (children, ?coldefs=coldefs,?rowdefs=rowdefs)
    static member inline StackLayout(children,?spacing) = StackLayout.init(children,?spacing=spacing)
    static member inline Button(?text,?clicked) = Button.init(?text=text,?clicked=clicked)
    static member inline Entry(?text,?textChanged) = Entry.init(?text=text,?textChanged=textChanged)
    static member inline Label(text) = Label.init text
    static member inline ListView(items) = ListView.init(items)
    static member inline TextCell(text) = TextCell.init text
    static member inline MenuItem(?text,?clicked) = MenuItem.init(?text=text,?clicked=clicked)