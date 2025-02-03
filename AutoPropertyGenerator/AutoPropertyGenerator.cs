using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AutoPropertyGenerator
{
    [Generator]

    public class AutoPropertyGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Ajoute un analyseur de syntaxe pour détecter les classes avec l'attribut [AutoProperty]
            context.RegisterForSyntaxNotifications(() => new ClassSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not ClassSyntaxReceiver receiver)
                return;

            foreach (var classDeclaration in receiver.Classes)
            {
                var className = classDeclaration.Identifier.Text;
                var generatedCode = new StringBuilder();

                generatedCode.AppendLine("using System;");
                generatedCode.AppendLine($"public partial class {className}");
                generatedCode.AppendLine("{");

                foreach (var field in classDeclaration.Members.OfType<FieldDeclarationSyntax>())
                {
                    foreach (var variable in field.Declaration.Variables)
                    {
                        var fieldType = field.Declaration.Type.ToString();
                        var fieldName = variable.Identifier.Text;

                        // Convertir le champ en propriété avec get; set;
                        generatedCode.AppendLine($"    public {fieldType} {fieldName} {{ get; set; }}");
                    }
                }

                generatedCode.AppendLine("}");

                // Ajoute la classe générée au projet
                context.AddSource($"{className}_AutoProperties.g.cs", SourceText.From(generatedCode.ToString(), Encoding.UTF8));
            }
        }
    }

    // Analyseur de syntaxe pour détecter les classes avec [AutoProperty]
    class ClassSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> Classes { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDecl &&
                classDecl.AttributeLists.Any(attr => attr.Attributes.Any(a => a.Name.ToString() == "AutoProperty")))
            {
                Classes.Add(classDecl);
            }
        }
    }
}