using BotMaster.Betterplace.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Betterplace
{
    internal sealed class BetterplaceClient
    {
        public Uri BaseAddress { get; }

        private readonly string currentEventId;

        public BetterplaceClient(string eventId)
        {
            BaseAddress = new Uri("https://api.betterplace.org/de/api_v4/");
            currentEventId = eventId;
        }

        public IObservable<Page> GetOpinionsPage(TimeSpan timeSpan = default)
        {
            return Observable.Create<Page>((observer, token) => Task.Run(async () =>
            {
                using var client = new HttpClient()
                {
                    BaseAddress = new Uri("https://api.betterplace.org/de/api_v4/")
                };

                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    using var pageResponse = await client.GetAsync($"fundraising_events/{currentEventId}/opinions.json", token);

                    if (!pageResponse.IsSuccessStatusCode)
                        throw new WebException($"{(short)pageResponse.StatusCode}: {pageResponse.ReasonPhrase}");

                    var page = JsonConvert.DeserializeObject<Page>(await pageResponse.Content.ReadAsStringAsync());

                    observer.OnNext(page);

                    if (timeSpan != default)
                        await Task.Delay(timeSpan, token);
                }

            }, token));
        }
    }
}
