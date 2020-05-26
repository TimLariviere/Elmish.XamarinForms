namespace Fabulous.XamarinForms.DynamicViews

open Fabulous
open Xamarin.Forms

module ViewHelpers =
    /// Checks whether an underlying control can be reused given the previous and new view elements
    let rec canReuseDynamicView (prevChild: DynamicViewElement) (newChild: DynamicViewElement) =
        if prevChild.TargetType = newChild.TargetType && canReuseAutomationId prevChild newChild then
            if newChild.TargetType.IsAssignableFrom(typeof<NavigationPage>) then
                canReuseNavigationPage prevChild newChild
//            elif newChild.TargetType.IsAssignableFrom(typeof<CustomEffect>) then
//                canReuseCustomEffect prevChild newChild
            else
                true
        else
            false

    /// Checks whether an underlying NavigationPage control can be reused given the previous and new view elements
    //
    // NavigationPage can be reused only if the pages don't change their type (added/removed pages don't prevent reuse)
    // E.g. If the first page switch from ContentPage to TabbedPage, the NavigationPage can't be reused.
    and internal canReuseNavigationPage (prevChild: DynamicViewElement) (newChild: DynamicViewElement) =
        true
//        let prevPages = prevChild.TryGetAttribute<ViewElement[]>("Pages")
//        let newPages = newChild.TryGetAttribute<ViewElement[]>("Pages")
//
//        match prevPages, newPages with
//        | ValueSome prevPages, ValueSome newPages -> (prevPages, newPages) ||> Seq.forall2 canReuseView
//        | _, _ -> true

    /// Checks whether the control can be reused given the previous and the new AutomationId.
    /// Xamarin.Forms can't change an already set AutomationId
    and internal canReuseAutomationId (prevChild: DynamicViewElement) (newChild: DynamicViewElement) =
        true
//        let prevAutomationId = prevChild.TryGetAttribute<string>("AutomationId")
//        let newAutomationId = newChild.TryGetAttribute<string>("AutomationId")
//
//        match prevAutomationId with
//        | ValueSome _ when prevAutomationId <> newAutomationId -> false
//        | _ -> true
        
    /// Checks whether the CustomEffect can be reused given the previous and the new Effect name
    /// The effect is instantiated by Effect.Resolve and can't be reused when asking for a new effect
    and internal canReuseCustomEffect (prevChild: DynamicViewElement) (newChild: DynamicViewElement) =
        true
//        let prevName = prevChild.TryGetAttribute<string>("Name")
//        let newName = newChild.TryGetAttribute<string>("Name")
//
//        match prevName with
//        | ValueSome _ when prevName <> newName -> false
//        | _ -> true
