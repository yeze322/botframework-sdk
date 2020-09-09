using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Botframework.LUParser.parser
{
    public partial class LuResource
    {
        [JsonProperty("sections")]
        public Section[] Sections { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("errors")]
        public Error[] Errors { get; set; }

        public LuResource(Section[] sections, string content, Error[] errors)
        {
            Sections = sections;
            Content = content;
            Errors = errors;
        }
    }
    public partial class Error
    {
        [JsonProperty("Message")]
        public string Message { get; set; }
        [JsonProperty("Range")]
        public Range Range { get; set; }
        [JsonProperty("Severity")]
        public string Severity { get; set; }
    }
    public partial class Range
    {
        [JsonProperty("Start")]
        public Position Start { get; set; }
        [JsonProperty("End")]
        public Position End { get; set; }
    }
    public partial class Position
    {
        [JsonProperty("Line")]
        public int Line { get; set; }
        [JsonProperty("Character")]
        public int Character { get; set; }
    }
    public partial class Section
    {
        [JsonProperty("Errors")]
        public Error[] Errors { get; set; }
        [JsonProperty("SectionType")]
        public SectionType SectionType { get; set; }
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Body")]
        public string Body { get; set; }
        [JsonProperty("UtteranceAndEntitiesMap")]
        public UtteranceAndEntitiesMap[] UtteranceAndEntitiesMap { get; set; }
        [JsonProperty("Entities")]
        public Entity[] Entities { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("IntentNameLine")]
        public string IntentNameLine { get; set; }
        [JsonProperty("Range")]
        public Range Range { get; set; }
    }
    public partial class Entity
    {
        [JsonProperty("Errors")]
        public object[] Errors { get; set; }
        [JsonProperty("SectionType")]
        public SectionType SectionType { get; set; }
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Body")]
        public string Body { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        [JsonProperty("Roles", NullValueHandling = NullValueHandling.Ignore)]
        public string Roles { get; set; }
        [JsonProperty("ListBody", NullValueHandling = NullValueHandling.Ignore)]
        public string[] ListBody { get; set; }
        [JsonProperty("Range")]
        public Range Range { get; set; }
        [JsonProperty("CompositeDefinition", NullValueHandling = NullValueHandling.Ignore)]
        public string CompositeDefinition { get; set; }
        [JsonProperty("RegexDefinition", NullValueHandling = NullValueHandling.Ignore)]
        public string RegexDefinition { get; set; }
        [JsonProperty("SynonymsOrPhraseList", NullValueHandling = NullValueHandling.Ignore)]
        public string[] SynonymsOrPhraseList { get; set; }
        [JsonProperty("Features", NullValueHandling = NullValueHandling.Ignore)]
        public string Features { get; set; }
    }
    public partial class UtteranceAndEntitiesMap
    {
        [JsonProperty("utterance")]
        public string Utterance { get; set; }
        [JsonProperty("entities")]
        public EntityElement[] Entities { get; set; }
        [JsonProperty("errorMsgs")]
        public object[] ErrorMsgs { get; set; }
        [JsonProperty("contextText")]
        public string ContextText { get; set; }
        [JsonProperty("range")]
        public Range Range { get; set; }
    }
    public partial class EntityElement
    {
        [JsonProperty("type")]
        public TypeEnum Type { get; set; }
        [JsonProperty("entity")]
        public string Entity { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("startPos", NullValueHandling = NullValueHandling.Ignore)]
        public int? StartPos { get; set; }
        [JsonProperty("endPos", NullValueHandling = NullValueHandling.Ignore)]
        public int? EndPos { get; set; }
    }
    public enum SectionType { 
        SimpleIntentSection,
        NestedIntentSection,
        EntitySection,
        NewEntitySection,
        ImportSection,
        ModelInfoSection,
        QnaSection
    };
    public enum TypeEnum { Entities, PatternAnyEntities };
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                SectionTypeConverter.Singleton,
                TypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
    internal class SectionTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SectionType) || t == typeof(SectionType?);
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "entitySection":
                    return SectionType.EntitySection;
                case "newEntitySection":
                    return SectionType.NewEntitySection;
                case "simpleIntentSection":
                    return SectionType.SimpleIntentSection;
                case "nestedIntentSection":
                    return SectionType.NestedIntentSection;
                case "importSection":
                    return SectionType.ImportSection;
                case "modelInfoSection":
                    return SectionType.ModelInfoSection;
                case "qnaSection":
                    return SectionType.QnaSection;
            }
            throw new Exception("Cannot unmarshal type SectionType");
        }
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (SectionType)untypedValue;
            switch (value)
            {
                case SectionType.EntitySection:
                    serializer.Serialize(writer, "entitySection");
                    return;
                case SectionType.NewEntitySection:
                    serializer.Serialize(writer, "newEntitySection");
                    return;
                case SectionType.SimpleIntentSection:
                    serializer.Serialize(writer, "simpleIntentSection");
                    return;
                case SectionType.NestedIntentSection:
                    serializer.Serialize(writer, "nestedIntentSection");
                    return;
                case SectionType.ImportSection:
                    serializer.Serialize(writer, "importSection");
                    return;
                case SectionType.ModelInfoSection:
                    serializer.Serialize(writer, "modelInfoSection");
                    return;
                case SectionType.QnaSection:
                    serializer.Serialize(writer, "qnaSection");
                    return;
            }
            throw new Exception("Cannot marshal type EntitySectionType");
        }
        public static readonly SectionTypeConverter Singleton = new SectionTypeConverter();
    }
    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "entities":
                    return TypeEnum.Entities;
                case "patternAnyEntities":
                    return TypeEnum.PatternAnyEntities;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }
        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            switch (value)
            {
                case TypeEnum.Entities:
                    serializer.Serialize(writer, "entities");
                    return;
                case TypeEnum.PatternAnyEntities:
                    serializer.Serialize(writer, "patternAnyEntities");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }
        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }
}
