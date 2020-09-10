using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.LUParser.parser
{
    class SimpleIntentSection: Section
    {
        SimpleIntentSection(LUFileParser.SimpleIntentSectionContext parseTree, string content)
        {
            this.SectionType = SectionType.SimpleIntentSection;
            this.UtteranceAndEntitiesMap = new List<UtteranceAndEntitiesMap>();
            this.Entities = new List<Entity>();
            this.Errors = new List<Error>();
            this.Body = String.Empty;

            if (parseTree != null)
            {
                this.Name;
            }
        }

        public string ExtractName(LUFileParser.SimpleIntentSectionContext parseTree)
        {
            return parseTree.intentDefinition().intentNameLine().intentName().GetText().Trim();
        }

        public string ExtractIntentNameLine(LUFileParser.SimpleIntentSectionContext parseTree)
        {
            return parseTree.intentDefinition().intentNameLine().GetText().Trim();
        }

        public List<UtteranceAndEntitiesMap> ExtractUtterancesAndEntitiesMap(LUFileParser.SimpleIntentSectionContext parseTree)
        {
            var utterancesAndEntitiesMap = new List<UtteranceAndEntitiesMap>();
            var errors = new List<Error>();

            if (parseTree.intentDefinition().intentBody() != null && parseTree.intentDefinition().intentBody().normalIntentBody() != null)
            {
                foreach (var errorIntentStr in parseTree.intentDefinition().intentBody().normalIntentBody().errorString())
                {
                    if (!String.IsNullOrEmpty(errorIntentStr.GetText().Trim()))
                    {
                        errors.Add(
                            Diagnostic.BuildDiagnostic(
                                message: "Invalid intent body line, did you miss '-' at line begin?",
                                context: errorIntentStr)
                        );
                    }
                }
            }

            foreach (var normalIntentStr in parseTree.intentDefinition().intentBody().normalIntentBody().normalIntentString())
            {
                UtteranceAndEntitiesMap utteranceAndEntities = null;

                try
                {
                    utteranceAndEntities = 
                }
            }
        }
    }
}
