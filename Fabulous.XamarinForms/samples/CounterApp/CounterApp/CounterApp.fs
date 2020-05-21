// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace CounterApp

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open System.Diagnostics
open System

module App = 
    type Model = 
      { Items:int list }

    type Msg = 
        | Next 
    

   

    let initModel () = { Items = [1..100] }

    let init () = initModel () , []

    let update msg model =
        match msg with
        | Next -> model,Cmd.none

    let random = Random(DateTime.UtcNow.Ticks|>int)
    let view (model: Model) dispatch =
        let views =
            [
            
             for item in model.Items do
                 let kind = random.Next(0,100)
                 if(kind<20) then
                    yield View.Label(fontSize=FontSize 40.,text= sprintf "ITEM %i" item,backgroundColor =Color.Aqua)
                    
                 elif (kind<40) then
                     yield View.Frame(
                            cornerRadius = 24.,
                            backgroundColor = Color.Yellow,
                            content=View.Label(fontSize=FontSize 24.,text= sprintf "ITEM %i" item))
                 elif (kind<70) then
                     yield View.Label(fontSize=FontSize 24.,text= sprintf "Item %i" item,backgroundColor=Color.Red)
                 
                 else
                    yield View.Label(fontSize=FontSize (14.),text= sprintf "Item %i" item,backgroundColor=Color.Green) 
                 ]
        View.ContentPage(
          content=View.StackLayout([
              View.Button(text = "Next", command = (fun () -> dispatch Next))
              View.CollectionView(
                items = views
              
              )
            ]))
             
    let program = 
        Program.mkProgram init update view

type CounterApp () as app = 
    inherit Application ()

    let runner =
        App.program
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app

#if DEBUG
    // Run LiveUpdate using: 
    //    
    do runner.EnableLiveUpdate ()
#endif


#if SAVE_MODEL_WITH_JSON
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Debug.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Debug.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Debug.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Debug.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex ->
            runner.OnError ("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Debug.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()

#endif
