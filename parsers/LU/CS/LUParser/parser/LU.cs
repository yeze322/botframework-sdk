using System;
namespace Microsoft.Botframework.LUParser.parser
{
    public class LU
    {
        public string Content { get; set; }

        public LU(string Content)
        {
            this.Content = Content;
        }

        public LuResource parse()
        {
            return null;
        }
    }
}
