using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using ReactiveUI;

namespace AllOverIt.ReactiveUI.Tests
{
    public class ReactiveProxyFixture : FixtureBase
    {
        private class PropertyDummy
        {
            internal class Comparer : IEqualityComparer<PropertyDummy>
            {
                internal static readonly IEqualityComparer<PropertyDummy> Instance = new Comparer();

                public bool Equals(PropertyDummy x, PropertyDummy y)
                {
                    if (x is null && y is null)
                    {
                        return true;
                    }

                    if (x is null || y is null)
                    {
                        return false;
                    }

                    return x.Equals(y);
                }

                public int GetHashCode(PropertyDummy dummy)
                {
                    return dummy.Value.GetHashCode();
                }
            }

            public string Value { get; set; }
        }

        // A non-observable model
        private class ModelDummy
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public PropertyDummy PropertyDummy { get; set; }
        }

        // Create a proxy to be used as an observable
        // The property setters are using different overloads of SetAndRaiseIfChanged
        private class ModelProxyDummy : ReactiveProxy<ModelDummy>
        {
            public ModelProxyDummy(ModelDummy dummy)
                : base(dummy)
            {
            }

            public string FirstName
            {
                get => Model.FirstName;
                set
                {
                    // Could alternatively use the return value to handle the equivalent of the onChanged action
                    FirstNameWasChanged = RaiseAndSetIfChanged(Model.FirstName, value, (model, newValue) => model.FirstName = newValue,
                        () =>
                        {
                            this.RaisePropertyChanging(nameof(FullName));
                        },
                        () =>
                        {
                            this.RaisePropertyChanged(nameof(FullName));
                        });
                }
            }

            public string LastName
            {
                get => Model.LastName;
                set
                {
                    LastNameWasChanged = RaiseAndSetIfChanged(Model.LastName, value, (model, newValue) => model.LastName = newValue);
                }
            }

            public int Age
            {
                get => Model.Age;
                set
                {
                    AgeWasChanged = RaiseAndSetIfChanged(Model.Age, value, (model, newValue) => model.Age = newValue, EqualityComparer<int>.Default, null, null);
                }
            }

            public int BadAge
            {
                get => Model.Age;
                set
                {
                    AgeWasChanged = RaiseAndSetIfChanged(Model.Age, value, null);
                }
            }

            public PropertyDummy PropertyDummy
            {
                get => Model.PropertyDummy;
                set
                {
                    PropertyDummyWasChanged = RaiseAndSetIfChanged(Model.PropertyDummy, value, (model, newValue) => model.PropertyDummy = newValue,
                        PropertyDummy.Comparer.Instance, null, null);
                }
            }

            public string FullName => $"{FirstName} {LastName}";
            public bool FirstNameWasChanged { get; private set; }
            public bool LastNameWasChanged { get; private set; }
            public bool AgeWasChanged { get; private set; }
            public bool PropertyDummyWasChanged { get; private set; }

            public void RaisePropertyChangingByName(string propertyName)
            {
                this.RaisePropertyChanging(propertyName);
            }

            public void RaisePropertyChangedByName(string propertyName)
            {
                this.RaisePropertyChanged(propertyName);
            }
        }

        private ModelDummy _model = new();
        private ModelProxyDummy _modelProxy;

        public ReactiveProxyFixture()
        {
            // Not using Create<ModelDummy>() as there are bool flags we need initialized to false
            _model.FirstName = Create<string>();
            _model.LastName = Create<string>();
            _model.Age = Create<int>();
            _model.PropertyDummy = Create<PropertyDummy>();

            _modelProxy = new ModelProxyDummy(_model);
        }

