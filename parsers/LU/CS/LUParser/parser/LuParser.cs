using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        static Object ExtractFileContent(Object fileContent, string content, Error[] errors, bool sectionEnabled)
        {
            var sections = new List<Section>();

            try
            {
                //var modelInfoSections = ExtractModelInfoSections(fileContent);
            } catch
            {

            }

            return null;
        }

        static List<Object> ExtractModelInfoSections(LUFileParser.FileContext fileContext)
        {
            if (fileContext == null)
            {
                return new List<object>();
            }

            var modelInfoSections = fileContext.paragraph().Select(x => x.modelInfoSection()).Where(x => x != null);

            // var modelInfoSectionList
            return null;
        }

        static Object GetFileContent(string text)
        {
            var chars = new AntlrInputStream(text);
            var lexer = new LUFileLexer(chars);
            var tokens = new CommonTokenStream(lexer);
            var parser = new LUFileParser(tokens);

            var fileContent = parser.file();

            var hey = fileContent.paragraph().Select(x => x.modelInfoSection());

            return null;
        }

        static bool IsSectionEnabled(Section[] sections)
        {
            var modelInfoSections = sections.Where(s => s.SectionType == SectionType.ModelInfoSection);
            bool enableSections = false;

            if (modelInfoSections.Any())
            {
                foreach (Section modelInfo in modelInfoSections)
                {
                    // TODO: this logic is not clear due to typing
                }
            }

            // TODO: this is a mock behavior
            return false;
        }
    }
}
