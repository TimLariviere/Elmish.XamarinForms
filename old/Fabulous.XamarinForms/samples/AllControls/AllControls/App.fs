namespace AllControls

open AllControls.SampleDefinition
open AllControls.Samples

open System
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module SampleHelpers =
    let getSampleDefinitionFromNode (node: Node) =
        match node with
        | Sample sample -> sample
        | SampleChooser sampleChooser ->
            { Title = sampleChooser.Title
              Init = (SampleChooser.init sampleChooser)
              Update = SampleChooser.update
              View = SampleChooser.view
              MapToCmd = SampleChooser.mapToCmd } |> boxSampleDefinition

module App =
    type Msg =
        | SampleMsg of obj
        | NavigateToRequested of Node
        | NavigationPopped
        | LowMemoryWarningReceived
        | AppThemeChanged of OSAppTheme
    
    /// For each sample, we store its definition along its current state
    type SampleState =
        { Definition: SampleDefinition
          Model: obj }
    
    /// All samples are stored as a reversed list
    /// The first item will be the sample currently showing on the screen
    type Model =
        { SampleStates: SampleState list }
            
    let displayMemoryWarningMessage() =
        Application.Current.MainPage.DisplayAlert("Low memory!", "Cleaning up data...", "OK") |> ignore
                
    let init () =
        let definition = SampleHelpers.getSampleDefinitionFromNode Samples.root
        let sampleState =
            { Definition = definition
              Model = definition.Init() }
        
        { SampleStates = [sampleState] }, []
        
    let mapExternalMsg (externalMsg: obj option) =
        match externalMsg with
        | Some (:? SampleChooser.ExternalMsg as msg) ->
            match msg with
            | SampleChooser.ExternalMsg.NavigateToNode node ->
                Cmd.ofMsg (NavigateToRequested node)
        | _ ->
            Cmd.none
        
    let update msg model =
        match msg with
        | SampleMsg sampleMsg ->
            match model.SampleStates with
            | [] -> model, []
            | sampleState::rest ->
                let newSampleModel, newCmdMsgs, externalMsg = sampleState.Definition.Update sampleMsg sampleState.Model
                let newSampleState = { sampleState with Model = newSampleModel }
                let newCmd = newCmdMsgs |> List.map (sampleState.Definition.MapToCmd >> (Cmd.map SampleMsg)) |> Cmd.batch
                let newExternalCmd = mapExternalMsg externalMsg
                { model with SampleStates = newSampleState :: rest }, Cmd.batch [newCmd; newExternalCmd]
            
        | NavigateToRequested node ->
            let definition = SampleHelpers.getSampleDefinitionFromNode node
            let sampleState =
                { Definition = definition
                  Model = definition.Init() }
            { model with SampleStates = sampleState :: model.SampleStates }, Cmd.none
                
        | NavigationPopped ->
            { model with SampleStates = model.SampleStates.Tail }, Cmd.none
            
        | LowMemoryWarningReceived ->
            displayMemoryWarningMessage()
            model, Cmd.none
            
        | AppThemeChanged appTheme ->
            match model.SampleStates with
            | [] -> model, []
            | sampleState::_ ->
                match sampleState.Model with
                | :? Samples.UseCases.AppTheming.Model ->
                    let appThemeMsg = AllControls.Samples.UseCases.AppTheming.Msg.SetRequestedAppTheme appTheme
                    model, Cmd.ofMsg (SampleMsg appThemeMsg)
                | _ -> model, Cmd.none
            
    let view model dispatch =
        View.NavigationPage(
            popped = (fun _ -> dispatch NavigationPopped),
            pages = [
                for sampleState in List.rev model.SampleStates ->
                    sampleState.Definition.View sampleState.Model (SampleMsg >> dispatch)
            ]
        )
    
type App () as app = 
    inherit Application ()
    do app.Resources.Add(Xamarin.Forms.StyleSheets.StyleSheet.FromAssemblyResource(System.Reflection.Assembly.GetExecutingAssembly(), "AllControls.styles.css"))
    
    do Device.SetFlags([
        "Shell_Experimental"; "CollectionView_Experimental"; "Visual_Experimental"; 
        "IndicatorView_Experimental"; "SwipeView_Experimental"; "MediaElement_Experimental"
        "AppTheme_Experimental"; "RadioButton_Experimental"; "Expander_Experimental"
    ])
    
    let runner = 
        Program.mkProgram App.init App.update App.view
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app
        
    let onRequestedThemeChanged = EventHandler<AppThemeChangedEventArgs>(fun _ args ->
        runner.Dispatch (App.Msg.AppThemeChanged args.RequestedTheme)
    )

    member __.Program = runner
        
    override this.OnStart() =
        Application.Current.RequestedThemeChanged.AddHandler(onRequestedThemeChanged)