module Match

    open Contacts
    open Files
    open FSharp.Data
    open Leads
    open System
    open System.Linq

    let matchOutput         
        (knownLeads:'InputFile) 
        (getMatched:seq<inputFile.Row> -> 'InputFile -> Runtime.CsvFile<'OutputRow>) 
        (comparison:inputFile.Row -> 'InputFile -> bool)
        (outputFileName:string) 
        (inputData:Runtime.CsvFile<inputFile.Row>) =

        let matchedOutput = getMatched inputData.Rows knownLeads
        matchedOutput.Save(outputFileName)
        inputData.Filter(fun row -> not (comparison row knownLeads))

    let matchAll (inputData:inputFile) (contacts:contactInput) (leads:leadInput) =
        inputData 
        |> matchOutput contacts getMatchedContactsByEmail contactEmailExists contactEmailOutputFile        
        |> matchOutput leads getMatchedLeadsByEmail leadEmailExists leadEmailOutputFile
        |> matchOutput contacts getMatchedContactsByName contactNameExists contactNameOutputFile
        |> matchOutput leads getMatchedLeadsByName leadNameExists leadNameOutputFile
        |> matchOutput contacts getMatchedContactsByFuzzyName contactFuzzyNameExists contactNameFuzzyOutputFile
        |> matchOutput leads getMatchedLeadsByFuzzyName leadFuzzyNameExists leadNameFuzzyOutputFile