namespace Fabulous.XamarinForms.DynamicViews

module Test =
    let label = View.Label(text = "Hello")
    
    let stackLayout =
        View.ContentPage(
            View.StackLayout([
                View.Label()
                View.Button()
                //View.ContentPage()
            ])
        )
        
    let listView =
        View.ListView([
           View.TextCell()
        ])