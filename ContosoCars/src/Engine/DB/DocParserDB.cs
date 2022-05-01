using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCars.Engine.DB
{
    public class DocParserDB : DbContext
    {
        public DocParserDB() : base("name=DocParserDB") { }

        public virtual DbSet<DBEmailReview> Reviews { get; set; }

    }
}
