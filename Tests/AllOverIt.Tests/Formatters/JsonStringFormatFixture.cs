using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Formatters;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Formatters
{
    public class JsonStringFormatFixture : FixtureBase
    {
        private const string BasicSourceString = @"{""key"":""value""}";

        [Fact]
        public void Should_Throw_When_Null()
        {
            Invoking(
                () =>
                {
                    _ = JsonString.Format(null);
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
                   _ = JsonString.Format(BasicSourceString, -1);
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
                   _ = JsonString.Format(BasicSourceString, 0);
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
                   _ = JsonString.Format(BasicSourceString, Create<int>());
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

            var actual = JsonString.Format(BasicSourceString);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Return_Bad_Output_For_Bad_Input()
        {
            var stringValue = Create<string>();
            var expected = 
$@"{stringValue}{{
  ab: cd
}}";

            var actual = JsonString.Format($"{stringValue}{{ab:cd}}");

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

            var actual = JsonString.Format(source);

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

            var actual = JsonString.Format(source);

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

            var actual = JsonString.Format(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Add_LineBreak_After_Unquoted_Comma()
        {
            var source = @"{""key1"":""value1"",""key2"":""value2""}";
            var expected =
@"{
  ""key1"": ""value1"",
  ""key2"": ""value2""
}";

            var actual = JsonString.Format(source);

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

            var actual = JsonString.Format(source);

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

            var actual = JsonString.Format(source);

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

            var actual = JsonString.Format(source);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Not_Add_Blank_Line_For_Empty_Object()
        {
            var source = @"{}";
            var expected =
@"{
}";

            var actual = JsonString.Format(source);

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

            var actual = JsonString.Format(source);

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

            var actual = JsonString.Format(source);

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

            var actual = JsonString.Format(source);

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

            var actual = JsonString.Format(source);

            actual.Should().Be(expected);
        }
    }
}
