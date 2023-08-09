using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelFactory
{
    public class DbInitializer
    {
        public static void Initialize(DebateDbContext context)
        {

            // Make sure normal tables exist. 
            context.Database.EnsureCreated();


            if (context.answers.Any())
            {
                return;
            }

            // Save new default data
            DebateUser sam = new DebateUser() { display_name = "Sam", email = "sambetts@cahooners.com" };
            DebateUser patricia = new DebateUser() { display_name = "Patricia", email = "hib@trescantos.com" };
            DebateUser bostrom = new DebateUser() { display_name = "Bostrom", email = "massivecunt@bigblackcocks.com" };
            DebateUser bex = new DebateUser() { display_name = "Bex", email = "bexaroo@scarycars.com" };

            ResponseType disagreeWithParentProposition = new ResponseType() { agree_with_parent = false, response_text = Constants.RESPONSE_DISAGREE };
            ResponseType agreeWithParentProposition = new ResponseType() { agree_with_parent = true, response_text = Constants.RESPONSE_AGREE };
            ResponseType disagreeWithParentEvidence = new ResponseType() { agree_with_parent = false, response_text = Constants.RESPONSE_DISAGREE_WITH_EVIDENCE };
            ResponseType agreeWithParentEvidence = new ResponseType() { agree_with_parent = true, response_text = Constants.RESPONSE_AGREE_WITH_EVIDENCE };
            ResponseType parentIsIrrelevant = new ResponseType() { agree_with_parent = false, response_text = Constants.RESPONSE_IRRELEVANT };

            ResponseType justSaying = new ResponseType() { agree_with_parent = null, response_text = "Just saying" };

            // Add default debates
            Debate gasDebate = new Debate("Gas vs electric cookers", "Gas cookers are way better - much quicker to cook with", sam);

            AddTagToDebate(gasDebate, "cooking");
            AddTagToDebate(gasDebate, "gas cookers");
            AddTagToDebate(gasDebate, "electric cookers");
            Answer bibRoot = gasDebate.root_answer.AddResponse("Nope", "Electric is better. It's easier to clean.", patricia, disagreeWithParentProposition)
                .AddResponse("Don't care", "Electric is shit to cook with, and gas isn't hard to clean", sam, disagreeWithParentProposition)
                    .AddResponse("That's cos you're not used to it", "", patricia, disagreeWithParentProposition);

            gasDebate.root_answer.AddResponse("I don't care", "", bostrom, justSaying)
                .AddResponse("Yeh Sam, nobody cares.", "", patricia, justSaying)
                    .AddResponse("Silence, you", "", patricia, justSaying);

            gasDebate.root_answer.AddResponse("I much prefer gas", "", bex, agreeWithParentProposition)
                .AddResponse("Cheers luv", "", sam, justSaying)
                    .AddResponse("That's not an argument", "", patricia, parentIsIrrelevant);







            context.debates.Add(gasDebate);
            context.SaveChanges();

            // Add stored procs
            var getTreeStoredProc = @"
CREATE PROCEDURE [dbo].[GetChildAnswersForAnswerID] 

	@parent_answerid int = null

AS
BEGIN

	SET NOCOUNT ON;


WITH
    [CTE_AnswerTree] AS
    (
    ( -- Anchor
        SELECT
            0 AS [level]
		, id, [title], [body], userid, parent_answerid, reaction_to_parentid
        , CAST(id AS VARCHAR(50)) AS [Path]
        FROM
        answers
        WHERE
        id = @parent_answerid
    )
    UNION ALL
    ( -- Recursive part
        SELECT
            [Prior].[level] + 1 AS [level]
		, [This].id, [This].[title], [This].[body], [This].userid, [This].parent_answerid, [This].reaction_to_parentid
        , CAST([Prior].[Path] + ' > ' + CAST([This].id AS VARCHAR(8)) AS VARCHAR(50)) AS [Path]
        FROM
        answers [This]
        INNER JOIN [CTE_AnswerTree] [Prior]
            ON [Prior].id = [This].parent_answerid
    )
    )
SELECT
    [level]
    , id, [title], [body], userid, parent_answerid, reaction_to_parentid
    , [Path] AS [Path]
FROM
    [CTE_AnswerTree]
ORDER BY
    [level];


END
                
            ";
            context.Database.ExecuteSqlRaw(getTreeStoredProc);

            // Delete
            var deleteTreeStoredProc = @"
CREATE PROCEDURE [dbo].[DeleteAnswerID] 

	@parent_answerid int = null

AS
BEGIN

	SET NOCOUNT ON;


WITH
    [CTE_AnswerTree] AS
    (
    ( -- Anchor
        SELECT
            0 AS [level]
		, id, parent_answerid
        FROM
            answers
        WHERE
            id = @parent_answerid
    )
    UNION ALL
    ( -- Recursive part
        SELECT
            [Prior].[level] + 1 AS [level]
		, [This].id, [This].parent_answerid
        FROM
        answers [This]
        INNER JOIN [CTE_AnswerTree] [Prior]
            ON [Prior].id = [This].parent_answerid
    )
    )
delete from answers where id in (
SELECT
    id
FROM
    [CTE_AnswerTree]
);


END
";
            context.Database.ExecuteSqlRaw(deleteTreeStoredProc);
        }

        private static void AddTagToDebate(Debate debate, string newTag)
        {
            debate.tags.Add(new DebateTags() { debate = debate, tag = new Tag() { tag_name = newTag } });
        }
    }
}
