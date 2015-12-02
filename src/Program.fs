open Compare
open FSharp.Data
open System
open System.Linq

let outputFolder = "../../data/output/"
let contactEmailOutputFile = outputFolder + "ContactMatchedEmails.csv"
let leadEmailOutputFile = outputFolder + "LeadMatchedEmails.csv"
let unMatchedOutputFile = outputFolder + "UnMatched.csv"

let deleteFiles files =
    for file in files do
        if System.IO.File.Exists(file) then
            System.IO.File.Delete(file)

[<EntryPoint>]
let main argv = 
    let inputData = inputFile.Load("../../data/input/InputFile.csv")
    let contacts = contactInput.Load("../../data/input/ContactExport.csv")
    let leads = leadInput.Load("../../data/input/LeadExport.csv")
    deleteFiles [contactEmailOutputFile; leadEmailOutputFile; unMatchedOutputFile]

    Console.WriteLine(sprintf "Total input data rows = %i " (inputData.Rows.Count()))
    Console.WriteLine(sprintf "Total contacts rows = %i " (contacts.Rows.Count()))
    Console.WriteLine(sprintf "Total leads rows = %i " (leads.Rows.Count()))

    // get matched contact emails
    let contactEmailOutput = getMatchedContactsByEmail inputData.Rows contacts
    contactEmailOutput.Save(contactEmailOutputFile)

    let contactEmails = 
        contactEmailOutput.Rows |> Seq.map(fun row -> row.``Email Address``.ToLower())

    let unMatched = 
        inputData.Filter(fun row -> not (contactEmails.Contains(row.``Email Address``.ToLower())))
 
    // get matched lead emails
    let leadEmailOutput = getMatchedLeadsByEmail unMatched.Rows leads
    leadEmailOutput.Save(leadEmailOutputFile)

    let leadEmails = 
        leadEmailOutput.Rows |> Seq.map(fun row -> row.``Email Address``.ToLower())

    let unMatched = 
        unMatched.Filter(fun row -> not (leadEmails.Contains(row.``Email Address``.ToLower())))

    unMatched.Save(unMatchedOutputFile)

    // matched by name exactly
    //Console.WriteLine("Checking names...")
    //let matchedNames = unMatched.Filter(fun row -> namesMatch row.``First Name`` row.``Last Name`` contacts leads)
    //let unMatchedNames = unMatched.Filter(fun row -> not (namesMatch row.``First Name`` row.``Last Name`` contacts leads))
    //Console.WriteLine(sprintf "Found %i matched names" (matchedNames.Rows.Count()))

    // last name plus first three letters of first name fuzzy


    //unMatchedNames.Save("../../data/output/NeedsReview.csv")

    0 
