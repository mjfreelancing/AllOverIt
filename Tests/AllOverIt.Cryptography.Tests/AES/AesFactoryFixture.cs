using AllOverIt.Cryptography.AES;
using AllOverIt.Cryptography.AES.Exceptions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Security.Cryptography;
using Xunit;

namespace AllOverIt.Cryptography.Tests.AES
{
    public class AesFactoryFixture : AesFixtureBase
    {
        private readonly AesFactory _factory = new();

        public class Create : AesFactoryFixture
        {
            [Fact]
            public void Should_Create_Aes_With_Default_Configuration()
            {
                var configuration = new AesEncryptionConfiguration();

                var expected = _factory.Create(configuration);
                var actual = _factory.Create();

                // Actual type is a private implementation
                actual.GetType().Name.Should().Be("AesImplementation");

                // The Key and IV will be different as they are random
                actual.Should().BeEquivalentTo(new
                {
                    Mode = configuration.Mode,
                    Padding = configuration.Padding,
                    KeySize = configuration.KeySize,
                    BlockSize = configuration.BlockSize,
                    FeedbackSize = configuration.FeedbackSize
                    //Key = configuration.Key,
                    //IV = configuration.IV
                });
            }

            [Fact]
            public void Should_Create_Aes_With_Provided_Configuration()
            {
                var configuration = CreateAesConfiguration();

                var actual = _factory.Create(configuration);

                // Actual type is a private implementation
                actual.GetType().Name.Should().Be("AesImplementation");

                actual.Should().BeEquivalentTo(new
                {
                    Mode = configuration.Mode,
                    Padding = configuration.Padding,
                    KeySize = configuration.KeySize,
                    BlockSize = configuration.BlockSize,
                    FeedbackSize = configuration.FeedbackSize,
                    Key = configuration.Key,
                    IV = configuration.IV
                });
            }
        }
    }
}
