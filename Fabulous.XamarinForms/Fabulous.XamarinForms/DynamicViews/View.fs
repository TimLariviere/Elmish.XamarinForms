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
    let ElementSetMenu = Attributes.ViewElement.scalarProperty null (fun (t: Element) -> Element.GetMenu(t)) (fun (v, t) -> Element.SetMenu(t, v))
    let ApplicationMainPage = Attributes.ViewElement.scalarProperty null (fun (a: Application) -> a.MainPage) (fun (page, app) -> app.MainPage <- page)
    let NavigableElementStyle = Attributes.Bindable.property NavigableElement.StyleProperty
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
    let MenuText = Attributes.Scalar.property "" (fun (v, t: Menu) -> t.Text <- v)
    let MenuSubmenus = Attributes.ViewElement.collection<Menu, _> (fun t -> t :> IList<_>)
    let MenuItems = Attributes.ViewElement.collection<Menu, _> (fun t -> t.Items :> IList<_>)
    let MenuItemText = Attributes.Bindable.property MenuItem.TextProperty
    let MenuItemClicked = Attributes.Event.handler<MenuItem> (fun t -> t.Clicked)
    let MenuItemAccelerator = Attributes.Bindable.property MenuItem.AcceleratorProperty
    let StyleSetters = Attributes.Scalar.collection<Style, _> (fun t -> t.Setters)
    
[<Struct>]
type LabelStyle(key: string, setters: (BindableProperty * obj) list) =
    interface IStyle with
        member x.AsViewElement() =
            let setters = x.Setters |> List.map (fun (k, v) -> Xamarin.Forms.Setter(Property = k, Value = v)) |> List.toArray
            let properties = [ ViewAttributes.StyleSetters.Value(setters) ]
            DynamicViewElement(typeof<Xamarin.Forms.Style>, (fun() -> Xamarin.Forms.Style(typeof<Xamarin.Forms.Label>) |> box), readOnlyDict [], readOnlyDict properties) :> IViewElement
            
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Key = key
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Setters = setters
            
    static member inline init(?key: string) =
        match key with
        | None -> LabelStyle(typeof<Xamarin.Forms.Label>.FullName, [])
        | Some key -> LabelStyle(key, [])
        
    member inline x.textColor(color: Xamarin.Forms.Color) =
        let setters = (Label.TextColorProperty, box color)::x.Setters
        LabelStyle(x.Key, setters)
        
    member inline x.padding(value: float) =
        let setters = (Label.PaddingProperty, box(Thickness(value)))::x.Setters
        LabelStyle(x.Key, setters)
    
[<Struct>]
type ButtonStyle(key: string, setters: (BindableProperty * obj) list) =
    interface IStyle with
        member x.AsViewElement() =
            let setters = x.Setters |> List.map (fun (k, v) -> Xamarin.Forms.Setter(Property = k, Value = v)) |> List.toArray
            let properties = [ ViewAttributes.StyleSetters.Value(setters) ]
            DynamicViewElement(typeof<Xamarin.Forms.Style>, (fun() -> Xamarin.Forms.Style(typeof<Xamarin.Forms.Button>) |> box), readOnlyDict [], readOnlyDict properties) :> IViewElement
            
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Key = key
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Setters = setters
            
    static member inline init(?key: string) =
        match key with
        | None -> ButtonStyle(typeof<Xamarin.Forms.Button>.FullName, [])
        | Some key -> ButtonStyle(key, [])
        
    member inline x.textColor(color: Xamarin.Forms.Color) =
        let setters = (Button.TextColorProperty, box color)::x.Setters
        ButtonStyle(x.Key, setters)
    
[<AbstractClass; Sealed; RequireQualifiedAccess>]
type StyleFor private () =
    static member inline Label(?key) = LabelStyle.init(?key=key)
    static member inline Button(?key) = ButtonStyle.init(?key=key)
    
