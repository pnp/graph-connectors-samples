namespace O365C.GraphConnector.MicrosoftLearn.Models;

/// <summary>
/// Star ratings
/// </summary>
public sealed class Rating
{
    /// <summary>
    /// Number of people who have rated this content.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Average of recorded ratings (1-5). This will be zero if no ratings have been recorded.
    /// </summary>
    public double Average { get; set; }
}