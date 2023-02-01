using System;
using BcyWebCrawler.Core.Context;

namespace BcyWebCrawler.Core.UT
{
    public class AuthorContextTest
    {
        private static void CreateContext(out AuthorContext authorContext)
        {
            authorContext = new AuthorContext();
            authorContext.Database.EnsureCreated();
        }

        #region Author

        [Fact]
        public void TestAuthors()
        {
            Author addAuthor = TestAddAuthor();
            IEnumerable<Author> readAuthors = TestReadAuthor();
            Assert.True(readAuthors.Where(author =>
            author.AuthorId == addAuthor.AuthorId).Any());
        }

        private static Author TestAddAuthor()
        {
            // Add
            GetAuthors(out AuthorContext authorContext, out long maxId);
            Author author = new Author() { AuthorId = maxId };
            authorContext.Authors.Add(author);
            authorContext?.SaveChanges();
            return author;
        }

        private static IEnumerable<Author> TestReadAuthor()
        {
            // Read
            GetAuthors(out AuthorContext authorContext, out long maxId);
            return authorContext.Authors;
        }

        private static void GetAuthors(out AuthorContext authorContext, out long maxId)
        {
            CreateContext(out authorContext);
            maxId = authorContext.Authors.Max(item => item.AuthorId);
            maxId++;
        }

        #endregion
    }
}

