using BooksMiddletier.Requests;
using BooksMiddletier.Responses;
using BooksMiddletier.SqlClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BooksApi.Controllers
{
  //  [Authorize(Users = "admin@mobxsoft.com")]
    public class ValuesController : ApiController
    {
        // GET api/values
        public TestResponse Get()
        {
            DbHelper db = new DbHelper();
            var result = db.ExecuteList<Result>("spBooksTestProc");
            return new TestResponse
            {
                ErrorCode = 0,
                ErrorMessage = "Succes",
                Results = result
            };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public TestResponse Post([FromBody]TestRequest request)
        {
            List<Result> results = new List<Result>();
            results.Add(new Result
            {
                Nume = request.Name,
                Prenume = request.Name,
                Varsta = request.Age
            });
            return new TestResponse
            {
                ErrorCode = 0,
                ErrorMessage = "Success",
                Results = results
            };
        }

        // PUT api/values/5
        public TestResponse Put(string id, [FromBody]TestRequest request)
        {
            List<Result> results = new List<Result>();
            results.Add(new Result
            {
                Nume = request.Name,
                Prenume = id,
                Varsta = request.Age
            });
            return new TestResponse
            {
                ErrorCode = 0,
                ErrorMessage = "Success",
                Results = results
            };
        }

        // DELETE api/values/5
        public TestResponse Delete(int id)
        {
            List<Result> results = new List<Result>();
            results.Add(new Result
            {
                Nume = "Maior",
                Prenume = "Alin",
                Varsta = id
            });
            return new TestResponse
            {
                ErrorCode = 0,
                ErrorMessage = "Success",
                Results = results
            };
        }
    }
}
