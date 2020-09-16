using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.LUParser.parser
{
    public class NewEntitySection: Entity
    {
        // TODO: pass this constant to a helper class.
        private char[] invalidCharsInIntentOrEntityName = { '<', '>', '*', '%', '&', ':', '\\', '$' };
        public NewEntitySection(LUFileParser.NewEntitySectionContext parseTree)
        {
            SectionType = SectionType.NewEntitySection;
            Errors = new List<Error>();
            Name = ExtractName(parseTree);
            Type = ExtractType(parseTree);
            Roles = ExtractRoles(parseTree);
            Features = ExtractFeatures(parseTree);
            CompositeDefinition = ExtractCompositeDefinition(parseTree);
            RegexDefinition = ExtractRegexDefinition(parseTree);
            ListBody = ExtractSynonymsOrPhraseList(parseTree);
            Id = $"{SectionType}_{Name}";
            var startPosition = new Position { Line = parseTree.Start.Line, Character = parseTree.Start.Column };
            var stopPosition = new Position { Line = parseTree.Stop.Line, Character = parseTree.Stop.Column + parseTree.Stop.Text.Length };
            Range = new Range { Start = startPosition, End = stopPosition };
        }

        public string ExtractName(LUFileParser.NewEntitySectionContext parseTree)
        {
            var entityName = String.Empty;
            if (parseTree.newEntityDefinition().newEntityLine().newEntityName() != null)
            {
                entityName = parseTree.newEntityDefinition().newEntityLine().newEntityName().GetText().Trim();
            } else if (parseTree.newEntityDefinition().newEntityLine().newEntityNameWithWS() != null) 
            {
                entityName = parseTree.newEntityDefinition().newEntityLine().newEntityNameWithWS().GetText().Trim();
            }
            else
            {
                Errors.Add(
                    Diagnostic.BuildDiagnostic(
                        message: "Invalid entity line, did you miss entity name after $?",
                        context: parseTree.newEntityDefinition().newEntityLine()
                    )
                );
            }

            if (!String.IsNullOrEmpty(entityName) && entityName.IndexOfAny(invalidCharsInIntentOrEntityName) >= 0)
            {
                Errors.Add(
                    Diagnostic.BuildDiagnostic(
                        message: $"Invalid entity line, entity name {entityName} cannot contain any of the following characters: [<, >, *, %, &, :, \\, $]",
                        context: parseTree.newEntityDefinition().newEntityLine()
                    )
                );
                return null;
            }
            else
            {
                return entityName;
            }
        }

        public string ExtractType(LUFileParser.NewEntitySectionContext parseTree)
        {
            if (parseTree.newEntityDefinition().newEntityLine().newEntityType() != null)
            {
                return parseTree.newEntityDefinition().newEntityLine().newEntityType().GetText().Trim();
            }

            return null;
        }

        public string ExtractRoles(LUFileParser.NewEntitySectionContext parseTree)
        {
            if (parseTree.newEntityDefinition().newEntityLine().newEntityRoles() != null)
            {
                return parseTree.newEntityDefinition().newEntityLine().newEntityRoles().newEntityRoleOrFeatures().GetText().Trim();
            }

            return null;
        }

        public string ExtractFeatures(LUFileParser.NewEntitySectionContext parseTree)
        {
            if (parseTree.newEntityDefinition().newEntityLine().newEntityUsesFeatures() != null)
            {
                return parseTree.newEntityDefinition().newEntityLine().newEntityUsesFeatures().newEntityRoleOrFeatures().GetText().Trim();
            }

            return null;
        }

        public string ExtractCompositeDefinition(LUFileParser.NewEntitySectionContext parseTree)
        {
            if (parseTree.newEntityDefinition().newEntityLine().newCompositeDefinition() != null)
            {
                return parseTree.newEntityDefinition().newEntityLine().newCompositeDefinition().GetText().Trim();
            }

            return null;
        }

        public string ExtractRegexDefinition(LUFileParser.NewEntitySectionContext parseTree)
        {
            if (parseTree.newEntityDefinition().newEntityLine().newRegexDefinition() != null)
            {
                return parseTree.newEntityDefinition().newEntityLine().newRegexDefinition().GetText().Trim();
            }

            return null;
        }

        public List<string> ExtractSynonymsOrPhraseList(LUFileParser.NewEntitySectionContext parseTree)
        {
            var synonymsOrPhraseList = new List<string>();
            if (parseTree.newEntityDefinition().newEntityListbody() != null)
            {
                foreach (var errorItemStr in parseTree.newEntityDefinition().newEntityListbody().errorString())
                {
                    if (!String.IsNullOrEmpty(errorItemStr.GetText().Trim()))
                    {
                        Errors.Add(
                            Diagnostic.BuildDiagnostic(
                                message: "Invalid list entity line, did you miss '-' at line begin?",
                                context: errorItemStr
                            )
                        );
                    }
                }
                foreach (var normalItemStr in parseTree.newEntityDefinition().newEntityListbody().normalItemString())
                {
                    var itemStr = normalItemStr.GetText().Trim();
                    synonymsOrPhraseList.Add(itemStr.Substring(1).Trim());
                }
            }

            if (!String.IsNullOrEmpty(Type) && Type.IndexOf('=') > -1 && synonymsOrPhraseList.Count == 0)
            {
                var errorMsg = $"no synonyms list found for list entity definition: \"{ parseTree.newEntityDefinition().newEntityLine().GetText()}\"";
                var error = Diagnostic.BuildDiagnostic(
                    message: errorMsg,
                    context: parseTree.newEntityDefinition().newEntityLine(),
                    severity: DiagnosticSeverity.Warn
                );
                Errors.Add(error);
            }
            return synonymsOrPhraseList;
        }
    }
}
