using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace MySourceGenerators.Unions
{
    [Generator]
    public class UnionOperatorGenerator : ISourceGenerator
    {
        private const string _attributeText = $@"
using System;
using System.Collections.Generic;

namespace {GeneratorConfig.Namespace}.Unions
{{
    public enum Op
    {{
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
    }}

    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class UnionOperatorAttribute : Attribute
    {{
        public HashSet<Op> Operators {{ get; }} = new HashSet<Op>();

        public UnionOperatorAttribute(Op operator1, params Op[] operators)
        {{
            this.Operators.Add(operator1);

            foreach (var op in operators)
            {{
                this.Operators.Add(op);
            }}
        }}
    }}
}}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization((i) => i.AddSource("UnionOperatorAttribute", _attributeText));

            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new UnionOperatorSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is UnionOperatorSyntaxReceiver receiver) ||
                receiver.Definitions.Count <= 0)
                return;

            foreach (var def in receiver.Definitions)
            {
                Execute(context, def);
            }
        }

        public void Execute(GeneratorExecutionContext context, UnionOperatorDefinition def)
        {
            var usingBuilder = new StringBuilder($@"using {GeneratorConfig.Namespace};
");

            var unionBuilder = new StringBuilder($@"
namespace {def.UnionDefinition.Namespace}
{{
    using System.Runtime.InteropServices;
");

            unionBuilder.Append($@"
    public partial struct {def.UnionDefinition.Name}
    {{");

            foreach (var op in def.Operators)
            {
                switch (op)
                {
                    case UnionOperatorDefinition.Op.UnaryPlus:
                        break;
                    case UnionOperatorDefinition.Op.UnaryMinus:
                        break;
                    case UnionOperatorDefinition.Op.LogicalNegation:
                        break;
                    case UnionOperatorDefinition.Op.BitwiseComplement:
                        break;
                    case UnionOperatorDefinition.Op.Increment:
                        break;
                    case UnionOperatorDefinition.Op.Decrement:
                        break;
                    case UnionOperatorDefinition.Op.True:
                        break;
                    case UnionOperatorDefinition.Op.False:
                        break;
                    case UnionOperatorDefinition.Op.Addition:
                        break;
                    case UnionOperatorDefinition.Op.Subtraction:
                        break;
                    case UnionOperatorDefinition.Op.Multiplication:
                        break;
                    case UnionOperatorDefinition.Op.Division:
                        break;
                    case UnionOperatorDefinition.Op.Remainder:
                        break;
                    case UnionOperatorDefinition.Op.LogicalAnd:
                        break;
                    case UnionOperatorDefinition.Op.LogicalOr:
                        break;
                    case UnionOperatorDefinition.Op.LogicalExclusiveOr:
                        break;
                    case UnionOperatorDefinition.Op.LeftShift:
                        break;
                    case UnionOperatorDefinition.Op.RightShift:
                        break;
                    case UnionOperatorDefinition.Op.Equality:
                        break;
                    case UnionOperatorDefinition.Op.Inequality:
                        break;
                    case UnionOperatorDefinition.Op.LessThan:
                        break;
                    case UnionOperatorDefinition.Op.GreaterThan:
                        break;
                    case UnionOperatorDefinition.Op.LessThanOrEqual:
                        break;
                    case UnionOperatorDefinition.Op.GreaterThanOrEqual:
                        break;
                }
            }

            unionBuilder.Append(@"
    }
}");
            usingBuilder.AppendLine(unionBuilder.ToString());
            context.AddSource($"Union_{def.UnionDefinition.Name}.Operators.cs", SourceText.From(usingBuilder.ToString(), Encoding.UTF8));
        }
    }
}