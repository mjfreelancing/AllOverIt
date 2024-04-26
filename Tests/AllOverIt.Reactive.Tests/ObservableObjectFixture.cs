using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections.Generic;

namespace AllOverIt.Reactive.Tests
{
    public class ObservableObjectFixture : FixtureBase
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

        // The properties are using different overloads of SetAndRaiseIfChanged
        private class ModelDummy : ObservableObject
        {
            private string _firstName;
            public string FirstName
            {
                get => _firstName;
                set
                {
                    // Could alternatively use the return value to handle the equivalent of the onChanged action
                    FirstNameWasChanged = RaiseAndSetIfChanged(ref _firstName, value,
                        () =>
                        {
                            RaisePropertyChanging(nameof(FullName));
                        },
                        () =>
                        {
                            RaisePropertyChanged(nameof(FullName));
                        });
                }
            }

            private string _lastName;
            public string LastName
            {
                get => _lastName;
                set
                {
                    LastNameWasChanged = RaiseAndSetIfChanged(ref _lastName, value);
                }
            }

            private int _age;
            public int Age
            {
                get => _age;
                set
                {
                    AgeWasChanged = RaiseAndSetIfChanged(ref _age, value, EqualityComparer<int>.Default, null, null);
                }
            }

            private PropertyDummy _propertyDummy;
            public PropertyDummy PropertyDummy
            {
                get => _propertyDummy;
                set
                {
                    PropertyDummyWasChanged = RaiseAndSetIfChanged(ref _propertyDummy, value, PropertyDummy.Comparer.Instance, null, null);
                }
            }

            public string FullName => $"{FirstName} {LastName}";
            public bool FirstNameWasChanged { get; private set; }
            public bool LastNameWasChanged { get; private set; }
            public bool AgeWasChanged { get; private set; }
            public bool PropertyDummyWasChanged { get; private set; }

            public void RaisePropertyChangingByName(string propertyName)
            {
                RaisePropertyChanging(propertyName);
            }

            public void RaisePropertyChangedByName(string propertyName)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        private ModelDummy _model = new();

        public ObservableObjectFixture()
        {
            // Not using Create<ModelDummy>() as there are bool flags we need initialized to false
            _model.FirstName = Create<string>();
            _model.LastName = Create<string>();
            _model.Age = Create<int>();
            _model.PropertyDummy = Create<PropertyDummy>();
        }

        public class SetAndRaiseIfChanged : ObservableObjectFixture
        {
            [Fact]
            public void Should_Raise_PropertyChanging()
            {
                var propertiesRaised = new List<string>();

                _model.PropertyChanging += (sender, eventArgs) =>
                {
                    propertiesRaised.Add(eventArgs.PropertyName);
                };

                _model.FirstName = Create<string>();
                _model.LastName = Create<string>();
                _model.Age = Create<int>();
                _model.PropertyDummy = Create<PropertyDummy>();

                propertiesRaised.Should().BeEquivalentTo(new[]
                {
                    nameof(ModelDummy.FirstName),
                    nameof(ModelDummy.FullName),
                    nameof(ModelDummy.LastName),
                    nameof(ModelDummy.Age),
                    nameof(ModelDummy.PropertyDummy)
                });

                _model.FirstNameWasChanged.Should().BeTrue();
                _model.LastNameWasChanged.Should().BeTrue();
                _model.AgeWasChanged.Should().BeTrue();
                _model.PropertyDummyWasChanged.Should().BeTrue();
            }

            [Fact]
            public void Should_Raise_PropertyChanged()
            {
                var propertiesRaised = new List<string>();

                _model.PropertyChanged += (sender, eventArgs) =>
                {
                    propertiesRaised.Add(eventArgs.PropertyName);
                };

                _model.FirstName = Create<string>();
                _model.LastName = Create<string>();
                _model.Age = Create<int>();
                _model.PropertyDummy = Create<PropertyDummy>();

                propertiesRaised.Should().BeEquivalentTo(new[]
                {
                    nameof(ModelDummy.FirstName),
                    nameof(ModelDummy.FullName),
                    nameof(ModelDummy.LastName),
                    nameof(ModelDummy.Age),
                    nameof(ModelDummy.PropertyDummy)
                });

                _model.FirstNameWasChanged.Should().BeTrue();
                _model.LastNameWasChanged.Should().BeTrue();
                _model.AgeWasChanged.Should().BeTrue();
                _model.PropertyDummyWasChanged.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Raise_PropertyChanging()
            {
                var raised = false;

                _model.PropertyChanging += (sender, eventArgs) =>
                {
                    raised = true;
                };

                var currentFirstName = _model.FirstName;
                var currentLastName = _model.LastName;
                var currentAge = _model.Age;
                var currentPropertyDummy = _model.PropertyDummy;

                _model.FirstName = currentFirstName;
                _model.LastName = currentLastName;
                _model.Age = currentAge;
                _model.PropertyDummy = currentPropertyDummy;

                raised.Should().BeFalse();

                _model.FirstNameWasChanged.Should().BeFalse();
                _model.LastNameWasChanged.Should().BeFalse();
                _model.AgeWasChanged.Should().BeFalse();
                _model.AgeWasChanged.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Raise_PropertyChanged()
            {
                var raised = false;

                _model.PropertyChanged += (sender, eventArgs) =>
                {
                    raised = true;
                };

                var currentFirstName = _model.FirstName;
                var currentLastName = _model.LastName;
                var currentAge = _model.Age;
                var currentPropertyDummy = _model.PropertyDummy;

                _model.FirstName = currentFirstName;
                _model.LastName = currentLastName;
                _model.Age = currentAge;
                _model.PropertyDummy = currentPropertyDummy;

                raised.Should().BeFalse();

                _model.FirstNameWasChanged.Should().BeFalse();
                _model.LastNameWasChanged.Should().BeFalse();
                _model.AgeWasChanged.Should().BeFalse();
                _model.AgeWasChanged.Should().BeFalse();
            }
        }

        public class RaisePropertyChanging : ObservableObjectFixture
        {
            [Fact]
            public void Should_Raise_PropertyChanging_Name()
            {
                string actual = null;

                _model.PropertyChanging += (sender, eventArgs) =>
                {
                    actual = eventArgs.PropertyName;
                };

                var expected = Create<string>();

                _model.RaisePropertyChangingByName(expected);

                actual.Should().Be(expected);
            }
        }

        public class RaisePropertyChanged : ObservableObjectFixture
        {
            [Fact]
            public void Should_Raise_PropertyChanged_Name()
            {
                string actual = null;

                _model.PropertyChanged += (sender, eventArgs) =>
                {
                    actual = eventArgs.PropertyName;
                };

                var expected = Create<string>();

                _model.RaisePropertyChangedByName(expected);

                actual.Should().Be(expected);
            }
        }
    }
}