using GraphConnectorDanToftBlog.Models;
using GraphConnectorDanToftBlog.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraphConnectorDanToftBlog {
    internal static class Bloghandler {
        public static async Task<IEnumerable<Blogpost>> GetBlogposts() {
            var blogWebRequest = await new HttpClient().GetAsync($"{Configuration.Base_Url}/index.json");
            var blogRawJson = await blogWebRequest.Content.ReadAsStringAsync();
            IEnumerable<Blogpost> posts = JsonSerializer.Deserialize<IEnumerable<Blogpost>>(blogRawJson).Where(x => x.section == "Posts" && x.content != ""); ;
            return posts;
        }
    }
}
