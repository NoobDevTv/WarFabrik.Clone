// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Runtime.InteropServices;

internal class Program
{
    class Json
    {
        public Dictionary<string, string> Test { get; set; }
    }
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        //var process = new Process()
        //{
        //    StartInfo = new("cmd -e", )
        //    {
        //        UseShellExecute = true
        //    },
        //};

        var json = "{\"Test\":{\"asd\":\"abc\", \"qwe\":\"hjk\"} }";

        var test = RuntimeInformation.FrameworkDescription;
        var des =
        System.Text.Json.JsonSerializer.Deserialize<Json>(json);

        //process.Start();

        Console.ReadKey();
    }
}