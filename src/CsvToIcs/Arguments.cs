// // // // /*
// // // //  * -----------------------------------------------------------------
// // // //  * © 2024 Hero Digital (https://herodigital.com/)
// // // //  * -----------------------------------------------------------------
// // // //  */

namespace CsvToIcs;

/// <summary>
/// Class for managing arguments
/// </summary>
public class Arguments
{
    /// <summary>
    /// The path to the .CSV file to read
    /// </summary>
    public string CsvFilePath { get; set; } = "c:\\CSVtoICS\\CSV\\events.csv";

    /// <summary>
    /// The path to the directory where the .ICS files will be saved
    /// </summary>
    public string IcsDirectoryPath { get; set; } = "c:\\CSVtoICS\\ICS";

    /// <summary>
    /// Whether the arguments are valid
    /// </summary>
    public bool IsValid { get; set; } = false;

    /// <summary>
    /// The validation message
    /// </summary>
    public string ValidationMessage { get; set; } = string.Empty;
}