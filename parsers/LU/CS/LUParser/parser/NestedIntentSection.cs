using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.LUParser.parser
{
    class NestedIntentSection: Section
    {
        List<SimpleIntentSection> SimpleIntentSections { get; set; }


        public NestedIntentSection(LUFileParser.NestedIntentSectionContext parseTree, string content)
        {
            this.SectionType = SectionType.NestedIntentSection;
            this.Name = ExtractName(parseTree);
            this.Body = String.Empty;

        }

        public string ExtractName(LUFileParser.NestedIntentSectionContext parseTree)
        {
            return parseTree.nestedIntentNameLine().nestedIntentName().GetText().Trim();
        }
    }
}
