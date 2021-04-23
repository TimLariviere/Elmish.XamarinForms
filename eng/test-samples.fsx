#r "paket: groupref fakebuild //"
#load "./.fake/test-samples.fsx/intellisense.fsx"
#load "_targets.fsx"

open Fake.Core
open Fake.Core.TargetOperators

Target.create "TestSamples" ignore

"Restore"
    ==> "BuildTools"
    ==> "BuildFabulousXamarinFormsDependencies"
    ==> "RunGeneratorForFabulousXamarinForms"
    ==> "RunGeneratorForFabulousXamarinFormsExtensions"
    ==> "BuildFabulousXamarinFormsSamples"
    ==> "RunFabulousXamarinFormsSamplesTests"
    ==> "TestSamples"

Target.runOrDefault "TestSamples"