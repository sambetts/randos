using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelFactory
{
    public class DebateDbContext : IdentityDbContext<ApplicationUser>
    {
        public DebateDbContext(DbContextOptions<DebateDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Answer>()
            .HasOne(a => a.parent_debate)
            .WithMany(b => b.answers);

            //modelBuilder.Entity<Answer>()
            //.HasOne(a => a.parent_debate)
            //.HasForeignKey<Debate>(a => a);

            // Unique keys
            modelBuilder.Entity<DebateUser>()
             .HasIndex(u => u.id)
             .IsUnique();
            modelBuilder.Entity<Tag>()
            .HasIndex(t => t.tag_name)
            .IsUnique();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var lf = new LoggerFactory();
            //lf.AddProvider(new MyLoggerProvider());
            optionsBuilder.UseLoggerFactory(lf);
        }

        public DbSet<DebateUser> users { get; set; }
        public DbSet<Answer> answers { get; set; }
        public DbSet<Tag> tags { get; set; }
        public DbSet<Link> links { get; set; }
        public DbSet<DebateTags> debate_tags { get; set; }
        public DbSet<Debate> debates { get; set; }
        public DbSet<ResponseType> response_types { get; set; }

        #region Custom DB Methods

        public void DeleteAnswerTree(int parentId)
        {
            using (SqlConnection con = new SqlConnection(this.Database.GetDbConnection().ConnectionString))
            {
                con.Open();

                var cmd = new SqlCommand("DeleteAnswerID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@parentId", System.Data.SqlDbType.Int).Value = parentId;

                cmd.ExecuteNonQuery();

            }
        }


        /// <summary>
        /// Convert DB answers into just the Tree structure equivalent. No scoring until later.
        /// </summary>
        public async Task<AnswerDataOnlyTreeNode> GetAnswerTree(int parentId)
        {
            var results = new List<AnswerDataOnlyTreeNode>();
            var allUsers = this.users.ToListAsync().Result;

            using (SqlConnection con = new SqlConnection(this.Database.GetDbConnection().ConnectionString))
            {
                con.Open();

                var cmd = new SqlCommand("GetChildAnswersForAnswerID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@parent_answerid", System.Data.SqlDbType.Int).Value = parentId;

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {

                    while (reader.Read())
                    {
                        // Read row
                        int i = 0;
                        int level = reader.GetInt32(i++);
                        int id = reader.GetInt32(i++);
                        string title = reader.GetString(i++);

                        string body = string.Empty;
                        if (!reader.IsDBNull(i++))
                        {
                            body = reader.GetString(i - 1);
                        }

                        int userId = reader.GetInt32(i++);
                        int? answerParentID = null;

                        if (!reader.IsDBNull(i++))
                        {
                            answerParentID = reader.GetInt32(i - 1);
                        }

                        int reactionId = 0;
                        ResponseType reaction = null;
                        if (!reader.IsDBNull(i++))
                        {
                            reactionId = reader.GetInt32(i - 1);

                            // Hack
                            reaction = this.response_types.Where(t => t.id == reactionId).FirstOrDefault();
                        }
                        string path = reader.GetString(i++);

                        // Create new DTO from DB results. 
                        var a = new AnswerDataOnlyTreeNode()
                        {
                            ID = id,
                            Title = title,
                            Body = body,
                            LoadPath = path,
                            ParentID = answerParentID,
                            CreatedBy = allUsers.Find(x => x.id == userId).ToModel(),
                            ReactionToParent = reaction?.ToModel()
                        };

                        results.Add(a);
                    }
                }
            }
            var root = results.Find(a => a.ID == parentId) as AnswerDataOnlyTreeNode;
            ConvertToTree(results, root);


            return root;
        }


        public async Task<AnswerWithRatingsTreeNode> GetAnswerTreeWithScores(int parentId)
        {
            var root = await GetAnswerTree(parentId);
            var rules = ScoringRules.LoadRules();
            var rootRating = new AnswerWithRatingsTreeNode(root, rules);

            return rootRating;
        }


        /// <summary>
        /// Whores
        /// </summary>
        private static void ConvertToTree(List<AnswerDataOnlyTreeNode> flatAnswersCache, AnswerDataOnlyTreeNode root)
        {
            foreach (var a in flatAnswersCache)
            {
                if (a.ParentID.HasValue && a.ParentID == root.ID)
                {
                    root.ChildAnswers.Add(a);
                }
            }

            foreach (var childAnswer in root.ChildAnswers)
            {
                ConvertToTree(flatAnswersCache, childAnswer);
            }
        }



        #endregion
    }
}
