module Contacts
    
    open Files
    open FieldMatching
    open FSharp.Data
    open System
    open System.Linq

    type contactInput = CsvProvider<"./data/templates/ContactInput.csv">
    type contactOutput = CsvProvider<"./data/templates/ContactOutput.csv">

    let contactEmailOutputFile = outputFolder + "ContactMatchedEmails.csv"
    let contactNameOutputFile = outputFolder + "ContactMatchedNames.csv"
    let contactNameFuzzyOutputFile = outputFolder + "ContactMatchedNamesFuzzy.csv"

    let convertContactToRecord (contact:contactInput.Row) : SourceRecord =
        {
            AccountLongID = contact.``Account Long ID``
            ContactLongID = contact.``Contact Long ID``
            LeadLongID = ""
            LeadOwner = ""
            FirstName = contact.``First Name``
            LastName = contact.``Last Name``
            Email = contact.Email
        }

    let getContactOutputRow 
        (inputRow:inputFile.Row) 
        (compareFunc:inputFile.Row -> SourceRecord -> bool) 
        (contacts:seq<SourceRecord>) = 

        let result = 
            contacts
            |> Seq.filter(fun row -> compareFunc inputRow row)
            |> Seq.map(fun row -> 
                        new contactOutput.Row(
                            httpPrefix + row.AccountLongID, 
                            httpPrefix + row.ContactLongID, 
                            inputRow.``First Name``, 
                            inputRow.``Last Name``, 
                            inputRow.Company,
                            inputRow.``Email Address``, 
                            inputRow.``Work City``, 
                            inputRow.``Work State``)
                       )
            |> Seq.head

        result

    let getBlankContactOutput =
        let contactOutput = new contactOutput()
        contactOutput.Take(0) // do this to remove sample row

    let getMatchedContactsByType
        (inputData:seq<inputFile.Row>) 
        (contacts:seq<SourceRecord>)
        (matchType:string)
        (rowExists:inputFile.Row -> seq<SourceRecord> -> bool)
        (rowMatches:inputFile.Row -> SourceRecord -> bool)
         =

        // matched contacts by type
        Console.WriteLine(sprintf "Checking contact records for %s..." matchType)
        let outputFile = getBlankContactOutput

        let contactMatchedRecords = 
            inputData
            |> Seq.filter(fun row -> rowExists row contacts)
            |> Seq.map(fun row -> getContactOutputRow row rowMatches contacts)
            |> Seq.toArray

        Console.WriteLine(sprintf "Found %i matched contact %s" (contactMatchedRecords.Count()) matchType)

        outputFile.Append contactMatchedRecords