namespace O365C.GraphConnector.MicrosoftLearn.Models;

/// <summary>
/// Identifier + Name pair used for levels, roles, and products.
/// </summary>
public class TaxonomyIdName
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Readable name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}