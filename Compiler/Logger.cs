using Compiler.ParseTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public record struct Underline(TextRange Range, string Text = "");
    public record struct Log(string FileName, string Input, string Title, Underline Cause, List<Underline> Hints)
    {
        public Log(Project project, string fileName, string title, Underline cause, List<Underline>? hints = null) : this(fileName, project.Files[fileName], title, cause, hints ?? new List<Underline>())
        {
        }

        public Log(ModuleFile moduleFile, string title, Underline cause, List<Underline>? hints = null) : this(moduleFile.Module.Project, moduleFile.FileName, title, cause, hints ?? new List<Underline>())
        {
        }

        public Log(Lexer lexer, string title, Underline cause, List<Underline>? hints = null) : this(lexer.FileName, lexer.Input, title, cause, hints ?? new List<Underline>())
        {
        }
    }


    public static class Logger
    {
        public static bool CompilationFailed { get; private set; } = false;

        private static string GetLineAt(string input, int pos)
        {
            while (pos > 0 && input[pos - 1] != '\n')
                pos--;

            int length = 0;
            while (pos + length < input.Length && input[pos + length] != '\n')
                length++;

            return input.Substring(pos, length);
        }

        public static void Error(string message)
        {
            CompilationFailed = true;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("error");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(": ");
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Error(in Log log)
        {
            CompilationFailed = true;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("error");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(": " + log.Title);

            var lastLine = Math.Max(log.Cause.Range.End.Line, log.Hints.Select(underline => underline.Range.End.Line).DefaultIfEmpty().Max());
            var margin = new string(' ', Math.Max(lastLine.ToString().Length, 2));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(margin.Substring(margin.Length - 1) + "-->");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($" {log.FileName}:{log.Cause.Range.Start.Line}:{log.Cause.Range.Start.Col}");

            var taggedUnderlinesGroups = log.Hints
                .Select(hint => (isCause: false, underline: hint))
                .Append((isCause: true, underline: log.Cause))
                .GroupBy(a => a.underline.Range.Start.Line)
                .OrderBy(g => g.Key)
                .Select(g => new { Line = g.Key, TaggedUnderlines = g.OrderByDescending(a => a.underline.Range.Start.Col) });
                
          
            var openUnderlines = new List<Underline>();
            foreach (var taggedUnderlineGroup in taggedUnderlinesGroups)
            {
                var lineContent = GetLineAt(log.Input, taggedUnderlineGroup.TaggedUnderlines.First().underline.Range.Start.Pos);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(taggedUnderlineGroup.Line.ToString().Center(margin.Length) + "| ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(lineContent);

                foreach (var (isCause, underline) in taggedUnderlineGroup.TaggedUnderlines)
                {
                    var tabsCount = lineContent.Take(underline.Range.Start.Col - 1).Count(c => c == '\t');
                    if (underline.Range.Start != underline.Range.End)
                        openUnderlines.Add(underline);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(margin + "| ");
                    if (isCause)
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(new string('\t', tabsCount) + new string(' ', underline.Range.Start.Col - 1 - tabsCount));
                    Console.Write(new string(isCause ? '^' : '-', underline.Range.End.Col - underline.Range.Start.Col));
                    Console.Write(' ');
                    Console.WriteLine(underline.Text);
                }   
            }
        }

        public static void UnexpectedToken(Lexer lexer, TokenType[] expectedTokens)
        {
            var causeMessage = expectedTokens.Length == 1
                ? $"expected token `{expectedTokens[0].ToFormat()}` but got `{lexer.Token.Type.ToFormat()}`"
                : $"expected one of {string.Join(", ", expectedTokens.Select(token => $"`{token.ToFormat()}`"))} but got `{lexer.Token.Type.ToFormat()}`";

            Error(new Log(lexer, "unexpected token", new Underline(lexer.Token.Range, causeMessage)));
        }

        public static void MismatchedTypesReturnVoid(ModuleFile moduleFile, Func func, AST.IType returnType, ReturnExpr returnExpr)
        {
            Error(new Log(moduleFile, "mismatched types", new(func.Proto.ReturnType.GetRange(), $"expected `{returnType}` because it is the return type"), new List<Underline> { new(returnExpr.Range, "found `void`") }));
        }


        public static void MismatchedTypesIf(ModuleFile moduleFile, AST.IType type, IfExpr ifExpr)
        {
            Error(new Log(moduleFile, "mismatched types", new(ifExpr.Condition.Range, $"expected `bool` because it's the condition of an if expression, found `{type}`")));
        }

        public static void MismatchedTypesOp(ModuleFile moduleFile, AST.IType expected, AST.IType found, BinOpNode opNode)
        {
            Error(new Log(moduleFile, "mismatched types", new(opNode.Range, $"cannot ${opNode.Op.ToSentenceFormat()} the type `${found} to the type `${expected}`")));
        }

        public static void MismatchedTypesVarDecl(ModuleFile moduleFile, AST.IType expected, AST.IType found, VarDeclStatement varDecl)
        {
            Error(new Log(moduleFile, "mismatched types", new(varDecl.Type.GetRange(), $"expected `{expected}`"), new List<Underline> { new(varDecl.Value!.Range, $"found `{found}`") }));
        }

        public static void MismatchedTypesIfElse(ModuleFile moduleFile, AST.IType thenType, AST.IType elseType, IExpr @else)
        {
            Error(new Log(moduleFile, "mismatched types", new(@else.Range, $"expected `{thenType}` because it's the type of the then branch, found `{elseType}`")));
        }

        public static void MismatchedTypesNoElse(ModuleFile moduleFile, AST.IType type, IfExpr ifExpr)
        {
            Error(new Log(moduleFile, "mismatched types", new(ifExpr.Range, $"expected `void` because there is no else branch, found `{type}`")));
        }

        public static void ValueNotFoundInScope(ModuleFile moduleFile, Identifier identifier)
        {
            Error(new Log(moduleFile, $"cannot find value `{identifier.Name}` in this scope", new(identifier.Range)));
        }

        public static void FuncNotFound(ModuleFile moduleFile, Identifier identifier)
        {
            Error(new Log(moduleFile, $"cannot find function `{identifier.Name}` in this scope", new(identifier.Range)));
        }

        public static void MethodNotFound(ModuleFile moduleFile, AST.IType type, Identifier identifier)
        {
            Error(new Log(moduleFile, $"cannot find method `{identifier.Name}` on the type `{type}`", new(identifier.Range)));
        }

        public static void FuncPrivate(ModuleFile moduleFile, Identifier identifier, Func func)
        {
            var hints = func.Proto.ModuleFile.Module.Project == moduleFile.Module.Project ?
                new List<Underline> { new(func.Proto.Identifier.Range, "consider adding `pub`") }
                : null;

            Error(new Log(moduleFile, $"function `{identifier.Name}` is private", new(identifier.Range), hints));
        }

        public static void TypePrivate(ModuleFile moduleFile, Identifier identifier, TypeDecl typeDecl)
        {
            var hints = typeDecl.ModuleFile.Module.Project == moduleFile.Module.Project ?
                new List<Underline> { new(typeDecl.Identifier.Range, "consider adding `pub`") }
                : null;

            Error(new Log(moduleFile, $"type `{identifier.Name}` is private", new(identifier.Range), hints));
        }

        public static void TypeNotFound(ModuleFile moduleFile, Identifier identifier)
        {
            Error(new Log(moduleFile, $"cannot find type `{identifier.Name}` in this scope", new(identifier.Range)));
        }

        public static void InvalidArgsCount(ModuleFile moduleFile, int paramsCount, CallExpr callExpr)
        {
            Error(new Log(moduleFile, "invalid arguments count", new(callExpr.Range, $"expected {paramsCount} arguments but got {callExpr.Args.Count}")));
        }

        public static void ExpectedTypeOrValueVarDecl(ModuleFile moduleFile, VarDeclStatement varDecl)
        {
            Error(new Log(moduleFile, "expected type or value", new(varDecl.Range, "expected either a type or a value, found neither")));
        }

        public static void InvalidStructField(ModuleFile moduleFile, AST.IType type, Identifier fieldIdentifier)
        {
            Error(new Log(moduleFile, $"invalid field `{fieldIdentifier.Name}`", new(fieldIdentifier.Range, $"field `{fieldIdentifier.Name}` does not exist in the struct `{type}`")));
        }
    }
}


/*

interface Func<T>
{
    public T Call();
}

 

*/