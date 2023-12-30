using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace O365C.GraphConnector.MicrosoftLearn.Models
{

    /// <summary>
    /// This represents a single page (unit) in a training module available
    /// on Microsoft Learn.
    /// </summary>
    [DebuggerDisplay("{Title} - [{Uid}]")]
    public sealed class ModuleUnit
    {
        /// <summary>
        /// Unique identifier for the unit. This value will be unique
        /// across all of Microsoft Learn.
        /// </summary>
        public string Uid { get; set; } = string.Empty;

        /// <summary>
        /// Title of the unit in the requested locale.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Expected/Average duration in minutes of this unit.
        /// If you add all the duration_in_minutes of the units associated with each module,
        /// you'll get the total module time.
        /// </summary>
        [JsonProperty("duration_in_minutes")]
        public int Duration { get; set; }

        /// <summary>
        /// Locale language for this unit. If the requested locale is not available,
        /// en-US will be used as a fallback.
        /// </summary>
        public string Locale { get; set; } = string.Empty;

        /// <summary>
        /// Last modified date for this unit.
        /// </summary>
        [JsonProperty("last_modified")]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => Title;
    }
}