#r "paket: groupref fakebuild //"
#load "./.fake/test-templates.fsx/intellisense.fsx"
#load "_targets.fsx"

open Fake.Core
open Fake.Core.TargetOperators

Target.create "TestTemplates" ignore

"DownloadNuGet"
    ==> "TestTemplatesNuGet"
    ==> "TestTemplates"

Target.runOrDefault "TestTemplates"