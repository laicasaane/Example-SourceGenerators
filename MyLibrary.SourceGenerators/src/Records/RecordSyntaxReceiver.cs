using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace MyLibrary.Records.SourceGen
{
    public class RecordSyntaxReceiver : ISyntaxContextReceiver
    {
        //public List<UnionDefinition> Definitions { get; } = new List<UnionDefinition>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (!(context.Node is StructDeclarationSyntax dec) ||
                dec.AttributeLists.Count <= 0)
                return;

            //if (UnionDefinition.TryCreate(context, dec, out var def))
            //    this.Definitions.Add(def);
        }
    }
}