[<Struct>]
type Application<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IApplication<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Application>, (fun () -> failwithf "Can't create Application"), readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(mainPage: #IPage<'msg>) =
        let properties = [ ViewAttributes.ApplicationMainPage.Value(mainPage.AsViewElement()) ]
        Application<'msg>([], properties)
        
    member inline x.menu(menu: #IMenu<'msg>) =
        let properties = ViewAttributes.ElementSetMenu.Value(menu.AsViewElement())::x.Properties
        Application<'msg>(x.Events, properties)
    
[<Struct>]
type ContentPage<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IPage<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.ContentPage>, Fabulous.XamarinForms.CustomContentPage >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(content: #IView<'msg>) =
        let properties = [ ViewAttributes.ContentPageContent.Value(content.AsViewElement()) ]
        ContentPage<'msg>([], properties)
            
    member inline x.automationId(id: string) =
        let properties = ViewAttributes.ElementAutomationId.Value(id)::x.Properties
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
type Grid<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with        
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Grid>, Xamarin.Forms.Grid >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
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
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::properties
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
type StackLayout<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.StackLayout>, Xamarin.Forms.StackLayout >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
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
type Button<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Button>, Xamarin.Forms.Button >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
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
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::properties
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
type Entry<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with        
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Entry>, Xamarin.Forms.Entry >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
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
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::properties
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
type Label<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Label>, Xamarin.Forms.Label >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(text: string) =
        let properties = [ ViewAttributes.LabelText.Value(text) ]
        Label<'msg>([], properties)
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        Label<'msg>(x.Events, properties)
        
    member inline x.style(style: LabelStyle) =
        let properties = ViewAttributes.NavigableElementStyle.Value((style :> IStyle).AsViewElement())::x.Properties
        Label<'msg>(x.Events, properties)
            
    member inline x.size(?width: double, ?height: double) =
        let properties = x.Properties
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::properties
        Label<'msg>(x.Events, properties)
    
    member inline x.alignment(?horizontal: LayoutOptions, ?vertical: LayoutOptions) =
        let properties = x.Properties
        let properties = match horizontal with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        let properties = match vertical with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        Label<'msg>(x.Events, properties)
    
    member inline x.isEnabled(isEnabled: bool) =
        let properties = ViewAttributes.VisualElementIsEnabled.Value(isEnabled)::x.Properties
        Label<'msg>(x.Events, properties)
        
    member inline x.font(fontSize: double) =
        let properties = ViewAttributes.LabelFontSize.Value(fontSize)::x.Properties
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
type Slider<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Slider>, Xamarin.Forms.Slider >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
            
    static member inline init(value: double, valueChanged: ValueChangedEventArgs -> 'msg, ?range: double * double) =
        let properties = [ViewAttributes.SliderValue.Value(value)]
        let properties = match range with None -> properties | Some (min, max) -> ViewAttributes.SliderMinimum.Value(min)::ViewAttributes.SliderMaximum.Value(max)::properties
        let events = [ViewAttributes.SliderValueChanged.Value(fun dispatch -> EventHandler<ValueChangedEventArgs>(fun _ args -> dispatch (valueChanged args)) :> obj)]
        Slider<'msg>(events, properties)
            
    member inline x.automationId(id: string) =
        let properties = (ViewAttributes.ElementAutomationId.Value(id))::x.Properties
        Slider<'msg>(x.Events, properties)
            
    member inline x.size(?width: double, ?height: double) =
        let properties = x.Properties
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::properties
        Slider<'msg>(x.Events, properties)
    
    member inline x.alignment(?horizontal: LayoutOptions, ?vertical: LayoutOptions) =
        let properties = x.Properties
        let properties = match horizontal with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        let properties = match vertical with None -> properties | Some v -> ViewAttributes.ViewHorizontalOptions.Value(v)::properties
        Slider<'msg>(x.Events, properties)
    
    member inline x.isEnabled(isEnabled: bool) =
        let properties = ViewAttributes.VisualElementIsEnabled.Value(isEnabled)::x.Properties
        Slider<'msg>(x.Events, properties)
        
[<Struct>]
type Switch<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Switch>, Xamarin.Forms.Switch >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
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
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::properties
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
type ListView<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IView<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.ListView>, Xamarin.Forms.ListView >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
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
        let properties = match width with None -> properties | Some v -> (ViewAttributes.VisualElementWidthRequest.Value(v))::properties
        let properties = match height with None -> properties | Some v -> (ViewAttributes.VisualElementHeightRequest.Value(v))::properties
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
type TextCell<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface ICell<'msg> with
        member x.AsViewElement() =
                DynamicViewElement(typeof<Xamarin.Forms.TextCell>, Xamarin.Forms.TextCell >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(text: string) =
        let properties = [ ViewAttributes.TextCellText.Value(text) ]
        TextCell<'msg>([], properties)
            
    member inline x.automationId(id: string) =
        let properties = ViewAttributes.ElementAutomationId.Value(id)::x.Properties
        TextCell<'msg>(x.Events, properties)
        
    member inline x.contextActions(actions: seq<IMenuItem<'msg>>) =
        let properties = ViewAttributes.CellContextActions.Value(actions |> Seq.map (fun a -> a.AsViewElement()) |> Seq.toArray)::x.Properties
        TextCell<'msg>([], properties)
        
[<Struct>]
type MainMenu<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IMenu<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Menu>, Xamarin.Forms.Menu >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(submenus: seq<#IMenu<'msg>>) =
        let properties = [ ViewAttributes.MenuSubmenus.Value(submenus |> Seq.map (fun a -> a.AsViewElement()) |> Seq.toArray) ]
        MainMenu<'msg>([], properties)
        
[<Struct>]
type Menu<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IMenu<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.Menu>, Xamarin.Forms.Menu >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
        
    static member inline init(items: seq<#IMenuItem<'msg>>) =
        let properties = [ ViewAttributes.MenuItems.Value(items |> Seq.map (fun a -> a.AsViewElement()) |> Seq.toArray) ]
        Menu<'msg>([], properties)
    
    static member inline init(text: string, items: seq<#IMenuItem<'msg>>) =
        let properties = [ ViewAttributes.MenuText.Value(text); ViewAttributes.MenuItems.Value(items |> Seq.map (fun a -> a.AsViewElement()) |> Seq.toArray) ]
        Menu<'msg>([], properties)
        
[<Struct>]
type MenuItem<'msg>(events: (DynamicEvent * DynamicEventFunc) list, properties: (DynamicProperty * obj) list) =
    interface IMenuItem<'msg> with
        member x.AsViewElement() =
            DynamicViewElement(typeof<Xamarin.Forms.MenuItem>, Xamarin.Forms.MenuItem >> box, readOnlyDict x.Events, readOnlyDict x.Properties) :> IViewElement
        
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Events = events
    [<EditorBrowsable(EditorBrowsableState.Never)>] member x.Properties = properties
    
    static member inline init(text: string, clicked: 'msg) =
        let properties = [ ViewAttributes.MenuItemText.Value(text) ]
        let events = [ ViewAttributes.MenuItemClicked.Value(fun dispatch -> EventHandler(fun _ _ -> dispatch clicked) :> obj) ]
        MenuItem<'msg>(events, properties)
            
    member inline x.automationId(id: string) =
        let properties = ViewAttributes.ElementAutomationId.Value(id)::x.Properties
        MenuItem<'msg>(x.Events, properties)
        
    member inline x.accelerator(accelerator: string) =
        let properties = ViewAttributes.MenuItemAccelerator.Value(Accelerator.FromString accelerator)::x.Properties
        MenuItem<'msg>(x.Events, properties)        
        
[<AbstractClass; Sealed>]
type View private () =
    static member inline Application(mainPage: #IPage<'msg>) = Application.init(mainPage)
    static member inline ContentPage(content: #IView<'msg>) = ContentPage.init(content)
    static member inline Grid(children, ?coldefs,?rowdefs) = Grid.init (children, ?coldefs=coldefs,?rowdefs=rowdefs)
    static member inline StackLayout(children,?orientation,?spacing) = StackLayout.init(children,?orientation=orientation,?spacing=spacing)
    static member inline Button(text,clicked) = Button.init(text,clicked)
    static member inline Entry(text,textChanged) = Entry.init(text,textChanged)
    static member inline Label(text) = Label.init text
    static member inline Slider(value,valueChanged,?range) = Slider.init(value,valueChanged,?range=range)
    static member inline Switch(isToggled,toggled) = Switch.init(isToggled,toggled)
    static member inline ListView(items) = ListView.init(items)
    static member inline TextCell(text) = TextCell.init text
    static member inline MainMenu(submenus) = MainMenu.init(submenus)
    static member inline Menu(items) = Menu.init(items)
    static member inline Menu(text,items) = Menu.init(text,items)
    static member inline MenuItem(text,clicked) = MenuItem.init(text,clicked)
    
module UnitTests =
    type ButtonViewer(element: IViewElement) =
        let dynamicElement = match element with :? DynamicViewElement as dyn -> dyn | _ -> failwithf "A DynamicViewElement was expected"
        do if not ((typeof<Xamarin.Forms.Button>).IsAssignableFrom(dynamicElement.TargetType)) then failwithf "A DynamicViewElement assignable to type 'Xamarin.Forms.Button' is expected, but '%s' was provided." dynamicElement.TargetType.FullName
        member x.Clicked() : 'msg =
            let mutable msgVal = null
            let eventHandler = dynamicElement.Events.[ViewAttributes.ButtonClicked](fun msg -> msgVal <- msg) :?> EventHandler
            eventHandler.Invoke(x, EventArgs())
            msgVal :?> 'msg
        
    type LabelViewer(element: IViewElement) =
        let dynamicElement = match element with :? DynamicViewElement as dyn -> dyn | _ -> failwithf "A DynamicViewElement was expected"
        do if not ((typeof<Xamarin.Forms.Label>).IsAssignableFrom(dynamicElement.TargetType)) then failwithf "A DynamicViewElement assignable to type 'Xamarin.Forms.Label' is expected, but '%s' was provided." dynamicElement.TargetType.FullName
        member x.Text = dynamicElement.Properties.[ViewAttributes.LabelText] :?> string
        
    type SliderViewer(element: IViewElement) =
        let dynamicElement = match element with :? DynamicViewElement as dyn -> dyn | _ -> failwithf "A DynamicViewElement was expected"
        do if not ((typeof<Xamarin.Forms.Slider>).IsAssignableFrom(dynamicElement.TargetType)) then failwithf "A DynamicViewElement assignable to type 'Xamarin.Forms.Slider' is expected, but '%s' was provided." dynamicElement.TargetType.FullName
        member x.Value = dynamicElement.Properties.[ViewAttributes.SliderValue] :?> double
        
    type SwitchViewer(element: IViewElement) =
        let dynamicElement = match element with :? DynamicViewElement as dyn -> dyn | _ -> failwithf "A DynamicViewElement was expected"
        do if not ((typeof<Xamarin.Forms.Switch>).IsAssignableFrom(dynamicElement.TargetType)) then failwithf "A DynamicViewElement assignable to type 'Xamarin.Forms.Switch' is expected, but '%s' was provided." dynamicElement.TargetType.FullName
        member x.IsToggled = dynamicElement.Properties.[ViewAttributes.SwitchIsToggled] :?> bool