// Copyright Fabulous contributors. See LICENSE.md for license.
namespace Fabulous.XamarinForms.Tests

open System.IO
open Fabulous.XamarinForms
open NUnit.Framework
open FsUnit
open Xamarin.Forms

module ViewConvertersTests =
    [<Test>]
    let ``Given a file system path, convertFabulousImageToXamarinFormsImageSource should return a FileImageSource``() =
        ImagePath "path/to/image.png"
        |> ViewConverters.convertFabulousImageToXamarinFormsImageSource
        |> should be instanceOfType<FileImageSource>
        
//    /// The UriImageSource class requires Xamarin.Forms.Init() to be called, which can't be done in a test project
//    [<Test>]
//    let ``Given a network path, convertFabulousImageToXamarinFormsImageSource should return a UriImageSource``() =
//        ImagePath "http://localhost/path/to/image.png"
//        |> ViewConverters.convertFabulousImageToXamarinFormsImageSource
//        |> should be instanceOfType<UriImageSource>

    [<Test>]
    let ``Given a byte array, convertFabulousImageToXamarinFormsImageSource should return a StreamImageSource``() =
        ImageBytes [| 0uy; 1uy |]
        |> ViewConverters.convertFabulousImageToXamarinFormsImageSource
        |> should be instanceOfType<StreamImageSource>

    [<Test>]
    let ``Given a stream, convertFabulousImageToXamarinFormsImageSource should return a StreamImageSource``() =
        ImageStream (new MemoryStream())
        |> ViewConverters.convertFabulousImageToXamarinFormsImageSource
        |> should be instanceOfType<StreamImageSource>

    [<Test>]
    let ``Given a Font, convertFabulousImageToXamarinFormsImageSource should return a FontImageSource``() =
        ImageFont (FontImageSource(Glyph = "\uf10b", FontFamily = "Material Design Icons"))
        |> ViewConverters.convertFabulousImageToXamarinFormsImageSource
        |> should be instanceOfType<FontImageSource>

    [<Test>]
    let ``Given an ImageSource, convertFabulousImageToXamarinFormsImageSource should return a ImageSource``() =
        ImageSrc (ImageSource.FromFile("path/to/image.png"))
        |> ViewConverters.convertFabulousImageToXamarinFormsImageSource
        |> should be instanceOfType<ImageSource>
        
    [<Test>]
    let ``Given a file system path, convertFabulousMediaToXamarinFormsMediaSource should return a FileMediaSource``() =
        MediaPath "path/to/video.mp4"
        |> ViewConverters.convertFabulousMediaToXamarinFormsMediaSource
        |> should be instanceOfType<FileMediaSource>
        
    [<Test>]
    let ``Given a network path, convertFabulousMediaToXamarinFormsMediaSource should return a UriMediaSource``() =
        MediaPath "http://localhost/path/to/video.mp4"
        |> ViewConverters.convertFabulousMediaToXamarinFormsMediaSource
        |> should be instanceOfType<UriMediaSource>

    [<Test>]
    let ``Given an MediaSource, convertFabulousMediaToXamarinFormsMediaSource should return a MediaSource``() =
        MediaSrc (MediaSource.FromFile("path/to/video.mp4"))
        |> ViewConverters.convertFabulousMediaToXamarinFormsMediaSource
        |> should be instanceOfType<MediaSource>
