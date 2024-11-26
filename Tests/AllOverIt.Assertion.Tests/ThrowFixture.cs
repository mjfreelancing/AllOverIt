using AllOverIt.Fixture;

namespace AllOverIt.Assertion.Tests
{
    public partial class ThrowFixture : FixtureBase
    {
        private class Exception1 : Exception
        {
            public string Arg1 { get; }

            public Exception1(string arg1)
                : base($"{arg1}")
            {
                Arg1 = arg1;
            }
        }

        private class Exception2 : Exception
        {
            public string Arg1 { get; }
            public bool Arg2 { get; }

            public Exception2(string arg1, bool arg2)
                : base($"{arg1}{arg2}")
            {
                Arg1 = arg1;
                Arg2 = arg2;
            }
        }

        private class Exception3 : Exception
        {
            public string Arg1 { get; }
            public bool Arg2 { get; }
            public int Arg3 { get; }

            public Exception3(string arg1, bool arg2, int arg3)
                : base($"{arg1}{arg2}{arg3}")
            {
                Arg1 = arg1;
                Arg2 = arg2;
                Arg3 = arg3;
            }
        }

        private class Exception4 : Exception
        {
            public string Arg1 { get; }
            public bool Arg2 { get; }
            public int Arg3 { get; }
            public string Arg4 { get; }

            public Exception4(string arg1, bool arg2, int arg3, string arg4)
                : base($"{arg1}{arg2}{arg3}{arg4}")
            {
                Arg1 = arg1;
                Arg2 = arg2;
                Arg3 = arg3;
                Arg4 = arg4;
            }
        }
    }
}
