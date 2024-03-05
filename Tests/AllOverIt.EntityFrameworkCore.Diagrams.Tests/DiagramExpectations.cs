namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests
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
    }
}