using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Translation.Data.Entities;

namespace Translation.Data
{
    public class SqLiteDbContext : DbContext
    {
        public SqLiteDbContext(DbContextOptions<SqLiteDbContext> options)
            : base(options)
        {
        }

        public DbSet<TranslationItem> TranslationItems { get; set; }
    }
}
