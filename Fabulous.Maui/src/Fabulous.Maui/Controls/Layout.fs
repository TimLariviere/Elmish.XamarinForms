namespace Fabulous.Maui.Controls

open Microsoft.Maui
open System.Collections.Generic

[<AbstractClass>]
type FabulousLayout() =
    inherit FabulousView()
    
    let createDefaultChildren () = List() :> IReadOnlyList<IView>
    
    interface ILayout with
        member this.Children = this.State.GetValueFn("Children", createDefaultChildren)
        member this.LayoutHandler = this.State.GetValue("LayoutHandler", null)
        member this.Add(_child) = failwith "Invalid use"
        member this.Remove(_child) = failwith "Invalid use"

