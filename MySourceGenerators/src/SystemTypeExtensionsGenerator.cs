using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace MySourceGenerators
{
    [Generator]
    public class SystemTypeExtensionsGenerator : ISourceGenerator
    {
        private static TypeDefinition[] _builtinTypes = new TypeDefinition[] {
            new TypeDefinition { Type = typeof(object), Name = "object" },
            new TypeDefinition { Type = typeof(bool), Name = "bool" },
            new TypeDefinition { Type = typeof(byte), Name = "byte" },
            new TypeDefinition { Type = typeof(sbyte), Name = "sbyte" },
            new TypeDefinition { Type = typeof(short), Name = "short" },
            new TypeDefinition { Type = typeof(ushort), Name = "ushort" },
            new TypeDefinition { Type = typeof(int), Name = "int" },
            new TypeDefinition { Type = typeof(uint), Name = "uint" },
            new TypeDefinition { Type = typeof(long), Name = "long" },
            new TypeDefinition { Type = typeof(ulong), Name = "ulong" },
            new TypeDefinition { Type = typeof(float), Name = "float" },
            new TypeDefinition { Type = typeof(double), Name = "double" },
            new TypeDefinition { Type = typeof(nint), Name = "nint" },
            new TypeDefinition { Type = typeof(nuint), Name = "nuint" },
            new TypeDefinition { Type = typeof(char), Name = "char" },
            new TypeDefinition { Type = typeof(string), Name = "string" },
            new TypeDefinition { Type = typeof(decimal), Name = "decimal" }
        };

        public void Initialize(GeneratorInitializationContext context)
        {
            var builder = new StringBuilder($@"
using System;

namespace {GeneratorConfig.Namespace}
{{
    public static partial class SystemTypeExtensions
    {{
        public static bool TryGetBuiltinName(this Type self, out string name)
        {{
            name = default;

            switch (self.Name)
            {{");

            foreach (var type in _builtinTypes)
            {
                builder.Append($@"
                case ""{type.Type.Name}"": name = ""{type.Name}""; break;");
            }

            builder.Append(@"
            }

            return !string.IsNullOrWhiteSpace(name);
        }

        public static string GetNiceName(this Type self)
        {
            if (self.TryGetBuiltinName(out var name))
                return name;

            return self.Name;
        }

        public static string GetNiceFullName(this Type self)
        {
            if (self.TryGetBuiltinName(out var name))
                return name;

            return self.FullName;
        }
    }
}
");
            context.RegisterForPostInitialization((i) => i.AddSource("SystemTypeExtensions", builder.ToString()));
        }

        public void Execute(GeneratorExecutionContext context)
        {
        }

        private struct TypeDefinition
        {
            public Type Type;
            public string Name;
        }
    }
}