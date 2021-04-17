using System;
using System.Collections.Generic;
using System.Numerics;
using MyLibrary;

namespace MySourceConsumer
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            var vIF = new IntFloat();
            vIF.Float = 4f;
        }
    }

    [Union(typeof((int Int, float Float)))]
    public partial struct IntFloat { }
}

namespace Examples
{
    public struct Union
    {
        public int Int;
        public float Float;
    }
}