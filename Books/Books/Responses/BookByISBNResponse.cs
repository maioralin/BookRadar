using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Books.Responses
{
    public class BookByISBNResponse
    {
        public ISBNn ISBN { get; set; }

        public class Identifiers
        {
            [JsonProperty("goodreads")]
            public List<string> GoodReads { get; set; }
        }

        public class Cover
        {
            [JsonProperty("small")]
            public string Small { get; set; }
            [JsonProperty("large")]
            public string Large { get; set; }
            [JsonProperty("medium")]
            public string Medium { get; set; }
        }

        public class Author
        {
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
        }


        public class ISBNn
        {
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("cover")]
            public Cover Cover { get; set; }
            [JsonProperty("publish_date")]
            public string PublishDate { get; set; }
            [JsonProperty("key")]
            public string Key { get; set; }
            [JsonProperty("authors")]
            public IList<Author> Authors { get; set; }
            [JsonProperty("identifiers")]
            public Identifiers Identifiers { get; set; }
        }
    }
}
