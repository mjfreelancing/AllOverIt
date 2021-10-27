using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Io;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Io
{
    public class FileUtilsFixture : FixtureBase
    {
        public class PathIsSubFolder : FileUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Parent_Path_Null()
            {
                Invoking(() => FileUtils.PathIsSubFolder(null, Create<string>()))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("parentPath");
            }

            [Fact]
            public void Should_Throw_When_Parent_Path_Empty()
            {
                Invoking(() => FileUtils.PathIsSubFolder(string.Empty, Create<string>()))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("parentPath");
            }

            [Fact]
            public void Should_Throw_When_Parent_Path_Whitespace()
            {
                Invoking(() => FileUtils.PathIsSubFolder(" ", Create<string>()))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("parentPath");
            }

            [Fact]
            public void Should_Throw_When_Child_Path_Offset_Null()
            {
                Invoking(() => FileUtils.PathIsSubFolder(Create<string>(), null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("childPath");
            }

            [Fact]
            public void Should_Throw_When_Child_Path_Offset_Empty()
            {
                Invoking(() => FileUtils.PathIsSubFolder(Create<string>(), string.Empty))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("childPath");
            }

            [Fact]
            public void Should_Throw_When_Child_Path_Offset_Whitespace()
            {
                Invoking(() => FileUtils.PathIsSubFolder(Create<string>(), " "))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("childPath");
            }

            [Fact]
            public void Should_Return_Is_SubFolder()
            {
                Assert.True(FileUtils.PathIsSubFolder(@"C:\", @"C:\abc"));
                Assert.True(FileUtils.PathIsSubFolder(@"C:\", @"C:\abc\"));
                Assert.True(FileUtils.PathIsSubFolder(@"C:\", @"C:\"));
            }

            [Fact]
            public void Should_Return_Is_Not_SubFolder()
            {
                Assert.False(FileUtils.PathIsSubFolder(@"C:\abc", @"C:\"));
                Assert.True(FileUtils.PathIsSubFolder(@"C:\", @"C:\d\abc"));
                Assert.False(FileUtils.PathIsSubFolder(@"D:\", @"C:\abc"));
            }
        }

        public class GetAbsolutePath : FileUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Path_Null()
            {
                Invoking(() => FileUtils.GetAbsolutePath(null, Create<string>()))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("sourcePath");
            }

            [Fact]
            public void Should_Throw_When_Source_Path_Empty()
            {
                Invoking(() => FileUtils.GetAbsolutePath(string.Empty, Create<string>()))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("sourcePath");
            }

            [Fact]
            public void Should_Throw_When_Source_Path_Whitespace()
            {
                Invoking(() => FileUtils.GetAbsolutePath(" ", Create<string>()))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("sourcePath");
            }

            [Fact]
            public void Should_Throw_When_Relative_Path_Offset_Null()
            {
                Invoking(() => FileUtils.GetAbsolutePath(Create<string>(), null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("relativePath");
            }

            [Fact]
            public void Should_Not_Throw_When_Relative_Path_Offset_Empty()
            {
                Invoking(() => FileUtils.GetAbsolutePath(Create<string>(), string.Empty))
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Relative_Path_Offset_Whitespace()
            {
                Invoking(() => FileUtils.GetAbsolutePath(Create<string>(), " "))
                  .Should()
                  .NotThrow();
            }

            [Theory]
            [InlineData("", @"C:\a\b\c")]
            [InlineData(@"..", @"C:\a\b")]
            [InlineData(@"..\", @"C:\a\b\")]
            [InlineData(@"..\d", @"C:\a\b\d")]
            public void Should_Return_Absolute_Path(string relativePath, string expected)
            {
                var actual = FileUtils.GetAbsolutePath(@"C:\a\b\c", relativePath);

                actual.Should().Be(expected);
            }
        }

        public class GetAbsoluteFileName : FileUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Name_Null()
            {
                Invoking(() => FileUtils.GetAbsoluteFileName(null, Create<string>()))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("sourceFilename");
            }

            [Fact]
            public void Should_Throw_When_Source_Name_Empty()
            {
                Invoking(() => FileUtils.GetAbsoluteFileName(string.Empty, Create<string>()))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("sourceFilename");
            }

            [Fact]
            public void Should_Throw_When_Source_Name_Whitespace()
            {
                Invoking(() => FileUtils.GetAbsoluteFileName(" ", Create<string>()))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("sourceFilename");
            }

            [Fact]
            public void Should_Throw_When_Relative_Path_Offset_Null()
            {
                Invoking(() => FileUtils.GetAbsoluteFileName(Create<string>(), null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("relativePath");
            }

            [Fact]
            public void Should_Throw_When_Relative_Path_Offset_Empty()
            {
                Invoking(() => FileUtils.GetAbsoluteFileName(Create<string>(), string.Empty))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("relativePath");
            }

            [Fact]
            public void Should_Throw_When_Relative_Path_Offset_Whitespace()
            {
                Invoking(() => FileUtils.GetAbsoluteFileName(Create<string>(), " "))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("relativePath");
            }

            [Theory]
            [InlineData(".", null, @"C:\a\b\c.txt")]
            [InlineData(@"..\", null, @"C:\a\c.txt")]
            [InlineData(@"..\d", "e.txt", @"C:\a\d\e.txt")]
            public void Should_Return_Absolute_FileName(string pathOffset, string newName, string expected)
            {
                var actual = FileUtils.GetAbsoluteFileName(@"C:\a\b\c.txt", pathOffset, newName);

                actual.Should().Be(expected);
            }
        }
    }
}
