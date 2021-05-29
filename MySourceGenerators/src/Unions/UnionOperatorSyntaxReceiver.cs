using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace MySourceGenerators.Unions
{
    public class UnionOperatorSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<UnionOperatorDefinition> Definitions { get; } = new List<UnionOperatorDefinition>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (!(context.Node is StructDeclarationSyntax dec) ||
                dec.AttributeLists.Count <= 0)
                return;

            if (UnionOperatorDefinition.TryCreate(context, dec, out var def))
                this.Definitions.Add(def);
        }
    }

    public class UnionOperatorDefinition
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
            LessAndGreaterThanOrEqual,
        }

        public enum OperandTypeHandlingStrategy
        {
            /// <summary>
            /// Allow implicit casting if operand types are different.
            /// </summary>
            Implicit,

            /// <summary>
            /// <para>Operands must be of the same type. Otherwise use the <see cref="UnionDefinition.InvalidValueAccess"/> argument to decide what should be returned by the operator.</para>
            /// <para>If no exception is thrown, a default value based on left operand will be returned.</para>
            /// </summary>
            Strict
        }

        public readonly struct Operator
        {
            public readonly Op Value;
            public readonly OperandTypeHandlingStrategy OperandTypeHandling;

            public Operator(Op value, OperandTypeHandlingStrategy typeHandling)
            {
                this.Value = value;
                this.OperandTypeHandling = typeHandling;
            }
        }

        public UnionDefinition UnionDefinition { get; private set; }

        public List<Operator> Operators { get; } = new List<Operator>();

        public static bool TryCreate(GeneratorSyntaxContext context, StructDeclarationSyntax dec, out UnionOperatorDefinition def)
        {
            if (!UnionDefinition.TryCreate(context, dec, out var unionDef))
            {
                def = default;
                return false;
            }

            var attributes = new List<AttributeSyntax>();

            foreach (var attribList in dec.AttributeLists)
            {
                foreach (var attrib in attribList.Attributes)
                {
                    var name = attrib.Name.ToString();

                    if (string.Equals(name, "UnionOperator") ||
                        string.Equals(name, "UnionOperatorAttribute"))
                    {
                        attributes.Add(attrib);
                        break;
                    }
                }
            }

            if (attributes.Count < 1)
            {
                def = default;
                return false;
            }

            var operators = new List<Operator>();
            var operatorSet = new HashSet<Op>();
            var operatorList = new List<Op>();

            foreach (var attribute in attributes)
            {
                var operandTypeHandling = OperandTypeHandlingStrategy.Implicit;

                foreach (var arg in attribute.ArgumentList.Arguments)
                {
                    if (!(arg.Expression is MemberAccessExpressionSyntax memberAccess))
                        continue;

                    var memberName = memberAccess.Expression.ToString();

                    if (string.Equals(memberName, nameof(Op)))
                    {
                        if (Enum.TryParse<Op>(memberAccess.Name.ToString(), true, out var value) &&
                            !operatorSet.Contains(value))
                        {
                            operatorSet.Add(value);
                            operatorList.Add(value);
                        }
                    }
                    else if (string.Equals(memberName, "OperandTypeHandling"))
                    {
                        if (Enum.TryParse<OperandTypeHandlingStrategy>(memberAccess.Name.ToString(), true, out var value))
                        {
                            operandTypeHandling = value;
                        }
                    }
                }

                foreach (var op in operatorList)
                {
                    operators.Add(new Operator(op, operandTypeHandling));
                }

                operatorList.Clear();
            }

            if (operators.Count < 1)
            {
                def = default;
                return false;
            }

            def = new UnionOperatorDefinition {
                UnionDefinition = unionDef
            };

            for (var i = 0; i < operators.Count; i++)
            {
                def.Operators.Add(operators[i]);
            }

            return true;
        }
    }
}