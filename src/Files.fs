module Files

    open FSharp.Data
    open System
    open System.IO
    open System.Linq
    
    type inputFile = CsvProvider<"./data/templates/InputTemplate.csv">

    type file = { 
        FullPath: string 
        Name: string
    }

    let outputFolder = "../../data/output/"
    let unMatchedOutputFile = outputFolder + "UnMatched.csv"

    let inputFileFolder = "../../data/"
    let input = { FullPath = inputFileFolder + "InputFile.csv"; Name = "InputFile.csv" }
    let contact = { FullPath = inputFileFolder + "ContactExport.csv"; Name = "ContactExport.csv" }
    let leads = { FullPath = inputFileFolder + "LeadExport.csv"; Name = "LeadExport.csv" }

    let userInputFiles = [input; contact; leads]

    let fileExists fullPath fileName = 
        if not (File.Exists(fullPath)) then
            Console.WriteLine(sprintf "The %s does not exist in the data folder" fileName)
            false
        else
            true

    let checkInputFiles (files:seq<file>) =
        files
        |> Seq.filter(fun file -> not (File.Exists(file.FullPath)))
        |> Seq.map(fun file ->Console.WriteLine(sprintf "The %s does not exist in the data folder" file.Name))
        |> Seq.length = 0            

    let deleteFiles files =
        for file in files do
            if File.Exists(file) then
                File.Delete(file)
    
    let ensureOutputFolderExists =
        if not (Directory.Exists(outputFolder)) then
            Directory.CreateDirectory(outputFolder) |> ignore

    let httpPrefix = "https://na28.salesforce.com/"
