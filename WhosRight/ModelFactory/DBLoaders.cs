using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelFactory
{
    public static class DBLoaders
    {

        public static async Task<Answer> ToAnswer(this NewAnswerDTO thisAnswer, DebateDbContext context, DebateUser userReplying)
        {
            var parent = await context.answers.Where(a => a.id == thisAnswer.ParentAnswerID).FirstOrDefaultAsync();
            var selectedType = await context.response_types.Where(r => r.id == thisAnswer.ReactionIDToParent).FirstOrDefaultAsync();

            return new Answer() { reaction_to_parent = selectedType, parent_answer = parent, title = thisAnswer.Title, user = userReplying, body = thisAnswer.Body };
        }

        public static List<Models.ResponseType> ToModelList(this List<ResponseType> dbList)
        {
            var modelList = new List<Models.ResponseType>();
            foreach (var item in dbList)
            {
                modelList.Add(item.ToModel());
            }

            return modelList;
        }
    }
}
