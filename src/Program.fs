open Contacts
open Files
open FSharp.Data
open Leads
open Match
open System
open System.Linq

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

        let unMatched = matchAll inputData contacts leads

        // save unmatched records
        Console.WriteLine("Saving unmatched records...")
        unMatched.Save(unMatchedOutputFile)

        0
    
    else
        1 
