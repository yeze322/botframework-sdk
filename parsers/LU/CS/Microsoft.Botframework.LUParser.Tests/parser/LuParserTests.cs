using System;
using Xunit;
using Microsoft.Botframework.LUParser.parser;


namespace Microsoft.Botframework.LUParser.Tests.parser
{
    public class LuParserTests
    {
        [Fact]
        public void ParseLuContent()
        {
            var luContent = "# Help"+ Environment.NewLine + "- help" + Environment.NewLine + "- I need help" + Environment.NewLine + "- please help";
            var result = LuParser.parse(luContent, false);
        }
    }
}
