using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksMiddletier.Responses
{
    public class BookByQueryResponse
    {
        [JsonProperty("numFound")]
        public long NumFound { get; set; }

        [JsonProperty("start")]
        public long Start { get; set; }

        [JsonProperty("docs")]
        public Doc[] Docs { get; set; }
    }

    public partial class Doc
    {
        [JsonProperty("author_name")]
        public string[] AuthorName { get; set; }

        [JsonProperty("isbn")]
        public string[] Isbn { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }
}
