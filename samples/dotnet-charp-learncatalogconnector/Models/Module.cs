using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using O365C.GraphConnector.MicrosoftLearn.Model;

namespace O365C.GraphConnector.MicrosoftLearn.Models
{

    /// <summary>
    /// This represents a single module in Microsoft Learn
    /// </summary>
    [DebuggerDisplay("{Title} - [{Uid}]")]
    public sealed class Module : SharedModel
    {
        /// <summary>
        /// First URL of the initial unit for the module.
        /// </summary>
        public string FirstUnitUrl { get; set; } = string.Empty;

        /// <summary>
        /// Number of units contained in this module.
        /// </summary>
        [JsonProperty("number_of_children")]
        public int NumberOfUnits { get; set; }

        /// <summary>
        /// List of unit UIDs
        /// </summary>
        public List<string> Units { get; set; } = new();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => Title;
    }
}