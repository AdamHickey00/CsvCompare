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

    let getMatchedContactsByEmail (inputData:seq<inputFile.Row>) (contacts:contactInput) =
        getMatchedContactsByType inputData contacts "emails" contactEmailExists contactEmailMatches

    let getMatchedContactsByName (inputData:seq<inputFile.Row>) (contacts:contactInput) =
        getMatchedContactsByType inputData contacts "names" contactNameExists contactNameMatches

    let getMatchedContactsByFuzzyName (inputData:seq<inputFile.Row>) (contacts:contactInput) =
        getMatchedContactsByType inputData contacts "fuzzy names" contactFuzzyNameExists contactNameFuzzyMatch

    let getMatchedLeadsByEmail (inputData:seq<inputFile.Row>) (leads:leadInput) =
        getMatchedLeadsByType inputData leads "emails" leadEmailExists leadEmailMatches

    let getMatchedLeadsByName (inputData:seq<inputFile.Row>) (leads:leadInput) =
        getMatchedLeadsByType inputData leads "names" leadNameExists leadNameMatches

    let getMatchedLeadsByFuzzyName (inputData:seq<inputFile.Row>) (leads:leadInput) =
        getMatchedLeadsByType inputData leads "fuzzy names" leadFuzzyNameExists leadNameFuzzyMatch

    let matchAll (inputData:inputFile) (contacts:contactInput) (leads:leadInput) =
        inputData 
        |> matchOutput contacts getMatchedContactsByEmail contactEmailExists contactEmailOutputFile        
        |> matchOutput leads getMatchedLeadsByEmail leadEmailExists leadEmailOutputFile
        |> matchOutput contacts getMatchedContactsByName contactNameExists contactNameOutputFile
        |> matchOutput leads getMatchedLeadsByName leadNameExists leadNameOutputFile
        |> matchOutput contacts getMatchedContactsByFuzzyName contactFuzzyNameExists contactNameFuzzyOutputFile
        |> matchOutput leads getMatchedLeadsByFuzzyName leadFuzzyNameExists leadNameFuzzyOutputFile