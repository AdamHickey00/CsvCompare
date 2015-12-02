module Compare

    open FSharp.Data
    open System
    open System.Linq

    type inputFile = CsvProvider<"./data/templates/InputTemplate.csv">
    type contactInput = CsvProvider<"./data/templates/ContactInput.csv">
    type leadInput = CsvProvider<"./data/templates/LeadInput.csv">
    type contactOutput = CsvProvider<"./data/templates/ContactOutput.csv">
    type leadOutput = CsvProvider<"./data/templates/LeadOutput.csv">

    let httpPrefix = "https://na28.salesforce.com/"

    let contactEmailExists (email:string) (contacts:contactInput) =
        contacts.Rows 
        |> Seq.exists(fun row -> not(String.IsNullOrEmpty(row.Email))
                                 && String.Equals(row.Email, email, StringComparison.OrdinalIgnoreCase))

    let getContactOutputRow (inputRow:inputFile.Row) (contacts:contactInput) = 
        let result = 
            contacts.Rows 
            |> Seq.filter(fun row -> not(String.IsNullOrEmpty(row.Email))
                                     && String.Equals(row.Email, inputRow.``Email Address``, StringComparison.OrdinalIgnoreCase))        
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

    let leadEmailExists (email:string) (leads:leadInput) =
        leads.Rows 
        |> Seq.exists(fun row -> not(String.IsNullOrEmpty(row.Email))
                                 && String.Equals(row.Email, email, StringComparison.OrdinalIgnoreCase))

    let getLeadOutputRow (inputRow:inputFile.Row) (leads:leadInput) =
        let result = 
            leads.Rows 
            |> Seq.filter(fun row -> not(String.IsNullOrEmpty(row.Email))
                                     && String.Equals(row.Email, inputRow.``Email Address``, StringComparison.OrdinalIgnoreCase))        
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

    let getMatchedContactsByEmail (inputData:seq<inputFile.Row>) (contacts:contactInput) =

        // matched contacts by email
        Console.WriteLine("Checking contact records for emails...")
        let contactEmailOutput = new contactOutput()
        let contactEmailOutput = contactEmailOutput.Take(0) // do this to remove sample row

        let contactMatchedEmails = 
            inputData
            |> Seq.filter(fun row -> contactEmailExists row.``Email Address`` contacts)
            |> Seq.map(fun row -> getContactOutputRow row contacts)
            |> Seq.toArray

        Console.WriteLine(sprintf "Found %i matched contact emails" (contactMatchedEmails.Count()))

        contactEmailOutput.Append contactMatchedEmails

    let getMatchedLeadsByEmail (inputData:seq<inputFile.Row>) (leads:leadInput) =

        // matched leads by email
        Console.WriteLine("Checking lead records for emails...")
        let leadEmailOutput = new leadOutput()
        let leadEmailOutput = leadEmailOutput.Take(0) // do this to remove sample row

        let leadMatchedEmails = 
            inputData
            |> Seq.filter(fun row -> leadEmailExists row.``Email Address`` leads)
            |> Seq.map(fun row -> getLeadOutputRow row leads)
            |> Seq.toArray

        Console.WriteLine(sprintf "Found %i matched lead emails" (leadMatchedEmails.Count()))

        leadEmailOutput.Append leadMatchedEmails