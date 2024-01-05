using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using O365C.GraphConnector.MicrosoftLearn.Models;

namespace O365C.GraphConnector.MicrosoftLearn.Services
{
    public interface ICatalogApiService
    {

        Task<LearnCatalog> GetCatalogAsync(string? locale = null, CatalogFilter? filters = null);
        Task<List<Module>> GetModulesAsync(string? locale = null, CatalogFilter? filter = null);
        // Task<List<Module>> GetModulesWithPopularityScoreAbove(double score);
        // Task<List<Module>> GetAllModules();
    }
}
