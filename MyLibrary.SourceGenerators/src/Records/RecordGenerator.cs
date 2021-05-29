using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace MyLibrary.Records.SourceGen
{
    [Generator]
    public class RecordGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            //context.RegisterForSyntaxNotifications(() => new UnionSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
        }
    }
}