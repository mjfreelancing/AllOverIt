using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reactive.Extensions;
using FluentAssertions;
using System;

namespace AllOverIt.Reactive.Tests.Extensions
{
    public class PropertyNotifyExtensionsFixture : FixtureBase
    {
        private class ModelDummy : ObservableObject
        {
            private string _name;
            public string Name
            {
                get => _name;
                set
                {
                    _ = RaiseAndSetIfChanged(ref _name, value);
                }
            }
        }

        private readonly ModelDummy _model = new();

        public PropertyNotifyExtensionsFixture()
        {
            _model.Name = Create<string>();
        }

        public class WhenPropertyChanging : PropertyNotifyExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Null()
            {
                Invoking(() =>
                {
                    PropertyNotifyExtensions.WhenPropertyChanging<ModelDummy, string>(null, model => model.Name);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("source");
            }

            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() =>
                {
                    PropertyNotifyExtensions.WhenPropertyChanging<ModelDummy, string>(_model, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Notify_Changing()
            {
                var expected = _model.Name;
                var actual = string.Empty;

                var subscription = _model
                    .WhenPropertyChanging(model => model.Name)
                    .Subscribe(name => actual = _model.Name);

                using (subscription)
                {
                    _model.Name = Create<string>();

                    actual.Should().Be(expected);
                }
            }
        }

        public class WhenPropertyChanged : PropertyNotifyExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Source_Null()
            {
                Invoking(() =>
                {
                    PropertyNotifyExtensions.WhenPropertyChanged<ModelDummy, string>(null, model => model.Name);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("source");
            }

            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() =>
                {
                    PropertyNotifyExtensions.WhenPropertyChanged<ModelDummy, string>(_model, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Notify_Changed()
            {
                var expected = Create<string>();
                var actual = string.Empty;

                var subscription = _model
                    .WhenPropertyChanged(model => model.Name)
                    .Subscribe(name => actual = _model.Name);

                using (subscription)
                {
                    _model.Name = expected;

                    actual.Should().Be(expected);
                }
            }
        }
    }
}