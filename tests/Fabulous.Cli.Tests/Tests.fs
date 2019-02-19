namespace FSharpDaemon.Tests

open System
open System.IO
open NUnit.Framework
open FsUnit
open FSharp.Compiler.PortaCode.Tests // for TestHelpers

module Versions = 
    let XamarinForms = "3.4.0.1009999"
    let NewtonsoftJson = "11.0.2"

[<TestClass>]
type TestClass () =

    // Paths are relative to "__SOURCE_DIRECTORY__/data" where the project files are written to
    let elmishExtraRefs =
        """
-r:CWD/../../../src/Fabulous.CustomControls/bin/Debug/netstandard2.0/Fabulous.CustomControls.dll
-r:CWD/../../../src/Fabulous.Core/bin/Debug/netstandard2.0/Fabulous.Core.dll
-r:CWD/../../../src/Fabulous.LiveUpdate/bin/Debug/netstandard2.0/Fabulous.LiveUpdate.dll
-r:NUGETFALLBACKFOLDER/newtonsoft.json/NEWTONSOFTJSONVERSION/lib/netstandard2.0/Newtonsoft.Json.dll
-r:PACKAGEDIR/xamarin.forms/XAMARINFORMSVERSION/lib/netstandard2.0/Xamarin.Forms.Core.dll
-r:PACKAGEDIR/xamarin.forms/XAMARINFORMSVERSION/lib/netstandard2.0/Xamarin.Forms.Platform.dll
-r:PACKAGEDIR/xamarin.forms/XAMARINFORMSVERSION/lib/netstandard2.0/Xamarin.Forms.Xaml.dll
"""
        |> fun s -> s.Replace("NEWTONSOFTJSONVERSION", Versions.NewtonsoftJson)
        |> fun s -> s.Replace("XAMARINFORMSVERSION", Versions.XamarinForms)

    let fixArgs name =
        let fileName = name + ".args.txt"
        File.ReadAllText(fileName)
        |> (fun s ->
            match Environment.OSVersion.Platform with
            | PlatformID.Unix ->
                match Environment.OSVersion.Platform with
                | PlatformID.Win32NT -> s
                | PlatformID.Unix -> s.Replace("/usr/local/share/dotnet/sdk/NuGetFallbackFolder", "/Users/vsts/.nuget/packages")
                | _ -> s
            | _ -> s
        )
        
        |> (fun s ->
            s.Replace("/Users/vsts/.nuget/packages/system.reflection.emit.ilgeneration/4.3.0/ref/netstandard1.0/System.Reflection.Emit.ILGeneration.dll", "/usr/local/share/dotnet/sdk/NuGetFallbackFolder/system.reflection.emit.ilgeneration/4.3.0/ref/netstandard1.0/System.Reflection.Emit.ILGeneration.dll")
             .Replace("/Users/vsts/.nuget/packages/system.reflection.emit.lightweight/4.3.0/ref/netstandard1.0/System.Reflection.Emit.Lightweight.dll", "/usr/local/share/dotnet/sdk/NuGetFallbackFolder/system.reflection.emit.lightweight/4.3.0/ref/netstandard1.0/System.Reflection.Emit.Lightweight.dll")
        )
        |> (fun s -> File.WriteAllText(fileName, s))

    let createNetStandardProjectArgs name =
        TestHelpers.createNetStandardProjectArgs name elmishExtraRefs
        fixArgs name

    let ElmishTestCase name code =
        let directory = __SOURCE_DIRECTORY__ + "/data"
        Directory.CreateDirectory directory |> ignore
        Environment.CurrentDirectory <- directory
        File.WriteAllText (name + ".fs", """
module TestCode
""" + code)
        createNetStandardProjectArgs name

        let args = 
            [| yield "dummy.exe"; 
               yield "--eval"; 
               yield "@" + name + ".args.txt" |]
        Assert.AreEqual(0, FSharp.Compiler.PortaCode.ProcessCommandLine.ProcessCommandLine(args))

    [<Test>]
    member this.TestCanEvaluateCounterApp () =
        Environment.CurrentDirectory <- __SOURCE_DIRECTORY__ + "/../../Samples/CounterApp/CounterApp"
        createNetStandardProjectArgs "CounterApp"
        Assert.AreEqual(0, FSharpDaemon.Driver.main( [| "dummy.exe"; "--eval"; "@CounterApp.args.txt" |]))

    [<Test>]
    member this.TestCanEvaluateTicTacToeApp () =
        Environment.CurrentDirectory <- __SOURCE_DIRECTORY__ + "/../../Samples/TicTacToe/TicTacToe"
        createNetStandardProjectArgs "TicTacToe"
        Assert.AreEqual(0, FSharpDaemon.Driver.main( [| "dummy.exe"; "--eval"; "@TicTacToe.args.txt" |]))

    [<Test>]
    member this.ViewRefSmoke() =
        ElmishTestCase "ViewRefSmoke" """
let theRef = Fabulous.DynamicViews.ViewRef<Xamarin.Forms.Label>()
       """

    [<Test>]
    member this.TestCallUnitFunction() =
        ElmishTestCase "TestCallUnitFunction" """
let theRef = FSharp.Core.LanguagePrimitives.GenericZeroDynamic<int>()
       """


