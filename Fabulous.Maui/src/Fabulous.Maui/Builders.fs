namespace Fabulous.Maui

open Fabulous
open Fabulous.Maui.Controls
open Microsoft.Maui
open System.Collections.Generic

module Attributes =
    let Page = AttributeKey<_>("Page")
    let View = AttributeKey<_>("View")
    let Children = AttributeKey<_>("Children")
    let Spacing = AttributeKey<int>("Spacing")
    let Text = AttributeKey<_>("Text")
    let CharacterSpacing = AttributeKey<_>("CharacterSpacing")
    let Font = AttributeKey<_>("Font")
    let HorizontalTextAlignment = AttributeKey<_>("HorizontalTextAlignment")
    let LineBreakMode = AttributeKey<_>("LineBreakMode")
    let LineHeight = AttributeKey<_>("LineHeight")
    let MaxLines = AttributeKey<_>("MaxLines")
    let Padding = AttributeKey<_>("Padding")
    let TextColor = AttributeKey<_>("TextColor")
    let TextDecorations = AttributeKey<_>("TextDecorations")
    let Clicked = AttributeKey<_>("Clicked")
    let Pressed = AttributeKey<_>("Pressed")
    let Released = AttributeKey<_>("Released")
    
module Handlers =
    let Window =
        Registrar.Register(
            nameof(FabulousWindow),
            (fun _ _ -> FabulousWindow()),
            (fun def _ currVe target ->
                let changes = [| { Key = "Page"; Value = Create (currVe.GetAttributeKeyed(Attributes.Page)) } |]
                target.State.ApplyChanges(def, changes)
            ),
            (fun _ _ _ _ _ -> ()),
            (fun _ _ -> ())
        )
        
    let Page = 
        Registrar.Register(
            nameof(FabulousPage),
            (fun _ _ -> Compat.FabContentPage()),
            (fun def _ currVe target ->
                let changes = [| { Key = "View"; Value = Create (currVe.GetAttributeKeyed(Attributes.View)) } |]
                target.State.ApplyChanges(def, changes)
            ),
            (fun _ _ _ _ _ -> ()),
            (fun _ _ -> ())
        )
        
    let StackLayout =
        Registrar.Register(
            nameof(FabulousStackLayout),
            (fun _ _ -> Compat.FabStackLayout()),
            (fun def _ currVe target ->
                let changes = [| { Key = "Children"; Value = Create (currVe.GetAttributeKeyed(Attributes.Children)) } |]
                target.State.ApplyChanges(def, changes)
            ),
            (fun _ _ _ _ _ -> ()),
            (fun _ _ -> ())
        )
        
    let Label = 
        Registrar.Register(
            nameof(FabulousLabel),
            (fun _ _ -> FabulousLabel()),
            (fun def _ currVe target ->
                let changes = [| { Key = "Text"; Value = Create (currVe.GetAttributeKeyed(Attributes.Text)) } |]
                target.State.ApplyChanges(def, changes)
            ),
            (fun _ _ _ _ _ -> ()),
            (fun _ _ -> ())
        )
        
    let Button = 
        Registrar.Register(
            nameof(FabulousButton),
            (fun _ _ -> FabulousButton()),
            (fun _ _ _ _ -> ()),
            (fun _ _ _ _ _ -> ()),
            (fun _ _ -> ())
        )

module Builders =
    let inline mk (attribKey: AttributeKey<'a>) (value: 'a) = KeyValuePair(attribKey.KeyValue, box value)
    
    type [<Struct>] WindowBuilder (attribs: KeyValuePair<int, obj> list) =
        static member inline init (page: IViewElement) = WindowBuilder([ mk Attributes.Page page ])
        member this.Build() = DynamicViewElement(Handlers.Window.Key, attribs)
            
    type [<Struct>] PageBuilder (attribs: KeyValuePair<int, obj> list) =
        static member inline init (view: IViewElement) = PageBuilder([ mk Attributes.View view ])
        member this.Build() = DynamicViewElement(Handlers.Page.Key, attribs)
            
    type [<Struct>] StackLayoutBuilder (attribs: KeyValuePair<int, obj> list) =
        static member inline init (children: seq<IViewElement>) = StackLayoutBuilder([ mk Attributes.Children (children |> Array.ofSeq) ])
        member this.Add<'a>(attribKey: AttributeKey<'a>, value: 'a) = StackLayoutBuilder(mk attribKey value :: attribs)
        member this.Build() = DynamicViewElement(Handlers.StackLayout.Key, attribs)
        member inline this.spacing(value: int) = this.Add(Attributes.Spacing, value)
            
    type [<Struct>] LabelBuilder (attribs: KeyValuePair<int, obj> list) =
        static member inline init(text: string) = LabelBuilder([ mk Attributes.Text text ])
        member this.Add<'a>(attribKey: AttributeKey<'a>, value: 'a) = LabelBuilder(mk attribKey value :: attribs)
        member this.Build() = DynamicViewElement(Handlers.Label.Key, attribs)
        member inline this.characterSpacing(value: float) = this.Add(Attributes.CharacterSpacing, value)
        member inline this.font(value: Font) = this.Add(Attributes.Font, value)
        member inline this.horizontalTextAlignment(value: TextAlignment) = this.Add(Attributes.HorizontalTextAlignment, value)
        member inline this.lineBreakMode(value: LineBreakMode) = this.Add(Attributes.LineBreakMode, value)
        member inline this.lineHeight(value: float) = this.Add(Attributes.LineHeight, value)
        member inline this.maxLines(value: int) = this.Add(Attributes.MaxLines, value)
        member inline this.padding(value: Thickness) = this.Add(Attributes.Padding, value)
        member inline this.textColor(value: Color) = this.Add(Attributes.TextColor, value)
        member inline this.textDecorations(value: TextDecorations) = this.Add(Attributes.TextDecorations, value)
        
    type [<Struct>] ButtonBuilder (attribs: KeyValuePair<int, obj> list) =
        static member inline init (text: string) (clicked: unit -> unit) = ButtonBuilder([ mk Attributes.Text text; mk Attributes.Clicked clicked ])
        member this.Add<'a>(attribKey: AttributeKey<'a>, value: 'a) = ButtonBuilder(mk attribKey value :: attribs)
        member this.Build() = DynamicViewElement(Handlers.Label.Key, attribs)
        member inline this.characterSpacing(value: float) = this.Add(Attributes.CharacterSpacing, value)
        member inline this.font(value: Font) = this.Add(Attributes.Font, value)
        member inline this.padding(value: Thickness) = this.Add(Attributes.Padding, value)
        member inline this.textColor(value: Color) = this.Add(Attributes.TextColor, value)
        member inline this.pressed(value: unit -> unit) = this.Add(Attributes.Pressed, value)
        member inline this.released(value: unit -> unit) = this.Add(Attributes.Released, value)

open Builders

[<AbstractClass; Sealed>]
type View private () =
    static member inline Window(page) = WindowBuilder.init page
    static member inline Page(view) = PageBuilder.init view
    static member inline StackLayout(children) = StackLayoutBuilder.init children
    static member inline Label(text) = LabelBuilder.init text
    static member inline Button(text, clicked) = ButtonBuilder.init text clicked