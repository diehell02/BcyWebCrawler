// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BcyWebCrawler.Core.Context
{
    public class AuthorContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Image> Images { get; set; }

        public string DbPath { get; }

        public AuthorContext()
        {
            string dir = $"{AppDomain.CurrentDomain.BaseDirectory}\\db\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            DbPath = Path.Join(dir, "author.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
    }
}
