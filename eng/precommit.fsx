#r "paket: groupref fakebuild //"
#load "./.fake/precommit.fsx/intellisense.fsx"
#load "_targets.fsx"

open Fake.Core
open Fake.Core.TargetOperators

Target.create "Precommit" ignore

"Clean"
    ==> "DownloadNuGet"
    ==> "Restore"
    ==> "FormatBindings"
    ==> "UpdateVersion"
    ==> "Precommit"

Target.runOrDefault "Precommit"