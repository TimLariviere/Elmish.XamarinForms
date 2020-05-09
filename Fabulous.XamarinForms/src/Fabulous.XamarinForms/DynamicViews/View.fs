namespace Fabulous.XamarinForms.DynamicViews

open Fabulous.XamarinForms
open Xamarin.Forms

type Command =
    { Execute: unit -> unit
      CanExecute: bool option }
    static member create canExecuteOpt execute =
        { Execute = execute; CanExecute = canExecuteOpt }

module ViewAttributes =
    let ElementAutomationIdProperty = Attribute<_>.BindableProperty Element.AutomationIdProperty
    let ViewHorizontalOptionsProperty = Attribute<_>.BindableProperty View.HorizontalOptionsProperty
    let ViewVerticalOptionsProperty = Attribute<_>.BindableProperty View.VerticalOptionsProperty
    let VisualElementWidthProperty = Attribute<_>.BindableProperty VisualElement.WidthRequestProperty
    let ContentPageContentProperty = Attribute<_>.BindableProperty ContentPage.ContentProperty
    let LabelHorizontalTextAlignmentProperty = Attribute<_>.BindableProperty Label.HorizontalTextAlignmentProperty
    let LabelTextProperty = Attribute<_>.BindableProperty Label.TextProperty
    let ButtonCommandProperty = Attribute<_>.BindableProperty Button.CommandProperty
    let ButtonTextProperty = Attribute<_>.BindableProperty Button.TextProperty
    let SwitchIsToggledProperty = Attribute<_>.BindableProperty Switch.IsToggledProperty
    let SwitchToggledEvent = EventAttribute<_>.EventHandler (fun t -> (t :?> Switch).Toggled)
    let SliderMinimumMaximumProperty = Attribute<_>.Property ((fun (v, t) -> ()), (fun t -> ()))
    let SliderValueProperty = Attribute<_>.BindableProperty Slider.ValueProperty
    let SliderValueChangedEvent = EventAttribute<_>.EventHandler (fun t -> (t :?> Slider).ValueChanged)
    let LayoutPaddingProperty = Attribute<_>.BindableProperty Layout.PaddingProperty
    let LayoutOfTChildrenProperty = Attribute<_>.CollectionProperty((fun (v, t) -> ()), (fun t -> ()))
    let StackLayoutOrientationProperty = Attribute<_>.BindableProperty StackLayout.OrientationProperty
    let ItemsViewOfTItemsProperty = Attribute<_>.CollectionProperty((fun (v, t) -> ()), (fun t -> ()))
    let TextCellTextProperty = Attribute<_>.BindableProperty TextCell.TextProperty
    
    let GridRowAttachedProperty = Attribute<int>.AttachedProperty Grid.RowProperty

