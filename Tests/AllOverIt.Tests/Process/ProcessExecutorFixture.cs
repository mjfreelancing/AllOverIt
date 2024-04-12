using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Process;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;

namespace AllOverIt.Tests.Process
{
    public class ProcessExecutorFixture : FixtureBase
    {
        [Fact]
        public void Should_Throw_When_Options_Null()
        {
            Invoking(() =>
            {
                _ = new ProcessExecutor(null);
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("options");
        }

        [Fact]
        public void Should_Set_Default_Process_StartInfo()
        {
            var fileName = Create<string>();

            var options = new ProcessExecutorOptions(fileName);

            var process = new ProcessExecutor(options);

            var actual = process._process.StartInfo;

            var expected = new
            {
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardInput = false,
                RedirectStandardError = false,
                RedirectStandardOutput = false,
                StandardInputEncoding = (Encoding) default,
                StandardErrorEncoding = (Encoding) default,
                StandardOutputEncoding = (Encoding) default,
                WorkingDirectory = string.Empty,
                FileName = fileName,
                Arguments = string.Empty,
                ArgumentList = new Collection<string>(),
                EnvironmentVariables = Environment.GetEnvironmentVariables(),
                Domain = string.Empty,
                UserName = string.Empty,
                Password = (SecureString) default,
                PasswordInClearText = (string) default,
                Verb = string.Empty,
                WindowStyle = ProcessWindowStyle.Normal,
                LoadUserProfile = false,
                ErrorDialog = false,
                ErrorDialogParentHandle = (IntPtr) default

#if !NETSTANDARD2_1
                ,
                UseCredentialsForNetworkingOnly = false
#endif

            };

            expected.Should().BeEquivalentTo(actual, options =>
            {
                options
                    .Excluding(info => info.Environment)
                    .Excluding(info => info.Verbs);

                return options;
            });
        }

        [Fact]
        public void Should_Set_Process_StartInfo()
        {
            var fileName = Create<string>();

            var options = new ProcessExecutorOptions(fileName)
            {
                // Only setting properties that impact this test
                WorkingDirectory = Create<string>(),
                Arguments = Create<string>(),
                EnvironmentVariables = Create<Dictionary<string, string>>(),
            };

            var process = new ProcessExecutor(options);

            var actual = process._process.StartInfo;

            var expected = new
            {
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardInput = false,
                RedirectStandardError = false,
                RedirectStandardOutput = false,
                StandardInputEncoding = (Encoding) default,
                StandardErrorEncoding = (Encoding) default,
                StandardOutputEncoding = (Encoding) default,
                WorkingDirectory = options.WorkingDirectory,
                FileName = fileName,
                Arguments = options.Arguments,
                ArgumentList = new Collection<string>(),
                //EnvironmentVariables = expectedEnvironmentVariables,
                Domain = string.Empty,
                UserName = string.Empty,
                Password = (SecureString) default,
                PasswordInClearText = (string) default,
                Verb = string.Empty,
                WindowStyle = ProcessWindowStyle.Normal,
                LoadUserProfile = false,
                ErrorDialog = false,
                ErrorDialogParentHandle = (IntPtr) default

#if !NETSTANDARD2_1
                ,
                UseCredentialsForNetworkingOnly = false
#endif

            };

            expected.Should().BeEquivalentTo(actual, options =>
            {
                options
                    .Excluding(info => info.Environment)
                    .Excluding(info => info.EnvironmentVariables)
                    .Excluding(info => info.Verbs);

                return options;
            });

            // the environment variables are a StringCollection, so we need to convert to IDictionary<string, string> for the comparison to work
            var actualKeys = new List<string>();
            var actualKeysEnumerator = actual.EnvironmentVariables.Keys.GetEnumerator();

            while (actualKeysEnumerator.MoveNext())
            {
                actualKeys.Add((string) actualKeysEnumerator.Current);
            }

            var actualEnvironmentVariables = actualKeys
                .Select(key => new KeyValuePair<string, string>(key, actual.EnvironmentVariables[key]))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var expectedEnvironmentVariables = Environment.GetEnvironmentVariables()
                .ToSerializedDictionary()
                .Concat(options.EnvironmentVariables);

            expectedEnvironmentVariables.Should().BeEquivalentTo(actualEnvironmentVariables);
        }
    }
}