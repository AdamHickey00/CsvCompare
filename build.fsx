// Include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"

open Fake

// Directories
let buildDir  = "./build"
let appReferences = !! "./src/*.sln"

// Targets
let clean _ = CleanDirs [buildDir]

let install () =
    let execParams : Fake.ProcessHelper.ExecParams = {
        Program = "./.paket/paket.exe"
        CommandLine = "install"
        WorkingDirectory = "./"
        Args = []
    }

    let result = Fake.ProcessHelper.shellExec execParams

    if result <> 0 then
        printfn "Error installing packages"
    else
        printfn "Packages successfully installed"

let build _ = MSBuildRelease buildDir "Build" appReferences |> Log "Build: "

Target "Clean" clean
Target "Install" install
Target "Build" build

// Build order
"Clean"
==> "Install"
==> "Build"

RunTargetOrDefault "Build"
