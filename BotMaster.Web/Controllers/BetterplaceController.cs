using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BotMaster.Runtime;
using BotMaster.Web.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace BotMaster.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BetterplaceController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider contentTypeProvider;
        private readonly Service botmasterService;
        private static bool b;

        public BetterplaceController(Service service)
        {
            contentTypeProvider = new FileExtensionContentTypeProvider();
            botmasterService = service;
        }

        [HttpGet("[action]")]
        public ActionResult<Alert> Alert()
        {
            if (b)
                botmasterService.Opinions.Enqueue(new Betterplace.Model.Opinion() { Author = new Betterplace.Model.Author() { Name = "Test" }, Donated_amount_in_cents = 100, Message = "Hallo, wie gehts euch so?" });
            b = !b;
            if (botmasterService.Opinions.TryDequeue(out var opinion))
            {
                return new Alert
                {
                    Amount = opinion.Donated_amount_in_cents,
                    Message = opinion.Message,
                    Name = opinion.Author?.Name
                };
            }
            else
            {
                return new Alert();
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            var file = new FileInfo(Path.Combine(".", "web", "alert.html"));
            contentTypeProvider.TryGetContentType(file.FullName, out var fileType);
            return File(file.OpenRead(), fileType);
        }
    }
}
