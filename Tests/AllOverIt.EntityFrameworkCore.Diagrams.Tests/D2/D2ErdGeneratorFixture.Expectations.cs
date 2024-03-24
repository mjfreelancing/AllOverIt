namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.D2
{
    internal static class DiagramExpectations
    {
        public static string DefaultDiagram => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              FirstName: TEXT(50) \[NOT NULL\] 
              LastName: TEXT(50) 
              Email: TEXT(50) \[NOT NULL\] 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              BloggerId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Content: TEXT(1024) \[NOT NULL\] 
              PostId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Title: TEXT(500) \[NOT NULL\] 
              Content: TEXT \[NOT NULL\] 
              BlogId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              JsonConfig: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
            }

            Blog.Id <-> AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }                
            """;

        public static string WithDirection(ErdOptions.DiagramDirection direction) => $$"""
            direction: {{direction.ToString().ToLower()}}

            Author: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              FirstName: TEXT(50) \[NOT NULL\] 
              LastName: TEXT(50) 
              Email: TEXT(50) \[NOT NULL\] 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              BloggerId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Content: TEXT(1024) \[NOT NULL\] 
              PostId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Title: TEXT(500) \[NOT NULL\] 
              Content: TEXT \[NOT NULL\] 
              BlogId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              JsonConfig: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
            }

            Blog.Id <-> AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }                
            """;

        public static string WithGlobalNotPreserveColumnOrder() => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Email: TEXT(50) \[NOT NULL\] 
              FirstName: TEXT(50) \[NOT NULL\] 
              LastName: TEXT(50) 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              BloggerId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              Content: TEXT(1024) \[NOT NULL\] 
              PostId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              BlogId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              Content: TEXT \[NOT NULL\] 
              Title: TEXT(500) \[NOT NULL\] 
            }

            Settings: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              JsonConfig: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
            }

            Author.Id <-> AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }
            """;

        public static string WithGlobalNullableNotVisible() => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              FirstName: TEXT(50) 
              LastName: TEXT(50) 
              Email: TEXT(50) 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              BloggerId: INTEGER { constraint: foreign_key }
              AuthorId: INTEGER { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              Description: TEXT(500) 
              WebSiteId: INTEGER { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              Content: TEXT(1024) 
              PostId: INTEGER { constraint: foreign_key }
              AuthorId: INTEGER { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              Title: TEXT(500) 
              Content: TEXT 
              BlogId: INTEGER { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              JsonConfig: TEXT 
              WebSiteId: INTEGER { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              Description: TEXT(500) 
            }

            Blog.Id <-> AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }
            """;

        public static string WithGlobalNullableIsNull() => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              FirstName: TEXT(50) 
              LastName: TEXT(50) \[NULL\] 
              Email: TEXT(50) 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              BloggerId: INTEGER { constraint: foreign_key }
              AuthorId: INTEGER { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              Description: TEXT(500) 
              WebSiteId: INTEGER { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              Content: TEXT(1024) 
              PostId: INTEGER { constraint: foreign_key }
              AuthorId: INTEGER { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              Title: TEXT(500) 
              Content: TEXT 
              BlogId: INTEGER { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              JsonConfig: TEXT 
              WebSiteId: INTEGER { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              Description: TEXT(500) 
            }

            Blog.Id <-> AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }
            """;

        public static string WithGlobalNullableIsNotNullLabel() => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER *Is Not Null* { constraint: primary_key }
              FirstName: TEXT(50) *Is Not Null* 
              LastName: TEXT(50) 
              Email: TEXT(50) *Is Not Null* 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER *Is Not Null* { constraint: primary_key }
              BloggerId: INTEGER *Is Not Null* { constraint: foreign_key }
              AuthorId: INTEGER *Is Not Null* { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER *Is Not Null* { constraint: primary_key }
              Description: TEXT(500) *Is Not Null* 
              WebSiteId: INTEGER *Is Not Null* { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER *Is Not Null* { constraint: primary_key }
              Content: TEXT(1024) *Is Not Null* 
              PostId: INTEGER *Is Not Null* { constraint: foreign_key }
              AuthorId: INTEGER *Is Not Null* { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER *Is Not Null* { constraint: primary_key }
              Title: TEXT(500) *Is Not Null* 
              Content: TEXT *Is Not Null* 
              BlogId: INTEGER *Is Not Null* { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER *Is Not Null* { constraint: primary_key }
              JsonConfig: TEXT *Is Not Null* 
              WebSiteId: INTEGER *Is Not Null* { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER *Is Not Null* { constraint: primary_key }
              Description: TEXT(500) *Is Not Null* 
            }

            Blog.Id <-> AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }
            """;

        public static string WithGlobalNullableIsNullLabel() => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              FirstName: TEXT(50) 
              LastName: TEXT(50) \#Is Null\# 
              Email: TEXT(50) 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              BloggerId: INTEGER { constraint: foreign_key }
              AuthorId: INTEGER { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              Description: TEXT(500) 
              WebSiteId: INTEGER { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              Content: TEXT(1024) 
              PostId: INTEGER { constraint: foreign_key }
              AuthorId: INTEGER { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              Title: TEXT(500) 
              Content: TEXT 
              BlogId: INTEGER { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              JsonConfig: TEXT 
              WebSiteId: INTEGER { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER { constraint: primary_key }
              Description: TEXT(500) 
            }

            Blog.Id <-> AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }
            """;

        public static string WithGlobalNullableNoShowMaxLength() => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              FirstName: TEXT \[NOT NULL\] 
              LastName: TEXT 
              Email: TEXT \[NOT NULL\] 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              BloggerId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Content: TEXT \[NOT NULL\] 
              PostId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Title: TEXT \[NOT NULL\] 
              Content: TEXT \[NOT NULL\] 
              BlogId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              JsonConfig: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT \[NOT NULL\] 
            }

            Blog.Id <-> AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }
            """;

        public static string WithGlobalCardinalityNoShowCrowsFoot() => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              FirstName: TEXT(50) \[NOT NULL\] 
              LastName: TEXT(50) 
              Email: TEXT(50) \[NOT NULL\] 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              BloggerId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Content: TEXT(1024) \[NOT NULL\] 
              PostId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Title: TEXT(500) \[NOT NULL\] 
              Content: TEXT \[NOT NULL\] 
              BlogId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              JsonConfig: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
            }

            Blog.Id -> AuthorBlog.BloggerId: ONE-TO-MANY

            Author.Id -> AuthorBlog.AuthorId: ONE-TO-MANY

            WebSite.Id -> Blog.WebSiteId: ONE-TO-MANY

            Post.Id -> Comment.PostId: ONE-TO-MANY

            Author.Id -> Comment.AuthorId: ONE-TO-MANY

            Blog.Id -> Post.BlogId: ONE-TO-MANY

            WebSite.Id -- Settings.WebSiteId: ONE-TO-ONE
            """;

        public static string WithGlobalCardinalityLabelStyleNotVisible() => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              FirstName: TEXT(50) \[NOT NULL\] 
              LastName: TEXT(50) 
              Email: TEXT(50) \[NOT NULL\] 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              BloggerId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Content: TEXT(1024) \[NOT NULL\] 
              PostId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Title: TEXT(500) \[NOT NULL\] 
              Content: TEXT \[NOT NULL\] 
              BlogId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              JsonConfig: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
            }

            Blog.Id <-> AuthorBlog.BloggerId:  {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> AuthorBlog.AuthorId:  {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Blog.WebSiteId:  {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId:  {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> Comment.AuthorId:  {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> Post.BlogId:  {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId:  {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }
            """;

        public static string WithGlobalCardinalityLabelStyleFontAttributes() => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              FirstName: TEXT(50) \[NOT NULL\] 
              LastName: TEXT(50) 
              Email: TEXT(50) \[NOT NULL\] 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              BloggerId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Content: TEXT(1024) \[NOT NULL\] 
              PostId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Title: TEXT(500) \[NOT NULL\] 
              Content: TEXT \[NOT NULL\] 
              BlogId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              JsonConfig: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
            }

            Blog.Id <-> AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
              style: {
                font-size: 11
                font-color: \#f01234
                bold: true
                underline: true
                italic: true
              }
            }

            Author.Id <-> AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
              style: {
                font-size: 11
                font-color: \#f01234
                bold: true
                underline: true
                italic: true
              }
            }

            WebSite.Id <-> Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
              style: {
                font-size: 11
                font-color: \#f01234
                bold: true
                underline: true
                italic: true
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
              style: {
                font-size: 11
                font-color: \#f01234
                bold: true
                underline: true
                italic: true
              }
            }

            Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
              style: {
                font-size: 11
                font-color: \#f01234
                bold: true
                underline: true
                italic: true
              }
            }

            Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
              style: {
                font-size: 11
                font-color: \#f01234
                bold: true
                underline: true
                italic: true
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
              style: {
                font-size: 11
                font-color: \#f01234
                bold: true
                underline: true
                italic: true
              }
            }
            """;

        public static string WithGlobalCardinalityOneToOneLabel() => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              FirstName: TEXT(50) \[NOT NULL\] 
              LastName: TEXT(50) 
              Email: TEXT(50) \[NOT NULL\] 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              BloggerId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Content: TEXT(1024) \[NOT NULL\] 
              PostId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Title: TEXT(500) \[NOT NULL\] 
              Content: TEXT \[NOT NULL\] 
              BlogId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              JsonConfig: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
            }

            Blog.Id <-> AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ^One-To-One^ {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }
            """;

        public static string WithGlobalCardinalityOneToManyLabel() => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              FirstName: TEXT(50) \[NOT NULL\] 
              LastName: TEXT(50) 
              Email: TEXT(50) \[NOT NULL\] 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              BloggerId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Content: TEXT(1024) \[NOT NULL\] 
              PostId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Title: TEXT(500) \[NOT NULL\] 
              Content: TEXT \[NOT NULL\] 
              BlogId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              JsonConfig: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
            }

            Blog.Id <-> AuthorBlog.BloggerId: <One-To-Many> {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> AuthorBlog.AuthorId: <One-To-Many> {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Blog.WebSiteId: <One-To-Many> {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: <One-To-Many> {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> Comment.AuthorId: <One-To-Many> {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> Post.BlogId: <One-To-Many> {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }
            """;

        public static string WithGroupDefaultStyle() => """
            direction: left

            alias: title

            alias.Author: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              FirstName: TEXT(50) \[NOT NULL\] 
              LastName: TEXT(50) 
              Email: TEXT(50) \[NOT NULL\] 
            }

            alias.AuthorBlog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              BloggerId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            alias.Blog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Content: TEXT(1024) \[NOT NULL\] 
              PostId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Title: TEXT(500) \[NOT NULL\] 
              Content: TEXT \[NOT NULL\] 
              BlogId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              JsonConfig: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
            }

            alias.Blog.Id <-> alias.AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            alias.Author.Id <-> alias.AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> alias.Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            alias.Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            alias.Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }
            """;

        public static string WithGroupCustomStyle() => """
            direction: left

            alias: title {
              style: {
                fill: red
                stroke: \#00ff00
                stroke-width: 2
                stroke-dash: 4
                opacity: 0.6
              }
            }

            alias.Author: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              FirstName: TEXT(50) \[NOT NULL\] 
              LastName: TEXT(50) 
              Email: TEXT(50) \[NOT NULL\] 
            }

            alias.AuthorBlog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              BloggerId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            alias.Blog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Content: TEXT(1024) \[NOT NULL\] 
              PostId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Title: TEXT(500) \[NOT NULL\] 
              Content: TEXT \[NOT NULL\] 
              BlogId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              JsonConfig: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
            }

            alias.Blog.Id <-> alias.AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            alias.Author.Id <-> alias.AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> alias.Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            alias.Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            alias.Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }
            """;

        public static string WithEntityCustomStyle() => """
            direction: left

            Author: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              FirstName: TEXT(50) \[NOT NULL\] 
              LastName: TEXT(50) 
              Email: TEXT(50) \[NOT NULL\] 
            }

            AuthorBlog: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              BloggerId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Blog: {
              shape: sql_table

              style: {
                fill: black
                stroke: \#ccbb88
                stroke-width: 4
                stroke-dash: 2
                opacity: 0.4
              }

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Comment: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Content: TEXT(1024) \[NOT NULL\] 
              PostId: INTEGER \[NOT NULL\] { constraint: foreign_key }
              AuthorId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Post: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Title: TEXT(500) \[NOT NULL\] 
              Content: TEXT \[NOT NULL\] 
              BlogId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            Settings: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              JsonConfig: TEXT \[NOT NULL\] 
              WebSiteId: INTEGER \[NOT NULL\] { constraint: foreign_key }
            }

            WebSite: {
              shape: sql_table

              Id: INTEGER \[NOT NULL\] { constraint: primary_key }
              Description: TEXT(500) \[NOT NULL\] 
            }

            Blog.Id <-> AuthorBlog.BloggerId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> AuthorBlog.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Blog.WebSiteId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Post.Id <-> Comment.PostId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Author.Id <-> Comment.AuthorId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            Blog.Id <-> Post.BlogId: ONE-TO-MANY {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-many
              }
            }

            WebSite.Id <-> Settings.WebSiteId: ONE-TO-ONE {
              source-arrowhead: {
                shape: cf-one-required
              }
              target-arrowhead: {
                shape: cf-one
              }
            }
            """;
    }
}