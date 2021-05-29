using System;
using System.Collections.Generic;

namespace MyLibrary.Unions
{
    public enum Op
    {
        /// <summary>
        /// +x
        /// </summary>
        UnaryPlus,

        /// <summary>
        /// -x
        /// </summary>
        UnaryMinus,

        /// <summary>
        /// !x
        /// </summary>
        LogicalNegation,

        /// <summary>
        /// ~x
        /// </summary>
        BitwiseComplement,

        /// <summary>
        /// ++x
        /// </summary>
        Increment,

        /// <summary>
        /// --x
        /// </summary>
        Decrement,

        /// <summary>
        /// x + y
        /// </summary>
        Addition,

        /// <summary>
        /// x - y
        /// </summary>
        Subtraction,

        /// <summary>
        /// x * y
        /// </summary>
        Multiplication,

        /// <summary>
        /// x / y
        /// </summary>
        Division,

        /// <summary>
        /// x % y
        /// </summary>
        Remainder,

        /// <summary>
        /// x &amp; y
        /// </summary>
        LogicalAnd,

        /// <summary>
        /// x | y
        /// </summary>
        LogicalOr,

        /// <summary>
        /// x ^ y
        /// </summary>
        LogicalExclusiveOr,

        /// <summary>
        /// x &lt;&lt; y
        /// </summary>
        LeftShift,

        /// <summary>
        /// x &gt;&gt; y
        /// </summary>
        RightShift,

        /// <summary>
        /// x &lt; y and x &gt; y
        /// </summary>
        LessAndGreaterThan,

        /// <summary>
        /// x &lt;= y and x &gt;= y
        /// </summary>
        LessAndGreaterThanOrEqual
    }

    public enum OperandTypeHandling
    {
        /// <summary>
        /// Allow implicit casting if operand types are different.
        /// </summary>
        Implicit,

        /// <summary>
        /// <para>Operands must be of the same type. Otherwise use the <see cref=""InvalidValueAccess""/> argument to decide the behaviour of the operator.</para>
        /// <para>If no exception is thrown, a default value based on the left operand will be returned.</para>
        /// </summary>
        Strict
    }

    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    public sealed class UnionOperatorAttribute : Attribute
    {
        public HashSet<Op> Operators { get; } = new HashSet<Op>();

        public OperandTypeHandling OperandTypeHandling { get; }

        public UnionOperatorAttribute(Op operator1, params Op[] operators)
        {
            this.OperandTypeHandling = OperandTypeHandling.Implicit;

            this.Operators.Add(operator1);

            foreach (var op in operators)
            {
                this.Operators.Add(op);
            }
        }

        public UnionOperatorAttribute(OperandTypeHandling operandTypeHandling, params Op[] operators)
        {
            this.OperandTypeHandling = operandTypeHandling;

            foreach (var op in operators)
            {
                this.Operators.Add(op);
            }
        }
    }
}