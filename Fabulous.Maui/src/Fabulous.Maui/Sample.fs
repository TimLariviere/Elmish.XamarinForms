namespace Fabulous.Maui

open Fabulous.Maui.View

module Sample =
    let view () =
        ContentPage(
            StackLayout([
                Button("Text")
                Label("Text")
            ])
        )
