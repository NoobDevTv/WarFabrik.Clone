using BotMaster.Runtime;
using BotMaster.Web.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BotMaster.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BetterplaceController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider contentTypeProvider;
        private readonly Service botmasterService;
#if DEBUG
        private static volatile bool b;
        private static volatile int demoId;
#endif

        public BetterplaceController(Service service)
        {
            contentTypeProvider = new FileExtensionContentTypeProvider();
            botmasterService = service;
        }

        [HttpGet("[action]")]
        public IEnumerable<Alert> Alert()
        {
#if DEBUG
            var r = new Random();
            var id = demoId++;
            if (b)
                botmasterService.Opinions.TryAdd(id, new Betterplace.Model.Opinion() { Id = id, Author = new Betterplace.Model.Author() { Name = "Test" }, Donated_amount_in_cents = r.Next(100,2000), Message = "Hallo, wie gehts euch so?", Created_at = DateTime.Now });
            b = !b;
#endif
            return botmasterService
                            .Opinions
                            .Values
                            .OrderBy(o => o.Id)
                            .Select(opinion => new Alert
                            {
                                Id = opinion.Id,
                                Amount = opinion.Donated_amount_in_cents,
                                Message = opinion.Message,
                                Name = opinion.Author?.Name,
                                Created = opinion.Created_at
                            })
                            .ToList();
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
