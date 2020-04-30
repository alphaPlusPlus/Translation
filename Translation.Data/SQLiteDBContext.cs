using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Translation.Data.Entities;

namespace Translation.Data
{
    public class SqlLiteDbContext : DbContext
    {
        public SqlLiteDbContext(DbContextOptions<SqlLiteDbContext> options)
            : base(options)
        {
        }

        public DbSet<TranslationItem> TranslationItems { get; set; }
    }
}
