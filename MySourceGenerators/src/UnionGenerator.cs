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

namespace MyLibrary
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class UnionAttribute : Attribute
    {
        public readonly Type TupleType;

        public UnionAttribute(Type tupleType)
        {
            this.TupleType = tupleType;
        }
    }
}
";

        private const int IndentSize = 4;

        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization((i) => i.AddSource("UnionAttribute", _attributeText));

            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver) ||
                receiver.Struct == null || receiver.Members == null)
                return;

            var structName = receiver.Struct.Identifier.ToString();
            var unionBuilder = new StringBuilder($@"

namespace {receiver.Namespace}
{{");
            var usingBuilder = new StringBuilder();

            foreach (var ns in receiver.GlobalNamespaces)
            {
                usingBuilder.AppendLine($"using {ns};");
            }

            if (receiver.LocalNamespaces.Count > 0)
            {
                unionBuilder.AppendLine();

                foreach (var ns in receiver.LocalNamespaces)
                {
                    unionBuilder.AppendLine($"    using {ns};");
                }
            }

            unionBuilder.Append($@"
    public partial struct {structName}
    {{");

            foreach (var member in receiver.Members)
            {
                unionBuilder.Append(@"
        public ");

                if (receiver.IsReadOnly)
                    unionBuilder.Append("readonly ");

                unionBuilder.Append($"{member.Type} {member.Name};");
            }

            unionBuilder.Append(@"
    }
}");
            usingBuilder.AppendLine(unionBuilder.ToString());

            context.AddSource($"Union_{structName}.cs", SourceText.From(usingBuilder.ToString(), Encoding.UTF8));
        }

        private class SyntaxReceiver : ISyntaxContextReceiver
        {
            public string Namespace { get; private set; }

            public StructDeclarationSyntax Struct { get; private set; }

            public bool IsReadOnly { get; private set; }

            public HashSet<string> GlobalNamespaces = new HashSet<string>();

            public HashSet<string> LocalNamespaces = new HashSet<string>();

            public List<Member> Members { get; } = new List<Member>();

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (!(context.Node is StructDeclarationSyntax dec) ||
                    dec.AttributeLists.Count <= 0)
                    return;

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
                    attribute.ArgumentList.Arguments.Count != 1)
                    return;

                var expression = attribute.ArgumentList.Arguments[0].Expression;

                if (!(expression is TypeOfExpressionSyntax typeOf) ||
                    !(typeOf.Type is TupleTypeSyntax tuple) ||
                    tuple.Elements.Count <= 0)
                    return;

                this.Struct = dec;
                GetGlobalNamespaces(context);
                GetLocalNamespaces(context);

                foreach (var element in tuple.Elements)
                {
                    GetName(context, element, out var type, out var name);
                    this.Members.Add(new Member(type, name));
                }

                foreach (var keyword in dec.Modifiers)
                {
                    if (string.Equals(keyword.ToString(), "readonly"))
                        this.IsReadOnly = true;
                }
            }

            private void GetName(GeneratorSyntaxContext context, TupleElementSyntax element, out string typeName, out string name)
            {
                typeName = element.Type.ToString();
                name = element.Identifier.ToString();

                if (!string.IsNullOrWhiteSpace(name))
                    return;

                var typeSymbol = context.SemanticModel.GetTypeInfo(element.Type).Type;

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

            private void GetGlobalNamespaces(GeneratorSyntaxContext context)
            {
                var compilation = context.Node?.Parent?.Parent as CompilationUnitSyntax;

                if (compilation == null)
                    return;

                foreach (var usingDirective in compilation.Usings)
                {
                    this.GlobalNamespaces.Add(usingDirective.Name.ToString());
                }
            }

            private void GetLocalNamespaces(GeneratorSyntaxContext context)
            {
                var namespaceDec = context.Node?.Parent as NamespaceDeclarationSyntax;

                if (namespaceDec == null)
                    return;

                this.Namespace = namespaceDec.Name.ToString();

                foreach (var usingDirective in namespaceDec.Usings)
                {
                    this.LocalNamespaces.Add(usingDirective.Name.ToString());
                }
            }
        }

        private readonly struct Member
        {
            public readonly string Type;
            public readonly string Name;

            public Member(string type, string name)
            {
                this.Type = type;
                this.Name = name;
            }
        }
    }
}