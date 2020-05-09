namespace Fabulous.XamarinForms.DynamicViews

open Fabulous
open Fabulous.DynamicViews
open Fabulous.XamarinForms
open Xamarin.Forms

module ViewAttributes =
    let setContentPageContentProperty v (target: obj) = (target :?> ContentPage).SetValue(ContentPage.ContentProperty, v)
    let unsetContentPageContentProperty (target: obj) = (target :?> ContentPage).ClearValue(ContentPage.ContentProperty)
    let setLabelTextProperty v (target: obj) = (target :?> Label).SetValue(Label.TextProperty, v)
    let unsetLabelTextProperty (target: obj) = (target :?> Label).ClearValue(Label.TextProperty)
    let setButtonTextProperty v (target: obj) = (target :?> Button).SetValue(Label.TextProperty, v)
    let unsetButtonTextProperty (target: obj) = (target :?> Button).ClearValue(Label.TextProperty)
    let setLayoutOfTChildrenProperty v (target: obj) = ()
    let unsetLayoutOfTChildrenProperty (target: obj) = ()
    let setTextCellTextProperty v (target: obj) = ()
    let unsetTextCellTextProperty (target: obj) = ()
    let setItemsViewOfTItemsProperty v (target: obj) = ()
    let unsetItemsViewOfTItemsProperty (target: obj) = ()

[<AbstractClass; Sealed>]
type ViewBuilders private () =
    static member inline BuildElement(attribCount) =
        AttributesBuilder(attribCount)
        
    static member inline BuildContentPage(attribCount, ?content) =
        let attribCount = match content with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildElement(attribCount)
        match content with None -> () | Some x -> attribs.Add(Property<_>(ViewAttributes.setContentPageContentProperty, ViewAttributes.unsetContentPageContentProperty, x))
        attribs
    
    static member inline BuildNavigableElement(attribCount) =
        let attribs = ViewBuilders.BuildElement(attribCount)
        attribs
    
    static member inline BuildVisualElement(attribCount) =
        let attribs = ViewBuilders.BuildNavigableElement(attribCount)
        attribs
    
    static member inline BuildView(attribCount) =
        let attribs = ViewBuilders.BuildVisualElement(attribCount)
        attribs
    
    static member inline BuildLabel(attribCount: int, ?text: string) =
        let attribCount = match text with None -> attribCount | Some x -> attribCount + 1
        let attribs = ViewBuilders.BuildView(attribCount)
        match text with None -> () | Some x -> attribs.Add(Property<_>(ViewAttributes.setLabelTextProperty, ViewAttributes.unsetLabelTextProperty, x))
        attribs
    
    static member inline BuildButton(attribCount: int, ?text: string) =
        let attribCount = match text with None -> attribCount | Some x -> attribCount + 1
        let attribs = ViewBuilders.BuildView(attribCount)
        match text with None -> () | Some x -> attribs.Add(Property<_>(ViewAttributes.setButtonTextProperty, ViewAttributes.unsetButtonTextProperty, x))
        attribs
    
    static member inline BuildLayoutOfT(attribCount: int, ?children: IView list) =
        let attribCount = match children with None -> attribCount | Some _ -> attribCount + 1
        let attribs = ViewBuilders.BuildView(attribCount)
        match children with None -> () | Some x -> attribs.Add(Property<_>(ViewAttributes.setLayoutOfTChildrenProperty, ViewAttributes.unsetLayoutOfTChildrenProperty, x))
        attribs
    
    static member inline BuildStackLayout(attribCount: int, ?children: IView list) =
        let attribs = ViewBuilders.BuildLayoutOfT(attribCount, ?children=children)
        attribs
        
    static member inline BuildItemsViewOfT(attribCount: int, ?items: ICell list) =
        let attribCount = match items with None -> attribCount | Some x -> attribCount + 1
        let attribs = ViewBuilders.BuildView(attribCount)
        match items with None -> () | Some x -> attribs.Add(Property<_>(ViewAttributes.setItemsViewOfTItemsProperty, ViewAttributes.unsetItemsViewOfTItemsProperty, x))
        attribs
        
    static member inline BuildListView(attribCount: int, ?items: ICell list) =
        let attribs = ViewBuilders.BuildItemsViewOfT(attribCount, ?items=items)
        attribs
        
    static member inline BuildCell(attribCount) =
        let attribs = ViewBuilders.BuildElement(attribCount)
        attribs
        
    static member inline BuildTextCell(attribCount, ?text: string) =
        let attribCount = match text with None -> attribCount | Some x -> attribCount + 1
        let attribs = ViewBuilders.BuildCell(attribCount)
        match text with None -> () | Some x -> attribs.Add(Property<_>(ViewAttributes.setTextCellTextProperty, ViewAttributes.unsetTextCellTextProperty, x))
        attribs
    
[<AbstractClass; Sealed>]
type View =
    static member inline ContentPage(?content: IView) =
        let attribs = ViewBuilders.BuildContentPage(0, ?content=content)
        DynamicPage(ContentPage, attribs.Close()) :> IPage<_>
    
    static member inline Label(?text: string) =
        let attribs = ViewBuilders.BuildLabel(0, ?text=text)
        DynamicView(Label, attribs.Close()) :> IView<_>
        
    static member inline Button(?text: string) =
        let attribs = ViewBuilders.BuildButton(0, ?text=text)
        DynamicView(Button, attribs.Close()) :> IView<_>

    static member inline StackLayout(?children: IView list) =
        let attribs = ViewBuilders.BuildStackLayout(0, ?children=children)
        DynamicView(StackLayout, attribs.Close()) :> IView<_>
        
    static member inline TextCell(?text: string) =
        let attribs = ViewBuilders.BuildTextCell(0, ?text=text)
        DynamicCell(TextCell, attribs.Close()) :> ICell<_>
        
    static member inline ListView(?items: ICell list) =
        let attribs = ViewBuilders.BuildListView(0, ?items=items)
        DynamicView(ListView, attribs.Close()) :> IView<_>