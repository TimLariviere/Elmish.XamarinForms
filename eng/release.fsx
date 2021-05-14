#r "paket: groupref fakebuild //"
#load "./.fake/release.fsx/intellisense.fsx"
#load "_targets.fsx"

open Fake.Core
open Fake.Core.TargetOperators

Target.create "Release" ignore

"Precommit"
    ==> "Build"
    ==> "TestTemplates"
    ==> "TestSamples"
    ==> "CreateGitHubRelease"
    ==> "PublishNuGetPackages"
    ==> "Release"

Target.runOrDefault "Release"