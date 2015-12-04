module Contacts
    
    open Files
    open FSharp.Data
    open System
    open System.Linq

    type contactInput = CsvProvider<"./data/templates/ContactInput.csv">
    type contactOutput = CsvProvider<"./data/templates/ContactOutput.csv">

    let contactEmailOutputFile = outputFolder + "ContactMatchedEmails.csv"
    let contactNameOutputFile = outputFolder + "ContactMatchedNames.csv"
    let contactNameFuzzyOutputFile = outputFolder + "ContactMatchedNamesFuzzy.csv"

    let contactEmailMatches (inputRow:inputFile.Row) (row:contactInput.Row) =           
        not(String.IsNullOrEmpty(row.Email))
            && String.Equals(row.Email, inputRow.``Email Address``, StringComparison.OrdinalIgnoreCase)

    let contactNameMatches (inputRow:inputFile.Row) (row:contactInput.Row) = 
        not(String.IsNullOrEmpty(row.``First Name``) 
            || String.IsNullOrEmpty(row.``Last Name``))
        && String.Equals(row.``First Name``, inputRow.``First Name``, StringComparison.OrdinalIgnoreCase)
        && String.Equals(row.``Last Name``, inputRow.``Last Name``, StringComparison.OrdinalIgnoreCase)

    let contactNameFuzzyMatch (inputRow:inputFile.Row) (row:contactInput.Row) = 

        if row.``First Name``.Length > 2 && inputRow.``First Name``.Length > 2 then
            // first three characters match plus last name
            let firstThreeInput = inputRow.``First Name``.Substring(0, 3)
            let firstThreeContact = row.``First Name``.Substring(0, 3)

            not(String.IsNullOrEmpty(row.``First Name``) || String.IsNullOrEmpty(row.``Last Name``))
            && String.Equals(firstThreeContact, firstThreeInput, StringComparison.OrdinalIgnoreCase)
            && String.Equals(row.``Last Name``, inputRow.``Last Name``, StringComparison.OrdinalIgnoreCase)
        else
            false

    let contactEmailExists (inputRow:inputFile.Row) (contacts:contactInput) =
        contacts.Rows 
        |> Seq.exists(fun row -> contactEmailMatches inputRow row)

    let contactNameExists (inputRow:inputFile.Row) (contacts:contactInput) =
        contacts.Rows 
        |> Seq.exists(fun row -> contactNameMatches inputRow row)

    let contactFuzzyNameExists (inputRow:inputFile.Row) (contacts:contactInput) =
        contacts.Rows 
        |> Seq.exists(fun row -> contactNameFuzzyMatch inputRow row)

    let getContactOutputRow (inputRow:inputFile.Row) (compareFunc:inputFile.Row -> contactInput.Row -> bool) (contacts:contactInput) = 
        let result = 
            contacts.Rows 
            |> Seq.filter(fun row -> compareFunc inputRow row)
            |> Seq.map(fun row -> 
                        new contactOutput.Row(
                            httpPrefix + row.``Account Long ID``, 
                            httpPrefix + row.``Contact Long ID``, 
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
        (contacts:contactInput)
        (matchType:string)
        (rowExists:inputFile.Row -> contactInput -> bool)
        (rowMatches:inputFile.Row -> contactInput.Row -> bool)
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