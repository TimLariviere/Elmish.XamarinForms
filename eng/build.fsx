#r "paket: groupref fakebuild //"
#load "./.fake/build.fsx/intellisense.fsx"
#load "_targets.fsx"

open Fake.Core
open Fake.Core.TargetOperators

Target.create "Fabulous" ignore
Target.create "Fabulous.CodeGen" ignore
Target.create "Fabulous.XamarinForms" ignore
Target.create "Build" ignore

"Restore"
    ==> "PackFabulous"
    ==> "RunFabulousTests"
    ==> "Fabulous"

"Restore"
    ==> "PackFabulousCodeGen"
    ==> "RunFabulousCodeGenTests"
    ==> "Fabulous.CodeGen"

"Restore"
    ==> "BuildTools"
    ==> "BuildFabulousXamarinFormsDependencies"
    ==> "RunGeneratorForFabulousXamarinForms"
    ==> "PackFabulousXamarinForms"
    ==> "RunFabulousXamarinFormsTests"
    ==> "PackFabulousXamarinFormsTemplates"
    ==> "RunGeneratorForFabulousXamarinFormsExtensions"
    ==> "PackFabulousXamarinFormsExtensions"
    ==> "Fabulous.XamarinForms"

"Clean"
    ==> "DownloadNuGet"
    ==> "Restore"
    ==> "Fabulous"
    ==> "Fabulous.CodeGen"
    ==> "Fabulous.XamarinForms"
    ==> "Build"

Target.runOrDefault "Build"