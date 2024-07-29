using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models
{
    public class QuestionBase
    {
        [BsonElement("questionId")]
        public required string QuestionId { get; set; }

        [BsonElement("title")]
        public required string Title { get; set; }
    }

    [BsonDiscriminator("QuestionChoice")]
    public class QuestionChoice : QuestionBase
    {
        [BsonElement("options")]
        public required List<string> Options { get; set; }

        [BsonElement("choicetype")]
        public required string ChoiceType { get; set; }
    }

    public class Form
    {
        public ObjectId Id { get; set; }

        [BsonElement("title")]
        public required string Title { get; set; }

        [BsonElement("description")]
        public required string Description { get; set; }

        [BsonElement("questions")]
        public List<QuestionBase> Questions { get; set; } = [];

        [BsonElement("ownerEmail")]
        public required string OwnerEmail { get; set; }
    }
    public class FormResponse
    {
        public ObjectId Id { get; set; }
        [BsonElement("formId")]
        public required string FormId { get; set; }

        [BsonElement("responses")]
        // <email, response>
        public required Dictionary<string, QuestionResponse> Responses { get; set; }
    }
    public class QuestionResponse
    {
        [BsonElement("answers")]
        // <questionid, answers>
        public required Dictionary<string, string[]> Answers { get; set; }
    }
}