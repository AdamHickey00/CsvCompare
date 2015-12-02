open Compare
open FSharp.Data
open System
open System.Linq

    // take contacts first, then check leads

    // 


    // leads
    // hyperlink + lead long id
    // record owner

    // contacts
    // hyperlink + contact id
    // hyperlink + account long id

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

    let contactMatchedEmails = 
        inputData.Rows
        |> Seq.filter(fun row -> contactEmailExists row.``Email Address`` contacts)
        |> Seq.map(fun row -> getContactOutputRow row contacts)
        |> Seq.toArray

    let contactEmailOutput = contactEmailOutput.Append contactMatchedEmails
    contactEmailOutput.Save("../../data/output/ContactMatchedEmails.csv")




    // matched leads by email

    //Console.WriteLine("Checking lead records for emails...")
    //let leadMatchedEmails = inputData.Filter(fun row -> emailMatches row.``Email Address`` contacts leads)
    //leadMatchedEmails.Save("../../data/output/LeadMatchedEmails.csv")
    //Console.WriteLine(sprintf "Found %i matched emails" (leadMatchedEmails.Rows.Count()))

    //let unMatched = inputData.Filter(fun row -> not (emailMatches row.``Email Address`` contacts leads))
    
    



    

    // matched by name exactly
    //Console.WriteLine("Checking names...")
    //let matchedNames = unMatched.Filter(fun row -> namesMatch row.``First Name`` row.``Last Name`` contacts leads)
    //let unMatchedNames = unMatched.Filter(fun row -> not (namesMatch row.``First Name`` row.``Last Name`` contacts leads))
    //Console.WriteLine(sprintf "Found %i matched names" (matchedNames.Rows.Count()))

    // last name plus first three letters of first name fuzzy


    //unMatchedNames.Save("../../data/output/NeedsReview.csv")

    0 
