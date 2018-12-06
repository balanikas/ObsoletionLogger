using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Obsoletion
{
    public class Parser
    {
        readonly HashSet<string> _legacySymbols;

        public Parser(string path)
        {
            _legacySymbols = File.ReadAllLines(path).ToHashSet();
        }

        public IEnumerable<Result> CreateResultForType(BaseTypeDeclarationSyntax typeSyntax)
        {
            return typeSyntax.AttributeLists.SelectMany(x => x.Attributes).Where(x => x.Name.ToString() == "Obsolete")
                .Select(x => CreateResultForType(x, typeSyntax.Identifier));
        }

        Result CreateResultForType(AttributeSyntax attribute, SyntaxToken identifier)
        {
            return new Result
            {
                
                Symbol = GetNamespace(identifier) + "." + identifier,
                Type = GetSymbolType(identifier),
                Recommendation = GetRecommendation(identifier),
                ObsoletionDate = GetDate(attribute),
                Message = GetMessage(attribute)
            };
        }


        public IEnumerable<Result> CreateResultForMember(SyntaxList<AttributeListSyntax> attributes, SyntaxToken identifier)
        {
            return attributes.SelectMany(x => x.Attributes).Where(x => x.Name.ToString() == "Obsolete")
                .Select(x => CreateResultForMember(x, identifier));
        }

        Result CreateResultForMember(AttributeSyntax attribute, SyntaxToken identifier)
        {
            return new Result
            {
                Symbol = GetNamespace(identifier) + "." +  GetEncapsulatingType(identifier).Identifier + "." + identifier,
                Type = GetSymbolType(identifier),
                Recommendation = GetRecommendation(identifier),
                ObsoletionDate = GetDate(attribute),
                Message = GetMessage(attribute),

            };
        }

        TypeDeclarationSyntax GetEncapsulatingType(SyntaxToken identifier)
        {
            var parent = identifier.Parent;
            while (!(parent is TypeDeclarationSyntax))
            {
                parent = parent.Parent;
            }

            return (TypeDeclarationSyntax)parent;
        }

        string GetNamespace(SyntaxToken identifier)
        {
            var parent = identifier.Parent;
            while (!(parent is NamespaceDeclarationSyntax))
            {
                parent = parent.Parent;
            }

            return ((NamespaceDeclarationSyntax)parent).Name.ToString();
        }

        string GetRecommendation(SyntaxToken identifier)
        {
            if (GetNamespace(identifier)
                .EndsWith(".Internal", StringComparison.InvariantCultureIgnoreCase))
            {
                return "internal - don't use";
            }

            //unused feature ATM
            //if (_legacySymbols.Contains(GetNamespace(identifier)) || _legacySymbols.Contains(GetSymbolType(identifier)))
            //{
            //    return "legacy - don't use";
            //}

            return "";

        }
        
        string GetSymbolType(SyntaxToken identifier)
        {
            switch (identifier.Parent)
            {
                case ClassDeclarationSyntax _:
                    return "class";
                case InterfaceDeclarationSyntax _:
                    return "interface";
                case StructDeclarationSyntax _ :
                    return "struct";
                case EnumDeclarationSyntax _:
                    return "enum";
                case MethodDeclarationSyntax _:
                    return "method";
                case PropertyDeclarationSyntax _:
                    return "property";
                case FieldDeclarationSyntax _:
                    return "field";
                case ConstructorDeclarationSyntax _:
                    return "constructor";
                case VariableDeclaratorSyntax x:
                    if (x.Parent.Parent is FieldDeclarationSyntax) return "field";
                    throw new InvalidOperationException();
                default:
                    throw new InvalidOperationException();
            }
        }

        string GetMessage(AttributeSyntax attribute)
        {
            return attribute.ArgumentList != null ? attribute.ArgumentList.Arguments.First().ToString() : "no message";
        }

        string GetDate(AttributeSyntax attribute)
        {
            if (attribute.ArgumentList == null)
            {
                return "";
            }

            var message = attribute.ArgumentList.Arguments.First().ToString();
            var month =  -1;
            int year;

            try
            {
                year = int.Parse(Regex.Match(message, @"\d{4}").Value);
            }
            catch (Exception)
            {
                return "";
            }
           
            foreach (var part in message.Split(" "))
            {
                if (DateTime.TryParse("1." + part + " 1900", out var date))
                {
                    month = date.Month;
                }
            }

            if (month == -1)
            {
                return "";
            }

            return new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToShortDateString();
        }
    }
}
