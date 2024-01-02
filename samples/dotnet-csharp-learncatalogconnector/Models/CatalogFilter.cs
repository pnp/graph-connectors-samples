using System;
using System.Collections.Generic;

namespace O365C.GraphConnector.MicrosoftLearn.Models;

/// <summary>
/// The types of objects which may be filtered from the catalog
/// </summary>
[Flags]
public enum CatalogTypes
{
    /// <summary>
    /// Modules
    /// </summary>
    Modules = 1,
    
    /// <summary>
    /// Module units
    /// </summary>
    Units = 2,

    /// <summary>
    /// Learning paths
    /// </summary>
    LearningPaths = 4,

    /// <summary>
    /// Certifications
    /// </summary>
    Certifications = 8,

    /// <summary>
    /// Exams
    /// </summary>
    Exams = 16,
    
    /// <summary>
    /// Instructor-led courses
    /// </summary>
    Courses = 32,
    
    /// <summary>
    /// Levels
    /// </summary>
    Levels = 64,
    
    /// <summary>
    /// Roles
    /// </summary>
    Roles = 128,
    
    /// <summary>
    /// Products
    /// </summary>
    Products = 256,

    /// <summary>
    /// Subjects
    /// </summary>
    Subjects = 512
}

/// <summary>
/// Filters to apply to a catalog retrieval
/// </summary>
public class CatalogFilter
{
    /// <summary>
    /// Zero or more specific content unique ids (UIDs) to retrieve.
    /// </summary>
    public List<string>? Uids { get; set; } = new();

    /// <summary>
    /// Types to retrieve - defaults to all types
    /// </summary>
    public CatalogTypes? Types { get; set; }

    /// <summary>
    /// An operator and datetime to filter by the last modified date of objects.
    /// Operator includes lt (less than), lte (less than or equal to), eq (equal to),
    /// gt (greater than), gte (greater than or equal to).
    /// When you use this parameter, the operator will default to 'gte' if not specified.
    /// 
    /// In addition, objects which do not have a last_modified field will be removed from the results.
    /// </summary>
    public string? LastModifiedExpression { get; set; }

    /// <summary>
    /// An operator and value to filter by the popularity value (in a range of 0-1) of objects.
    /// Operator includes lt (less than), lte (less than or equal to), eq (equal to),
    /// gt (greater than), gte (greater than or equal to).
    /// When you use this parameter, the operator will default to gte if not specified.
    /// 
    /// In addition, objects which do not have a popularity value field will be removed
    /// from the results - this means only Modules and Paths can be returned if you add this filter.
    /// </summary>
    public string? PopularityExpression { get; set; }

    /// <summary>
    /// Filter the results to a specific set of levels.
    ///
    /// Note: objects which do not have a level field will be removed from the results.
    /// </summary>
    public List<string>? Levels { get; set; }

    /// <summary>
    /// Filter the results to a specific set of roles.
    ///
    /// Note: objects which do not have a roles field will be removed from the results.
    /// </summary>
    public List<string>? Roles { get; set; }

    /// <summary>
    /// Filter the results to a specific set of products.
    /// 
    /// Note: objects which do not have a products field will be removed from the results.
    /// </summary>
    public List<string>? Products { get; set; }

    /// <summary>
    /// Filter the results to a specific set of subjects.
    /// 
    /// Note: objects which do not have a subjects field will be removed from the results.
    /// </summary>
    public List<string>? Subjects { get; set; }
}