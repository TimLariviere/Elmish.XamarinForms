namespace Fabulous.Maui.Controls

open Microsoft.Maui
open Fabulous.Maui

[<AbstractClass>]
type FabulousElement() as this =
    let state = StateWithHandler(this)
    
    member __.State = state
    abstract Handler: IViewHandler
    
    interface IBaseElement with
        member __.State = state :> State
        
    interface IElementWithHandler with
        member __.Handler = this.Handler