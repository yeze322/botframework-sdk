using System;
using Xunit;
using Microsoft.Botframework.LUParser.parser;
using Newtonsoft.Json;
using System.IO;

namespace Microsoft.Botframework.LUParser.Tests.parser
{
    public class LuParserTests
    {
        [Fact]
        public void ParseLuContent()
        {
            // var luContent = "# Help"+ Environment.NewLine + "- help" + Environment.NewLine + "- I need help" + Environment.NewLine + "- please help";

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Fixtures", "LU_Sections.txt");
            Console.WriteLine(path);
            // var folders = Directory.GetDirectories(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            // var files = Directory.GetFiles(path);
            var luContent = File.ReadAllText(path);
            luContent = luContent.Substring(0, luContent.Length - 1);
            var result = LuParser.parse(luContent);
            LuResource expected = JsonConvert.DeserializeObject<LuResource>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Fixtures", "LU_Sections.json")));

            var serializedResult = JsonConvert.SerializeObject(result).Replace("\\r", "");
            var serializedExpected = JsonConvert.SerializeObject(expected).Replace("\\r", "");

            Console.WriteLine(serializedResult);
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine(serializedExpected);

            Assert.Equal(serializedResult, serializedExpected);
        }
    }
}
