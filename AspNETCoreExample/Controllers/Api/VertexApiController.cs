using System.Text;
using Microsoft.AspNetCore.Mvc;
using TechStrat.GPT.UI.Models;
using TechStrat.RGPT.UI.Models;
using TechStrat.RGPT.UI.Services;

namespace TechStrat.GPT.UI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/vertex")]
    [ApiController]
    public class VertexApiController : ControllerBase
    {                
       private readonly IFreeformService _freeformService;       
        private string _userInput;
        public VertexApiController(ILogger<VertexApiController> logger, IFreeformService freeformService)
        {                                  
            _freeformService = freeformService;            
        }

        [HttpPost]
        public async Task<IActionResult> Post(RequestModel request)
        {
            var groudingModel = new List<KeyPairModel>();
            _userInput = request.Input;           
            AddResponseHeaders();
            
            var response = _freeformService.StreamGenerateContent(_userInput);

            var stream = response.GetResponseStream();
            await HttpContext.Response.Body.FlushAsync();

            while (await stream.MoveNextAsync())
            {
                var responseItem = stream.Current;

                if (responseItem.Candidates == null || responseItem.Candidates[0].Content == null || responseItem.Candidates[0].Content.Parts == null)
                {
                    continue;
                }

                var contentText = responseItem.Candidates[0].Content.Parts[0].Text;                
                var gcpGrounding = responseItem.Candidates[0].GroundingMetadata;

                if (gcpGrounding != null)
                {
                    foreach (var gs in gcpGrounding.GroundingChunks)
                    {
                        var retriveCtx = gs.RetrievedContext;
                        groudingModel.Add(new KeyPairModel
                        {
                            Key = retriveCtx.Title,
                            Value = retriveCtx.Uri
                        });
                    }
                }

                var chunkBytes = Encoding.UTF8.GetBytes(contentText);
                await HttpContext.Response.WriteAsync($"{chunkBytes.Length:X}\r\n");
                await HttpContext.Response.Body.WriteAsync(chunkBytes, 0, chunkBytes.Length);
                await HttpContext.Response.WriteAsync("\r\n");
                await HttpContext.Response.Body.FlushAsync();
            }

            if (groudingModel.Count > 0)
            {

                var htmlBytes = Encoding.UTF8.GetBytes("<h5>Grounding Sources</h5>");
                await HttpContext.Response.WriteAsync($"{htmlBytes.Length:X}\r\n");
                await HttpContext.Response.Body.WriteAsync(htmlBytes, 0, htmlBytes.Length);
                await HttpContext.Response.WriteAsync("\r\n");
                await HttpContext.Response.Body.FlushAsync();

                foreach (var grd in groudingModel)
                {
                    var groundingBytes = Encoding.UTF8.GetBytes($"<p><a href='{grd.Value}'>{grd.Key}</a></p>");
                    await HttpContext.Response.WriteAsync($"{groundingBytes.Length:X}\r\n");

                    await HttpContext.Response.Body.WriteAsync(groundingBytes, 0, groundingBytes.Length);
                    await HttpContext.Response.WriteAsync("\r\n");
                    await HttpContext.Response.Body.FlushAsync();
                }
            }
            await HttpContext.Response.WriteAsync("0\r\n\r\n");
            await HttpContext.Response.Body.FlushAsync();
            return new EmptyResult();
        }
        
        private void AddResponseHeaders()
        {
            HttpContext.Response.Headers.Append("Transfer-Encoding", "chunked");
            HttpContext.Response.Headers.Append("Content-Type", "text/plain; charset=utf-8");
            HttpContext.Response.Headers.Append("Connection", "keep-alive");
            HttpContext.Response.Headers.Append("Cache-Control", "no-cache");
        }       
    }
}
