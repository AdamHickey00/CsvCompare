module Compare

    open FSharp.Data
    open System
    open System.Linq

    type inputFile = CsvProvider<"./data/templates/InputTemplate.csv">
    type contactInput = CsvProvider<"./data/templates/ContactInput.csv">
    type leadInput = CsvProvider<"./data/templates/LeadInput.csv">
    type contactOutput = CsvProvider<"./data/templates/ContactOutput.csv">
    type leadOutput = CsvProvider<"./data/templates/LeadInputOutput.csv">

    let httpPrefix = "https://na28.salesforce.com/"

    let contactEmailExists (email:string) (contacts:contactInput) =
        contacts.Rows 
        |> Seq.exists(fun row -> String.Equals(row.Email, email, StringComparison.OrdinalIgnoreCase))

    let getContactOutputRow (inputRow:inputFile.Row) (contacts:contactInput) = 
        let result = 
            contacts.Rows 
            |> Seq.filter(fun row -> String.Equals(row.Email, inputRow.``Email Address``, StringComparison.OrdinalIgnoreCase))        
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

    let emailMatches (email:string) (contacts:contactInput) (leads:leadInput) =
        let existsInContacts = 
            contacts.Rows 
            |> Seq.exists(fun row -> String.Equals(row.Email, email, StringComparison.OrdinalIgnoreCase))

        let existsInLeads = 
            leads.Rows 
            |> Seq.exists(fun row -> String.Equals(row.Email, email, StringComparison.OrdinalIgnoreCase))

        existsInContacts || existsInLeads

    let namesMatch (firstName:string) (lastName:string) (contacts:contactInput) (leads:leadInput) =
        let existsInContacts = 
            contacts.Rows 
            |> Seq.exists(fun row -> 
                            String.Equals(row.``First Name``, firstName, StringComparison.OrdinalIgnoreCase)
                            && String.Equals(row.``Last Name``, lastName, StringComparison.OrdinalIgnoreCase))

        let existsInLeads = 
            leads.Rows 
            |> Seq.exists(fun row -> 
                            String.Equals(row.``First Name``, firstName, StringComparison.OrdinalIgnoreCase)
                            && String.Equals(row.``Last Name``, lastName, StringComparison.OrdinalIgnoreCase))

        existsInContacts || existsInLeads