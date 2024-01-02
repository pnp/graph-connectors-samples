namespace O365C.GraphConnector.MicrosoftLearn.Model
{

    /// <summary>
    /// Associated content to study for a specific certification.
    /// </summary>
    public sealed class StudyGuide
    {
        /// <summary>
        /// The type of reference. This can be 'module', 'learningPath', etc.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The UID of the item referenced in this study guide.
        /// </summary>
        public string Uid { get; set; } = string.Empty;
    }
}