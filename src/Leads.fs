module Leads

    open Files
    open FSharp.Data
    open System
    open System.Linq

    type leadInput = CsvProvider<"./data/templates/LeadInput.csv">    
    type leadOutput = CsvProvider<"./data/templates/LeadOutput.csv">

    let leadEmailOutputFile = outputFolder + "LeadMatchedEmails.csv"
    let leadNameOutputFile = outputFolder + "LeadMatchedNames.csv"
    let leadNameFuzzyOutputFile = outputFolder + "LeadMatchedNamesFuzzy.csv"

    let leadEmailMatches (inputRow:inputFile.Row) (row:leadInput.Row) =           
        not(String.IsNullOrEmpty(row.Email))
            && String.Equals(row.Email, inputRow.``Email Address``, StringComparison.OrdinalIgnoreCase)

    let leadNameMatches (inputRow:inputFile.Row) (row:leadInput.Row) = 
        not(String.IsNullOrEmpty(row.``First Name``) 
            || String.IsNullOrEmpty(row.``Last Name``))
        && String.Equals(row.``First Name``, inputRow.``First Name``, StringComparison.OrdinalIgnoreCase)
        && String.Equals(row.``Last Name``, inputRow.``Last Name``, StringComparison.OrdinalIgnoreCase)

    let leadNameFuzzyMatch (inputRow:inputFile.Row) (row:leadInput.Row) = 

        if row.``First Name``.Length > 2 && inputRow.``First Name``.Length > 2 then
            // first three characters match plus last name
            let firstThreeInput = inputRow.``First Name``.Substring(0, 3)
            let firstThreeLead = row.``First Name``.Substring(0, 3)

            not(String.IsNullOrEmpty(row.``First Name``) || String.IsNullOrEmpty(row.``Last Name``))
            && String.Equals(firstThreeLead, firstThreeInput, StringComparison.OrdinalIgnoreCase)
            && String.Equals(row.``Last Name``, inputRow.``Last Name``, StringComparison.OrdinalIgnoreCase)
        else
            false

    let leadEmailExists (inputRow:inputFile.Row) (leads:leadInput) =
        leads.Rows 
        |> Seq.exists(fun row -> leadEmailMatches inputRow row)

    let leadNameExists (inputRow:inputFile.Row) (leads:leadInput) =
        leads.Rows 
        |> Seq.exists(fun row -> leadNameMatches inputRow row)

    let leadFuzzyNameExists (inputRow:inputFile.Row) (leads:leadInput) =
        leads.Rows 
        |> Seq.exists(fun row -> leadNameFuzzyMatch inputRow row)

    let getLeadOutputRow (inputRow:inputFile.Row) (compareFunc:inputFile.Row -> leadInput.Row -> bool) (leads:leadInput) =
        let result = 
            leads.Rows 
            |> Seq.filter(fun row -> compareFunc inputRow row)
            |> Seq.map(fun row -> 
                        new leadOutput.Row(
                            httpPrefix + row.``Lead Long ID``, 
                            row.``Lead Owner``, 
                            inputRow.``First Name``, 
                            inputRow.``Last Name``, 
                            inputRow.Company,
                            inputRow.``Email Address``, 
                            inputRow.``Work City``, 
                            inputRow.``Work State``)
                       )
            |> Seq.head

        result

    let getBlankLeadOutput =
        let leadOutput = new leadOutput()
        leadOutput.Take(0) // do this to remove sample row

    let getMatchedLeadsByType
        (inputData:seq<inputFile.Row>) 
        (contacts:leadInput)
        (matchType:string)
        (rowExists:inputFile.Row -> leadInput -> bool)
        (rowMatches:inputFile.Row -> leadInput.Row -> bool)
         =

        // matched leads by type
        Console.WriteLine(sprintf "Checking lead records for %s..." matchType)
        let outputFile = getBlankLeadOutput

        let leadMatchedRecords = 
            inputData
            |> Seq.filter(fun row -> rowExists row contacts)
            |> Seq.map(fun row -> getLeadOutputRow row rowMatches contacts)
            |> Seq.toArray

        Console.WriteLine(sprintf "Found %i matched lead %s" (leadMatchedRecords.Count()) matchType)

        outputFile.Append leadMatchedRecords