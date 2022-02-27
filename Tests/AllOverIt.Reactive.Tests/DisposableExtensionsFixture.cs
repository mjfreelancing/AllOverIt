using AllOverIt.Fixture;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using AllOverIt.Reactive.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Reactive.Tests
{
    public class DisposableExtensionsFixture : FixtureBase
    {
        private class DummyDisposable : IDisposable
        {
            private readonly Guid _guid;

            public DummyDisposable(Guid guid)
            {
                _guid = guid;
            }

            public void Dispose()
            {
                if (!Counter.ContainsKey(_guid))
                {
                    Counter[_guid] = 1;
                }
                else
                {
                    Counter[_guid]++;
                }
            }
        }

        private static readonly IDictionary<Guid, int> Counter = new Dictionary<Guid, int>();

        public class DisposeUsing : DisposableExtensionsFixture
        {
            [Fact]
            public void Should_Dispose_Disposables()
            {
                var guid = Guid.NewGuid();

                using (var disposables = new CompositeDisposable())
                {
                    DisposableExtensions.DisposeUsing(new DummyDisposable(guid), disposables);
                    DisposableExtensions.DisposeUsing(new DummyDisposable(guid), disposables);
                    DisposableExtensions.DisposeUsing(new DummyDisposable(guid), disposables);
                }

                Counter[guid].Should().Be(3);
            }
        }
    }
}