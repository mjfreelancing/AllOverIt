namespace GenericCacheDemo.Models
{
    internal sealed class InfoModel
    {
        public int Value1 { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Part of the demo")]
        private int Value2 { get; }

        internal string Value3 { get; }
        public static int Value4 { get; }
        internal static double Value5 { get; }
    }
}