using System;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class StringFormatExtensionsFixture : FixtureBase
    {
        private static readonly string _basicSourceString = @"{""key"":""value""}";

        [Fact]
        public void Should_Throw_When_Null()
        {
            Invoking(
                () =>
                {
                    _ = StringFormatExtensions.FormatJsonString(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("jsonValue");
        }

        [Fact]
        public void Should_Throw_When_IndentSize_Negative()
        {
            Invoking(
               () =>
               {
                   _ = StringFormatExtensions.FormatJsonString(_basicSourceString, -1);
               })
               .Should()
               .Throw<ArgumentOutOfRangeException>()
               .WithMessage("The indent size cannot be negative. (Parameter 'indentSize')");
        }

        [Fact]
        public void Should_Not_Throw_When_IndentSize_Zero()
        {
            Invoking(
               () =>
               {
                   _ = StringFormatExtensions.FormatJsonString(_basicSourceString, 0);
               })
               .Should()
               .NotThrow();
        }

        [Fact]
        public void Should_Not_Throw_When_IndentSize_Not_Negative()
        {
            Invoking(
               () =>
               {
                   _ = StringFormatExtensions.FormatJsonString(_basicSourceString, Create<int>());
               })
               .Should()
               .NotThrow();
        }

        [Fact]
        public void Should_Add_Space_After_Colon()
        {
            var expected =
@"{
  ""key"": ""value""
}";

            var actual = StringFormatExtensions.FormatJsonString(_basicSourceString);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Return_Same_String_When_Not_Json()
        {
            var expected = Create<string>();
            var actual = StringFormatExtensions.FormatJsonString(expected);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Ignore_Unquoted_Whitespace()
        {
            var source = @"{""key"":""value""    }";
            var expected =
@"{
  ""key"": ""value""
}";

            var actual = StringFormatExtensions.FormatJsonString(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Keep_Quoted_Whitespace()
        {
            var source = @"{""key"":""val ue""  }";
            var expected =
@"{
  ""key"": ""val ue""
}";

            var actual = StringFormatExtensions.FormatJsonString(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Keep_Quoted_Comma()
        {
            var source = @"{""key"":""val,ue""  }";
            var expected =
@"{
  ""key"": ""val,ue""
}";

            var actual = StringFormatExtensions.FormatJsonString(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Add_LineBreak_After_Unqouted_Comma()
        {
            var source = @"{""key1"":""value1"",""key2"":""value2""}";
            var expected =
@"{
  ""key1"": ""value1"",
  ""key2"": ""value2""
}";

            var actual = StringFormatExtensions.FormatJsonString(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Add_LineBreak_And_Indent_After_Opening_Brace()
        {
            var source = @"{""key1"":{""key2"":""value2""}}";
            var expected =
@"{
  ""key1"": {
    ""key2"": ""value2""
  }
}";

            var actual = StringFormatExtensions.FormatJsonString(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Add_LineBreak_And_Indent_After_Opening_Bracket()
        {
            var source = @"{""key1"":[""value1"", ""val , ue2"", ""value3""]}";
            var expected =
@"{
  ""key1"": [
    ""value1"",
    ""val , ue2"",
    ""value3""
  ]
}";

            var actual = StringFormatExtensions.FormatJsonString(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Move_Closing_Bracket_To_Next_Line()
        {
            var source = @"{""key1"":[""value1""]}";
            var expected =
@"{
  ""key1"": [
    ""value1""
  ]
}";

            var actual = StringFormatExtensions.FormatJsonString(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Not_Add_Blank_Line_For_Empty_Object()
        {
            var source = @"{}";
            var expected =
@"{
}";

            var actual = StringFormatExtensions.FormatJsonString(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Not_Add_LineBreak_After_Quoted_Opening_Bracket()
        {
            var source = @"{""key1"":[""val[ue1""]}";
            var expected =
@"{
  ""key1"": [
    ""val[ue1""
  ]
}";

            var actual = StringFormatExtensions.FormatJsonString(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Not_Move_Quoted_Closing_Bracket_To_Next_Line()
        {
            var source = @"{""key1"":[""val]ue1""]}";
            var expected =
@"{
  ""key1"": [
    ""val]ue1""
  ]
}";

            var actual = StringFormatExtensions.FormatJsonString(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Not_Add_Blank_Line_For_Empty_Array()
        {
            var source = @"{""key1"":[]}";
            var expected =
                @"{
  ""key1"": [
  ]
}";

            var actual = StringFormatExtensions.FormatJsonString(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Remove_Extra_Whitespace()
        {
            var source = @"    {   ""key1""    :


""value1""   ,  ""key2"":   ""value2""   }

";

            var expected =
                @"{
  ""key1"": ""value1"",
  ""key2"": ""value2""
}";

            var actual = StringFormatExtensions.FormatJsonString(source);

            actual.Should().Be(expected);
        }
    }
}
