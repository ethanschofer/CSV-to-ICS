using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using static System.Net.Mime.MediaTypeNames;

namespace CsvToIcs;

public class Program
{
    /// <summary>
    /// This is the main entry point for the application.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        if (HelpCheck(args))
        {
            Console.ReadKey();
            return;
        }

        // Process the arguments
        var arguments = ProcessArguments(args);

        // Validate the arguments
        arguments = ValidateArguments(arguments);

        // Tell the user what is happening
        Console.WriteLine("Ready to convert your .CSV to .ICS files?");
        Console.WriteLine($"The application will read the .CSV file: {arguments.CsvFilePath}");
        Console.WriteLine($"The application will save the .ICS files here: {arguments.IcsDirectoryPath}");
        Console.WriteLine("Press any key when ready...");
        Console.ReadKey();

        // Clear the target directory
        ClearDirectory(arguments.IcsDirectoryPath);

        // Call the method to convert the .CSV to .ICS
        ConvertCsvToIcs(arguments.CsvFilePath, arguments.IcsDirectoryPath);

        // Let the user know the conversion is complete
        Console.WriteLine("Conversion complete!");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Check if the user is asking for help
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static bool HelpCheck(string[] args)
    {
        // If there are no arguments return false
        if (args.Length <= 0) return false;

        // Get the first argument
        var arg1 = args[0];

        // If it's not help, return false
        if (arg1 is not ("-h" or "--help")) return false;

        // Print help and return true
        PrintHelp();
        return true;
    }

    /// <summary>
    /// Convert args to strongly typed arguments
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static Arguments ProcessArguments(string[] args)
    {
        // Instantiate the arguments, contains default arguments
        var arguments = new Arguments();

        // If no args, just return the default arguments
        if (args == null)
        {
            return arguments;
        }

        // If arguments are passed, use them as the paths
        if (args.Length <= 0) return arguments;

        // Get argument1 - CSV file path
        var arg1 = args.Length > 0 ? args[0] : string.Empty;

        // Get argument2 - ICS directory path
        var arg2 = args.Length > 1 ? args[1] : string.Empty;

        // If argument1 is not empty, set the CSV file path
        if (!string.IsNullOrEmpty(arg1))
        {
            arguments.CsvFilePath = arg1;
        }

        // If argument2 is not empty, set the ICS directory path
        if (!string.IsNullOrEmpty(arg2))
        {
            arguments.IcsDirectoryPath = arg2;
        }

        return arguments;
    }

    /// <summary>
    /// Validate the arguments
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static Arguments ValidateArguments(Arguments arguments)
    {
        // Create a file info for the .CSV file
        var fileInfo = new FileInfo(arguments.CsvFilePath);

        // Check if the file exists
        if (!fileInfo.Exists)
        {
            // If it doesn't exist, set the validation state and message
            arguments.IsValid = false;
            arguments.ValidationMessage = $"The .CSV file {arguments.CsvFilePath} does not exist.";

            // Return the arguments
            return arguments;
        }

        // Check if the directory exists
        var directoryInfo = new DirectoryInfo(arguments.IcsDirectoryPath);

        // If the directory doesn't exist, print a message and return
        if (!directoryInfo.Exists)
        {
            // If it doesn't exist, set the validation state and message
            arguments.IsValid = false;
            arguments.ValidationMessage =
                $"The directory {arguments.IcsDirectoryPath}, the location where .ICS files are to be saved, does not exist.";

            // Return the arguments
            return arguments;
        }

        // Set the validation state and message
        arguments.IsValid = true;
        arguments.ValidationMessage = "Arguments are valid.";

        // Return the arguments
        return arguments;
    }

    /// <summary>
    /// Clear the target directory
    /// </summary>
    /// <param name="icsDirectoryPath"></param>
    public static void ClearDirectory(string icsDirectoryPath)
    {
        // Create a directory info object
        var directoryInfo = new DirectoryInfo(icsDirectoryPath);

        // Get the files in the directory
        var files = directoryInfo.GetFiles();

        // Loop through the files
        foreach (var file in files)
        {
            // Delete the file
            file.Delete();
        }
    }

