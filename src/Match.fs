module Match

    open Contacts
    open Files
    open FSharp.Data
    open Leads
    open System
    open System.Linq

    let matchOutput 
        (inputData:Runtime.CsvFile<inputFile.Row>)
        (knownLeads:'InputFile) 
        (getMatched:seq<inputFile.Row> -> 'InputFile -> Runtime.CsvFile<'OutputRow>) 
        (comparison:inputFile.Row -> 'InputFile -> bool)
        (outputFileName:string) =

        let matchedOutput = getMatched inputData.Rows knownLeads
        matchedOutput.Save(outputFileName)
        inputData.Filter(fun row -> not (comparison row knownLeads))

    let matchAll (inputData:inputFile) (contacts:contactInput) (leads:leadInput) =

        // get matched contact emails
        let unMatched = matchOutput inputData contacts getMatchedContactsByEmail contactEmailExists contactEmailOutputFile
 
        // get matched lead emails
        let unMatched = matchOutput unMatched leads getMatchedLeadsByEmail leadEmailExists leadEmailOutputFile

        // get matched contact names
        let unMatched = matchOutput unMatched contacts getMatchedContactsByName contactNameExists contactNameOutputFile

        // get matched lead names
        let unMatched = matchOutput unMatched leads getMatchedLeadsByName leadNameExists leadNameOutputFile

        // get matched fuzzy contact names
        let unMatched = matchOutput unMatched contacts getMatchedContactsByFuzzyName contactFuzzyNameExists contactNameFuzzyOutputFile

        // get matched fuzzy lead names
        matchOutput unMatched leads getMatchedLeadsByFuzzyName leadFuzzyNameExists leadNameFuzzyOutputFile