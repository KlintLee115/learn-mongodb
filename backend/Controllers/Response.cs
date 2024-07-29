using System.Text.Json;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ResponseController(MongoDBContext dbContext) : ControllerBase
    {
        private readonly MongoDBContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        [HttpGet()]
        public async Task<IActionResult> GetResponses([FromQuery] string formId)
        {
            try
            {
                if (string.IsNullOrEmpty(formId)) return BadRequest("Invalid formId format.");

                var filter = Builders<FormResponse>.Filter.Eq(fr => fr.FormId, formId);

                var responses = await _dbContext.Responses.Find(filter).FirstOrDefaultAsync();

                if (responses == null) return NotFound("Responses not found.");

                return new OkObjectResult(responses);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> AddResponse([FromBody] AddResponseDto req)
        {
            try
            {
                if (string.IsNullOrEmpty(req.FormId) || !ObjectId.TryParse(req.FormId, out ObjectId _formId)) return BadRequest("Invalid formId format.");

                var filter = Builders<Form>.Filter.Eq(f => f.Id, _formId);

                var form = await _dbContext.Forms.Find(filter).FirstOrDefaultAsync();

                if (form == null) return NotFound("Form not found.");

                var question = form.Questions.FirstOrDefault(q => q.QuestionId == req.QuestionId);

                if (question == null) return NotFound("Question not found.");

                if (question is QuestionChoice qc && req.Answers.Length > 1 && qc.ChoiceType != "multi") return StatusCode(500, "It' not a multichoice question");

                var formResponseFilter = Builders<FormResponse>.Filter.Eq(f => f.FormId, req.FormId);

                var formResponse = await _dbContext.Responses.Find(formResponseFilter).FirstOrDefaultAsync();

                if (formResponse == null)
                {
                    formResponse = new FormResponse
                    {
                        FormId = req.FormId,
                        Responses = []
                    };

                    var newAnswer = new QuestionResponse
                    {
                        Answers = []
                    };

                    newAnswer.Answers[req.QuestionId] = req.Answers;

                    formResponse.Responses[req.Email] = newAnswer;

                    await _dbContext.Responses.InsertOneAsync(formResponse);
                    return new OkObjectResult(formResponse);
                }

                QuestionResponse questionsResponse = formResponse.Responses.GetValueOrDefault(req.Email) ?? new QuestionResponse
                {
                    Answers = []
                };

                questionsResponse.Answers[req.QuestionId] = req.Answers;
                formResponse.Responses[req.Email] = questionsResponse;

                var replaceOptions = new ReplaceOptions { IsUpsert = true };
                var result = await _dbContext.Responses.ReplaceOneAsync(formResponseFilter, formResponse, replaceOptions);

                return new OkObjectResult(result);

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

    }

    public class AddResponseDto
    {
        public required string FormId { get; set; }
        public required string QuestionId { get; set; }
        public required string[] Answers { get; set; }
        public required string Email { get; set; }
    }
}