    /// <summary>
    /// Convert the csv file to .ics files
    /// </summary>
    /// <param name="csvFilePath"></param>
    /// <param name="icsFilePath"></param>
    public static void ConvertCsvToIcs(string csvFilePath, string icsFilePath)
    {
        try
        {
            // The CSV file will have a header row
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            // Create the stream reader
            using var reader = new StreamReader(csvFilePath);

            // Create the CSV reader
            using var csv = new CsvReader(reader, config);

            // Tell the user what's happening
            Console.WriteLine("Reading .CSV File...");

            // Read records from the CSV file
            var records = csv.GetRecords<Event>();

            // Create a TextInfo, which will be used to title case the event titles
            var textInfo = new CultureInfo("en-US", false).TextInfo;

            // Loop through the records
            foreach (var record in records)
            {
                WriteIcsCard(textInfo, record, icsFilePath);
            }
        }
        catch (Exception e)
        {

            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Write the .ICS card
    /// </summary>
    /// <param name="textInfo"></param>
    /// <param name="record"></param>
    /// <param name="icsFilePath"></param>
    public static void WriteIcsCard(TextInfo textInfo, Event record, string icsFilePath)
    {
        // Title case the title, for naming the file
        var title = textInfo.ToTitleCase(record.Title);

        // Replace spaces with hyphens
        var cleanedTitle = ProcessTitle(title);
        var sluggedTitle = cleanedTitle.Replace(" ", "-");

        var startDate = record.StartDate.ToShortDateString();
        startDate = startDate.Replace("/","-").Replace(":", "-").Replace(" ", "_");

        var startTime = record.StartDate.ToShortTimeString();
        startTime = startTime.Replace("/", "-").Replace(":", "-").Replace(" ", "_");

        // Create the file name
        var icsFileName = $"{icsFilePath}\\{sluggedTitle}_{startDate}_{startTime}.ics";

        // Tell the user what's happening
        Console.WriteLine($"Writing {title}...");

        // Create the stream writer
        using var writer = new StreamWriter(icsFileName, false, Encoding.UTF8);

        // Write ICS Header
        writer.WriteLine("BEGIN:VCALENDAR");
        writer.WriteLine("VERSION:2.0");
        writer.WriteLine("CALSCALE:GREGORIAN");

        // Write each event in ICS format
        writer.WriteLine("BEGIN:VEVENT");
        writer.WriteLine($"SUMMARY:{cleanedTitle}");
        writer.WriteLine($"DESCRIPTION:{record.Description}");
        writer.WriteLine($"DTSTART:{record.StartDate:yyyyMMddTHHmmssZ}");
        writer.WriteLine($"DTEND:{record.EndDate:yyyyMMddTHHmmssZ}");
        writer.WriteLine($"LOCATION:{record.Location}");
        writer.WriteLine("END:VEVENT");

        // Write ICS Footer
        writer.WriteLine("END:VCALENDAR");
    }

    /// <summary>
    /// Clean up the title
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ProcessTitle(string input)
    {
        // If it's empty, do nothing
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // If its too long, trim it
        if (input.Length > 100)
        {
            input = input.Substring(0, 100);
        }

        // Remove special characters
        return Regex.Replace(input, "[^0-9A-Za-z _-]", "");
    }

    /// <summary>
    /// Print the help text
    /// </summary>
    static void PrintHelp()
    {
        Console.WriteLine("Help:");
        Console.WriteLine("------");
        Console.WriteLine("Executing the .CSV to .ICS converter:");
        Console.WriteLine();
        Console.WriteLine("Environment Variable:");
        Console.WriteLine("  Create an environment variable PATH entry pointing to the directory containing the .exe file CsvToIcs.exe.");
        Console.WriteLine();
        Console.WriteLine("Runtime Parameters:");
        Console.WriteLine("  arg1             .CSV File Path");
        Console.WriteLine("  arg2             .ICS Destination Directory");
        Console.WriteLine();
        Console.WriteLine("  Example (Default):         csvtoics \"C:\\CsvToIcs\\CSV\\events.csv\" \"C:\\CsvToIcs\\ICS\"");
        Console.WriteLine();
        Console.WriteLine(".CSV File Format:");
        Console.WriteLine("  Title,Description,StartDate,EndDate,Location");
        Console.WriteLine();
        Console.WriteLine("  Example:");
        Console.WriteLine("  \"Event Title\",\"Event Description\",\"01-01-2022 12:00 PM\",\"01-01-2022 1:00 PM\",\"Event Location\"");
    }
}