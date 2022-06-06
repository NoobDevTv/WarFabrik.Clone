using BotMaster.Betterplace.Model;
using Newtonsoft.Json;
using System.Reactive.Linq;

namespace BotMaster.Betterplace
{
    internal static class BetterplaceClient
    {
        public static IObservable<Page> GetOpinionsPage(string currentEventId, TimeSpan timeSpan = default) 
            => Observable
            .Using(
                CreateNewContext,
                serviceContext =>
                {
                    var interval
                    = Observable
                        .Concat(Observable.Return(0L),
                            Observable.Interval(timeSpan));

                    return interval
                        .Select(_ => Observable.FromAsync(token => serviceContext.HttpClient.GetAsync($"fundraising_events/{currentEventId}/opinions.json", token)))
                        .Concat()
                        .Where(response => response.IsSuccessStatusCode)
                        .Select(response => Observable.FromAsync(() => response.Content.ReadAsStringAsync()))
                        .Concat()
                        .Retry()
                        .Select(pageContent => JsonConvert.DeserializeObject<Page>(pageContent));
                }
            );

        private static BetterplaceContext CreateNewContext()
        {
            var client
                = new HttpClient()
                {
                    BaseAddress = new Uri("https://api.betterplace.org/de/api_v4/")
                };

            return new(client);
        }

        private record BetterplaceContext(HttpClient HttpClient) : IDisposable
        {
            public void Dispose()
            {
                HttpClient.Dispose();
            }
        }
    }
}