        public class Constructor : ReactiveProxyFixture
        {
            [Fact]
            public void Should_Throw_When_Model_Null()
            {
                Invoking(() =>
                {
                    _ = new ModelProxyDummy(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("model");
            }
        }

        public class SetAndRaiseIfChanged : ReactiveProxyFixture
        {
            [Fact]
            public void Should_Throw_When_SetValue_Null()
            {
                Invoking(() =>
                {
                    _modelProxy.BadAge = Create<int>();
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("setValue");
            }

            [Fact]
            public void Should_Raise_PropertyChanging()
            {
                var propertiesRaised = new List<string>();

                _modelProxy.PropertyChanging += (sender, eventArgs) =>
                {
                    propertiesRaised.Add(eventArgs.PropertyName);
                };

                _modelProxy.FirstName = Create<string>();
                _modelProxy.LastName = Create<string>();
                _modelProxy.Age = Create<int>();
                _modelProxy.PropertyDummy = Create<PropertyDummy>();

                propertiesRaised.Should().BeEquivalentTo(new[]
                {
                    nameof(ModelProxyDummy.FirstName),
                    nameof(ModelProxyDummy.FullName),
                    nameof(ModelProxyDummy.LastName),
                    nameof(ModelProxyDummy.Age),
                    nameof(ModelProxyDummy.PropertyDummy)
                });

                _modelProxy.FirstNameWasChanged.Should().BeTrue();
                _modelProxy.LastNameWasChanged.Should().BeTrue();
                _modelProxy.AgeWasChanged.Should().BeTrue();
                _modelProxy.PropertyDummyWasChanged.Should().BeTrue();

                _modelProxy.Should().BeEquivalentTo(_model);
            }

            [Fact]
            public void Should_Raise_PropertyChanged()
            {
                var propertiesRaised = new List<string>();

                _modelProxy.PropertyChanged += (sender, eventArgs) =>
                {
                    propertiesRaised.Add(eventArgs.PropertyName);
                };

                _modelProxy.FirstName = Create<string>();
                _modelProxy.LastName = Create<string>();
                _modelProxy.Age = Create<int>();
                _modelProxy.PropertyDummy = Create<PropertyDummy>();

                propertiesRaised.Should().BeEquivalentTo(new[]
                {
                    nameof(ModelProxyDummy.FirstName),
                    nameof(ModelProxyDummy.FullName),
                    nameof(ModelProxyDummy.LastName),
                    nameof(ModelProxyDummy.Age),
                    nameof(ModelProxyDummy.PropertyDummy)
                });

                _modelProxy.FirstNameWasChanged.Should().BeTrue();
                _modelProxy.LastNameWasChanged.Should().BeTrue();
                _modelProxy.AgeWasChanged.Should().BeTrue();
                _modelProxy.PropertyDummyWasChanged.Should().BeTrue();

                _modelProxy.Should().BeEquivalentTo(_model);
            }

            [Fact]
            public void Should_Not_Raise_PropertyChanging()
            {
                var raised = false;

                _modelProxy.PropertyChanging += (sender, eventArgs) =>
                {
                    raised = true;
                };

                var currentFirstName = _modelProxy.FirstName;
                var currentLastName = _modelProxy.LastName;
                var currentAge = _modelProxy.Age;
                var currentPropertyDummy = _modelProxy.PropertyDummy;

                _modelProxy.FirstName = currentFirstName;
                _modelProxy.LastName = currentLastName;
                _modelProxy.Age = currentAge;
                _modelProxy.PropertyDummy = currentPropertyDummy;

                raised.Should().BeFalse();

                _modelProxy.FirstNameWasChanged.Should().BeFalse();
                _modelProxy.LastNameWasChanged.Should().BeFalse();
                _modelProxy.AgeWasChanged.Should().BeFalse();
                _modelProxy.AgeWasChanged.Should().BeFalse();

                _modelProxy.Should().BeEquivalentTo(_model);
            }

            [Fact]
            public void Should_Not_Raise_PropertyChanged()
            {
                var raised = false;

                _modelProxy.PropertyChanged += (sender, eventArgs) =>
                {
                    raised = true;
                };

                var currentFirstName = _modelProxy.FirstName;
                var currentLastName = _modelProxy.LastName;
                var currentAge = _modelProxy.Age;
                var currentPropertyDummy = _modelProxy.PropertyDummy;

                _modelProxy.FirstName = currentFirstName;
                _modelProxy.LastName = currentLastName;
                _modelProxy.Age = currentAge;
                _modelProxy.PropertyDummy = currentPropertyDummy;

                raised.Should().BeFalse();

                _modelProxy.FirstNameWasChanged.Should().BeFalse();
                _modelProxy.LastNameWasChanged.Should().BeFalse();
                _modelProxy.AgeWasChanged.Should().BeFalse();
                _modelProxy.AgeWasChanged.Should().BeFalse();

                _modelProxy.Should().BeEquivalentTo(_model);
            }
        }

        public class RaisePropertyChanging : ReactiveProxyFixture
        {
            [Fact]
            public void Should_Raise_PropertyChanging_Name()
            {
                string actual = null;

                _modelProxy.PropertyChanging += (sender, eventArgs) =>
                {
                    actual = eventArgs.PropertyName;
                };

                var expected = Create<string>();

                _modelProxy.RaisePropertyChangingByName(expected);

                actual.Should().Be(expected);

                _modelProxy.Should().BeEquivalentTo(_model);
            }
        }

        public class RaisePropertyChanged : ReactiveProxyFixture
        {
            [Fact]
            public void Should_Raise_PropertyChanged_Name()
            {
                string actual = null;

                _modelProxy.PropertyChanged += (sender, eventArgs) =>
                {
                    actual = eventArgs.PropertyName;
                };

                var expected = Create<string>();

                _modelProxy.RaisePropertyChangedByName(expected);

                actual.Should().Be(expected);

                _modelProxy.Should().BeEquivalentTo(_model);
            }
        }
    }
}