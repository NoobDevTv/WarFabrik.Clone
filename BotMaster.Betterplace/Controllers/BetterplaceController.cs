using BotMaster.Betterplace.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive;

namespace BotMaster.Betterplace.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BetterplaceController : ControllerBase
    {
        public OpinionsCache OpinionsCache { get; }

        private readonly FileExtensionContentTypeProvider contentTypeProvider;

        public BetterplaceController(OpinionsCache opinionsCache)
        {
            contentTypeProvider = new FileExtensionContentTypeProvider();
            OpinionsCache = opinionsCache;
        }


        [HttpGet("[action]")]
        public IEnumerable<Alert> Alert()
        {
            return OpinionsCache.Alerts.Reverse().ToList();
        }

        [HttpGet]
        public IActionResult Get()
        {
            var file = new FileInfo(Path.Combine(".", "additionalfiles", "web", "betterplace", "alert.html"));
            contentTypeProvider.TryGetContentType(file.FullName, out var fileType);
            return File(file.OpenRead(), fileType);
        }
    }
}
