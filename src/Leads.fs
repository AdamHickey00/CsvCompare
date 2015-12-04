module Leads

    open Files
    open FieldMatching
    open FSharp.Data
    open System
    open System.Linq

    type leadInput = CsvProvider<"./data/templates/LeadInput.csv">    
    type leadOutput = CsvProvider<"./data/templates/LeadOutput.csv">

    let leadEmailOutputFile = outputFolder + "LeadMatchedEmails.csv"
    let leadNameOutputFile = outputFolder + "LeadMatchedNames.csv"
    let leadNameFuzzyOutputFile = outputFolder + "LeadMatchedNamesFuzzy.csv"

    let convertLeadToRecord (lead:leadInput.Row) : SourceRecord =
        {
            AccountLongID = ""
            ContactLongID = ""
            LeadLongID = lead.``Lead Long ID``
            LeadOwner = lead.``Lead Owner``
            FirstName = lead.``First Name``
            LastName = lead.``Last Name``
            Email = lead.Email
        }

    let getLeadOutputRow (inputRow:inputFile.Row) (compareFunc:inputFile.Row -> SourceRecord -> bool) (leads:seq<SourceRecord>) =
        let result = 
            leads 
            |> Seq.filter(fun row -> compareFunc inputRow row)
            |> Seq.map(fun row -> 
                        new leadOutput.Row(
                            httpPrefix + row.LeadLongID, 
                            row.LeadOwner, 
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
        (contacts:seq<SourceRecord>)
        (matchType:string)
        (rowExists:inputFile.Row -> seq<SourceRecord> -> bool)
        (rowMatches:inputFile.Row -> SourceRecord -> bool)
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
