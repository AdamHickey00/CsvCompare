open Compare
open FSharp.Data
open System
open System.Linq

[<EntryPoint>]
let main argv = 
    let inputData = inputFile.Load("../../data/input/InputFile.csv")
    let contacts = contactInput.Load("../../data/input/ContactExport.csv")
    let leads = leadInput.Load("../../data/input/LeadExport.csv")   

    Console.WriteLine(sprintf "Total input data rows = %i " (inputData.Rows.Count()))
    Console.WriteLine(sprintf "Total contacts rows = %i " (contacts.Rows.Count()))
    Console.WriteLine(sprintf "Total leads rows = %i " (leads.Rows.Count()))

    // matched contacts by email
    Console.WriteLine("Checking contact records for emails...")
    let contactEmailOutput = new contactOutput()
    let contactEmailOutput = contactEmailOutput.Take(0) // do this to remove sample row

    let contactMatchedEmails = 
        inputData.Rows
        |> Seq.filter(fun row -> contactEmailExists row.``Email Address`` contacts)
        |> Seq.map(fun row -> getContactOutputRow row contacts)
        |> Seq.toArray

    Console.WriteLine(sprintf "Found %i matched contact emails" (contactMatchedEmails.Count()))

    let contactEmailOutput = contactEmailOutput.Append contactMatchedEmails
    contactEmailOutput.Save("../../data/output/ContactMatchedEmails.csv")

    // matched leads by email
    Console.WriteLine("Checking lead records for emails...")
    let leadEmailOutput = new leadOutput()
    let leadEmailOutput = leadEmailOutput.Take(0) // do this to remove sample row

    let leadMatchedEmails = 
        inputData.Rows
        |> Seq.filter(fun row -> leadEmailExists row.``Email Address`` leads)
        |> Seq.map(fun row -> getLeadOutputRow row leads)
        |> Seq.toArray

    Console.WriteLine(sprintf "Found %i matched lead emails" (leadMatchedEmails.Count()))

    let leadEmailOutput = leadEmailOutput.Append leadMatchedEmails
    leadEmailOutput.Save("../../data/output/LeadMatchedEmails.csv")

    // get list of found emails and filter out input data by those emails




    // matched by name exactly
    //Console.WriteLine("Checking names...")
    //let matchedNames = unMatched.Filter(fun row -> namesMatch row.``First Name`` row.``Last Name`` contacts leads)
    //let unMatchedNames = unMatched.Filter(fun row -> not (namesMatch row.``First Name`` row.``Last Name`` contacts leads))
    //Console.WriteLine(sprintf "Found %i matched names" (matchedNames.Rows.Count()))

    // last name plus first three letters of first name fuzzy


    //unMatchedNames.Save("../../data/output/NeedsReview.csv")

    0 
