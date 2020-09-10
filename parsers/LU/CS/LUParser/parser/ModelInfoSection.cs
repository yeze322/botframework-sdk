using System.Collections.Generic;
using static LUFileParser;

namespace Microsoft.Botframework.LUParser.parser
{
    public class ModelInfoSection: Section
    {
        public string ModelInfo { get; set; }

        public ModelInfoSection(ModelInfoSectionContext parseTree)
        {
            this.SectionType = SectionType.ModelInfoSection;
            this.ModelInfo = parseTree.modelInfoDefinition().ToString();
            this.Errors = new List<Error>();
            this.Id = this.SectionType.ToString() + this.ModelInfo;
            Position startPosition = new Position { Line = parseTree.Start.Line, Character = parseTree.Start.Column };
            Position stopPosition = new Position { Line = parseTree.Stop.Line, Character = parseTree.Stop.Column };
            this.Range = new Range();
            this.Range.Start = startPosition;
            this.Range.End = stopPosition;
        }
    }
}
