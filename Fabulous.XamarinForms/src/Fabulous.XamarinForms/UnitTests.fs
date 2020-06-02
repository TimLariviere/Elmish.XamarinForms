namespace Fabulous.XamarinForms

open Fabulous
open Fabulous.XamarinForms.DynamicViews

module UnitTests =
    /// Looks for a view element with the given Automation ID in the view hierarchy.
    /// This function is not optimized for efficiency and may execute slowly.
    let rec tryFindViewElement automationId (element: IViewElement) =
        match element with
        | :? DynamicViewElement as dynamicElement ->
            let elementAutomationId = dynamicElement.TryGetPropertyValue<string>(ViewAttributes.ElementAutomationId)
            match elementAutomationId with
            | ValueSome automationIdValue when automationIdValue = automationId -> Some element
            | _ ->
                let childElements =
                    match dynamicElement.TryGetPropertyValue<IViewElement>(ViewAttributes.ContentPageContent) with
                    | ValueSome content -> [| content |]
                    | ValueNone ->
                        match dynamicElement.TryGetPropertyValue<IViewElement[]>(ViewAttributes.LayoutOfTChildren) with
                        | ValueNone -> [||]
                        | ValueSome children -> children

                childElements
                |> Seq.choose (tryFindViewElement automationId)
                |> Seq.tryHead
        | _ -> None
     
    /// Looks for a view element with the given Automation ID in the view hierarchy
    /// Throws an exception if no element is found
    let findAutomationId automationId (builder: IViewElementBuilder<'msg>) =
        let element = builder.AsViewElement() :?> DynamicViewElement
        match tryFindViewElement automationId element with
        | None -> failwithf "No element with automation id '%s' found" automationId
        | Some viewElement -> viewElement