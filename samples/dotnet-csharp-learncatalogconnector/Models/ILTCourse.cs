using Newtonsoft.Json;
using O365C.GraphConnector.MicrosoftLearn.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace O365C.GraphConnector.MicrosoftLearn.Models;

/// <summary>
/// This represents an instructor-led course record in Microsoft Learn.
/// </summary>
[DebuggerDisplay("{Title} - [{Uid}]")]
public sealed class InstructorLedCourse
{
    /// <summary>
    /// Unique identifier for the instructor-led course. This value will be unique
    /// across all of Microsoft Learn.
    /// </summary>
    public string Uid { get; set; } = string.Empty;

    /// <summary>
    /// The instructor-led course number identifier.
    /// </summary>
    [JsonProperty("course_number")]
    public string CourseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Title of the instructor-led course in the requested locale.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// A string that provides a short description of the instructor-led class.
    /// The value is expressed as an HTML paragraph tag with the inner text being the summary.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// The average time this instructor-led course takes to complete in hours.
    /// </summary>
    [JsonProperty("duration_in_hours")]
    public int Duration { get; set; }

    /// <summary>
    /// A fully qualified URL to a 100x100 SVG image that represents
    /// the achievement image with a transparent background.
    /// </summary>
    [JsonProperty("icon_url")]
    public string IconUrl { get; set; } = string.Empty;

    /// <summary>
    /// Last modified date for this course.
    /// </summary>
    [JsonProperty("last_modified")]
    public DateTime LastModified { get; set; }

    /// <summary>
    /// A list of languages this course is offered in.
    /// </summary>
    public List<string> Locales { get; set; } = new();

    /// <summary>
    /// Certification associated with the instructor-led course.
    /// </summary>
    public string Certification { get; set; } = string.Empty;

    /// <summary>
    /// Exam associated with the instructor-led course.
    /// </summary>
    public string Exam { get; set; } = string.Empty;

    /// <summary>
    /// A list of the levels associated with this course, which indicate
    /// how much experience in the role is necessary to understand all aspects
    /// of this module. Details about the units can be referenced in the level records.
    /// </summary>
    public List<string> Levels { get; set; } = new();

    /// <summary>
    /// A list of the job roles that this certification is relevant to.
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// A list of relevant products this exam covers.
    /// Details about the products can be referenced in the product records.
    /// </summary>
    public List<string> Products { get; set; } = new();

    /// <summary>
    /// A list of the associated content to study for this course.
    /// Details about the objects can be referenced in their associated records.
    /// </summary>
    [JsonProperty("study_guide")]
    public List<StudyGuide> StudyGuide { get; set; } = new();

    /// <summary>
    /// URL for this instructor-led course. This includes the
    /// locale and tracking query string information.
    /// </summary>
    [JsonProperty("url")]
    public string TrackedUrl { get; set; } = string.Empty;

    /// <summary>
    /// Returns the base URL for this instructor-led course.
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
    public override string ToString() => $"{CourseNumber} - {Title}";
}