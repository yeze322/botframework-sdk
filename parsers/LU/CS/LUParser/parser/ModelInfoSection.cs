using System.Collections.Generic;
using static LUFileParser;

namespace Microsoft.Botframework.LUParser.parser
{
    public class ModelInfoSection
    {
        public SectionType SectionType { get; set; }

        public string ModelInfo { get; set; }

        public List<Error> Errors { get; set; }

        public string Id { get; set; }

        public Position startPosition = new Position();

        public Position stopPosition = new Position();

        public Range Range;

        public ModelInfoSection(ModelInfoSectionContext parseTree)
        {
            this.SectionType = SectionType.ModelInfoSection;
            this.ModelInfo = parseTree.modelInfoDefinition().ToString();
            this.Errors = new List<Error>();
            this.Id = this.SectionType.ToString() + this.ModelInfo;
            this.startPosition.Line = parseTree.Start.Line;
            this.startPosition.Character = parseTree.Start.Column;
            this.stopPosition.Line = parseTree.Stop.Line;
            this.stopPosition.Character = parseTree.Stop.Column;
            this.Range = new Range();
            this.Range.Start = this.startPosition;
            this.Range.End = this.stopPosition;
        }
    }
}
