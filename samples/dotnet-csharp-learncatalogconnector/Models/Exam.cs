using Newtonsoft.Json;
using O365C.GraphConnector.MicrosoftLearn.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace O365C.GraphConnector.MicrosoftLearn.Models;

/// <summary>
/// This represents a single certification exam supported by Microsoft Learn.
/// </summary>
[DebuggerDisplay("{Title} - [{Uid}]")]
public sealed class Exam
{
    /// <summary>
    /// Unique identifier for the certification exam. This value will be unique
    /// across all of Microsoft Learn.
    /// </summary>
    public string Uid { get; set; } = string.Empty;

    /// <summary>
    /// Title of the certification exam in the requested locale.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Subtitle of the certification exam in the requested locale.
    /// </summary>
    public string Subtitle { get; set; } = string.Empty;

    /// <summary>
    /// The display name for the exam in the locale requested, or US English as a fallback.
    /// </summary>
    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// A fully qualified URL to a 100x100 SVG image that represents
    /// the achievement image with a transparent background.
    /// </summary>
    [JsonProperty("icon_url")]
    public string IconUrl { get; set; } = string.Empty;

    /// <summary>
    /// Last modified date for this exam.
    /// </summary>
    [JsonProperty("last_modified")]
    public DateTime LastModified { get; set; }

    /// <summary>
    /// A fully qualified URL to the PDF outlining the skills measured by this exam.
    /// </summary>
    [JsonProperty("pdf_download_url")]
    public string? PdfUrl { get; set; }

    /// <summary>
    /// A fully qualified URL to the practice test associated with this exam.
    /// </summary>
    [JsonProperty("practice_test_url")]
    public string? PracticeTestUrl { get; set; }

    /// <summary>
    /// A list of languages this exam is offered in.
    /// </summary>
    public List<string> Locales { get; set; } = new();

    /// <summary>
    /// A list of the associated course UIDs. Details about the courses can be referenced in
    /// the course records.
    /// </summary>
    public List<string> Courses { get; set; } = new();

    /// <summary>
    /// A list of the levels associated with this exam, which indicate
    /// how much experience in the role is necessary to understand all aspects
    /// of this module. Details about the units can be referenced in the level records.
    /// </summary>
    public List<string> Levels { get; set; } = new();

    /// <summary>
    /// A list of the job roles that this exam is relevant to.
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// A list of relevant products this exam covers.
    /// Details about the products can be referenced in the product records.
    /// </summary>
    public List<string> Products { get; set; } = new();

    /// <summary>
    /// A list of providers for this exam. The type describes which provider and
    /// a fully qualified URL with a link to schedule an exam with the provider.
    /// </summary>
    public List<ExamProvider> Providers { get; set; } = new();

    /// <summary>
    /// A list of the associated content to study for this certification.
    /// Details about the objects can be referenced in their associated records.
    /// </summary>
    [JsonProperty("study_guide")]
    public List<StudyGuide> StudyGuide { get; set; } = new();

    /// <summary>
    /// URL for this certification exam. This includes the
    /// locale and tracking query string information.
    /// </summary>
    [JsonProperty("url")]
    public string TrackedUrl { get; set; } = string.Empty;

    /// <summary>
    /// Returns the base URL for this certification exam.
    /// This is the URL with no tracking data or locale.
    /// </summary>
    public string Url
    {
        get
        {
            var uri = new Uri(TrackedUrl);
            var parts = uri.GetLeftPart(UriPartial.Path).Split('/');
            return $"{uri.Scheme}://{uri.Host}/" + string.Join('/', parts.Skip(4));
        }
    }

    /// <summary>
    /// Returns a textual version of this object.
    /// </summary>
    /// <returns>String</returns>
    public override string ToString() => Title;
}