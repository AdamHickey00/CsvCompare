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
        (getMatched:seq<inputFile.Row> -> 'InputFile -> Runtime.CsvFile<'OutputRow>) 
        (comparison:inputFile.Row -> 'InputFile -> bool)
        (outputFileName:string) 
        (inputData:Runtime.CsvFile<inputFile.Row>) =

        let matchedOutput = getMatched inputData.Rows knownLeads
        matchedOutput.Save(outputFileName)
        inputData.Filter(fun row -> not (comparison row knownLeads))

    let getMatchedContactsByEmail (inputData:seq<inputFile.Row>) (contacts:seq<SourceRecord>) =
        getMatchedContactsByType inputData contacts "emails" contactEmailExists emailMatches

    let getMatchedContactsByName (inputData:seq<inputFile.Row>) (contacts:seq<SourceRecord>) =
        getMatchedContactsByType inputData contacts "names" contactNameExists nameMatches

    let getMatchedContactsByFuzzyName (inputData:seq<inputFile.Row>) (contacts:seq<SourceRecord>) =
        getMatchedContactsByType inputData contacts "fuzzy names" contactFuzzyNameExists fuzzyNameMatches

    let getMatchedLeadsByEmail (inputData:seq<inputFile.Row>) (leads:seq<SourceRecord>) =
        getMatchedLeadsByType inputData leads "emails" leadEmailExists emailMatches

    let getMatchedLeadsByName (inputData:seq<inputFile.Row>) (leads:seq<SourceRecord>) =
        getMatchedLeadsByType inputData leads "names" leadNameExists nameMatches

    let getMatchedLeadsByFuzzyName (inputData:seq<inputFile.Row>) (leads:seq<SourceRecord>) =
        getMatchedLeadsByType inputData leads "fuzzy names" leadFuzzyNameExists fuzzyNameMatches

    let matchAll (inputData:inputFile) (contactInput:contactInput) (leadInput:leadInput) =
        
        let contacts = 
            contactInput.Rows |> Seq.map(fun row -> Contacts.convertContactToRecord row)

        let leads = 
            leadInput.Rows |> Seq.map(fun row -> Leads.convertLeadToRecord row)

        inputData 
        |> matchOutput contacts getMatchedContactsByEmail contactEmailExists contactEmailOutputFile        
        |> matchOutput leads getMatchedLeadsByEmail leadEmailExists leadEmailOutputFile
        |> matchOutput contacts getMatchedContactsByName contactNameExists contactNameOutputFile
        |> matchOutput leads getMatchedLeadsByName leadNameExists leadNameOutputFile
        |> matchOutput contacts getMatchedContactsByFuzzyName contactFuzzyNameExists contactNameFuzzyOutputFile
        |> matchOutput leads getMatchedLeadsByFuzzyName leadFuzzyNameExists leadNameFuzzyOutputFile