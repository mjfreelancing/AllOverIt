using AllOverIt.Fixture;
using System;
using System.Reflection;

namespace AllOverIt.Tests.Reflection
{
    public partial class ReflectionHelperFixture : FixtureBase
    {
        private class DummyBaseClass
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public virtual double Prop3 { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
#pragma warning disable CA1822 // Mark members as static
            public void Method1()
#pragma warning restore CA1822 // Mark members as static
            {
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Prevent CA1822")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "It's part of the test")]
#pragma warning disable CA1822 // Mark members as static
            private void Method2()
#pragma warning restore CA1822 // Mark members as static
            {
            }
        }

        private class DummySuperClass : DummyBaseClass
        {
            private readonly int _value;
            public override double Prop3 { get; set; }

            private long Prop4 { get; set; }
            
            public bool Prop5 { set { _ = value; } }    // automatic properties must have a getter

            public bool Prop6 { get; }

            public int Field1;

            public DummySuperClass()
            {
            }

            public DummySuperClass(int value)
            {
                _value = value;
            }

            public void Method3()
            {
                // Both to prevent IDE0051 (member unused)
                Prop4 = 1;
                Method4();
            }

            private void Method4()
            {
                Field1 = 1;     // Prevent CS0649 (Field is never assigned)
                _ = Prop4;      // Prevent IDE0052 (Prop4 access never used)
            }

            public int Method5()
            {
                return _value;
            }

            public int Method6(int arg)
            {
                return arg;
            }
        }

        private class DummyMemberInfo : MemberInfo
        {
            public override object[] GetCustomAttributes(bool inherit)
            {
                throw new NotImplementedException();
            }

            public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }

            public override bool IsDefined(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }

            public override Type DeclaringType { get; }

            // ReSharper disable once UnassignedGetOnlyAutoProperty
            public override MemberTypes MemberType { get; }

            public override string Name { get; }

            public override Type ReflectedType { get; }
        }
    }
}
