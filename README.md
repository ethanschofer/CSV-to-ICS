# CSV to ICS #

A console application that imports a .CSV file of events and converts them to .ICS files that can be imported to a calendar.

## The .CSV File ##

- The .CSV file must contain a header row
- The header row must be: 

```
Title,Description,StartDate,EndDate,Location
```

- The start date and end date should be in the format mm/dd/yyyy hh:ss Z

```
4/12/2024 11:00 AM
```

## Set Up ##

- The application: CsvToIcs.exe can be placed anywhere, but I recommend in C"\Program Files\CsvToIcs
- Create a PATH entry to point to the .exe
- Now the app can be run as just csvtoics from a command line
- For importing and exporting, you can use any location, but the default locations are:

.CSV file location

```
C:\CSVtoICS\CSV\events.csv
```

.ICS directory

```
C:\CSVtoICS\ICS
```

## Running the application ##

- The application is run from the command line
- To run with the default file locations

```
csvtoics "C:\Users\MyUserName\Documents\CsvToIcs\events.csv" "C:\Users\MyUserName\CsvToIcs\ICSFiles"
```