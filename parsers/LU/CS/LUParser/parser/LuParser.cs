using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Antlr4.Runtime;

namespace Microsoft.Botframework.LUParser.parser
{
    class LuParser
    {
        static Object ParseWithRef(string text, LuResource luResource)
        {
            if (String.IsNullOrEmpty(text))
            {
                return new LuResource(new Section[] { }, String.Empty, new Error[] { });
            }

            // TODO: bool? sectionEnabled = luResource != null ? IsSectionEnabled(luResource.Sections) : null;

            return null;
        }

        static Object ExtractFileContent(Object fileContent, string content, Error[] errors, bool? sectionEnabled)
        {
            var sections = new List<ModelInfoSection>();

            try
            {
                //var modelInfoSections = ExtractModelInfoSections(fileContent);
            } catch
            {

            }

            try
            {
                var isSectionEnabled = sectionEnabled == null ? IsSectionEnabled(sections) : sectionEnabled;

                var nestedIntentSections = ExtractFileContent
            } catch
            {

            }

            return null;
        }

        static List<ModelInfoSection> ExtractModelInfoSections(LUFileParser.FileContext fileContext)
        {
            if (fileContext == null)
            {
                return new List<ModelInfoSection>();
            }

            var modelInfoSections = fileContext.paragraph().Select(x => x.modelInfoSection()).Where(x => x != null);

            var modelInfoSectionList = modelInfoSections.Select(x => new ModelInfoSection(x));

            return null;
        }

        static List<ModelInfoSection> ExtractNestedIntentSections(LUFileParser.FileContext fileContext, string content)
        {
            if (fileContext == null)
            {
                return new List<ModelInfoSection>();
            }

            var modelInfoSections = fileContext.paragraph().Select(x => x.modelInfoSection()).Where(x => x != null);

            var modelInfoSectionList = modelInfoSections.Select(x => new ModelInfoSection(x));

            return null;
        }


        static Object GetFileContent(string text)
        {
            var chars = new AntlrInputStream(text);
            var lexer = new LUFileLexer(chars);
            var tokens = new CommonTokenStream(lexer);
            var parser = new LUFileParser(tokens);

            var fileContent = parser.file();

            var modelInfoSectionList = fileContent.paragraph().Select(x => x.modelInfoSection());

            return null;
        }

        static bool IsSectionEnabled(List<ModelInfoSection> sections)
        {
            var modelInfoSections = sections.Where(s => s.SectionType == SectionType.ModelInfoSection);
            bool enableSections = false;

            if (modelInfoSections.Any())
            {
                foreach (ModelInfoSection modelInfo in modelInfoSections)
                {
                    var line = modelInfo.ModelInfo;
                    var kvPair = Regex.Split(line, @"@\b(enableSections).(.*)=").Select(item => item.Trim()).ToArray();
                    if (kvPair.Length == 4)
                    {
                        if (String.Equals(kvPair[1], "enableSections", StringComparison.InvariantCultureIgnoreCase) && String.Equals(kvPair[3], "true", StringComparison.InvariantCultureIgnoreCase))
                        {
                            enableSections = true;
                            break;
                        }
                    }
                }
            }

            // TODO: this is a mock behavior
            return enableSections;
        }
    }
}
