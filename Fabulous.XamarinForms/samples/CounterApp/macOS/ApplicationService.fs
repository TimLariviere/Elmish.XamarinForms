namespace CounterApp.MacOS

open AppKit
open CounterApp

type ApplicationService() =
    interface IApplicationService with
        member x.CloseApplication() =
            NSApplication.SharedApplication.Terminate(null)

