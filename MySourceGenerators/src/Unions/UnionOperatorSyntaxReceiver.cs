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
            /// true
            /// </summary>
            True,

            /// <summary>
            /// false
            /// </summary>
            False,

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
            /// x & y
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
            /// x << y
            /// </summary>
            LeftShift,

            /// <summary>
            /// x >> y
            /// </summary>
            RightShift,

            /// <summary>
            /// x == y
            /// </summary>
            Equality,

            /// <summary>
            /// x != y
            /// </summary>
            Inequality,

            /// <summary>
            /// x < y
            /// </summary>
            LessThan,

            /// <summary>
            /// x > y
            /// </summary>
            GreaterThan,

            /// <summary>
            /// x <= y
            /// </summary>
            LessThanOrEqual,

            /// <summary>
            /// x >= y
            /// </summary>
            GreaterThanOrEqual
        }

        public UnionDefinition UnionDefinition { get; private set; }

        public HashSet<Op> Operators { get; } = new HashSet<Op>();

        public static bool TryCreate(GeneratorSyntaxContext context, StructDeclarationSyntax dec, out UnionOperatorDefinition def)
        {
            if (!UnionDefinition.TryCreate(context, dec, out var unionDef))
            {
                def = default;
                return false;
            }

            AttributeSyntax attribute = null;

            foreach (var attribList in dec.AttributeLists)
            {
                foreach (var attrib in attribList.Attributes)
                {
                    var name = attrib.Name.ToString();

                    if (string.Equals(name, "UnionOperator") ||
                        string.Equals(name, "UnionOperatorAttribute"))
                    {
                        attribute = attrib;
                        break;
                    }
                }
            }

            if (attribute == null ||
                attribute.ArgumentList == null ||
                attribute.ArgumentList.Arguments.Count < 1)
            {
                def = default;
                return false;
            }

            var operators = new List<Op>();

            foreach (var arg in attribute.ArgumentList.Arguments)
            {
                if (!(arg.Expression is MemberAccessExpressionSyntax memberAccess) ||
                    !string.Equals(memberAccess.Expression.ToString(), nameof(Op)))
                    continue;

                if (Enum.TryParse<Op>(memberAccess.Name.ToString(), true, out var value))
                {
                    operators.Add(value);
                }
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