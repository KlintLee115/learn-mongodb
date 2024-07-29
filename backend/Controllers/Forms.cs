using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.Json;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class FormsController(MongoDBContext dbContext) : ControllerBase
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true
        };
        private readonly MongoDBContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        [HttpGet()]
        public async Task<IActionResult> GetForms([FromQuery] string? ownerEmail, [FromQuery] string? formId)
        {

            try
            {
                FilterDefinition<Form> filter = Builders<Form>.Filter.Empty;

                if (!string.IsNullOrEmpty(ownerEmail))
                {
                    filter = Builders<Form>.Filter.Eq("ownerEmail", ownerEmail);

                    var forms = await _dbContext.Forms.Find(filter).ToListAsync();

                    var serializedForms = forms.Select(form => new
                    {
                        Id = form.Id.ToString(),
                        form.Title,
                        form.Description,
                        form.Questions,
                        form.OwnerEmail
                    });

                    return new OkObjectResult(serializedForms);
                }
                else if (!string.IsNullOrEmpty(formId) && ObjectId.TryParse(formId, out ObjectId objectId))
                {
                    filter = Builders<Form>.Filter.Eq("_id", objectId);
                    var form = await _dbContext.Forms.Find(filter).FirstAsync();

                    var serializedForms = new
                    {
                        id = form.Id.ToString(),
                        form.Title,
                        form.Description,
                        Questions = form.Questions.ToDictionary(q =>
                        q.QuestionId,
                        q =>
                        {
                            if (q is QuestionChoice qc)
                            {
                                return new
                                {
                                    qc.ChoiceType,
                                    qc.Title,
                                    qc.Options
                                } as object;
                            }
                            else
                            {
                                return new
                                {
                                    q.Title
                                };
                            }

                        }),
                        form.OwnerEmail
                    };

                    return Ok(serializedForms);
                }
                else
                {
                    return BadRequest("Invalid formId format.");
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> CreateForm([FromBody] CreateFormDTO reqBody)
        {

            try
            {
                var formsCollection = _dbContext.Forms;

                Form newForm = new()
                {
                    Title = string.Empty,
                    Description = string.Empty,
                    Questions = [],
                    OwnerEmail = reqBody.OwnerEmail
                };

                await formsCollection.InsertOneAsync(newForm);

                var result = new OkObjectResult(new { formId = newForm.Id.ToString() });

                return result;
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateFormField([FromBody] PutFormDTO req)
        {
            try
            {
                var filter = Builders<Form>.Filter.Eq("_id", new ObjectId(req.FormId));
                var update = Builders<Form>.Update.Set(req.Field, req.Value);

                var updateResult = await _dbContext.Forms.UpdateOneAsync(filter, update);

                return updateResult.MatchedCount > 0
                    ? new OkObjectResult("Form updated successfully.")
                    : new NotFoundObjectResult("Form not found.");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }

    public class CreateFormDTO
    {
        public required string OwnerEmail { get; set; }
    }

    public class PutFormDTO
    {
        public required string FormId { get; set; }
        public required string Field { get; set; }
        public required string Value { get; set; }

    }
}