using System;

namespace RoslynPad.Runtime
{
    public static class ObjectExtensions
    {
        public static T Dump<T>(this T o)
        {
            Dumped?.Invoke(o, DumpTarget.Text);
            return o;
        }

        public static T DumpToPropertyGrid<T>(this T o)
        {
            Dumped?.Invoke(o, DumpTarget.PropertyGrid);
            return o;
        }

        public static event Action<object, DumpTarget> Dumped;
    }

    public enum DumpTarget
    {
        Text,
        PropertyGrid,
    }
}
