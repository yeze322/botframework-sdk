using System;
using Xunit;
using Microsoft.Botframework.LUParser.parser;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace Microsoft.Botframework.LUParser.Tests.parser
{
    public class LuParserTests
    {
        [Fact]
        public void ParseLuContent()
        {
            // var luContent = "# Help"+ Environment.NewLine + "- help" + Environment.NewLine + "- I need help" + Environment.NewLine + "- please help";

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Fixtures/LU.txt");
            Console.WriteLine(path);
            // var folders = Directory.GetDirectories(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            // var files = Directory.GetFiles(path);
            var luContent = File.ReadAllText(path);
            // var result = LuParser.parse(luContent, false);
            LuResource expected = JsonConvert.DeserializeObject<LuResource>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Fixtures/LU_Sections.json")));

            //var obj1Str = JsonConvert.SerializeObject(result);
            var obj2Str = JsonConvert.SerializeObject(expected);

            // Assert.Equal(obj1Str, obj2Str);
        }
    }
}
