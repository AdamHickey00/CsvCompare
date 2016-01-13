## About

CsvCompare searches through multiple csv files and returns
matches bazed on email, name, and fuzzy name.

## Building

The solution can be built using visual studio 2015.

Or it can be built on mono,

Initial build:
```
mono ./.paket/paket.bootstrapper.exe
```
Then,
```
mono ./.paket/paket.exe install
```

Mono build
```shell
./build.sh
```

## Template Files

Template files are in the repository in the
src/data/templates folder. The headers in these files
must match their corresponding input files.

	1. ContactInput.csv - Contacts sample with header
	2. ContactOutput.csv - Contacts output with ids prepended
	3. InputTemplate.csv - 3rd party input file sample with header
	4. LeadInput.csv - Leads sample with header
	5. LeadOutput.csv - Leads output with ids prepended

## Input Files

These are the source files the program uses to compare.
They are located in src/data. They should be named
as such:

	1. InputFile.csv - 3rd Party file to use as source
	2. ContactExport.csv - Known contacts
	3. LeadExport.csv - Known leads

## Output

The program sorts through the input files and outputs the
following files in src/data/output:

	1. ContactMatchedEmails.csv - Contacts found by email
	2. ContactMatchedNames.csv - Contacts found by exact full name match
	3. ContactMatchedNamesFuzzy.csv - Contacts found by fuzzy name match
	4. LeadMatchedEmails.csv - Leads found by email
	5. LeadMatchedNames.csv - Leads found by exact full name match
	6. LeadMatchedNamesFuzzy.csv - Leads found by fuzzy name match
	7. UnMatched.csv - No matches found
