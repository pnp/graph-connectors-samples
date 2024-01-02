using System;
using System.Collections.Generic;
using System.Linq;

namespace O365C.GraphConnector.MicrosoftLearn.Models
{

    /// <summary>
    /// This is the root object returned by the Catalog API
    /// </summary>
    public class LearnCatalog
    {
        /// <summary>
        /// List of published Microsoft Learn training modules.
        /// </summary>
        public List<Module> Modules { get; set; } = new();

        /// <summary>
        /// List of published Microsoft Learn units in modules
        /// </summary>
        public List<ModuleUnit> Units { get; set; } = new();

        /// <summary>
        /// List of published Microsoft Learn learning paths.
        /// </summary>
        public List<LearningPath> LearningPaths { get; set; } = new();

        /// <summary>
        /// List of published Microsoft Learn certifications.
        /// </summary>
        public List<Certification> Certifications { get; set; } = new();

        /// <summary>
        /// List of published Microsoft Learn exams.
        /// </summary>
        public List<Exam> Exams { get; set; } = new();

        /// <summary>
        /// List of published Microsoft Learn instructor-led courses.
        /// </summary>
        public List<InstructorLedCourse> Courses { get; set; } = new();

        /// <summary>
        /// Levels used in modules/paths.
        /// </summary>
        public List<Level> Levels { get; set; } = new();

        /// <summary>
        /// Roles used in modules/paths.
        /// </summary>
        public List<Role> Roles { get; set; } = new();

        /// <summary>
        /// Products used in modules/paths.
        /// </summary>
        public List<Product> Products { get; set; } = new();

        /// <summary>
        /// Subjects used in modules/paths.
        /// </summary>
        public List<Subject> Subjects { get; set; } = new();

        /// <summary>
        /// Returns the modules tied to a specific Learning path.
        /// </summary>
        /// <param name="path">Path to get modules for</param>
        /// <returns>Enumerable list of modules</returns>
        public IEnumerable<Module> ModulesForPath(LearningPath path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return path.Modules.Select(uid => Modules.SingleOrDefault(m2 => m2.Uid == uid))
                .Where(module => module != null)
                .Cast<Module>();
        }
    }
}