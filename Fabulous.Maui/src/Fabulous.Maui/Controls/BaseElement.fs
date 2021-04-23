namespace Fabulous.Maui.Controls

open Fabulous.Maui

type IBaseElement =
    abstract State : State
    
type FabulousBaseElement() =
    let state = State()
    
    member __.State = state
    
    interface IBaseElement with
        member __.State = state