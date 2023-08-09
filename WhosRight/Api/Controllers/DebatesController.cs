using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.EntityFrameworkCore;
using ModelFactory;
using ModelFactory.Config;
using Models;
using Models.Entities;
using Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Api.Controllers
{
    [Route("[controller]")]
    public class DebatesController : BaseController
    {
        public DebatesController(DebateDbContext context, ServerSideKeyVaultSettings settings) : base(context, settings)
        {
        }

        // GET: /Debates
        public async Task<IActionResult> GetAllDebates()
        {
            // Todo: cache this to not murder scalability
            var allDebates = _context.debates.Include(d => d.answers).ToList();
            var displayDebates = new List<DebateDTO>();

            // Get user data
            var manager = UserMetadataManager.CreateInstance(_settings.ConnectionStrings.Storage);
            var user = _context.users.Where(u => u.email == this.User.Identity.Name).SingleOrDefault();

            UserMetaTableEntity userMeta = null;
            if (user != null)
            {
                userMeta = await manager.GetUserMeta(user.ToModel());
            }
            else
            {
                userMeta = new UserMetaTableEntity();
            }

            foreach (var debate in allDebates)
            {
                // Deep load the answer to get full scores, winner, etc. 
                // Obviously shitty performance but fine for a PoC
                var deepLoadAnswer = await _context.GetAnswerTreeWithScores(debate.root_answer.id);
                var modelObj = new DebateDTO(deepLoadAnswer);

                // Add tags
                var tags = await _context.debate_tags.Include(i => i.tag).Where(t => t.debate == debate).ToListAsync();
                foreach (var tagAssociation in tags)
                {
                    modelObj.Tags.Add(tagAssociation.tag.tag_name);
                }
                displayDebates.Add(modelObj);
            }

            return Ok(new HomePageModel() { Debates = displayDebates, UserMeta = userMeta.ToModel() });
        }

        // GET: Debates/DetailsByWebsafeName/dogs_vs_cats
        [Route("[action]/{webSafeName}")]
        public async Task<ActionResult<DebateDetailsPage>> DetailsByWebsafeName(string webSafeName)
        {
            if (string.IsNullOrEmpty(webSafeName))
            {
                return BadRequest();
            }
            var debateAnswer = await _context.answers
                .Include(a => a.parent_debate)
                .Where(a => a.title == StringUtils.GetNameFromWebSafeString(webSafeName))
                .SingleOrDefaultAsync();

            if (debateAnswer == null)
                return NotFound();

            return await Details(debateAnswer.parent_debate.id, null);
        }

        // GET: Debates/5
        [Route("{id}")]
        public async Task<ActionResult<DebateDetailsPage>> Details(int? id, Guid? version)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Just load the base answer with the view. The page JavaScript will load the rest. 
            var answer = _context.answers.Where(m => m.id == id)
                .Include(u => u.user)
                .FirstOrDefault();

            if (answer == null)
            {
                return NotFound();
            }

            // Build history from Cosmos
            var history = new List<DebateHistory>();
            const string DB_NAME = "DebateHistory";
            const string COLLECTION_NAME = "DebateChangeLog";
            var cosmosClient = new DocumentClient(new Uri(_settings.CosmosDbUrl), _settings.CosmosDbKey);

            // Create DB is doesn't exist (emulator needs to be running)
            var db = await cosmosClient.CreateDatabaseIfNotExistsAsync(new Database { Id = DB_NAME });

            // Create DB collection if doesn't exist
            var coll = await cosmosClient.CreateDocumentCollectionIfNotExistsAsync(
                    db.Resource.SelfLink, new DocumentCollection { Id = COLLECTION_NAME }
            );


            var historyQuery = cosmosClient.CreateDocumentQuery<AnswerHistoryDTO>(UriFactory.CreateDocumentCollectionUri(DB_NAME, COLLECTION_NAME))
                .Where(d => d.AnswerTree.ID == id);

            // Get history
            var docHistory = historyQuery.ToList();

            foreach (var change in docHistory)
            {
                history.Add(new DebateHistory() { ChangeDateTime = change.TriggeredOn, User = change.TriggeredBy, ID = change.Id });
            }


            AnswerWithRatingsTreeNode answerTree = null;

            // Are we asking for a specific version of the debate?
            if (version.HasValue)
            {
                // Dig up old version
                Uri versionUrl = UriFactory.CreateDocumentCollectionUri(DB_NAME, COLLECTION_NAME);
                FeedOptions feedOptions = new FeedOptions { MaxItemCount = 1 };
                IQueryable<AnswerHistoryDTO> oldVersion = cosmosClient.CreateDocumentQuery<AnswerHistoryDTO>(versionUrl,
                    $"Select * from {COLLECTION_NAME} where {COLLECTION_NAME}.id='{version}'",
                    feedOptions);

                // Recalculate
                // Hack due to API limitation
                answerTree = new AnswerWithRatingsTreeNode(oldVersion.AsEnumerable().FirstOrDefault().AnswerTree);
            }
            else
            {
                // Metadata. Update user "seen" record
                UserMetadataManager manager = UserMetadataManager.CreateInstance(_settings.ConnectionStrings.Storage);
                var user = _context.users.Where(u => u.email == this.User.Identity.Name).SingleOrDefault();

                if (user != null)
                {
                    // This probably shit
                    var userMeta = await manager.GetUserMeta(user.ToModel());
                    userMeta.ToModel().AddDebate(answer.ToModel());
                    await manager.Update(userMeta);
                }

                answerTree = await _context.GetAnswerTreeWithScores(id.Value);
            }


            var model = new DebateDetailsPage()
            {
                Debate = new DebateDTO(answerTree),
                AllResponseTypes = _context.response_types.ToList().ToModelList(),
                Changes = history
            };

            return Ok(model);
        }


        // GET: Debates/GetResponseTypes
        [Route("{action}")]
        public async Task<ActionResult<List<Models.ResponseType>>> GetResponseTypes()
        {

            // Just load the base answer with the view. The page JavaScript will load the rest. 
            var typesb = await _context.response_types.ToListAsync();

            var ts = new List<Models.ResponseType>();
            foreach (var t in typesb)
            {
                ts.Add(t.ToModel());
            }

            return Ok(ts);
        }
    }
}
