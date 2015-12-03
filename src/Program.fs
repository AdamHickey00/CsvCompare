open Contacts
open Files
open FSharp.Data
open Leads
open System
open System.Linq

let matchContactOutput 
    (inputData:Runtime.CsvFile<inputFile.Row>)
    (contacts:contactInput) 
    (getMatched:seq<inputFile.Row> -> contactInput -> Runtime.CsvFile<contactOutput.Row>) 
    (comparison:inputFile.Row -> contactInput -> bool)
    (outputFileName:string) =

    let matchedOutput = getMatched inputData.Rows contacts
    matchedOutput.Save(outputFileName)
    inputData.Filter(fun row -> not (comparison row contacts))

[<EntryPoint>]
let main argv = 
    let allInputFilesPresent = checkInputFiles

    if allInputFilesPresent then
        ensureOutputFolderExists

        let inputData = inputFile.Load(inputFileLocation)
        let contacts = contactInput.Load(contactsLocation)
        let leads = leadInput.Load(leadsLocation)

        deleteFiles [contactEmailOutputFile; 
                     leadEmailOutputFile; 
                     contactNameOutputFile; 
                     leadNameOutputFile; 
                     contactNameFuzzyOutputFile;
                     leadNameFuzzyOutputFile
                     unMatchedOutputFile]

        Console.WriteLine(sprintf "Total input data rows = %i " (inputData.Rows.Count()))
        Console.WriteLine(sprintf "Total contacts rows = %i " (contacts.Rows.Count()))
        Console.WriteLine(sprintf "Total leads rows = %i " (leads.Rows.Count()))

        // get matched contact emails
        let unMatched = matchContactOutput inputData contacts getMatchedContactsByEmail contactEmailExists contactEmailOutputFile
 
        // get matched lead emails
        let leadEmailOutput = getMatchedLeadsByEmail unMatched.Rows leads
        leadEmailOutput.Save(leadEmailOutputFile)
        let unMatched = unMatched.Filter(fun row -> not (leadEmailExists row leads))

        // get matched contact names
        let unMatched = matchContactOutput unMatched contacts getMatchedContactsByName contactNameExists contactNameOutputFile

        // get matched lead names
        let leadNameOutput = getMatchedLeadsByName unMatched.Rows leads
        leadNameOutput.Save(leadNameOutputFile)
        let unMatched = unMatched.Filter(fun row -> not (leadNameExists row leads))

        // get matched fuzzy contact names
        let unMatched = matchContactOutput unMatched contacts getMatchedContactsByFuzzyName contactFuzzyNameExists contactNameFuzzyOutputFile

        // get matched fuzzy lead names
        let leadFuzzyNameOutput = getMatchedLeadsByFuzzyName unMatched.Rows leads
        leadFuzzyNameOutput.Save(leadNameFuzzyOutputFile)
        let unMatched = unMatched.Filter(fun row -> not (leadFuzzyNameExists row leads))

        // save unmatched records
        Console.WriteLine("Saving unmatched records...")
        unMatched.Save(unMatchedOutputFile)

        0
    
    else
        1 
