using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace MyLibrary.Unions.SourceGen
{
    [Generator]
    public class UnionOperatorGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
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
            var usingBuilder = new StringBuilder($@"using System;
using {GeneratorConfig.Namespace};
");

            var unionBuilder = new StringBuilder($@"
namespace {def.UnionDefinition.Namespace}
{{
    using System.Runtime.InteropServices;
");
            var unionName = def.UnionDefinition.Name;

            unionBuilder.Append($@"
    public partial struct {unionName}
    {{");

            foreach (var op in def.Operators)
            {
                var isStrict = op.OperandTypeHandling == OperandTypeHandling.Strict;

                switch (op.Value)
                {
                    case Op.UnaryPlus:
                        UnaryPlus(def, unionBuilder);
                        break;

                    case Op.UnaryMinus:
                        UnaryMinus(def, unionBuilder);
                        break;

                    case Op.LogicalNegation:
                        UnaryOperatorReturnBool(def, "!", unionBuilder);
                        break;

                    case Op.BitwiseComplement:
                        UnaryBitwiseComplement(def, unionBuilder);
                        break;

                    case Op.Increment:
                        UnaryIncrement(def, unionBuilder);
                        break;

                    case Op.Decrement:
                        UnaryDecrement(def, unionBuilder);
                        break;

                    case Op.Addition:
                        if (isStrict)
                            BinaryOperatorStrict(def, "+", unionBuilder);
                        else
                            BinaryOperator(def, "+", unionBuilder);
                        break;

                    case Op.Subtraction:
                        if (isStrict)
                            BinaryOperatorStrict(def, "-", unionBuilder);
                        else
                            BinaryOperator(def, "-", unionBuilder);
                        break;

                    case Op.Multiplication:
                        if (isStrict)
                            BinaryOperatorStrict(def, "*", unionBuilder);
                        else
                            BinaryOperator(def, "*", unionBuilder);
                        break;

                    case Op.Division:
                        if (isStrict)
                            BinaryOperatorStrict(def, "/", unionBuilder);
                        else
                            BinaryOperator(def, "/", unionBuilder);
                        break;

                    case Op.Remainder:
                        if (isStrict)
                            BinaryOperatorStrict(def, "%", unionBuilder);
                        else
                            BinaryOperator(def, "%", unionBuilder);
                        break;

                    case Op.LogicalAnd:
                        if (isStrict)
                            BinaryOperatorStrict(def, "&", unionBuilder);
                        else
                            BinaryOperator(def, "&", unionBuilder);
                        break;

                    case Op.LogicalOr:
                        if (isStrict)
                            BinaryOperatorStrict(def, "|", unionBuilder);
                        else
                            BinaryOperator(def, "|", unionBuilder);
                        break;

                    case Op.LogicalExclusiveOr:
                        if (isStrict)
                            BinaryOperatorStrict(def, "^", unionBuilder);
                        else
                            BinaryOperator(def, "^", unionBuilder);
                        break;

                    case Op.LeftShift:
                        if (isStrict)
                            BinaryOperatorStrict(def, "<<", unionBuilder);
                        else
                            BinaryOperator(def, "<<", unionBuilder);
                        break;

                    case Op.RightShift:
                        if (isStrict)
                            BinaryOperatorStrict(def, ">>", unionBuilder);
                        else
                            BinaryOperator(def, ">>", unionBuilder);
                        break;

                    case Op.LessAndGreaterThan:
                        if (isStrict)
                        {
                            BinaryOperatorReturnBoolStrict(def, ">", unionBuilder);
                            BinaryOperatorReturnBoolStrict(def, "<", unionBuilder);
                        }
                        else
                        {
                            BinaryOperatorReturnBool(def, ">", unionBuilder);
                            BinaryOperatorReturnBool(def, "<", unionBuilder);
                        }
                        break;

                    case Op.LessAndGreaterThanOrEqual:
                        if (isStrict)
                        {
                            BinaryOperatorReturnBoolStrict(def, ">=", unionBuilder);
                            BinaryOperatorReturnBoolStrict(def, "<=", unionBuilder);
                        }
                        else
                        {
                            BinaryOperatorReturnBool(def, ">=", unionBuilder);
                            BinaryOperatorReturnBool(def, "<=", unionBuilder);
                        }
                        break;
                }
            }

            unionBuilder.Append(@"
    }
}");
            usingBuilder.AppendLine(unionBuilder.ToString());
            context.AddSource($"Union_{def.UnionDefinition.Name}.Operators.cs", SourceText.From(usingBuilder.ToString(), Encoding.UTF8));
        }

        private static void UnaryPlus(UnionOperatorDefinition def, StringBuilder builder)
        {
            var unionName = def.UnionDefinition.Name;
            var members = def.UnionDefinition.Members;
            var pre_ = def.UnionDefinition.GetMemberPrefix();

            builder.Append($@"
        public static {unionName} operator +(in {unionName} operand)
        {{");

            builder.Append($@"
            switch (operand.{pre_}ValueType)
            {{");

            foreach (var member in members)
            {
                var cast = BuiltInTypeHelper.GetPrimitiveCast(member.Type);

                builder.Append($@"
                case Type.{member.Name}: return new {unionName}({cast}+operand.{pre_}{member.Name});");
            }

            builder.Append($@"
            }}

            return operand;
        }}
");
        }

        private static void UnaryMinus(UnionOperatorDefinition def, StringBuilder builder)
        {
            var unionName = def.UnionDefinition.Name;
            var members = def.UnionDefinition.Members;
            var pre_ = def.UnionDefinition.GetMemberPrefix();

            builder.Append($@"
        public static {unionName} operator -(in {unionName} operand)
        {{");

            builder.Append($@"
            switch (operand.{pre_}ValueType)
            {{");

            foreach (var member in members)
            {
                if (BuiltInTypeHelper.IsUlong(member.Type))
                    continue;

                var cast = BuiltInTypeHelper.GetPrimitiveCast(member.Type);

                builder.Append($@"
                case Type.{member.Name}: return new {unionName}({cast}-operand.{pre_}{member.Name});");
            }

            builder.Append($@"
            }}

            return operand;
        }}
");
        }

        private static void UnaryBitwiseComplement(UnionOperatorDefinition def, StringBuilder builder)
        {
            var unionName = def.UnionDefinition.Name;
            var members = def.UnionDefinition.Members;
            var pre_ = def.UnionDefinition.GetMemberPrefix();

            builder.Append($@"
        public static {unionName} operator ~(in {unionName} operand)
        {{");

            builder.Append($@"
            switch (operand.{pre_}ValueType)
            {{");

            foreach (var member in members)
            {
                if (BuiltInTypeHelper.IsFloat(member.Type) ||
                    BuiltInTypeHelper.IsDouble(member.Type))
                    continue;

                var cast = BuiltInTypeHelper.GetPrimitiveCast(member.Type);

                builder.Append($@"
                case Type.{member.Name}: return new {unionName}({cast}~operand.{pre_}{member.Name});");
            }

            builder.Append($@"
            }}

            return operand;
        }}
");
        }

        private static void UnaryIncrement(UnionOperatorDefinition def, StringBuilder builder)
        {
            var unionName = def.UnionDefinition.Name;
            var members = def.UnionDefinition.Members;
            var pre_ = def.UnionDefinition.GetMemberPrefix();

            builder.Append($@"
        public static {unionName} operator ++(in {unionName} operand)
        {{");

            builder.Append($@"
            switch (operand.{pre_}ValueType)
            {{");

            foreach (var member in members)
            {
                var cast = BuiltInTypeHelper.GetPrimitiveCast(member.Type);

                builder.Append($@"
                case Type.{member.Name}: return new {unionName}({cast}(operand.{pre_}{member.Name} + 1));");
            }

            builder.Append($@"
            }}

            return operand;
        }}
");
        }

        private static void UnaryDecrement(UnionOperatorDefinition def, StringBuilder builder)
        {
            var unionName = def.UnionDefinition.Name;
            var members = def.UnionDefinition.Members;
            var pre_ = def.UnionDefinition.GetMemberPrefix();

            builder.Append($@"
        public static {unionName} operator --(in {unionName} operand)
        {{");

            builder.Append($@"
            switch (operand.{pre_}ValueType)
            {{");

            foreach (var member in members)
            {
                var cast = BuiltInTypeHelper.GetPrimitiveCast(member.Type);

                builder.Append($@"
                case Type.{member.Name}: return new {unionName}({cast}(operand.{pre_}{member.Name} - 1));");
            }

            builder.Append($@"
            }}

            return operand;
        }}
");
        }

        private static void UnaryOperatorReturnBool(UnionOperatorDefinition def, string op, StringBuilder builder)
        {
            var unionName = def.UnionDefinition.Name;
            var members = def.UnionDefinition.Members;
            var pre_ = def.UnionDefinition.GetMemberPrefix();

            builder.Append($@"
        public static bool operator {op}(in {unionName} operand)
        {{");

            builder.Append($@"
            switch (operand.{pre_}ValueType)
            {{");

            foreach (var member in members)
            {
                builder.Append($@"
                case Type.{member.Name}: return {op}operand.{pre_}{member.Name};");
            }

            builder.Append($@"
            }}

            return false;
        }}
");
        }

        private static void BinaryOperator(UnionOperatorDefinition def, string op, StringBuilder builder)
        {
            var unionName = def.UnionDefinition.Name;
            var members = def.UnionDefinition.Members;
            var pre_ = def.UnionDefinition.GetMemberPrefix();

            builder.Append($@"
        public static {unionName} operator {op}(in {unionName} lhs, in {unionName} rhs)
        {{");

            builder.Append($@"
            switch (lhs.{pre_}ValueType)
            {{");

            for (var i = 0; i < members.Count; i++)
            {
                var lhs = members[i];

                builder.Append($@"
                case Type.{lhs.Name}:
                {{
                    switch (rhs.{pre_}ValueType)
                    {{");

                for (var k = 0; k < members.Count; k++)
                {
                    var rhs = members[k];
                    BuiltInTypeHelper.GetPrimitiveCast(lhs.Type, rhs.Type, out var lhsCast, out var rhsCast, out var cast);

                    builder.Append($@"
                        case Type.{rhs.Name}: return new {unionName}({cast}({lhsCast}lhs.{pre_}{lhs.Name} {op} {rhsCast}rhs.{pre_}{rhs.Name}));");
                }

                builder.Append($@"
                    }}

                    return new {unionName}(lhs.{pre_}ValueType);
                }}");
            }

            builder.Append($@"
            }}

            return new {unionName}(lhs.{pre_}ValueType);
        }}
");
        }

        private static void BinaryOperatorStrict(UnionOperatorDefinition def, string op, StringBuilder builder)
        {
            var unionName = def.UnionDefinition.Name;
            var throwException = def.UnionDefinition.InvalidValueAccess == InvalidValueAccess.ThrowException;
            var members = def.UnionDefinition.Members;
            var pre_ = def.UnionDefinition.GetMemberPrefix();

            builder.Append($@"
        public static {unionName} operator {op}(in {unionName} lhs, in {unionName} rhs)
        {{");

            if (throwException)
            {
                builder.Append($@"
            if (lhs.{pre_}ValueType != rhs.{pre_}ValueType)
                throw new InvalidCastException($""Cannot cast right operand from '{{rhs.GetUnderlyingType().GetNiceFullName()}}' to '{{lhs.GetUnderlyingType().GetNiceFullName()}}'"");
");
            }
            else
            {
                builder.Append($@"
            if (lhs.{pre_}ValueType != rhs.{pre_}ValueType)
                return new {unionName}(lhs.{pre_}ValueType);
");
            }

            builder.Append($@"
            switch (lhs.{pre_}ValueType)
            {{");

            foreach (var member in members)
            {
                var cast = BuiltInTypeHelper.GetPrimitiveCast(member.Type);

                builder.Append($@"
                case Type.{member.Name}: return new {unionName}({cast}(lhs.{pre_}{member.Name} {op} rhs.{pre_}{member.Name}));");
            }

            builder.Append($@"
            }}

            return new {unionName}(lhs.{pre_}ValueType);
        }}
");
        }

        private static void BinaryOperatorReturnBool(UnionOperatorDefinition def, string op, StringBuilder builder)
        {
            var unionName = def.UnionDefinition.Name;
            var members = def.UnionDefinition.Members;
            var pre_ = def.UnionDefinition.GetMemberPrefix();

            builder.Append($@"
        public static bool operator {op}(in {unionName} lhs, in {unionName} rhs)
        {{");

            builder.Append($@"
            switch (lhs.{pre_}ValueType)
            {{");

            for (var i = 0; i < members.Count; i++)
            {
                var lhs = members[i];

                builder.Append($@"
                case Type.{lhs.Name}:
                {{
                    switch (rhs.{pre_}ValueType)
                    {{");

                for (var k = 0; k < members.Count; k++)
                {
                    var rhs = members[k];
                    BuiltInTypeHelper.GetPrimitiveCast(lhs.Type, rhs.Type, out var lhsCast, out var rhsCast);

                    builder.Append($@"
                        case Type.{rhs.Name}: return {lhsCast}lhs.{pre_}{lhs.Name} {op} {rhsCast}rhs.{pre_}{rhs.Name};");
                }

                builder.Append(@"
                    }

                    return false;
                }");
            }

            builder.Append(@"
            }

            return false;
        }
");
        }

        private static void BinaryOperatorReturnBoolStrict(UnionOperatorDefinition def, string op, StringBuilder builder)
        {
            var unionName = def.UnionDefinition.Name;
            var throwException = def.UnionDefinition.InvalidValueAccess == InvalidValueAccess.ThrowException;
            var members = def.UnionDefinition.Members;
            var pre_ = def.UnionDefinition.GetMemberPrefix();

            builder.Append($@"
        public static bool operator {op}(in {unionName} lhs, in {unionName} rhs)
        {{");

            if (throwException)
            {
                builder.Append($@"
            if (lhs.{pre_}ValueType != rhs.{pre_}ValueType)
                throw new InvalidCastException($""Cannot cast right operand from '{{rhs.GetUnderlyingType().GetNiceFullName()}}' to '{{lhs.GetUnderlyingType().GetNiceFullName()}}'"");
");
            }
            else
            {
                builder.Append($@"
            if (lhs.{pre_}ValueType != rhs.{pre_}ValueType)
                return false;
");
            }

            builder.Append($@"
            switch (lhs.{pre_}ValueType)
            {{");

            foreach (var member in members)
            {
                builder.Append($@"
                case Type.{member.Name}: return lhs.{pre_}{member.Name} {op} rhs.{pre_}{member.Name};");
            }

            builder.Append(@"
            }

            return false;
        }
");
        }
    }
}
