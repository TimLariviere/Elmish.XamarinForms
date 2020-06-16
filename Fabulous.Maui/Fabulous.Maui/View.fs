namespace Fabulous.Maui

open Fabulous

[<Struct>]
type ContentPage(events: (string * DynamicEventFunc) list, properties: (string * obj) list) =
    interface IPage with
        member x.Build() = null

    static member inline init(content: #IView) =
        ContentPage([], [ ("Content", content.Build()) ])
       
module X =
    let build (c: IView) = c.Build()
        
[<Struct>]
type StackLayout(events: (string * DynamicEventFunc) list, properties: (string * obj) list) =
    interface IView with
        member x.Build() = null
        
    static member inline init(children: seq<IView>) =
        StackLayout([], [ ("Children", children |> Seq.map X.build |> Array.ofSeq :> obj) ])
        
[<Struct>]
type Button(events: (string * DynamicEventFunc) list, properties: (string * obj) list) =
    interface IView with
        member x.Build() = null

    static member inline init(text: string) =
        Button([], [ ("Text", text :> obj) ])
        
[<Struct>]
type Label(events: (string * DynamicEventFunc) list, properties: (string * obj) list) =
    interface IView with
        member x.Build() = null

    static member inline init(text: string) =
        Label([], [ ("Text", text :> obj) ])
    
[<AbstractClass; Sealed>]
type View private () =
    static member inline ContentPage(content: #IView) = ContentPage.init(content)
    static member inline StackLayout(children: seq<IView>) = StackLayout.init(children)
    static member inline Button(text) = Button.init(text)
    static member inline Label(text) = Label.init(text)