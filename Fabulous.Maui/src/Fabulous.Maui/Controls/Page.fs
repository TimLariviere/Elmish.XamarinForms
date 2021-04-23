namespace Fabulous.Maui.Controls

open Microsoft.Maui

type FabulousPage() =
    inherit FabulousBaseElement()
    
    interface IPage with
        member this.View
            with get () = this.State.GetValue("View", null)
            and set _value = failwith "Invalid setter usage"