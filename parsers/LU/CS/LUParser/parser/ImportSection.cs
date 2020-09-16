using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Microsoft.Botframework.LUParser.parser
{
    public class ImportSection: Section
    {
        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Path")]
        public string Path { get; set; }

        public ImportSection(LUFileParser.ImportSectionContext parseTree)
        {
            Errors = new List<Error>();
            SectionType = SectionType.ImportSection;
            var result = ExtractDescriptionAndPath(parseTree);
            Description = result.description;
            Path = result.path;
            Id = $"{SectionType}_{Name}";
            var startPosition = new Position { Line = parseTree.Start.Line, Character = parseTree.Start.Column };
            var stopPosition = new Position { Line = parseTree.Stop.Line, Character = parseTree.Stop.Column + parseTree.Stop.Text.Length };
            Range = new Range { Start = startPosition, End = stopPosition };
        }

        public (string description, string path) ExtractDescriptionAndPath(LUFileParser.ImportSectionContext parseTree)
        {
            var importStr = parseTree.importDefinition().IMPORT().GetText();

            string description = null;
            string path = null;

            // TODO: check this regex correct logic
            var groups = Regex.Matches(importStr, @"\[([^\]]*)\]\(([^\)]*)\)");

            if (groups.Count == 3)
            {
                description = groups[1].ToString().Trim();
                path = groups[2].ToString().Trim();

                if (String.IsNullOrEmpty(path))
                {
                    var errorMsg = $"LU file reference path is empty: \"{ parseTree.GetText() }\"";
                    var error = Diagnostic.BuildDiagnostic(message: errorMsg, context: parseTree);

                    Errors.Add(error);
                }
            }

            return (description, path);
        }
    }
}
