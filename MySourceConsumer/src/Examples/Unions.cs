using MyLibrary.Unions;

namespace MySourceConsumer.Unions
{
    [Union(typeof((sbyte, byte)), InvalidValueAccess.ThrowException)]
    [UnionOperator(Op.UnaryPlus, Op.UnaryMinus, Op.BitwiseComplement, Op.Increment, Op.Decrement)]
    [UnionOperator(Op.Addition, Op.Subtraction, Op.LessAndGreaterThan, Op.LessAndGreaterThanOrEqual)]
    public readonly partial struct ByteNumber { }

    [Union(typeof((short, ushort)), InvalidValueAccess.ThrowException)]
    [UnionOperator(Op.UnaryPlus, Op.UnaryMinus, Op.BitwiseComplement, Op.Increment, Op.Decrement)]
    [UnionOperator(Op.Addition, Op.Subtraction, Op.LessAndGreaterThan, Op.LessAndGreaterThanOrEqual)]
    public readonly partial struct ShortNumber { }

    [Union(typeof((int, uint)), InvalidValueAccess.ThrowException)]
    [UnionOperator(Op.UnaryPlus, Op.UnaryMinus, Op.BitwiseComplement, Op.Increment, Op.Decrement)]
    [UnionOperator(Op.Addition, Op.Subtraction, Op.LessAndGreaterThan, Op.LessAndGreaterThanOrEqual)]
    public readonly partial struct IntNumber { }

    [Union(typeof((long, ulong)), InvalidValueAccess.ThrowException)]
    [UnionOperator(Op.UnaryPlus, Op.UnaryMinus, Op.BitwiseComplement, Op.Increment, Op.Decrement)]
    [UnionOperator(Op.Addition, Op.Subtraction, Op.LessAndGreaterThan, Op.LessAndGreaterThanOrEqual)]
    public readonly partial struct LongNumber { }

    [Union(typeof((sbyte, byte, short, ushort, int, uint, long, ulong)), InvalidValueAccess.ThrowException)]
    [UnionOperator(Op.UnaryPlus, Op.UnaryMinus, Op.BitwiseComplement, Op.Increment, Op.Decrement)]
    [UnionOperator(Op.Addition, Op.Subtraction, Op.LessAndGreaterThan, Op.LessAndGreaterThanOrEqual)]
    public readonly partial struct IntegerNumber { }

    [Union(typeof((float, double)), InvalidValueAccess.ThrowException)]
    [UnionOperator(Op.UnaryPlus, Op.UnaryMinus, Op.Increment, Op.Decrement)]
    [UnionOperator(Op.Addition, Op.Subtraction, Op.LessAndGreaterThan, Op.LessAndGreaterThanOrEqual)]
    public readonly partial struct RealNumber { }
}
