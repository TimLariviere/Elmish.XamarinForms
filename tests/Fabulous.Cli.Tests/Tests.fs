namespace FSharpDaemon.Tests

open System
open System.IO
open NUnit.Framework
open FsUnit
open TestHelpers

[<TestClass>]
type TestClass () =

    [<Test>]
    member this.TestCanEvaluateCounterApp () =
        Environment.CurrentDirectory <- __SOURCE_DIRECTORY__ + "/../../Samples/CounterApp/CounterApp"
        createNetStandardProjectArgs "CounterApp" elmishExtraRefs
        Assert.AreEqual(0, FSharpDaemon.Driver.main( [| "dummy.exe"; "--eval"; "@CounterApp.args.txt" |]))

    [<Test>]
    member this.TestCanEvaluateTicTacToeApp () =
        Environment.CurrentDirectory <- __SOURCE_DIRECTORY__ + "/../../Samples/TicTacToe/TicTacToe"
        createNetStandardProjectArgs "TicTacToe" elmishExtraRefs
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


