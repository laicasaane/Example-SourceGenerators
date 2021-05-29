using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace MySourceGenerators.Unions
{
    public class UnionSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<UnionDefinition> Definitions { get; } = new List<UnionDefinition>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (!(context.Node is StructDeclarationSyntax dec) ||
                dec.AttributeLists.Count <= 0)
                return;

            if (UnionDefinition.TryCreate(context, dec, out var def))
                this.Definitions.Add(def);
        }
    }

    public class UnionDefinition
    {
        public readonly struct MemberDefinition
        {
            public readonly string Type;
            public readonly string Name;

            public MemberDefinition(string type, string name)
            {
                this.Type = type;
                this.Name = name;
            }
        }

        public enum InvalidValueAccessStrategy
        {
            Allow, ReturnDefault, ThrowException
        }

        public string Namespace { get; private set; }

        public string Name { get; private set; }

        public StructDeclarationSyntax Struct { get; private set; }

        public bool IsReadOnly { get; private set; }

        public InvalidValueAccessStrategy InvalidValueAccess { get; private set; }

        public HashSet<string> GlobalNamespaces { get; } = new HashSet<string>();

        public HashSet<string> LocalNamespaces { get; } = new HashSet<string>();

        public List<MemberDefinition> Members { get; } = new List<MemberDefinition>();

        public string GetMemberPrefix()
            => (this.IsReadOnly && this.InvalidValueAccess == InvalidValueAccessStrategy.Allow) ? string.Empty : "m_";

        public static bool TryCreate(GeneratorSyntaxContext context, StructDeclarationSyntax dec, out UnionDefinition def)
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

            def = new UnionDefinition();
            def.Name = dec.Identifier.ToString();
            def.InvalidValueAccess = InvalidValueAccessStrategy.Allow;
            def.GetGlobalNamespaces(dec);
            def.GetLocalNamespaces(dec);

            foreach (var element in tuple.Elements)
            {
                GetName(context.SemanticModel, element, out var type, out var name);
                def.Members.Add(new MemberDefinition(type, name));
            }

            foreach (var keyword in dec.Modifiers)
            {
                if (string.Equals(keyword.ToString(), "readonly"))
                    def.IsReadOnly = true;
            }

            if (memberAccess != null &&
                string.Equals(memberAccess.Expression.ToString(), nameof(InvalidValueAccess)))
            {
                if (Enum.TryParse<InvalidValueAccessStrategy>(memberAccess.Name.ToString(), true, out var value))
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
                case SpecialType.System_Object: name = "Object"; break;
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