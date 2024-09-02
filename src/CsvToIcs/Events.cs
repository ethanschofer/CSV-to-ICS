// // // // /*
// // // //  * -----------------------------------------------------------------
// // // //  * © 2024 Hero Digital (https://herodigital.com/)
// // // //  * -----------------------------------------------------------------
// // // //  */

namespace CsvToIcs;

/// <summary>
/// Define the Event class to match the CSV structure
/// </summary>
public class Event
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
}