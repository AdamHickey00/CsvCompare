open Contacts
open Files
open FSharp.Data
open Leads
open Match
open System
open System.Linq

[<EntryPoint>]
let main argv = 

    if checkInputFiles(userInputFiles) then
        ensureOutputFolderExists

        let inputData = inputFile.Load(input.FullPath)
        let contacts = contactInput.Load(contact.FullPath)
        let leads = leadInput.Load(leads.FullPath)

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
