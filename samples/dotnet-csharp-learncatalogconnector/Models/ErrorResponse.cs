namespace O365C.GraphConnector.MicrosoftLearn.Models;

/// <summary>
/// Error response type returned when there is a server error.
/// </summary>
internal class ErrorResponse
{
    /// <summary>
    /// Error code
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string? Message { get; set; }
}