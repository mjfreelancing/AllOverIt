using AllOverIt.Cryptography.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Tests.Extensions
{
    public class HashAlgorithmNameExtensionsFixture : FixtureBase
    {
        public class CreateHashAlgorithm : HashAlgorithmNameExtensionsFixture
        {
            [Theory]
            [ClassData(typeof(HashAlgorithmTestData))]
            public void Should_Create_Hash_Algorithm(HashAlgorithmName algorithmName, HashAlgorithm expected)
            {
                var actual = HashAlgorithmNameExtensions.CreateHashAlgorithm(algorithmName);

                actual.Should().BeOfType(expected.GetType());
            }
        }

        public class GetHashSize : HashAlgorithmNameExtensionsFixture
        {
            [Theory]
            [ClassData(typeof(HashSizeTestData))]
            public void Should_Get_Hash_Size(HashAlgorithmName algorithmName, int expected)
            {
                var actual = HashAlgorithmNameExtensions.GetHashSize(algorithmName);

                actual.Should().Be(expected);
            }
        }

        private class HashAlgorithmTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { HashAlgorithmName.MD5, MD5.Create() };
                yield return new object[] { HashAlgorithmName.SHA1, SHA1.Create() };
                yield return new object[] { HashAlgorithmName.SHA256, SHA256.Create() };
                yield return new object[] { HashAlgorithmName.SHA384, SHA384.Create() };
                yield return new object[] { HashAlgorithmName.SHA512, SHA512.Create() };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class HashSizeTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { HashAlgorithmName.MD5, 128 };
                yield return new object[] { HashAlgorithmName.SHA1, 160 };
                yield return new object[] { HashAlgorithmName.SHA256, 256 };
                yield return new object[] { HashAlgorithmName.SHA384, 384 };
                yield return new object[] { HashAlgorithmName.SHA512, 512 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}