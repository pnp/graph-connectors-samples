using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using O365C.GraphConnector.MicrosoftLearn.Models;

namespace O365C.GraphConnector.MicrosoftLearn.Model
{

    /// <summary>
    /// Shared data between Learning Paths and Modules.
    /// </summary>
    public abstract class SharedModel
    {
        /// <summary>
        /// A string that provides a short description of the module or learning path.
        /// The value is expressed as an HTML paragraph tag with the inner text being the summary.
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// A list of the levels associated with this module or path, which indicate
        /// how much experience in the role is necessary to understand all aspects
        /// of this module. Details about the units can be referenced in the level records.
        /// </summary>
        public List<string> Levels { get; set; } = new();

        /// <summary>
        /// A list of the job roles that this module or learning path is relevant to.
        /// </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// A list of relevant products this module or learning path covers.
        /// Details about the products can be referenced in the product records.
        /// </summary>
        public List<string> Products { get; set; } = new();

        /// <summary>
        /// A list of relevant subjects this module or learning path covers.
        /// </summary>
        public List<string> Subjects { get; set; } = new();

        /// <summary>
        /// Unique identifier for the module or path. This value will be unique
        /// across all of Microsoft Learn.
        /// </summary>
        public string Uid { get; set; } = string.Empty;

        /// <summary>
        /// Title of the module or path in the requested locale.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Expected/Average duration in minutes of this module or path.
        /// </summary>
        [JsonProperty("duration_in_minutes")]
        public int Duration { get; set; }

        /// <summary>
        /// Star rating for this module or path.
        /// </summary>
        public Rating? Rating { get; set; }

        /// <summary>
        /// A normalized value from 0-1 indicating the popularity of the module or path.
        /// </summary>
        public double Popularity { get; set; }

        /// <summary>
        /// A fully qualified URL to a 100x100 SVG image that represents
        /// the achievement image with a transparent background.
        /// </summary>
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; } = string.Empty;

        /// <summary>
        /// A fully qualified URL to a PNG image that represents the achievement image
        /// with a rectangular opaque background, suited for social media or tile experiences.
        /// If it isn't available, this property will be empty.
        /// </summary>
        [JsonProperty("social_image_url")]
        public string SocialImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Locale language for this module or path. If the requested locale is not available,
        /// en-US will be used as a fallback.
        /// </summary>
        public string Locale { get; set; } = string.Empty;

        /// <summary>
        /// Last modified date for this module or path.
        /// </summary>
        [JsonProperty("last_modified")]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// URL for this module or path. This includes the
        /// locale and tracking query string information.
        /// </summary>
        [JsonProperty("url")]
        public string TrackedUrl { get; set; } = string.Empty;

        /// <summary>
        /// Returns the base URL for this module or path.
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
    }
}