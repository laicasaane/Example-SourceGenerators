using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySourceGenerators
{
    [Generator]
    public class UnionGenerator : ISourceGenerator
    {
        private const string _attributeText = @"
using System;

namespace Unions
{
    public enum InvalidValueAccess
    {
        Allow, ReturnDefault, ThrowException
    }

    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class UnionAttribute : Attribute
    {
        public Type TupleType { get; }

        public InvalidValueAccess InvalidValueAccess { get; }

        public UnionAttribute(Type tupleType)
        {
            this.TupleType = tupleType;
        }

        public UnionAttribute(Type tupleType, InvalidValueAccess invalidValueAccess)
        {
            this.TupleType = tupleType;
            this.InvalidValueAccess = invalidValueAccess;
        }
    }

    public class InvalidUnionValueException : InvalidCastException
    {
        public InvalidUnionValueException() : base() { }

        public InvalidUnionValueException(string message) : base(message) { }

        public InvalidUnionValueException(string message, Exception innerException) : base(message, innerException) { }
    }
}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization((i) => i.AddSource("UnionAttribute", _attributeText));

            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver) ||
                receiver.Structs.Count <= 0)
                return;

            foreach (var def in receiver.Structs)
            {
                Execute(context, def);
            }
        }

        public void Execute(GeneratorExecutionContext context, StructDefinition def)
        {
            var usingBuilder = new StringBuilder();
            var unionBuilder = new StringBuilder($@"
namespace {def.Namespace}
{{");

            AppendUsings(def, unionBuilder, usingBuilder);
            AppendEnum_Type(def, unionBuilder);
            AppendProp_ValueType(def, unionBuilder);
            AppendProps(def, unionBuilder);

            if (def.IsReadOnly && def.InvalidValueAccess == InvalidValueAccess.Allow)
            {
                AppendConstructorsReadOnlyUnsafe(def, unionBuilder);
                AppendMethod_GetUnderlyingTypeReadOnlyUnsafe(def, unionBuilder);
                AppendMethod_GetHashCodeReadOnlyUnsafe(def, unionBuilder);
                AppendMethod_EqualsReadOnlyUnsafe(def, unionBuilder);
                AppendMethod_ToStringReadOnlyUnsafe(def, unionBuilder);
                AppendOperator_ImplicitReadOnlyUnsafe(def, unionBuilder);
            }
            else
            {
                AppendConstructors(def, unionBuilder);
                AppendMethod_Set(def, unionBuilder);
                AppendMethod_GetUnderlyingType(def, unionBuilder);
                AppendMethod_GetHashCode(def, unionBuilder);
                AppendMethod_Equals(def, unionBuilder);
                AppendMethod_ToString(def, unionBuilder);
                AppendOperator_Implicit(def, unionBuilder);
            }

            unionBuilder.Append(@"
    }
}");
            usingBuilder.AppendLine(unionBuilder.ToString());
            context.AddSource($"Union_{def.Name}.cs", SourceText.From(usingBuilder.ToString(), Encoding.UTF8));
        }

        private static void AppendMethod_GetUnderlyingType(StructDefinition def, StringBuilder builder)
        {
            builder.Append($@"
        public System.Type GetUnderlyingType()");

            if (def.Members.Count < 3)
            {
                builder.Append(@"
        {");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
            if (this.m_ValueType == Type.{member.Name})
                return this.m_{member.Name}.GetType();
");
                }

                builder.Append($@"
            return this.GetType();
        }}
");
            }
            else
            {
                builder.Append($@"
        {{
            switch (this.m_ValueType)
            {{");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
                case Type.{member.Name}: return this.m_{member.Name}.GetType();");
                }

                builder.Append($@"
            }}

            return this.GetType();
        }}
");
            }
        }

        private static void AppendMethod_GetUnderlyingTypeReadOnlyUnsafe(StructDefinition def, StringBuilder builder)
        {
            builder.Append($@"
        public System.Type GetUnderlyingType()");

            if (def.Members.Count < 3)
            {
                builder.Append(@"
        {");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
            if (this.ValueType == Type.{member.Name})
                return this.{member.Name}.GetType();
");
                }

                builder.Append(@"
            return this.GetType();
        }
");
            }
            else
            {
                builder.Append($@"
        {{
            switch (this.ValueType)
            {{");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
                case Type.{member.Name}: return this.{member.Name}.GetType();");
                }

                builder.Append(@"
            }

            return this.GetType();
        }
");
            }
        }

        private static void AppendMethod_Set(StructDefinition def, StringBuilder builder)
        {
            foreach (var member in def.Members)
            {
                builder.Append($@"
        public void Set({member.Type} value)
        {{
            this.m_ValueType = Type.{member.Name};
            this.m_{member.Name} = value;
        }}
");
            }
        }

        private static void AppendMethod_ToString(StructDefinition def, StringBuilder builder)
        {
            builder.Append(@"
        public override string ToString()
        {");

            if (def.Members.Count < 3)
            {
                foreach (var member in def.Members)
                {
                    builder.Append($@"
            if (this.m_ValueType == Type.{member.Name})
                return this.m_{member.Name}.ToString();
");
                }

                builder.Append(@"
            return string.Empty;
        }");
            }
            else
            {
                builder.Append($@"
            switch (this.m_ValueType)
            {{");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
                case Type.{member.Name}: return this.m_{member.Name}.ToString();");
                }

                builder.Append(@"
            }

            return string.Empty;
        }");
            }
        }

        private static void AppendMethod_ToStringReadOnlyUnsafe(StructDefinition def, StringBuilder builder)
        {
            builder.Append(@"
        public override string ToString()
        {");

            if (def.Members.Count < 3)
            {
                foreach (var member in def.Members)
                {
                    builder.Append($@"
            if (this.ValueType == Type.{member.Name})
                return this.{member.Name}.ToString();
");
                }

                builder.Append(@"
            return string.Empty;
        }");
            }
            else
            {
                builder.Append($@"
            switch (this.ValueType)
            {{");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
                case Type.{member.Name}: return this.{member.Name}.ToString();");
                }

                builder.Append(@"
            }

            return string.Empty;
        }");
            }
        }

        private static void AppendMethod_GetHashCode(StructDefinition def, StringBuilder builder)
        {
            builder.Append(@"
        public override int GetHashCode()
        {
            var hash = EqualityComparer<Type>.Default.GetHashCode(this.m_ValueType) * -1521134295;
");

            if (def.Members.Count < 3)
            {
                foreach (var member in def.Members)
                {
                    builder.Append($@"
            if (this.m_ValueType == Type.{member.Name})
                return hash + EqualityComparer<{member.Type}>.Default.GetHashCode(this.m_{member.Name});
");
                }

                builder.Append(@"
            return hash;
        }
");
            }
            else
            {
                builder.Append($@"
            switch (this.m_ValueType)
            {{");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
                case Type.{member.Name}: return hash + EqualityComparer<{member.Type}>.Default.GetHashCode(this.m_{member.Name});");
                }

                builder.Append(@"
            }

            return hash;
        }
");
            }
        }

        private static void AppendMethod_GetHashCodeReadOnlyUnsafe(StructDefinition def, StringBuilder builder)
        {
            builder.Append(@"
        public override int GetHashCode()
        {
            var hash = EqualityComparer<Type>.Default.GetHashCode(this.ValueType) * -1521134295;
");

            if (def.Members.Count < 3)
            {
                foreach (var member in def.Members)
                {
                    builder.Append($@"
            if (this.ValueType == Type.{member.Name})
                return hash + EqualityComparer<{member.Type}>.Default.GetHashCode(this.{member.Name});
");
                }

                builder.Append(@"
            return hash;
        }
");
            }
            else
            {
                builder.Append($@"
            switch (this.ValueType)
            {{");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
                case Type.{member.Name}: return hash + EqualityComparer<{member.Type}>.Default.GetHashCode(this.{member.Name});");
                }

                builder.Append(@"
            }

            return hash;
        }
");
            }
        }

        private static void AppendMethod_Equals(StructDefinition def, StringBuilder builder)
        {
            builder.Append($@"
        public override bool Equals(object obj)
            => obj is {def.Name} other && Equals(in this, in other);

        public bool Equals({def.Name} other)
            => Equals(in this, in other);

        public bool Equals(in {def.Name} other)
            => Equals(in this, in other);
");

            builder.Append($@"
        public static bool Equals(in {def.Name} a, in {def.Name} b)");

            if (def.Members.Count < 3)
            {
                builder.Append($@"
        {{
            if (a.m_ValueType != b.m_ValueType)
                return false;
");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
            if (a.m_ValueType == Type.{member.Name})
                return EqualityComparer<{member.Type}>.Default.Equals(a.m_{member.Name}, b.m_{member.Name});
");
                }

                builder.Append(@"
            return false;
        }
");
            }
            else
            {
                builder.Append($@"
        {{
            if (a.m_ValueType != b.m_ValueType)
                return false;

            switch (a.m_ValueType)
            {{");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
                case Type.{member.Name}: return EqualityComparer<{member.Type}>.Default.Equals(a.m_{member.Name}, b.m_{member.Name});");
                }

                builder.Append(@"
            }

            return false;
        }
");
            }

            builder.Append($@"
        public static bool operator ==(in {def.Name} left, in {def.Name} right)
            => Equals(in left, in right);

        public static bool operator !=(in {def.Name} left, in {def.Name} right)
            => !Equals(in left, in right);
");
        }

        private static void AppendMethod_EqualsReadOnlyUnsafe(StructDefinition def, StringBuilder builder)
        {
            builder.Append($@"
        public override bool Equals(object obj)
            => obj is {def.Name} other && Equals(in this, in other);

        public bool Equals({def.Name} other)
            => Equals(in this, in other);

        public bool Equals(in {def.Name} other)
            => Equals(in this, in other);
");

            builder.Append($@"
        public static bool Equals(in {def.Name} a, in {def.Name} b)");

            if (def.Members.Count < 3)
            {
                builder.Append($@"
        {{
            if (a.ValueType != b.ValueType)
                return false;
");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
            if (a.ValueType == Type.{member.Name})
                return EqualityComparer<{member.Type}>.Default.Equals(a.{member.Name}, b.{member.Name});
");
                }

                builder.Append(@"
            return false;
        }
");
            }
            else
            {
                builder.Append($@"
        {{
            if (a.ValueType != b.ValueType)
                return false;

            switch (a.ValueType)
            {{");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
                case Type.{member.Name}: return EqualityComparer<{member.Type}>.Default.Equals(a.{member.Name}, b.{member.Name});");
                }

                builder.Append(@"
            }

            return false;
        }
");
            }

            builder.Append($@"
        public static bool operator ==(in {def.Name} left, in {def.Name} right)
            => Equals(in left, in right);

        public static bool operator !=(in {def.Name} left, in {def.Name} right)
            => !Equals(in left, in right);
");
        }

        private static void AppendOperator_Implicit(StructDefinition def, StringBuilder builder)
        {
            var last = def.Members.Count - 1;

            switch (def.InvalidValueAccess)
            {
                case InvalidValueAccess.ThrowException:
                    {
                        for (var i = 0; i < def.Members.Count; i++)
                        {
                            var member = def.Members[i];

                            if (i <= 0)
                                builder.AppendLine();

                            builder.Append($@"
        public static implicit operator {def.Name}({member.Type} value)
            => new {def.Name}(value);
");

                            builder.Append($@"
        public static implicit operator {member.Type}(in {def.Name} value)
        {{
            if (value.m_ValueType == Type.{member.Name})
                return value.m_{member.Name};

            throw new InvalidUnionValueException($""Cannot implicitly convert underlying type '{{value.GetUnderlyingType().Name}}' to '{member.Type}'"");
        }}");

                            if (i < last)
                                builder.AppendLine();
                        }
                        break;
                    }

                case InvalidValueAccess.ReturnDefault:
                    {
                        for (var i = 0; i < def.Members.Count; i++)
                        {
                            var member = def.Members[i];

                            if (i <= 0)
                                builder.AppendLine();

                            builder.Append($@"
        public static implicit operator {def.Name}({member.Type} value)
            => new {def.Name}(value);
");

                            builder.Append($@"
        public static implicit operator {member.Type}(in {def.Name} value)
        {{
            if (value.m_ValueType == Type.{member.Name})
                return value.m_{member.Name};

            return default;
        }}");

                            if (i < last)
                                builder.AppendLine();
                        }
                        break;
                    }

                default:
                    {
                        for (var i = 0; i < def.Members.Count; i++)
                        {
                            var member = def.Members[i];

                            if (i <= 0)
                                builder.AppendLine();

                            builder.Append($@"
        public static implicit operator {def.Name}({member.Type} value)
            => new {def.Name}(value);
");

                            builder.Append($@"
        public static implicit operator {member.Type}(in {def.Name} value)
            => value.m_{member.Name};");

                            if (i < last)
                                builder.AppendLine();
                        }
                        break;
                    }
            }
        }

        private static void AppendOperator_ImplicitReadOnlyUnsafe(StructDefinition def, StringBuilder builder)
        {
            var last = def.Members.Count - 1;

            for (var i = 0; i < def.Members.Count; i++)
            {
                var member = def.Members[i];

                if (i <= 0)
                    builder.AppendLine();

                builder.Append($@"
        public static implicit operator {def.Name}({member.Type} value)
            => new {def.Name}(value);
");

                builder.Append($@"
        public static implicit operator {member.Type}(in {def.Name} value)
            => value.{member.Name};");

                if (i < last)
                    builder.AppendLine();
            }
        }

        private static void AppendConstructors(StructDefinition def, StringBuilder builder)
        {
            var last = def.Members.Count - 1;

            for (var i = 0; i < def.Members.Count; i++)
            {
                var member = def.Members[i];

                if (i > 0)
                    builder.AppendLine();

                builder.Append($@"
        public {def.Name}({member.Type} value)
        {{
            this.m_ValueType = Type.{member.Name};
");

                for (var k = 0; k < def.Members.Count; k++)
                {
                    if (k == i)
                        continue;

                    var memberOther = def.Members[k];

                    builder.Append($@"
            this.m_{memberOther.Name} = default;");
                }

                builder.Append($@"

            this.m_{member.Name} = value;
        }}");

                if (i == last)
                    builder.AppendLine();
            }
        }

        private static void AppendConstructorsReadOnlyUnsafe(StructDefinition def, StringBuilder builder)
        {
            var last = def.Members.Count - 1;

            for (var i = 0; i < def.Members.Count; i++)
            {
                var member = def.Members[i];

                if (i > 0)
                    builder.AppendLine();

                builder.Append($@"
        public {def.Name}({member.Type} value)
        {{
            this.ValueType = Type.{member.Name};
");

                for (var k = 0; k < def.Members.Count; k++)
                {
                    if (k == i)
                        continue;

                    var memberOther = def.Members[k];

                    builder.Append($@"
            this.{memberOther.Name} = default;");
                }

                builder.Append($@"

            this.{member.Name} = value;
        }}");

                if (i == last)
                    builder.AppendLine();
            }
        }

        private static void AppendProps(StructDefinition def, StringBuilder builder)
        {
            if (def.IsReadOnly && def.InvalidValueAccess == InvalidValueAccess.Allow)
            {
                foreach (var member in def.Members)
                {
                    builder.Append($@"
        [FieldOffset(1)]
        public readonly {member.Type} {member.Name};
");
                }

                return;
            }

            var readonlyKeyword = def.IsReadOnly ? "readonly " : string.Empty;

            foreach (var member in def.Members)
            {
                builder.Append($@"
        [FieldOffset(1)]
        private {readonlyKeyword}{member.Type} m_{member.Name};
");
            }

            switch (def.InvalidValueAccess)
            {
                case InvalidValueAccess.ThrowException:
                    {
                        foreach (var member in def.Members)
                        {
                            builder.Append($@"
        public {member.Type} {member.Name}
        {{
            get
            {{
                if (this.m_ValueType == Type.{member.Name})
                    return this.m_{member.Name};

                throw new InvalidUnionValueException($""Cannot convert underlying type '{{GetUnderlyingType().Name}}' to '{member.Type}'"");
            }}
        }}
");
                        }
                        break;
                    }

                case InvalidValueAccess.ReturnDefault:
                    {
                        foreach (var member in def.Members)
                        {
                            builder.Append($@"
        public {member.Type} {member.Name}
        {{
            get
            {{
                if (this.m_ValueType == Type.{member.Name})
                    return this.m_{member.Name};

                return default;
            }}
        }}
");
                        }
                        break;
                    }

                default:
                    {
                        foreach (var member in def.Members)
                        {
                            builder.Append($@"
        public {member.Type} {member.Name} => this.m_{member.Name};
");
                        }
                        break;
                    }
            }
        }

        private static void AppendEnum_Type(StructDefinition def, StringBuilder builder)
        {
            var underlyingType = string.Empty;
            var memberCount = (ulong)def.Members.Count;

            if (memberCount <= 255)
                underlyingType = "byte";
            else if (memberCount <= 32767)
                underlyingType = "short";
            else if (memberCount <= 65535)
                underlyingType = "ushort";
            else if (memberCount <= 2147483647)
                underlyingType = "int";
            else if (memberCount <= 42949672955)
                underlyingType = "uint";
            else if (memberCount <= 9223372036854775807)
                underlyingType = "long";
            else
                underlyingType = "ulong";

            builder.Append($@"
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public partial struct {def.Name}
    {{
        public enum Type : {underlyingType}
        {{");

            foreach (var member in def.Members)
            {
                builder.Append($@"
            {member.Name},");

            }

            builder.Append(@"
        }
");
        }

        private static void AppendProp_ValueType(StructDefinition def, StringBuilder builder)
        {
            if (def.IsReadOnly && def.InvalidValueAccess == InvalidValueAccess.Allow)
            {
                builder.Append(@"
        [FieldOffset(0)]
        public readonly Type ValueType;
");
            }
            else
            {
                var readonlyKeyword = def.IsReadOnly ? "readonly " : string.Empty;

                builder.Append($@"
        [FieldOffset(0)]
        private {readonlyKeyword}Type m_ValueType;

        public Type ValueType => this.m_ValueType;
");
            }
        }

        private static void AppendUsings(StructDefinition def, StringBuilder unionBuilder, StringBuilder usingBuilder)
        {
            foreach (var ns in def.GlobalNamespaces)
            {
                usingBuilder.AppendLine($"using {ns};");
            }

            if (def.LocalNamespaces.Count > 0)
            {
                unionBuilder.AppendLine();

                foreach (var ns in def.LocalNamespaces)
                {
                    unionBuilder.AppendLine($"    using {ns};");
                }
            }
        }

        private class SyntaxReceiver : ISyntaxContextReceiver
        {
            public List<StructDefinition> Structs { get; } = new List<StructDefinition>();

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (!(context.Node is StructDeclarationSyntax dec) ||
                    dec.AttributeLists.Count <= 0)
                    return;

                if (StructDefinition.TryCreate(context, dec, out var def))
                    this.Structs.Add(def);
            }
        }

        public readonly struct Member
        {
            public readonly string Type;
            public readonly string Name;

            public Member(string type, string name)
            {
                this.Type = type;
                this.Name = name;
            }
        }

        public enum InvalidValueAccess
        {
            Allow, ReturnDefault, ThrowException
        }

        public class StructDefinition
        {
            public string Namespace { get; private set; }

            public string Name { get; private set; }

            public StructDeclarationSyntax Struct { get; private set; }

            public bool IsReadOnly { get; private set; }

            public InvalidValueAccess InvalidValueAccess { get; private set; }

            public HashSet<string> GlobalNamespaces = new HashSet<string>();

            public HashSet<string> LocalNamespaces = new HashSet<string>();

            public List<Member> Members { get; } = new List<Member>();

            public static bool TryCreate(GeneratorSyntaxContext context, StructDeclarationSyntax dec, out StructDefinition def)
            {
                AttributeSyntax attribute = null;

                foreach (var attribList in dec.AttributeLists)
                {
                    foreach (var attrib in attribList.Attributes)
                    {
                        var name = attrib.Name.ToString();

                        if (string.Equals(name, "Union") ||
                            string.Equals(name, "UnionAttribute"))
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

                TypeOfExpressionSyntax typeOf = null;
                MemberAccessExpressionSyntax memberAccess = null;

                foreach (var arg in attribute.ArgumentList.Arguments)
                {
                    if (arg.Expression is TypeOfExpressionSyntax typeOfSyntax)
                    {
                        typeOf = typeOfSyntax;
                    }
                    else if (arg.Expression is MemberAccessExpressionSyntax memberAccessSyntax)
                    {
                        memberAccess = memberAccessSyntax;
                    }
                }

                if (typeOf == null || !(typeOf.Type is TupleTypeSyntax tuple) ||
                    tuple.Elements.Count <= 0)
                {
                    def = default;
                    return false;
                }

                def = new StructDefinition();
                def.Name = dec.Identifier.ToString();
                def.InvalidValueAccess = InvalidValueAccess.Allow;
                def.GetGlobalNamespaces(dec);
                def.GetLocalNamespaces(dec);

                foreach (var element in tuple.Elements)
                {
                    GetName(context.SemanticModel, element, out var type, out var name);
                    def.Members.Add(new Member(type, name));
                }

                foreach (var keyword in dec.Modifiers)
                {
                    if (string.Equals(keyword.ToString(), "readonly"))
                        def.IsReadOnly = true;
                }

                if (memberAccess != null &&
                    string.Equals(memberAccess.Expression.ToString(), nameof(InvalidValueAccess)))
                {
                    if (Enum.TryParse<InvalidValueAccess>(memberAccess.Name.ToString(), true, out var value))
                    {
                        def.InvalidValueAccess = value;
                    }
                }

                return true;
            }

            private static void GetName(SemanticModel semanticModel, TupleElementSyntax element,
                                        out string typeName, out string name)
            {
                typeName = element.Type.ToString();
                name = element.Identifier.ToString();

                if (!string.IsNullOrWhiteSpace(name))
                    return;

                var typeSymbol = semanticModel.GetTypeInfo(element.Type).Type;

                switch (typeSymbol.SpecialType)
                {
                    case SpecialType.System_Boolean: name = "Bool"; break;
                    case SpecialType.System_Char: name = "Char"; break;
                    case SpecialType.System_SByte: name = "SByte"; break;
                    case SpecialType.System_Byte: name = "Byte"; break;
                    case SpecialType.System_Int16: name = "Short"; break;
                    case SpecialType.System_Int32: name = "Int"; break;
                    case SpecialType.System_Int64: name = "Long"; break;
                    case SpecialType.System_UInt16: name = "UShort"; break;
                    case SpecialType.System_UInt32: name = "UInt"; break;
                    case SpecialType.System_UInt64: name = "ULong"; break;
                    case SpecialType.System_Decimal: name = "Decimal"; break;
                    case SpecialType.System_Single: name = "Float"; break;
                    case SpecialType.System_Double: name = "Double"; break;
                    case SpecialType.System_String: name = "String"; break;
                    case SpecialType.System_IntPtr: name = "IntPtr"; break;
                    case SpecialType.System_UIntPtr: name = "UIntPtr"; break;

                    default:
                        name = typeName.Replace("[]", "Array")
                                       .Replace("?", "_Nullable")
                                       .Replace('<', '_')
                                       .Replace(">", "")
                                       .Replace(", ", "_")
                                       .Replace(',', '_')
                                       .Replace(' ', '_');
                        break;
                }
            }

            private void GetGlobalNamespaces(StructDeclarationSyntax dec)
            {
                var compilation = dec.Parent?.Parent as CompilationUnitSyntax;

                if (compilation == null)
                    return;

                foreach (var usingDirective in compilation.Usings)
                {
                    this.GlobalNamespaces.Add(usingDirective.Name.ToString());
                }
            }

            private void GetLocalNamespaces(StructDeclarationSyntax dec)
            {
                var namespaceDec = dec.Parent as NamespaceDeclarationSyntax;

                if (namespaceDec == null)
                    return;

                this.Namespace = namespaceDec.Name.ToString();

                foreach (var usingDirective in namespaceDec.Usings)
                {
                    this.LocalNamespaces.Add(usingDirective.Name.ToString());
                }
            }
        }
    }
}