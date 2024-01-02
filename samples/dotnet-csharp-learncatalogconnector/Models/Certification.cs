using Newtonsoft.Json;
using O365C.GraphConnector.MicrosoftLearn.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace O365C.GraphConnector.MicrosoftLearn.Models
{

    /// <summary>
    /// This represents a certification type available from Microsoft Learn
    /// </summary>
    [DebuggerDisplay("{Title} - [{Uid}]")]
    public sealed class Certification
    {
        /// <summary>
        /// Unique identifier for the certification. This value will be unique
        /// across all of Microsoft Learn.
        /// </summary>
        public string Uid { get; set; } = string.Empty;

        /// <summary>
        /// Title of the certification in the requested locale.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Subtitle of the certification in the requested locale.
        /// </summary>
        public string Subtitle { get; set; } = string.Empty;

        /// <summary>
        /// A fully qualified URL to a 100x100 SVG image that represents
        /// the achievement image with a transparent background.
        /// </summary>
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; } = string.Empty;

        /// <summary>
        /// Last modified date for this certification.
        /// </summary>
        [JsonProperty("last_modified")]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// The type of certification. The possible values are 'fundamentals', 'mce', 'mcsa', 'mcsd', 'mcse', 'mos', 'mta', 'role-based', and 'specialty'.
        /// </summary>
        [JsonProperty("certification_type")]
        public string CertificationType { get; set; } = string.Empty;

        /// <summary>
        /// A list of the associated exams required for this certification. Details about the exams
        /// can be referenced in the certification records.
        /// </summary>
        public List<string> Exams { get; set; } = new();

        /// <summary>
        /// A list of the levels associated with this certification, which indicate
        /// how much experience in the role is necessary to understand all aspects
        /// of this module. Details about the units can be referenced in the level records.
        /// </summary>
        public List<string> Levels { get; set; } = new();

        /// <summary>
        /// A list of the job roles that this certification is relevant to.
        /// </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// A list of the products that this certification is relevant to.
        /// </summary>
        public List<string> Products { get; set; } = new();

        /// <summary>
        /// A list of the associated content to study for this certification.
        /// Details about the objects can be referenced in their associated records.
        /// </summary>
        [JsonProperty("study_guide")]
        public List<StudyGuide> StudyGuide { get; set; } = new();

        /// <summary>
        /// URL for this certification. This includes the
        /// locale and tracking query string information.
        /// </summary>
        [JsonProperty("url")]
        public string TrackedUrl { get; set; } = string.Empty;

        /// <summary>
        /// Returns the base URL for this certification.
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
}