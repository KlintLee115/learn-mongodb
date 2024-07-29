using backend.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController(MongoDBContext mongoDBContext) : ControllerBase
    {
        private readonly MongoDBContext _dbContext = mongoDBContext;

        [HttpPost()]
        public async Task<IActionResult> AddQuestion([FromBody] AddQuestionRequest request)
        {

            if (string.IsNullOrEmpty(request.FormId) || !ObjectId.TryParse(request.FormId, out ObjectId _formId))
            {
                return new BadRequestObjectResult("Invalid formId format.");
            }

            var questionId = Guid.NewGuid().ToString();

            QuestionBase question = request.QChoiceType != null
        ? new QuestionChoice
        {
            ChoiceType = request.QChoiceType,
            QuestionId = questionId,
            Title = request.Title,
            Options = []
        }
        : new QuestionBase
        {
            QuestionId = questionId,
            Title = request.Title
        };

            var filter = Builders<Form>.Filter.Eq("_id", _formId);
            var update = Builders<Form>.Update.Push(f => f.Questions, question);

            var result = await _dbContext.Forms.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0 ? Ok() : NotFound();
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteQuestion([FromBody] DeleteQuestionRequest request)
        {

            if (string.IsNullOrEmpty(request.FormId) || !ObjectId.TryParse(request.FormId, out ObjectId _formId))
            {
                return new BadRequestObjectResult("Invalid formId format.");
            }

            var filter = Builders<Form>.Filter.Eq("_id", _formId);
            var update = Builders<Form>.Update.PullFilter(f => f.Questions, q => q.QuestionId == request.QuestionId);

            var result = await _dbContext.Forms.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0 ? Ok() : NotFound();
        }

        [HttpPut("title")]
        public async Task<IActionResult> EditQuestionTitle([FromBody] EditQuestionTitleRequest request)
        {

            if (string.IsNullOrEmpty(request.FormId) || !ObjectId.TryParse(request.FormId, out ObjectId _formId))
            {
                return new BadRequestObjectResult("Invalid formId format.");
            }

            var filter = Builders<Form>.Filter.Eq(f => f.Id, _formId);
            var form = await _dbContext.Forms.Find(filter).FirstOrDefaultAsync();

            if (form == null) return NotFound();

            var question = form.Questions.FirstOrDefault(q => q.QuestionId == request.QuestionId);
            if (question != null && request.Title != null)
            {
                question.Title = request.Title;
                var updateResult = await _dbContext.Forms.ReplaceOneAsync(filter, form);

                return Ok();

            }

            return new BadRequestObjectResult("Dont know whats desired");
        }

        [HttpPut("type")]
        public async Task<IActionResult> EditQuestionType([FromBody] ChangeQuestionTypeRequest request)
        {
            if (string.IsNullOrEmpty(request.FormId) || !ObjectId.TryParse(request.FormId, out ObjectId _formId))
            {
                return new BadRequestObjectResult("Invalid formId format.");
            }

            if (request.NewType != "single" && request.NewType != "multi") return new BadRequestObjectResult("New type must be single or multi.");

            var filter = Builders<Form>.Filter.Eq(f => f.Id, _formId);
            var form = await _dbContext.Forms.Find(filter).FirstOrDefaultAsync();

            if (form == null) return NotFound();

            var question = form.Questions.FirstOrDefault(q => q.QuestionId == request.QuestionId);

            if (question == null) return NotFound();

            if (question is QuestionChoice qc)
            {
                qc.ChoiceType = request.NewType;

                var update = Builders<Form>.Update.Set(f => f.Questions, form.Questions);
                var updateResult = await _dbContext.Forms.UpdateOneAsync(filter, update);

                return updateResult.ModifiedCount > 0 ? Ok() : new BadRequestObjectResult("Update failed.");
            }

            return new BadRequestObjectResult("Dont know whats desired");
        }

        [HttpPost("option")]
        public async Task<IActionResult> AddOption([FromBody] AlterOptionRequest request)
        {

            if (string.IsNullOrEmpty(request.FormId) || !ObjectId.TryParse(request.FormId, out ObjectId _formId))
            {
                return new BadRequestObjectResult("Invalid formId format.");
            }

            var filter = Builders<Form>.Filter.Eq(f => f.Id, _formId);
            var form = await _dbContext.Forms.Find(filter).FirstOrDefaultAsync();

            if (form == null) return NotFound();

            var question = form.Questions.FirstOrDefault(q => q.QuestionId == request.QuestionId);
            if (question == null) return NotFound();

            if (request.Option == null) return new BadRequestObjectResult("Option must be defined");

            QuestionChoice qc = (QuestionChoice)question;

            qc.Options.Add(request.Option);

            var updateResult = await _dbContext.Forms.ReplaceOneAsync(filter, form);

            return updateResult.ModifiedCount > 0 ? Ok() : new BadRequestObjectResult("Update failed.");
        }

        [HttpDelete("option")]
        public async Task<IActionResult> RemoveOption([FromBody] AlterOptionRequest request)
        {

            if (string.IsNullOrEmpty(request.FormId) || !ObjectId.TryParse(request.FormId, out ObjectId _formId))
            {
                return new BadRequestObjectResult("Invalid formId format.");
            }

            var filter = Builders<Form>.Filter.Eq(f => f.Id, _formId);
            var form = await _dbContext.Forms.Find(filter).FirstOrDefaultAsync();

            if (form == null) return NotFound();

            var question = form.Questions.FirstOrDefault(q => q.QuestionId == request.QuestionId);
            if (question == null) return NotFound();

            if (request.Option == null) return new BadRequestObjectResult("Option must be defined");

            QuestionChoice qc = (QuestionChoice)question;

            if (qc.Options.Contains(request.Option))
            {
                qc.Options.Remove(request.Option);

                var updateResult = await _dbContext.Forms.ReplaceOneAsync(filter, form);

                return updateResult.ModifiedCount > 0 ? Ok() : new BadRequestObjectResult("Update failed.");
            }

            return new OkObjectResult("Nothing changed");
        }
    }

    public class AddQuestionRequest
    {
        public required string FormId { get; set; }
        public required string Title { get; set; }
        public string? QChoiceType { get; set; }
    }

    public class DeleteQuestionRequest
    {
        public required string FormId { get; set; }
        public required string QuestionId { get; set; }
    }

    public class EditQuestionRequest
    {
        public required string FormId { get; set; }
        public required string QuestionId { get; set; }
    }

    public class ChangeQuestionTypeRequest : EditQuestionRequest
    {
        public required string NewType { get; set; }
    }

    public class EditQuestionTitleRequest : EditQuestionRequest
    {
        public required string Title { get; set; }
    }

    public class AlterOptionRequest : EditQuestionRequest
    {
        public required string Option { get; set; }
    }

}