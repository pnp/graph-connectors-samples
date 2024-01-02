// using System;
// using System.Collections.Generic;
// using System.Text.Json.Serialization;


// public class Root
// {
//     public List<object> modules { get; set; }
//     //public List<Unit> units { get; set; }
//     public List<object> learningPaths { get; set; }
//     public List<object> appliedSkills { get; set; }
//     public List<object> certifications { get; set; }
//     public List<object> mergedCertifications { get; set; }
//     public List<object> exams { get; set; }
//     public List<object> courses { get; set; }
//     public List<object> levels { get; set; }
//     public List<object> products { get; set; }
//     public List<object> roles { get; set; }
//     public List<object> subjects { get; set; }
// }
// public class ModuleResponse
// {
//     public List<Module> modules { get; set; }
// }

// public class Module
// {
//     [JsonPropertyName("summary")]
//     public string Summary { get; set; }

//     [JsonPropertyName("levels")]
//     public List<string> Levels { get; set; }

//     [JsonPropertyName("roles")]
//     public List<string> Roles { get; set; }

//     [JsonPropertyName("products")]
//     public List<string> Products { get; set; }

//     [JsonPropertyName("subjects")]
//     public List<string> Subjects { get; set; }

//     [JsonPropertyName("uid")]
//     public string Uid { get; set; }

//     [JsonPropertyName("type")]
//     public string Type { get; set; }

//     [JsonPropertyName("title")]
//     public string Title { get; set; }

//     [JsonPropertyName("duration_in_minutes")]
//     public int DurationInMinutes { get; set; }

//     [JsonPropertyName("rating")]
//     public Rating Rating { get; set; }

//     [JsonPropertyName("popularity")]
//     public double Popularity { get; set; }

//     [JsonPropertyName("icon_url")]
//     public string IconUrl { get; set; }

//     [JsonPropertyName("social_image_url")]
//     public string SocialImageUrl { get; set; }

//     [JsonPropertyName("locale")]
//     public string Locale { get; set; }

//     [JsonPropertyName("last_modified")]
//     public DateTime LastModified { get; set; }

//     [JsonPropertyName("url")]
//     public string Url { get; set; }

//     [JsonPropertyName("firstUnitUrl")]
//     public string FirstUnitUrl { get; set; }

//     [JsonPropertyName("units")]
//     public List<string> Units { get; set; }

//     [JsonPropertyName("number_of_children")]
//     public int NumberOfChildren { get; set; }
// }

// // public class Rating
// // {
// //     public int count { get; set; }
// //     public double average { get; set; }
// // }





// // public class Unit
// // {
// //     public string uid { get; set; }
// //     public string type { get; set; }
// //     public string title { get; set; }
// //     public int duration_in_minutes { get; set; }
// //     public string locale { get; set; }
// //     public DateTime last_modified { get; set; }
// // }