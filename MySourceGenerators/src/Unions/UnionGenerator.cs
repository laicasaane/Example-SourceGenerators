using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace MySourceGenerators.Unions
{
    [Generator]
    public class UnionGenerator : ISourceGenerator
    {
        private const string _attributeText = $@"
using System;

namespace {GeneratorConfig.Namespace}.Unions
{{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class UnionAttribute : Attribute
    {{
        public Type TupleType {{ get; }}

        public InvalidValueAccess InvalidValueAccess {{ get; }}

        public UnionAttribute(Type tupleType)
        {{
            this.TupleType = tupleType;
        }}

        public UnionAttribute(Type tupleType, InvalidValueAccess invalidValueAccess)
        {{
            this.TupleType = tupleType;
            this.InvalidValueAccess = invalidValueAccess;
        }}
    }}

    public enum InvalidValueAccess
    {{
        Allow, ReturnDefault, ThrowException
    }}

    public class InvalidValueAccessException : InvalidCastException
    {{
        public InvalidValueAccessException() : base() {{ }}

        public InvalidValueAccessException(string message) : base(message) {{ }}

        public InvalidValueAccessException(string message, Exception innerException) : base(message, innerException) {{ }}
    }}
}}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization((i) => i.AddSource("UnionAttribute", _attributeText));

            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new UnionSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is UnionSyntaxReceiver receiver) ||
                receiver.Definitions.Count <= 0)
                return;

            foreach (var def in receiver.Definitions)
            {
                Execute(context, def);
            }
        }

        public void Execute(GeneratorExecutionContext context, UnionDefinition def)
        {
            var usingBuilder = new StringBuilder($@"using {GeneratorConfig.Namespace};
");

            var unionBuilder = new StringBuilder($@"
namespace {def.Namespace}
{{
    using System.Runtime.InteropServices;
");

            AppendUsings(def, unionBuilder, usingBuilder);

            unionBuilder.Append($@"
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public partial struct {def.Name}
    {{");

            AppendEnum_Type(def, unionBuilder);
            AppendProp_ValueType(def, unionBuilder);
            AppendProps(def, unionBuilder);
            AppendConstructors(def, unionBuilder);
            AppendConstructorTypeParam(def, unionBuilder);

            if (!def.IsReadOnly)
                AppendMethod_Set(def, unionBuilder);

            AppendMethod_TryGet(def, unionBuilder);
            AppendMethod_GetUnderlyingType(def, unionBuilder);
            AppendMethod_GetHashCode(def, unionBuilder);
            AppendMethod_Equals(def, unionBuilder);
            AppendMethod_ToString(def, unionBuilder);
            AppendOperator_Implicit(def, unionBuilder);

            unionBuilder.Append(@"
    }
}");
            usingBuilder.AppendLine(unionBuilder.ToString());
            context.AddSource($"Union_{def.Name}.cs", SourceText.From(usingBuilder.ToString(), Encoding.UTF8));
        }

        private static void AppendMethod_TryGet(UnionDefinition def, StringBuilder builder)
        {
            var pre_ = def.GetMemberPrefix();

            foreach (var member in def.Members)
            {
                builder.Append($@"
        public bool TryGet(out {member.Type} value)
        {{
            if (this.{pre_}ValueType != Type.{member.Name})
            {{
                value = default;
                return false;
            }}

            value = this.{pre_}{member.Name};
            return true;
        }}
");
            }
        }

        private static void AppendMethod_GetUnderlyingType(UnionDefinition def, StringBuilder builder)
        {
            var pre_ = def.GetMemberPrefix();

            builder.Append($@"
        public System.Type GetUnderlyingType()");

            if (def.Members.Count < 3)
            {
                builder.Append(@"
        {");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
            if (this.{pre_}ValueType == Type.{member.Name})
                return this.{pre_}{member.Name}.GetType();
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
            switch (this.{pre_}ValueType)
            {{");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
                case Type.{member.Name}: return this.{pre_}{member.Name}.GetType();");
                }

                builder.Append(@"
            }

            return this.GetType();
        }
");
            }
        }

        private static void AppendMethod_Set(UnionDefinition def, StringBuilder builder)
        {
            var pre_ = def.GetMemberPrefix();

            foreach (var member in def.Members)
            {
                builder.Append($@"
        public void Set({member.Type} value)
        {{
            this.{pre_}ValueType = Type.{member.Name};
            this.{pre_}{member.Name} = value;
        }}
");
            }
        }

        private static void AppendMethod_ToString(UnionDefinition def, StringBuilder builder)
        {
            var pre_ = def.GetMemberPrefix();

            builder.Append(@"
        public override string ToString()
        {");

            if (def.Members.Count < 3)
            {
                foreach (var member in def.Members)
                {
                    builder.Append($@"
            if (this.{pre_}ValueType == Type.{member.Name})
                return this.{pre_}{member.Name}.ToString();
");
                }

                builder.Append(@"
            return string.Empty;
        }");
            }
            else
            {
                builder.Append($@"
            switch (this.{pre_}ValueType)
            {{");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
                case Type.{member.Name}: return this.{pre_}{member.Name}.ToString();");
                }

                builder.Append(@"
            }

            return string.Empty;
        }");
            }
        }

        private static void AppendMethod_GetHashCode(UnionDefinition def, StringBuilder builder)
        {
            var pre_ = def.GetMemberPrefix();

            builder.Append($@"
        public override int GetHashCode()
        {{
            var hash = EqualityComparer<Type>.Default.GetHashCode(this.{pre_}ValueType) * -1521134295;
");

            if (def.Members.Count < 3)
            {
                foreach (var member in def.Members)
                {
                    builder.Append($@"
            if (this.{pre_}ValueType == Type.{member.Name})
                return hash + EqualityComparer<{member.Type}>.Default.GetHashCode(this.{pre_}{member.Name});
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
            switch (this.{pre_}ValueType)
            {{");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
                case Type.{member.Name}: return hash + EqualityComparer<{member.Type}>.Default.GetHashCode(this.{pre_}{member.Name});");
                }

                builder.Append(@"
            }

            return hash;
        }
");
            }
        }

        private static void AppendMethod_Equals(UnionDefinition def, StringBuilder builder)
        {
            var pre_ = def.GetMemberPrefix();

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
            if (a.{pre_}ValueType != b.{pre_}ValueType)
                return false;
");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
            if (a.{pre_}ValueType == Type.{member.Name})
                return EqualityComparer<{member.Type}>.Default.Equals(a.{pre_}{member.Name}, b.{pre_}{member.Name});
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
            if (a.{pre_}ValueType != b.{pre_}ValueType)
                return false;

            switch (a.{pre_}ValueType)
            {{");

                foreach (var member in def.Members)
                {
                    builder.Append($@"
                case Type.{member.Name}: return EqualityComparer<{member.Type}>.Default.Equals(a.{pre_}{member.Name}, b.{pre_}{member.Name});");
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

        private static void AppendOperator_Implicit(UnionDefinition def, StringBuilder builder)
        {
            var pre_ = def.GetMemberPrefix();
            var last = def.Members.Count - 1;

            switch (def.InvalidValueAccess)
            {
                case UnionDefinition.InvalidValueAccessStrategy.ThrowException:
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
            if (value.{pre_}ValueType == Type.{member.Name})
                return value.{pre_}{member.Name};

            throw new InvalidValueAccessException($""Cannot implicitly convert underlying type '{{value.GetUnderlyingType().GetNiceFullName()}}' to '{member.Type}'"");
        }}");

                            if (i < last)
                                builder.AppendLine();
                        }
                        break;
                    }

                case UnionDefinition.InvalidValueAccessStrategy.ReturnDefault:
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
            if (value.{pre_}ValueType == Type.{member.Name})
                return value.{pre_}{member.Name};

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
            => value.{pre_}{member.Name};");

                            if (i < last)
                                builder.AppendLine();
                        }
                        break;
                    }
            }
        }

        private static void AppendConstructorTypeParam(UnionDefinition def, StringBuilder builder)
        {
            var pre_ = def.GetMemberPrefix();

            builder.Append($@"
        public {def.Name}(Type type)
        {{
            this.{pre_}ValueType = type;
");

            foreach (var member in def.Members)
            {
                builder.Append($@"
            this.{pre_}{member.Name} = default;");
            }

            builder.Append(@"
        }
");
        }

        private static void AppendConstructors(UnionDefinition def, StringBuilder builder)
        {
            var pre_ = def.GetMemberPrefix();
            var last = def.Members.Count - 1;

            for (var i = 0; i < def.Members.Count; i++)
            {
                var member = def.Members[i];

                if (i > 0)
                    builder.AppendLine();

                builder.Append($@"
        public {def.Name}({member.Type} value)
        {{
            this.{pre_}ValueType = Type.{member.Name};
");

                for (var k = 0; k < def.Members.Count; k++)
                {
                    if (k == i)
                        continue;

                    var memberOther = def.Members[k];

                    builder.Append($@"
            this.{pre_}{memberOther.Name} = default;");
                }

                builder.Append($@"

            this.{pre_}{member.Name} = value;
        }}");

                if (i == last)
                    builder.AppendLine();
            }
        }

        private static void AppendProps(UnionDefinition def, StringBuilder builder)
        {
            if (def.IsReadOnly && def.InvalidValueAccess == UnionDefinition.InvalidValueAccessStrategy.Allow)
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
                case UnionDefinition.InvalidValueAccessStrategy.ThrowException:
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

                throw new InvalidValueAccessException($""Cannot convert underlying type '{{GetUnderlyingType().GetNiceFullName()}}' to '{member.Type}'"");
            }}
        }}
");
                        }
                        break;
                    }

                case UnionDefinition.InvalidValueAccessStrategy.ReturnDefault:
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

        private static void AppendProp_ValueType(UnionDefinition def, StringBuilder builder)
        {
            if (def.IsReadOnly && def.InvalidValueAccess == UnionDefinition.InvalidValueAccessStrategy.Allow)
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

        private static void AppendEnum_Type(UnionDefinition def, StringBuilder builder)
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

        private static void AppendUsings(UnionDefinition def, StringBuilder unionBuilder, StringBuilder usingBuilder)
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
    }
}