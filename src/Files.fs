module Files

    open FSharp.Data
    open System
    open System.IO

    type inputFile = CsvProvider<"./data/templates/InputTemplate.csv">

    let outputFolder = "../../data/output/"
    let unMatchedOutputFile = outputFolder + "UnMatched.csv"

    let inputFileFolder = "../../data/"
    let inputFileLocation = inputFileFolder + "InputFile.csv"
    let contactsLocation = inputFileFolder + "ContactExport.csv"
    let leadsLocation = inputFileFolder + "LeadExport.csv"

    let checkInputFiles =
        if not (File.Exists(inputFileLocation)) then
            Console.WriteLine("The InputFile.csv does not exist in the data folder")
            false

        else if not (File.Exists(contactsLocation)) then
            Console.WriteLine("The ContactExport.csv does not exist in the data folder")
            false 

        else if not (File.Exists(leadsLocation)) then
            Console.WriteLine("The LeadExport.csv does not exist in the data folder")
            false

        else
            true

    let deleteFiles files =
        for file in files do
            if File.Exists(file) then
                File.Delete(file)
    
    let ensureOutputFolderExists =
        if not (Directory.Exists(outputFolder)) then
            Directory.CreateDirectory(outputFolder) |> ignore

    let httpPrefix = "https://na28.salesforce.com/"
