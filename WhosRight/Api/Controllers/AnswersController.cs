using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelFactory;
using ModelFactory.Config;
using Models;
using Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// For handling answers to debates. Called by AJAX.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AnswersAPIController : BaseController
    {
        public AnswersAPIController(DebateDbContext context, ServerSideKeyVaultSettings settings) : base(context, settings)
        {
        }

        // GET: api/AnswersAPI/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnswer([FromRoute] int id)
        {
            // Sanity
            if (id < 1)
            {
                return NotFound();
            }

            // Load answer from DB stored-proc
            var answer = await _context.GetAnswerTreeWithScores(id);

            var debate = new DebateDTO(answer);
            return Ok(debate);
        }

        // DELETE: api/AnswersAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswer([FromRoute] int id)
        {
            // Sanity
            if (id < 1)
            {
                return NotFound();
            }

            var answer = await _context.answers.FindAsync(id);
            if (answer == null)
            {
                return NotFound();
            }
            _context.answers.Remove(answer);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // POST: api/AnswersAPI/PostAnswer
        [HttpPost("{action}")]
        public async Task<IActionResult> PostAnswer([FromBody] NewAnswerDTO newAnswerPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string postingIdentity = User.Identity.Name;

            // Form new answer
            var replyingUser = await _context.users.Where(u => u.email == postingIdentity).FirstOrDefaultAsync();
#if DEBUG
            if (replyingUser == null)
                replyingUser = await _context.users.Where(u => u.email == "sambetts@cahooners.com").FirstOrDefaultAsync();
#endif

            if (replyingUser == null)
            {
                return BadRequest("No user found with ID '" + postingIdentity + "'");
            }
            if (string.IsNullOrEmpty(newAnswerPost.Title))
            {
                return BadRequest("Answer text is null/empty");
            }

            // Create new reply
            var newAnswer = await newAnswerPost.ToAnswer(_context, replyingUser);

            // Sanity on lookups
            if (newAnswer.parent_answer == null)
            {
                // Answers always need to have a valid parent
                return BadRequest(ModelState);
            }
            if (newAnswer.reaction_to_parent == null)
            {
                // Answers always need to have a valid parent
                return BadRequest(ModelState);
            }
            // Save to DB
            _context.answers.Add(newAnswer);
            await _context.SaveChangesAsync();

            var newAnswerResult = new AnswerWithRatingsTreeNode(new AnswerDataOnlyTreeNode(newAnswer.ToModel()));


            // Email the person being replied to (if there is a root)
            // Todo: fix the bug where only replying to an answer gives a parent user
            var messageQueueManager = new MessageQueueManager(_settings);
            var parentAnswer = newAnswer.parent_answer;
            if (parentAnswer.user != null)
            {
                var modelUser = replyingUser.ToModel();
                var email = new AnswerReplyEmailAlertLogDTO(modelUser, parentAnswer.ToModel());
                email.Body = $"User '{replyingUser.email}' just replied to your answer with title {parentAnswer.title}. He says '{newAnswerPost.Title}'. I heard it's on?";
                email.Subject = "New Reply";
                await messageQueueManager.SendEmailToQueue(email);
            }

            // Heads up there's a new answer to index
            await messageQueueManager.SendNewAnswerToQueue(newAnswerResult);


            // Return DTO
            return CreatedAtAction("PostAnswer", new { id = newAnswer.id }, newAnswerResult);
        }
    }
}
