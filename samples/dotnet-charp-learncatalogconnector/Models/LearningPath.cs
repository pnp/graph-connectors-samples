using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using O365C.GraphConnector.MicrosoftLearn.Model;

namespace O365C.GraphConnector.MicrosoftLearn.Models
{

    /// <summary>
    /// This represents a single Learning Path on Microsoft Learn
    /// </summary>
    [DebuggerDisplay("{Title} - [{Uid}]")]
    public sealed class LearningPath : SharedModel
    {
        /// <summary>
        /// A fully qualified URL to the first module of the learning
        /// path in Microsoft Learn in the requested locale.
        /// </summary>
        public string FirstModuleUrl { get; set; } = string.Empty;

        /// <summary>
        ///A list of the associated module UIDs. Details about the
        /// modules can be referenced in the module records.
        /// </summary>
        public List<string> Modules { get; set; } = new();

        /// <summary>
        /// Number of modules in this learning path.
        /// </summary>
        [JsonProperty("number_of_children")]
        public int NumberOfModules { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => Title;
    }
}