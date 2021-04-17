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
            unsafe
            {
                Write<ByteNumber>();
                Write<ShortNumber>();
                Write<IntNumber>();
                Write<LongNumber>();
                Write<IntegerNumber>();
                Write<RealNumber>();
                Write<Number>();

                void Write<T>() where T : unmanaged
                {
                    Console.WriteLine($"Size of {typeof(T).Name} = {sizeof(T)}");
                }
            }
        }
    }

    [Union(typeof((int Int, float Float)))]
    public partial struct IntFloat { }

    [Union(typeof((sbyte, byte)))]
    public readonly partial struct ByteNumber { }

    [Union(typeof((short, ushort)))]
    public readonly partial struct ShortNumber { }

    [Union(typeof((int, uint)))]
    public readonly partial struct IntNumber { }

    [Union(typeof((long, ulong)))]
    public readonly partial struct LongNumber { }

    [Union(typeof((sbyte, byte, short, ushort, int, uint, long, ulong)))]
    public readonly partial struct IntegerNumber { }

    [Union(typeof((float, double)))]
    public readonly partial struct RealNumber { }

    [Union(typeof((sbyte, byte, short, ushort, int, uint, long, ulong, float, double)))]
    public readonly partial struct Number { }
}
