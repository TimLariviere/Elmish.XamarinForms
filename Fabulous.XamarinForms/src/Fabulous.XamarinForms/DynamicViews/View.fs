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
    let ElementAutomationId = Attributes.Bindable.property Element.AutomationIdProperty
    let VisualElementIsEnabled = Attributes.Bindable.property VisualElement.IsEnabledProperty
    let VisualElementWidthRequest = Attributes.Bindable.property VisualElement.WidthRequestProperty
    let VisualElementHeightRequest = Attributes.Bindable.property VisualElement.HeightRequestProperty
    let ContentPageContent = Attributes.ViewElement.bindableProperty ContentPage.ContentProperty
    let ViewHorizontalOptions = Attributes.Bindable.property View.HorizontalOptionsProperty
    let ViewVerticalOptions = Attributes.Bindable.property View.VerticalOptionsProperty
    let LayoutPadding = Attributes.Bindable.property Layout.PaddingProperty
    let LayoutOfTChildren = Attributes.ViewElement.collection<Layout<_>, _> (fun t -> t.Children)
    let GridColumnDefinitions = Attributes.Scalar.collection<Grid, _> (fun t -> t.ColumnDefinitions :> IList<_>)
    let GridRowDefinitions = Attributes.Scalar.collection<Grid, _> (fun t -> t.RowDefinitions :> IList<_>)
    let GridColumn = Attributes.Bindable.property Grid.ColumnProperty
    let GridRow = Attributes.Bindable.property Grid.RowProperty
    let StackLayoutOrientation = Attributes.Bindable.property StackLayout.OrientationProperty
    let StackLayoutSpacing = Attributes.Bindable.property StackLayout.SpacingProperty
    let ButtonText = Attributes.Bindable.property Button.TextProperty
    let ButtonClicked = Attributes.Event.handler<Button> (fun t -> t.Clicked)
    let LabelFontSize = Attributes.Bindable.property Label.FontSizeProperty
    let LabelText = Attributes.Bindable.property Label.TextProperty
    let LabelHorizontalTextAlignment = Attributes.Bindable.property Label.HorizontalTextAlignmentProperty
    let LabelVerticalTextAlignment = Attributes.Bindable.property Label.VerticalTextAlignmentProperty
    let LabelTextColor = Attributes.Bindable.property Label.TextColorProperty
    let InputViewText = Attributes.Bindable.property InputView.TextProperty
    let InputViewTextChanged = Attributes.Event.handlerOf<InputView, _> (fun t -> t.TextChanged)
    let EntryCompleted = Attributes.Event.handler<Entry> (fun t -> t.Completed)
    let SliderMaximum = Attributes.Bindable.property Slider.MaximumProperty
    let SliderMinimum = Attributes.Bindable.property Slider.MinimumProperty
    let SliderValue = Attributes.Bindable.property Slider.ValueProperty
    let SliderValueChanged = Attributes.Event.handlerOf<Slider, _> (fun t -> t.ValueChanged)
    let SwitchIsToggled = Attributes.Bindable.property Switch.IsToggledProperty
    let SwitchToggled = Attributes.Event.handlerOf<Switch, _> (fun t -> t.Toggled)
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
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        ContentPage<'msg>(x.Events, properties)
            
    member inline x.size(?width: double, ?height: double) =
        let properties = x.Properties
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::x.Properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::x.Properties
        ContentPage<'msg>(x.Events, properties)
    
    member inline x.isEnabled(isEnabled: bool) =
        let properties = ViewAttributes.VisualElementIsEnabled.Value(isEnabled)::x.Properties
        ContentPage<'msg>(x.Events, properties)
        
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
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        Grid<'msg>(x.Events, properties)
            
    member inline x.size(?width: double, ?height: double) =
        let properties = x.Properties
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::x.Properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::x.Properties
        Grid<'msg>(x.Events, properties)
    
    member inline x.alignment(?horizontal: LayoutOptions, ?vertical: LayoutOptions) =
        let properties = x.Properties
        let properties = match horizontal with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        let properties = match vertical with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        Grid<'msg>(x.Events, properties)
    
    member inline x.isEnabled(isEnabled: bool) =
        let properties = ViewAttributes.VisualElementIsEnabled.Value(isEnabled)::x.Properties
        Grid<'msg>(x.Events, properties)
        
    member inline x.padding(uniform: double) =
        let properties = ViewAttributes.LayoutPadding.Value(Thickness uniform)::x.Properties
        Grid<'msg>(x.Events, properties)
            
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
    
    static member inline init(children: IView<'msg> list, ?orientation: StackOrientation, ?spacing: double) =
        let properties = [ ViewAttributes.LayoutOfTChildren.Value(children |> List.map (fun x -> x.AsViewElement()) |> Array.ofList) ]
        let properties = match orientation with None -> properties | Some v -> ViewAttributes.StackLayoutOrientation.Value(v)::properties
        let properties = match spacing with None -> properties | Some v -> ViewAttributes.StackLayoutSpacing.Value(v)::properties
        StackLayout<'msg>([], properties)
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        StackLayout<'msg>(x.Events, properties)
            
    member inline x.size(?width: double, ?height: double) =
        let properties = x.Properties
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::properties
        StackLayout<'msg>(x.Events, properties)
    
    member inline x.alignment(?horizontal: LayoutOptions, ?vertical: LayoutOptions) =
        let properties = x.Properties
        let properties = match horizontal with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        let properties = match vertical with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        StackLayout<'msg>(x.Events, properties)
    
    member inline x.isEnabled(isEnabled: bool) =
        let properties = ViewAttributes.VisualElementIsEnabled.Value(isEnabled)::x.Properties
        StackLayout<'msg>(x.Events, properties)
        
    member inline x.padding(uniform: double) =
        let properties = ViewAttributes.LayoutPadding.Value(Thickness uniform)::x.Properties
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
    
    static member inline init(text: string, clicked: 'msg) =
        let properties = [ViewAttributes.ButtonText.Value(text)]
        let events = [ ViewAttributes.ButtonClicked.Value(fun dispatch -> EventHandler(fun _ _ -> dispatch clicked) :> obj) ]
        Button<'msg>(events, properties)
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        Button<'msg>(x.Events, properties)
            
    member inline x.size(?width: double, ?height: double) =
        let properties = x.Properties
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::x.Properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::x.Properties
        Button<'msg>(x.Events, properties)
    
    member inline x.alignment(?horizontal: LayoutOptions, ?vertical: LayoutOptions) =
        let properties = x.Properties
        let properties = match horizontal with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        let properties = match vertical with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        Button<'msg>(x.Events, properties)
    
    member inline x.isEnabled(isEnabled: bool) =
        let properties = ViewAttributes.VisualElementIsEnabled.Value(isEnabled)::x.Properties
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
    
    static member inline init(text: string, textChanged: TextChangedEventArgs -> 'msg) =
        let properties = [ ViewAttributes.InputViewText.Value(text) ]
        let events = [ ViewAttributes.InputViewTextChanged.Value(fun dispatch -> EventHandler<TextChangedEventArgs>(fun _ args -> textChanged args |> dispatch) :> obj) ]
        Entry<'msg>(events, properties)
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        Entry<'msg>(x.Events, properties)
            
    member inline x.size(?width: double, ?height: double) =
        let properties = x.Properties
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::x.Properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::x.Properties
        Entry<'msg>(x.Events, properties)
    
    member inline x.alignment(?horizontal: LayoutOptions, ?vertical: LayoutOptions) =
        let properties = x.Properties
        let properties = match horizontal with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        let properties = match vertical with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        Entry<'msg>(x.Events, properties)
    
    member inline x.isEnabled(isEnabled: bool) =
        let properties = ViewAttributes.VisualElementIsEnabled.Value(isEnabled)::x.Properties
        Entry<'msg>(x.Events, properties)
            
    member inline x.gridColumn(column: int) =
        let properties = ViewAttributes.GridColumn.Value(column)::x.Properties
        Entry<'msg>(x.Events, properties)
        
    member inline x.onCompleted(msg: 'msg) =
        let events = ViewAttributes.EntryCompleted.Value(fun dispatch -> EventHandler(fun _ _ -> dispatch msg) :> obj)::x.Events
        Entry<'msg>(events, x.Properties)
          
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
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        Label<'msg>(x.Events, properties)
            
    member inline x.size(?width: double, ?height: double) =
        let properties = x.Properties
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::x.Properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::x.Properties
        Label<'msg>(x.Events, properties)
    
    member inline x.alignment(?horizontal: LayoutOptions, ?vertical: LayoutOptions) =
        let properties = x.Properties
        let properties = match horizontal with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        let properties = match vertical with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        Label<'msg>(x.Events, properties)
    
    member inline x.isEnabled(isEnabled: bool) =
        let properties = ViewAttributes.VisualElementIsEnabled.Value(isEnabled)::x.Properties
        Label<'msg>(x.Events, properties)
        
    member inline x.font(fontSize: NamedSize) =
        let properties = ViewAttributes.LabelFontSize.Value(Device.GetNamedSize(fontSize, typeof<Label>))::x.Properties
        Label<'msg>(x.Events, properties)
        
    member inline x.textAlignment(?horizontal: TextAlignment, ?vertical: TextAlignment) =
        let properties = x.Properties
        let properties = match horizontal with None -> properties | Some v -> ViewAttributes.LabelHorizontalTextAlignment.Value(v)::properties
        let properties = match vertical with None -> properties | Some v -> ViewAttributes.LabelVerticalTextAlignment.Value(v)::properties
        Label<'msg>(x.Events, properties)
        
    member inline x.textColor(color: Color) =
        let properties = ViewAttributes.LabelTextColor.Value(color)::x.Properties
        Label<'msg>(x.Events, properties)
            
    member inline x.gridColumn(column: int) =
        let properties = ViewAttributes.GridColumn.Value(column)::x.Properties
        Label<'msg>(x.Events, properties)
        
[<Struct>]
type Slider<'msg>(events: (DynamicEvent * DynamicEventValue) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Slider>, Xamarin.Forms.Slider >> box, readOnlyDict events, readOnlyDict properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
            
    static member inline init(value: double, valueChanged: ValueChangedEventArgs -> 'msg) =
        let properties = [ViewAttributes.SliderValue.Value(value)] 
        let events = [ViewAttributes.SliderValueChanged.Value(fun dispatch -> EventHandler<ValueChangedEventArgs>(fun _ args -> dispatch (valueChanged args)) :> obj)]
        Slider<'msg>(events, properties)
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        Slider<'msg>(x.Events, properties)
            
    member inline x.size(?width: double, ?height: double) =
        let properties = x.Properties
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::x.Properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::x.Properties
        Slider<'msg>(x.Events, properties)
    
    member inline x.alignment(?horizontal: LayoutOptions, ?vertical: LayoutOptions) =
        let properties = x.Properties
        let properties = match horizontal with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        let properties = match vertical with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        Slider<'msg>(x.Events, properties)
    
    member inline x.isEnabled(isEnabled: bool) =
        let properties = ViewAttributes.VisualElementIsEnabled.Value(isEnabled)::x.Properties
        Slider<'msg>(x.Events, properties)
        
    member inline x.range(min: double, max: double) =
        let properties = x.Properties
        let properties = ViewAttributes.SliderMinimum.Value(min)::properties
        let properties = ViewAttributes.SliderMaximum.Value(max)::properties
        Slider<'msg>(x.Events, properties)
        
[<Struct>]
type Switch<'msg>(events: (DynamicEvent * DynamicEventValue) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Switch>, Xamarin.Forms.Switch >> box, readOnlyDict events, readOnlyDict properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
            
    static member inline init(isToggled: bool, toggled: ToggledEventArgs -> 'msg) =
        let properties = [ViewAttributes.SwitchIsToggled.Value(isToggled)] 
        let events = [ViewAttributes.SwitchToggled.Value(fun dispatch -> EventHandler<ToggledEventArgs>(fun _ args -> dispatch (toggled args)) :> obj)]
        Switch<'msg>(events, properties)
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        Switch<'msg>(x.Events, properties)
            
    member inline x.size(?width: double, ?height: double) =
        let properties = x.Properties
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::x.Properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::x.Properties
        Switch<'msg>(x.Events, properties)
    
    member inline x.alignment(?horizontal: LayoutOptions, ?vertical: LayoutOptions) =
        let properties = x.Properties
        let properties = match horizontal with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        let properties = match vertical with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        Switch<'msg>(x.Events, properties)
    
    member inline x.isEnabled(isEnabled: bool) =
        let properties = ViewAttributes.VisualElementIsEnabled.Value(isEnabled)::x.Properties
        Switch<'msg>(x.Events, properties)
        
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
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        ListView<'msg>(x.Events, properties)
            
    member inline x.size(?width: double, ?height: double) =
        let properties = x.Properties
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::x.Properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::x.Properties
        ListView<'msg>(x.Events, properties)
    
    member inline x.alignment(?horizontal: LayoutOptions, ?vertical: LayoutOptions) =
        let properties = x.Properties
        let properties = match horizontal with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        let properties = match vertical with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        ListView<'msg>(x.Events, properties)
    
    member inline x.isEnabled(isEnabled: bool) =
        let properties = ViewAttributes.VisualElementIsEnabled.Value(isEnabled)::x.Properties
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
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        TextCell<'msg>(x.Events, properties)
        
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
    
    static member inline init(text: string, clicked: 'msg) =
        let properties = [ ViewAttributes.MenuItemText.Value(text) ]
        let events = [ ViewAttributes.MenuItemClicked.Value(fun dispatch -> EventHandler(fun _ _ -> dispatch clicked) :> obj) ]
        MenuItem<'msg>(events, properties)
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        MenuItem<'msg>(x.Events, properties)
        
[<AbstractClass; Sealed>]
type View private () =
    static member inline ContentPage(content) = ContentPage.init(content)
    static member inline Grid(children, ?coldefs,?rowdefs) = Grid.init (children, ?coldefs=coldefs,?rowdefs=rowdefs)
    static member inline StackLayout(children,?orientation,?spacing) = StackLayout.init(children,?orientation=orientation,?spacing=spacing)
    static member inline Button(text,clicked) = Button.init(text,clicked)
    static member inline Entry(text,textChanged) = Entry.init(text,textChanged)
    static member inline Label(text) = Label.init text
    static member inline Slider(value,valueChanged) = Slider.init(value,valueChanged)
    static member inline Switch(isToggled,toggled) = Switch.init(isToggled,toggled)
    static member inline ListView(items) = ListView.init(items)
    static member inline TextCell(text) = TextCell.init text
    static member inline MenuItem(text,clicked) = MenuItem.init(text,clicked)