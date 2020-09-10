using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Tree;

namespace Microsoft.Botframework.LUParser.parser
{
    static class Visitor
    {
        static Object VisitNormalIntentStringContext(LUFileParser.NormalIntentStringContext context)
        {
            var utterance = String.Empty;
            var entities = new List<Entity>();
            var errorMessages = new List<string>();

            // TODO: Check that this interface is actually implemented in the iterable
            foreach (ITerminalNode innerNode in context.children)
            {
                switch (innerNode.Symbol.Type)
                {
                    case LUFileParser.DASH:
                        break;
                    case LUFileParser.EXPRESSION:
                        var utteranceToken;
                        break;
                }
            }
        }

        static List<string> TokenizeUtterance(string utterance)
        {
            var splitString = new List<string>();
            var curatedList = splitString;
            var curatedEntity = null;

        }
    }
}
