using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenTracing;

namespace jaeger_csharp_frontend_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ITracer tracer;
        private readonly HttpClient httpClient;

        public ValuesController(ITracer tracer, HttpClient httpClient)
        {
            this.tracer = tracer;
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("http://localhost:5000/api/");
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            using (IScope scope = tracer.BuildSpan("waitingForValues").StartActive(finishSpanOnDispose: true))
            {
                var response = await this.httpClient.GetStringAsync("values");
                return JsonConvert.DeserializeObject<List<string>>(response);
            }
        }
    }
}
