using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Zip;
using FluentAssertions;
using System.IO.Compression;
using System.Text;

namespace AllOverIt.Tests.Zip
{
    public class ZipPackageFixture : FixtureBase
    {
        private ZipPackage _zipPackage;

        public ZipPackageFixture()
        {
            _zipPackage = new ZipPackage();
        }

        public class Constructor : ZipPackageFixture
        {
            [Fact]
            public void Should_Throw_When_Accessing_Content_Before_Complete()
            {
                Invoking(() => _ = _zipPackage.Content)
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("The archive must be completed to access the stream.");
            }
        }

        public class AddEntryAsync : ZipPackageFixture
        {
            [Fact]
            public async Task Should_Throw_When_EntryName_Null()
            {
                await Invoking(async () => await _zipPackage.AddEntryAsync(null!, GetRandomContent(), CancellationToken.None))
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("entryName");
            }

            [Fact]
            public async Task Should_Throw_When_EntryName_Empty()
            {
                await Invoking(async () => await _zipPackage.AddEntryAsync(string.Empty, GetRandomContent(), CancellationToken.None))
                    .Should()
                    .ThrowAsync<ArgumentException>()
                    .WithNamedMessageWhenEmpty("entryName");
            }

            [Fact]
            public async Task Should_Throw_When_EntryName_Whitespace()
            {
                await Invoking(async () => await _zipPackage.AddEntryAsync("   ", GetRandomContent(), CancellationToken.None))
                    .Should()
                    .ThrowAsync<ArgumentException>()
                    .WithNamedMessageWhenEmpty("entryName");
            }

            [Fact]
            public async Task Should_Add_Entry_Successfully()
            {
                var entryName = Create<string>();
                var contentText = Create<string>();
                var content = Encoding.UTF8.GetBytes(contentText);

                await _zipPackage.AddEntryAsync(entryName, GetRandomContent(), CancellationToken.None);
                _zipPackage.Complete();

                // Verify the zip content
                using var memoryStream = new MemoryStream();
                await _zipPackage.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
                archive.Entries.Should().HaveCount(1);
                archive.Entries[0].Name.Should().Be(entryName);
            }

            [Fact]
            public async Task Should_Add_Multiple_Entries_Successfully()
            {
                var entry1Name = Create<string>();
                var entry1Text = Create<string>();
                var entry1Content = Encoding.UTF8.GetBytes(entry1Text);

                var entry2Name = Create<string>();
                var entry2Text = Create<string>();
                var entry2Content = Encoding.UTF8.GetBytes(entry2Text);

                await _zipPackage.AddEntryAsync(entry1Name, entry1Content, CancellationToken.None);
                await _zipPackage.AddEntryAsync(entry2Name, entry2Content, CancellationToken.None);

                _zipPackage.Complete();

                // Verify the zip content
                using var memoryStream = new MemoryStream();
                await _zipPackage.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
                archive.Entries.Should().HaveCount(2);
                archive.Entries[0].Name.Should().Be(entry1Name);
                archive.Entries[1].Name.Should().Be(entry2Name);
            }

            [Fact]
            public async Task Should_Add_Entry_With_Empty_Content()
            {
                var entryName = Create<string>();
                var content = Array.Empty<byte>();

                await _zipPackage.AddEntryAsync(entryName, content, CancellationToken.None);
                _zipPackage.Complete();

                // Verify the zip content
                using var memoryStream = new MemoryStream();
                await _zipPackage.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
                archive.Entries.Should().HaveCount(1);
                archive.Entries[0].Name.Should().Be(entryName);
                archive.Entries[0].Length.Should().Be(0);
            }

            [Fact]
            public async Task Should_Throw_When_Archive_Disposed()
            {
                var entryName = Create<string>();
                var content = Encoding.UTF8.GetBytes(Create<string>());

                _zipPackage.Dispose();

                await Invoking(async () => await _zipPackage.AddEntryAsync(entryName, content, CancellationToken.None))
                    .Should()
                    .ThrowAsync<InvalidOperationException>()
                    .WithMessage("The archive has already been disposed.");
            }

