using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AllOverIt.Mapping.Tests
{
    internal static class ObjectMapperTypes
    {
        internal enum DummyEnum
        {
            Value1,
            Value2
        }

        internal class DummySource1
        {
            public int Prop1 { get; set; }
            private int Prop2 { get; set; }
            public string Prop3 { get; set; }
            internal int Prop4 { get; set; }
            public int? Prop5 { get; set; }
            public int Prop6 { get; set; }
            public string Prop7a { get; set; }
            public int Prop8 { get; private set; }
            public IEnumerable<string> Prop9 { get; set; }
            public DummyEnum Prop12 { get; set; }
            public int Prop13 { get; set; }

            public DummySource1()
            {
                Prop2 = 10;
            }

            public int GetProp2()
            {
                return Prop2;
            }
        }

        internal class DummySource2 : DummySource1
        {
            public IReadOnlyCollection<string> Prop10 { get; set; }
            public IEnumerable<string> Prop11 { get; set; }
        }

        internal class DummyTarget
        {
            public float Prop1 { get; set; }        // This is an int on the source

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Part of the test")]
            private int Prop2 { get; set; }

            public string Prop3 { get; set; }
            internal int Prop4 { get; set; }
            public int Prop5 { get; set; }
            public int? Prop6 { get; set; }
            public string Prop7b { get; set; }
            public int Prop8 { get; private set; }
            public IEnumerable<string> Prop9 { get; set; }
            public IEnumerable<string> Prop10 { get; set; }
            public IReadOnlyCollection<string> Prop11 { get; set; }
            public int Prop12 { get; set; }
            public DummyEnum Prop13 { get; set; }
        }

        internal class DummyRootGrandChildSource
        {
            public int Prop2 { get; set; }
            public IEnumerable<int> Prop3 { get; set; }

            public DummyRootGrandChildSource()
            {
                Prop2 = 2;
                Prop3 = new[] { 1, 2, 3 };
            }
        }

        internal class DummyRootGrandChildTarget
        {
            public double? Prop2 { get; set; }
            public IEnumerable<double> Prop3 { get; set; }
        }

        internal class DummyRootChildSource
        {
            public int Prop1 { get; set; }
            public DummyRootGrandChildSource Prop2a { get; set; }
            public DummyRootGrandChildSource Prop2b { get; set; }

            public DummyRootChildSource()
            {
                Prop1 = 1;
                Prop2a = new();

                Prop2b = new DummyRootGrandChildSource
                {
                    Prop2 = Prop2a.Prop2 + 1,
                    Prop3 = new[] { 1, 2, 3 }.Concat(Prop2a.Prop3).ToList()
                };
            }
        }

        internal class DummyRootChildTarget
        {
            public double Prop1 { get; set; }
            public DummyRootGrandChildSource Prop2a { get; set; }
            public DummyRootGrandChildTarget Prop2b { get; set; }
        }

        internal class DummyRootParentSource
        {
            public DummyRootChildSource RootA { get; set; } = new DummyRootChildSource();       // Testing this initialization scenario
            public DummyRootChildSource RootB { get; set; }
            public DummyRootChildSource RootC { get; set; }

            public DummyRootParentSource()
            {
                RootA = new();

                RootB = new();
                RootB.Prop1 = RootA.Prop1 + 1;
                RootB.Prop2a.Prop2 = RootA.Prop2a.Prop2 + 2;
                RootB.Prop2a.Prop3 = RootA.Prop2a.Prop3.Concat(new[] { 4, 5, 6 });
                RootB.Prop2b.Prop2 = RootA.Prop2b.Prop2 + 3;
                RootB.Prop2b.Prop3 = RootA.Prop2b.Prop3.Concat(new[] { 7, 8, 9 });

                RootC = new();
                RootC.Prop1 = RootB.Prop1 + 1;
                RootC.Prop2a.Prop2 = RootB.Prop2a.Prop2 + 2;
                RootC.Prop2a.Prop3 = RootB.Prop2a.Prop3.Concat(new[] { 4, 5, 6 });
                RootC.Prop2b.Prop2 = RootB.Prop2b.Prop2 + 3;
                RootC.Prop2b.Prop3 = RootB.Prop2b.Prop3.Concat(new[] { 7, 8, 9 });
            }
        }

        internal class DummyRootParentTarget
        {
            public DummyRootChildSource RootA { get; set; }
            public DummyRootChildTarget RootB { get; set; }
            public DummyRootChildSource RootC { get; set; }
        }

        internal class DummyEnumerableRootSource
        {
            public IEnumerable<DummyRootParentSource> Prop1 { get; set; }
        }

        internal class DummyEnumerableRootTarget
        {
            public IEnumerable<DummyRootParentTarget> Prop1 { get; set; }
        }

        internal class DummyAbstractBase
        {
            public int Prop1 { get; init; }     // Note the 'init' and not 'set'
        }

        internal class DummyConcrete1 : DummyAbstractBase
        {
        }

        internal class DummyConcrete2 : DummyAbstractBase
        {
        }

        internal class DummySourceHost
        {
            public DummySource1 Prop1 { get; set; }
        }

        internal class DummyAbstractTarget
        {
            public DummyAbstractBase Prop1 { get; set; }
        }

        internal class DummyObservableCollectionHost
        {
            public ObservableCollection<string> Prop1 { get; set; }
        }

        internal class DummyDictionarySource
        {
            public IDictionary<string, int> Prop1 { get; set; }
            public IDictionary<string, int> Prop2 { get; set; }
        }

        internal class DummyDictionaryTarget
        {
            public IDictionary<string, int> Prop1 { get; set; }
            public IDictionary<string, double> Prop2 { get; set; }
        }

        internal class DummyEnumerableSource
        {
            public IEnumerable<int> Prop1 { get; set; }
        }

        internal class DummyArrayTarget
        {
            public int[] Prop1 { get; set; }
        }
    }
}