using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Obsoletion
{
    public class FileProcessor
    {
        private readonly Parser _parser;
        public FileProcessor(Parser parser)
        {
            _parser = parser;
        }

        public IEnumerable<Result> ProcessFile(string file)
        {
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllTextAsync(file).Result);
            var root = (CompilationUnitSyntax) tree.GetRoot();

            var results = new List<Result>();

            if (!(root.Members.FirstOrDefault() is NamespaceDeclarationSyntax nsSyntax))
            {
                return results;
            }

            foreach (var classSyntax in nsSyntax.Members.OfType<ClassDeclarationSyntax>())
            {
                results.AddRange(_parser.CreateResultForType(classSyntax));

                foreach (var member in classSyntax.Members)
                {
                    switch (member)
                    {
                        case MethodDeclarationSyntax method:
                        {
                            results.AddRange(_parser.CreateResultForMember(method.AttributeLists, method.Identifier));
                            break;
                        }
                        case PropertyDeclarationSyntax property:
                        {
                            results.AddRange(_parser.CreateResultForMember(property.AttributeLists, property.Identifier));
                            break;
                        }
                        case FieldDeclarationSyntax field:
                        {
                            results.AddRange(_parser.CreateResultForMember(field.AttributeLists, field.Declaration.Variables.First().Identifier));
                            break;
                        }
                        case ConstructorDeclarationSyntax constructor:
                        {
                            results.AddRange(_parser.CreateResultForMember(constructor.AttributeLists, constructor.Identifier));
                            break;
                        }
                    }
                }
            }

            foreach (var structSyntax in nsSyntax.Members.OfType<StructDeclarationSyntax>())
            {
                results.AddRange(_parser.CreateResultForType(structSyntax));

                foreach (var member in structSyntax.Members)
                {
                    switch (member)
                    {
                        case MethodDeclarationSyntax method:
                        {
                            results.AddRange(_parser.CreateResultForMember(method.AttributeLists, method.Identifier));
                            break;
                        }
                        case PropertyDeclarationSyntax property:
                        {
                            results.AddRange(_parser.CreateResultForMember(property.AttributeLists, property.Identifier));
                            break;
                        }
                        case FieldDeclarationSyntax field:
                        {
                            results.AddRange(_parser.CreateResultForMember(field.AttributeLists, field.Declaration.Variables.First().Identifier));
                            break;
                        }
                        case ConstructorDeclarationSyntax constructor:
                        {
                            results.AddRange(_parser.CreateResultForMember(constructor.AttributeLists, constructor.Identifier));
                            break;
                        }
                    }
                }
            }

            foreach (var interfaceSyntax in nsSyntax.Members.OfType<InterfaceDeclarationSyntax>())
            {
                results.AddRange(_parser.CreateResultForType(interfaceSyntax));

                foreach (var member in interfaceSyntax.Members)
                {
                    switch (member)
                    {
                        case MethodDeclarationSyntax method:
                        {
                            results.AddRange(_parser.CreateResultForMember(method.AttributeLists, method.Identifier));
                            break;
                        }
                        case PropertyDeclarationSyntax property:
                        {
                            results.AddRange(_parser.CreateResultForMember(property.AttributeLists, property.Identifier));
                            break;
                        }
                        case FieldDeclarationSyntax field:
                        {
                            results.AddRange(_parser.CreateResultForMember(field.AttributeLists, field.Declaration.Variables.First().Identifier));
                            break;
                        }
                        case ConstructorDeclarationSyntax constructor:
                        {
                            results.AddRange(_parser.CreateResultForMember(constructor.AttributeLists, constructor.Identifier));
                            break;
                        }
                    }
                }
            }

            foreach (var enumSyntax in nsSyntax.Members.OfType<EnumDeclarationSyntax>())
            {
                results.AddRange(_parser.CreateResultForType(enumSyntax));
            }

            return results;
        }
    }
}