            [Fact]
            public async Task Should_Throw_When_Archive_Completed()
            {
                var entryName = Create<string>();
                var content = Encoding.UTF8.GetBytes(Create<string>());

                _zipPackage.Complete();

                await Invoking(async () => await _zipPackage.AddEntryAsync(entryName, content, CancellationToken.None))
                    .Should()
                    .ThrowAsync<InvalidOperationException>()
                    .WithMessage("The archive has already been disposed.");
            }

            [Fact]
            public async Task Should_Throw_When_CancellationToken_Cancelled()
            {
                var entryName = Create<string>();
                var content = Encoding.UTF8.GetBytes(Create<string>());
                var cancellationTokenSource = new CancellationTokenSource();

                cancellationTokenSource.Cancel();

                await Invoking(async () => await _zipPackage.AddEntryAsync(entryName, content, cancellationTokenSource.Token))
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class Complete : ZipPackageFixture
        {
            [Fact]
            public void Should_Complete_Successfully()
            {
                Invoking(() => _zipPackage.Complete())
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Allow_Access_To_Content_After_Complete()
            {
                _zipPackage.Complete();

                Invoking(() => _ = _zipPackage.Content)
                    .Should()
                    .NotThrow();

                _zipPackage.Content.Should().NotBeNull();
                _zipPackage.Content.Should().BeOfType<MemoryStream>();
            }

            [Fact]
            public void Should_Throw_When_Completing_Already_Completed_Archive()
            {
                _zipPackage.Complete();

                Invoking(() => _zipPackage.Complete())
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("The archive has already been completed.");
            }

            [Fact]
            public void Should_Throw_When_Completing_Disposed_Archive()
            {
                _zipPackage.Dispose();

                Invoking(() => _zipPackage.Complete())
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("The archive stream has been disposed.");
            }

            [Fact]
            public async Task Should_Create_Valid_Zip_Archive_After_Complete()
            {
                var entryName = "test.txt";
                var contentText = "Hello, World!";
                var content = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(contentText));

                await _zipPackage.AddEntryAsync(entryName, content, CancellationToken.None);
                _zipPackage.Complete();

                // Verify we can read the zip content properly
                using var memoryStream = new MemoryStream();
                await _zipPackage.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
                archive.Entries.Should().HaveCount(1);

                var entry = archive.Entries[0];
                entry.Name.Should().Be(entryName);

                using var entryStream = entry.Open();
                using var reader = new StreamReader(entryStream);
                var readContent = await reader.ReadToEndAsync();

                readContent.Should().Be(contentText);
            }

            [Fact]
            public void Should_Reset_Stream_Position_After_Complete()
            {
                _zipPackage.Complete();

                _zipPackage.Content.Position.Should().Be(0);
            }
        }

        public class Content : ZipPackageFixture
        {
            [Fact]
            public void Should_Return_Same_Stream_Instance()
            {
                _zipPackage.Complete();

                var content1 = _zipPackage.Content;
                var content2 = _zipPackage.Content;

                content1.Should().BeSameAs(content2);
            }

            [Fact]
            public void Should_Return_MemoryStream()
            {
                _zipPackage.Complete();

                _zipPackage.Content.Should().BeOfType<MemoryStream>();
            }
        }

        public class Dispose : ZipPackageFixture
        {
            [Fact]
            public void Should_Dispose_Successfully()
            {
                Invoking(() => _zipPackage.Dispose())
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Allow_Multiple_Dispose_Calls()
            {
                _zipPackage.Dispose();

                Invoking(() => _zipPackage.Dispose())
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Accessing_Content_After_Dispose()
            {
                _zipPackage.Dispose();

                Invoking(() => _ = _zipPackage.Content)
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("The archive stream has been disposed.");
            }

            [Fact]
            public async Task Should_Throw_When_Adding_Entry_After_Dispose()
            {
                var entryName = Create<string>();
                var content = Encoding.UTF8.GetBytes(Create<string>());

                _zipPackage.Dispose();

                await Invoking(async () => await _zipPackage.AddEntryAsync(entryName, content, CancellationToken.None))
                    .Should()
                    .ThrowAsync<InvalidOperationException>()
                    .WithMessage("The archive has already been disposed.");
            }
        }

        public class Integration : ZipPackageFixture
        {
            [Fact]
            public async Task Should_Create_Complete_Zip_Workflow()
            {
                var cancellationToken = CancellationToken.None;

                // Add multiple entries with different content types
                await _zipPackage.AddEntryAsync("readme.txt",
                    new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes("This is a readme file")),
                    cancellationToken);

                await _zipPackage.AddEntryAsync("data.json",
                    new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes("{\"id\": 1, \"name\": \"test\"}")),
                    cancellationToken);

                await _zipPackage.AddEntryAsync("empty.txt",
                    new ReadOnlyMemory<byte>(Array.Empty<byte>()),
                    cancellationToken);

                // Complete the package
                _zipPackage.Complete();

                // Verify the complete zip package
                using var memoryStream = new MemoryStream();
                await _zipPackage.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
                archive.Entries.Should().HaveCount(3);

                // Verify each entry
                var readmeEntry = archive.Entries.First(e => e.Name == "readme.txt");
                using (var readmeStream = readmeEntry.Open())
                using (var reader = new StreamReader(readmeStream))
                {
                    var content = await reader.ReadToEndAsync();
                    content.Should().Be("This is a readme file");
                }

                var jsonEntry = archive.Entries.First(e => e.Name == "data.json");
                using (var jsonStream = jsonEntry.Open())
                using (var reader = new StreamReader(jsonStream))
                {
                    var content = await reader.ReadToEndAsync();
                    content.Should().Be("{\"id\": 1, \"name\": \"test\"}");
                }

                var emptyEntry = archive.Entries.First(e => e.Name == "empty.txt");
                emptyEntry.Length.Should().Be(0);
            }

            [Fact]
            public async Task Should_Handle_Large_Content()
            {
                // Create a large content (1MB)
                var largeContent = new byte[1024 * 1024];
                for (int i = 0; i < largeContent.Length; i++)
                {
                    largeContent[i] = (byte)(i % 256);
                }

                await _zipPackage.AddEntryAsync("large_file.bin", largeContent, CancellationToken.None);

                _zipPackage.Complete();

                // Verify the large content was stored correctly
                using var memoryStream = new MemoryStream();
                await _zipPackage.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
                archive.Entries.Should().HaveCount(1);

                var entry = archive.Entries[0];
                entry.Name.Should().Be("large_file.bin");

                using var entryStream = entry.Open();
                using var outputStream = new MemoryStream();
                await entryStream.CopyToAsync(outputStream);

                var extractedContent = outputStream.ToArray();
                extractedContent.Should().BeEquivalentTo(largeContent);
            }

            [Fact]
            public async Task Should_Handle_Special_Characters_In_Entry_Names()
            {
                var specialEntries = new[]
                {
                    "file with spaces.txt",
                    "file-with-dashes.txt",
                    "file_with_underscores.txt",
                    "file.with.dots.txt",
                    "file123numbers.txt",
                    "folder/subfolder/file.txt"
                };

                foreach (var entryName in specialEntries)
                {
                    await _zipPackage.AddEntryAsync(entryName, Encoding.UTF8.GetBytes($"Content of {entryName}"), CancellationToken.None);
                }

                _zipPackage.Complete();

                // Verify all entries with special characters
                using var memoryStream = new MemoryStream();
                await _zipPackage.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
                archive.Entries.Should().HaveCount(specialEntries.Length);

                foreach (var expectedEntry in specialEntries)
                {
                    archive.Entries.Should().Contain(e => e.FullName == expectedEntry);
                }
            }
        }

        private byte[] GetRandomContent()
        {
            return Encoding.UTF8.GetBytes(Create<string>());
        }
    }
}