[<AbstractClass; Sealed>]
type ViewBuilders private () =
    static member inline BuildElement(attribCount, ?automationId: string) =
        let attribCount = match automationId with None -> attribCount | Some _ -> attribCount + 1
        let attribs = AttributesBuilder(attribCount)
        match automationId with None -> () | Some x -> attribs.Add(ViewAttributes.ElementAutomationIdProperty, x)
        attribs
        
    static member inline BuildContentPage(attribCount, ?content, ?automationId) =
        let attribCount = match content with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildElement(attribCount, ?automationId=automationId)
        match content with None -> () | Some x -> attribs.Add(ViewAttributes.ContentPageContentProperty, x)
        attribs
    
    static member inline BuildNavigableElement(attribCount, ?automationId: string) =
        let attribs = ViewBuilders.BuildElement(attribCount, ?automationId=automationId)
        attribs
    
    static member inline BuildVisualElement(attribCount, ?width: double, ?automationId: string) =
        let attribCount = match width with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildNavigableElement(attribCount, ?automationId=automationId)
        match width with None -> () | Some x -> attribs.Add(ViewAttributes.VisualElementWidthProperty, x)
        attribs
    
    static member inline BuildView(attribCount, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribCount = match horizontalOptions with None -> attribCount | Some _ -> attribCount + 1
        let attribCount = match verticalOptions with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildVisualElement(attribCount, ?width=width, ?automationId=automationId)
        match horizontalOptions with None -> () | Some x -> attribs.Add(ViewAttributes.ViewHorizontalOptionsProperty, x)
        match verticalOptions with None -> () | Some x -> attribs.Add(ViewAttributes.ViewVerticalOptionsProperty, x)
        attribs
    
    static member inline BuildLabel(attribCount: int, ?horizontalTextAlignment: TextAlignment, ?text: string, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribCount = match text with None -> attribCount | Some _ -> attribCount + 1
        let attribCount = match text with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildView(attribCount, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        match horizontalTextAlignment with None -> () | Some x -> attribs.Add(ViewAttributes.LabelHorizontalTextAlignmentProperty, x)
        match text with None -> () | Some x -> attribs.Add(ViewAttributes.LabelTextProperty, x)
        attribs
    
    static member inline BuildButton(attribCount: int, ?command: (unit -> unit), ?commandCanExecute: bool, ?text: string, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribCount = match command with None -> attribCount | Some _ -> attribCount + 1
        let attribCount = match text with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildView(attribCount, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        match command with None -> () | Some x -> attribs.Add(ViewAttributes.ButtonCommandProperty, Command.create commandCanExecute x)
        match text with None -> () | Some x -> attribs.Add(ViewAttributes.ButtonTextProperty, x)
        attribs
    
    static member inline BuildSwitch(attribCount: int, ?isToggled: bool, ?toggled: (ToggledEventArgs -> unit), ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribCount = match isToggled with None -> attribCount | Some _ -> attribCount + 1
        let attribCount = match toggled with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildView(attribCount, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        match isToggled with None -> () | Some x -> attribs.Add(ViewAttributes.SwitchIsToggledProperty, x)
        match toggled with None -> () | Some x -> attribs.Add(ViewAttributes.SwitchToggledEvent, x)
        attribs
    
    static member inline BuildSlider(attribCount: int, ?minimumMaximum: (double * double), ?value: double, ?valueChanged: (ValueChangedEventArgs -> unit), ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribCount = match minimumMaximum with None -> attribCount | Some _ -> attribCount + 1
        let attribCount = match value with None -> attribCount | Some _ -> attribCount + 1
        let attribCount = match valueChanged with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildView(attribCount, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        match minimumMaximum with None -> () | Some x -> attribs.Add(ViewAttributes.SliderMinimumMaximumProperty, x)
        match value with None -> () | Some x -> attribs.Add(ViewAttributes.SliderValueProperty, x)
        match valueChanged with None -> () | Some x -> attribs.Add(ViewAttributes.SliderValueChangedEvent, x)
        attribs
        
    static member inline BuildLayout(attribCount: int, ?padding: Thickness, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribCount = match padding with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildView(attribCount, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        match padding with None -> () | Some x -> attribs.Add(ViewAttributes.LayoutPaddingProperty, x)
        attribs
    
    static member inline BuildLayoutOfT(attribCount: int, ?children: IView list, ?padding: Thickness, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribCount = match children with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildLayout(attribCount, ?padding=padding, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        match children with None -> () | Some x -> attribs.Add(ViewAttributes.LayoutOfTChildrenProperty, x)
        attribs
    
    static member inline BuildStackLayout(attribCount: int, ?orientation: StackOrientation, ?children: IView list, ?padding: Thickness, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribCount = match orientation with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildLayoutOfT(attribCount, ?children=children, ?padding=padding, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        match orientation with None -> () | Some x -> attribs.Add(ViewAttributes.StackLayoutOrientationProperty, x)
        attribs
        
    static member inline BuildItemsViewOfT(attribCount: int, ?items: ICell list, ?width: double, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?automationId: string) =
        let attribCount = match items with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildView(attribCount, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        match items with None -> () | Some x -> attribs.Add(ViewAttributes.ItemsViewOfTItemsProperty, x)
        attribs
        
    static member inline BuildListView(attribCount: int, ?items: ICell list, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribs = ViewBuilders.BuildItemsViewOfT(attribCount, ?items=items, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        attribs
        
    static member inline BuildCell(attribCount, ?automationId: string) =
        let attribs = ViewBuilders.BuildElement(attribCount, ?automationId=automationId)
        attribs
        
    static member inline BuildTextCell(attribCount, ?text: string, ?automationId: string) =
        let attribCount = match text with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildCell(attribCount, ?automationId=automationId)
        match text with None -> () | Some x -> attribs.Add(ViewAttributes.TextCellTextProperty, x)
        attribs
    
[<AbstractClass; Sealed>]
type View =
    static member inline ContentPage(?content: IView, ?automationId: string) =
        let attribs = ViewBuilders.BuildContentPage(0, ?content=content, ?automationId=automationId)
        DynamicPage(ContentPage, attribs.Close()) :> IPage<_>
    
    static member inline Label(?horizontalTextAlignment, ?text: string, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribs = ViewBuilders.BuildLabel(0, ?horizontalTextAlignment=horizontalTextAlignment, ?text=text, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        DynamicView(Label, attribs.Close()) :> IView<_>
        
    static member inline Button(?command: (unit -> unit), ?commandCanExecute: bool, ?text: string, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribs = ViewBuilders.BuildButton(0, ?command=command, ?commandCanExecute=commandCanExecute, ?text=text, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        DynamicView(Button, attribs.Close()) :> IView<_>
        
    static member inline Switch(?isToggled: bool, ?toggled: ToggledEventArgs -> unit, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribs = ViewBuilders.BuildSwitch(0, ?isToggled=isToggled, ?toggled=toggled, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        DynamicView(Switch, attribs.Close()) :> IView<_>
        
    static member inline Slider(?minimumMaximum: (double * double), ?value: double, ?valueChanged: (ValueChangedEventArgs -> unit), ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribs = ViewBuilders.BuildSlider(0, ?minimumMaximum=minimumMaximum, ?value=value, ?valueChanged=valueChanged, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        DynamicView(Slider, attribs.Close()) :> IView<_>

    static member inline StackLayout(?orientation: StackOrientation, ?children: IView list, ?padding: Thickness, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribs = ViewBuilders.BuildStackLayout(0, ?orientation=orientation, ?children=children, ?padding=padding, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        DynamicView(StackLayout, attribs.Close()) :> IView<_>
        
    static member inline ListView(?items: ICell list, ?horizontalOptions: LayoutOptions, ?verticalOptions: LayoutOptions, ?width: double, ?automationId: string) =
        let attribs = ViewBuilders.BuildListView(0, ?items=items, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions, ?width=width, ?automationId=automationId)
        DynamicView(ListView, attribs.Close()) :> IView<_>
        
    static member inline TextCell(?text: string, ?automationId: string) =
        let attribs = ViewBuilders.BuildTextCell(0, ?text=text, ?automationId=automationId)
        DynamicCell(TextCell, attribs.Close()) :> ICell<_>