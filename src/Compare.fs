module Compare

    open FSharp.Data
    open System
    open System.Linq

    type inputFile = CsvProvider<"./data/templates/InputTemplate.csv">

    let outputFolder = "../../data/output/"
    let unMatchedOutputFile = outputFolder + "UnMatched.csv"

    let deleteFiles files =
        for file in files do
            if System.IO.File.Exists(file) then
                System.IO.File.Delete(file)

    
    let httpPrefix = "https://na28.salesforce.com/"
