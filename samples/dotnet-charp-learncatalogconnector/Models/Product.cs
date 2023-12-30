using System.Collections.Generic;

namespace O365C.GraphConnector.MicrosoftLearn.Models;

/// <summary>
/// Tree of products available in Microsoft Learn
/// </summary>
public sealed class Product : TaxonomyIdName
{
    /// <summary>
    /// Child products related to this product.
    /// </summary>
    public List<TaxonomyIdName> Children { get; set; } = new();
}