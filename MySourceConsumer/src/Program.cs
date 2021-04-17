using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using MyLibrary;

namespace MySourceConsumer
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            var vIF = new IntFloat();
            vIF.Float = 5f;
        }
    }

    [Union(typeof((int Int, float Float)))]
    public partial struct IntFloat { }
}

namespace Examples
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public partial struct Union
    {
        public enum Type : byte
        {
            Int,
            Float,
            Long,
        }

        [FieldOffset(0)]
        public readonly Type ValueType;

        [FieldOffset(1)]
        public readonly int Int;

        [FieldOffset(1)]
        public readonly float Float;

        [FieldOffset(1)]
        public readonly long Long;

        public Union(int value)
        {
            this.ValueType = Type.Int;
            this.Float = default;
            this.Long = default;
            this.Int = value;
        }

        public Union(float value)
        {
            this.ValueType = Type.Float;
            this.Int = default;
            this.Long = default;
            this.Float = value;
        }

        public Union(long value)
        {
            this.ValueType = Type.Long;
            this.Int = default;
            this.Float = default;
            this.Long = value;
        }

        public static implicit operator Union(int value)
            => new(value);

        public static implicit operator Union(float value)
            => new(value);

        public static implicit operator Union(long value)
            => new(value);

        public static implicit operator int(in Union value)
            => value.Int;

        public static implicit operator float(in Union value)
            => value.Float;

        public static implicit operator long(in Union value)
            => value.Long;
    }
}