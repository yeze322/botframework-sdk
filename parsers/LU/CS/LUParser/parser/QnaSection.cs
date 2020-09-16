using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Microsoft.Botframework.LUParser.parser
{
    public class QnaSection : Section
    {
        // TODO: not sure if serialization is needed for pairs
        public class QnaTuple
        {
            [JsonProperty("key")]
            public string key { get; set; }

            [JsonProperty("value")]
            public string value { get; set; }
        }

        [JsonProperty("Questions")]
        public List<string> Questions { get; set; }

        [JsonProperty("FilterPairs")]
        public List<QnaTuple> FilterPairs { get; set; }

        public QnaSection(LUFileParser.QnaSectionContext parseTree)
        {
            SectionType = SectionType.QnaSection;
            Questions = new List<string>() { ExtractQuestion(parseTree) };
            var result = ExtractMoreQuestions(parseTree);
            Questions.AddRange(result.questions);
            Errors = result.errors;
            var result2 = ExtractFilterPairs(parseTree);
            FilterPairs = result2.filterPairs;
            Errors.AddRange(result2.errors);
        }

        public string ExtractQuestion(LUFileParser.QnaSectionContext parseTree)
        {
            return parseTree.qnaDefinition().qnaQuestion().questionText().GetText().Trim();
        }

        public (List<string> questions, List<Error> errors) ExtractMoreQuestions(LUFileParser.QnaSectionContext parseTree)
        {
            var questions = new List<string>();
            var errors = new List<Error>();
            var questionsBody = parseTree.qnaDefinition().moreQuestionsBody();
            foreach (var errorQuestionStr in questionsBody.errorQuestionString())
            {
                if (!String.IsNullOrEmpty(errorQuestionStr.GetText().Trim()))
                {
                    errors.Add(
                        Diagnostic.BuildDiagnostic(
                            message: $"Invalid QnA question line, did you miss '-' at line begin",
                            context: errorQuestionStr
                        )
                    );
                }
            }

            foreach (var question in questionsBody.moreQuestion())
            {
                var questionText = question.GetText().Trim();
                questions.Add(questionText.Substring(1).Trim());
            }

            return (questions, errors);
        }

        public (List<QnaTuple> filterPairs, List<Error> errors) ExtractFilterPairs(LUFileParser.QnaSectionContext parseTree)
        {
            var filterPairs = new List<QnaTuple>();
            var errors = new List<Error>();
            var filterSection = parseTree.qnaDefinition().qnaAnswerBody().filterSection();
            if (filterSection != null)
            {
                if (filterSection.errorFilterLine() != null)
                {
                    foreach (var errorFilterLineStr in filterSection.errorFilterLine())
                    {
                        if (!String.IsNullOrEmpty(errorFilterLineStr.GetText().Trim()))
                        {
                            errors.Add(
                                Diagnostic.BuildDiagnostic(
                                    message: $"Invalid QnA filter line, did you miss '-' at line begin",
                                    context: errorFilterLineStr
                                )
                            );
                        }
                    }
                }

                foreach (var filterLine in filterSection.filterLine())
                {
                    var filterLineText = filterLine.GetText().Trim();
                    filterLineText = filterLineText.Substring(1).Trim();
                    var filterPair = filterLineText.Split('=');
                    var key = filterPair[0].Trim();
                    var value = filterPair[1].Trim();
                    filterPairs.Add(new QnaTuple{ key = key, value = value});
                }
            }

            return (filterPairs, errors);
        }

        public string ExtractAnswer(LUFileParser.QnaSectionContext parseTree)
        {
            var multiLineAnswer = parseTree.qnaDefinition().qnaAnswerBody().multiLineAnswer().GetText().Trim();
            // trim first and last line
            // TODO: validate this regex
            var answerRegexp = new Regex(@"^```(markdown)?\r*\n(?<answer>(.|\n|\r\n|\t| )*)\r?\n.*?```$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var answer = answerRegexp.Matches(multiLineAnswer);

            return null;
        }
    }
}
