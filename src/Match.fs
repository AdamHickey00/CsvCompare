module Match

    open Contacts
    open FieldMatching
    open Files    
    open FSharp.Data
    open Leads
    open System
    open System.Linq

    let matchOutput         
        (knownLeads:'InputFile) 
        (getMatched:array<inputFile.Row> -> 'InputFile -> Runtime.CsvFile<'OutputRow>) 
        (comparison:inputFile.Row -> 'InputFile -> bool)
        (outputFileName:string) 
        (inputData:array<inputFile.Row>) =

        let matchedOutput = getMatched inputData knownLeads
        matchedOutput.Save(outputFileName)

        inputData |> Array.filter(fun row -> not (comparison row knownLeads))

    let getMatchedContactsByEmail (inputData:array<inputFile.Row>) (contacts:array<SourceRecord>) =
        getMatchedContactsByType inputData contacts "emails" emailExists emailMatches

    let getMatchedLeadsByEmail (inputData:array<inputFile.Row>) (leads:array<SourceRecord>) =
        getMatchedLeadsByType inputData leads "emails" emailExists emailMatches

    let getMatchedContactsByName (inputData:array<inputFile.Row>) (contacts:array<SourceRecord>) =
        getMatchedContactsByType inputData contacts "names" nameExists nameMatches

    let getMatchedLeadsByName (inputData:array<inputFile.Row>) (leads:array<SourceRecord>) =
        getMatchedLeadsByType inputData leads "names" nameExists nameMatches

    let getMatchedContactsByFuzzyName (inputData:array<inputFile.Row>) (contacts:array<SourceRecord>) =
        getMatchedContactsByType inputData contacts "fuzzy names" fuzzyNameExists fuzzyNameMatches

    let getMatchedLeadsByFuzzyName (inputData:array<inputFile.Row>) (leads:array<SourceRecord>) =
        getMatchedLeadsByType inputData leads "fuzzy names" fuzzyNameExists fuzzyNameMatches

    let matchAll (inputData:inputFile) (contactInput:contactInput) (leadInput:leadInput) =
        
        let contacts = 
            contactInput.Rows 
            |> Seq.map(fun row -> Contacts.convertContactToRecord row)
            |> Seq.toArray

        let leads = 
            leadInput.Rows 
            |> Seq.map(fun row -> Leads.convertLeadToRecord row)
            |> Seq.toArray

        inputData.Rows 
        |> Seq.toArray
        |> matchOutput contacts getMatchedContactsByEmail emailExists contactEmailOutputFile        
        |> matchOutput leads getMatchedLeadsByEmail emailExists leadEmailOutputFile
        |> matchOutput contacts getMatchedContactsByName nameExists contactNameOutputFile
        |> matchOutput leads getMatchedLeadsByName nameExists leadNameOutputFile
        |> matchOutput contacts getMatchedContactsByFuzzyName fuzzyNameExists contactNameFuzzyOutputFile
        |> matchOutput leads getMatchedLeadsByFuzzyName fuzzyNameExists leadNameFuzzyOutputFile