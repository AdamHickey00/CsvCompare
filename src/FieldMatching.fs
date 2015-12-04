module FieldMatching

    open Files
    open System

    type SourceRecord = {
        AccountLongID:string
        ContactLongID:string
        LeadLongID:string
        LeadOwner:string
        FirstName:string
        LastName:string
        Email:string
    }

    let emailMatches (inputRow:inputFile.Row) (record:SourceRecord) =           
        not(String.IsNullOrEmpty(record.Email))
            && String.Equals(record.Email, inputRow.``Email Address``, StringComparison.OrdinalIgnoreCase)

    let nameMatches (inputRow:inputFile.Row) (record:SourceRecord) = 
        not(String.IsNullOrEmpty(record.FirstName) 
            || String.IsNullOrEmpty(record.LastName))
        && String.Equals(record.FirstName, inputRow.``First Name``, StringComparison.OrdinalIgnoreCase)
        && String.Equals(record.LastName, inputRow.``Last Name``, StringComparison.OrdinalIgnoreCase)

    let fuzzyNameMatches (inputRow:inputFile.Row) (record:SourceRecord) = 

        if record.FirstName.Length > 2 && inputRow.``First Name``.Length > 2 then
            // first three characters match plus last name
            let firstThreeInput = inputRow.``First Name``.Substring(0, 3)
            let firstThreeContact = record.FirstName.Substring(0, 3)

            not(String.IsNullOrEmpty(record.FirstName) || String.IsNullOrEmpty(record.LastName))
            && String.Equals(firstThreeContact, firstThreeInput, StringComparison.OrdinalIgnoreCase)
            && String.Equals(record.LastName, inputRow.``Last Name``, StringComparison.OrdinalIgnoreCase)
        else
            false