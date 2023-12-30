using Newtonsoft.Json;
using System.Diagnostics;

namespace O365C.GraphConnector.MicrosoftLearn.Models;

/// <summary>
/// A company that provides exams - this can be in-person, or on-demand delivery.
/// </summary>
[DebuggerDisplay("{Type}: {ExamUrl}")]
public sealed class ExamProvider
{
    /// <summary>
    /// The provider type.
    /// </summary>
    [JsonProperty("providerType")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The URL for the provider's exam.
    /// </summary>
    public string ExamUrl { get; set; } = string.Empty;
}