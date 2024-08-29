using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public record struct Underline(string Text, TextRange Range);
    public record struct Log(string FileName, string Input, string Title, Underline Cause, List<Underline> Hints)
    {
        public Log(Project project, string fileName, string title, Underline cause, List<Underline>? hints = null) : this(fileName, project.Files[fileName], title, cause, hints ?? new List<Underline>())
        {
        }

        public Log(Lexer lexer, string title, Underline cause, List<Underline>? hints = null) : this(lexer.FileName, lexer.Input, title, cause, hints ?? new List<Underline>())
        {
        }
    }


    public static class Logger
    {
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("error");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(": ");
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Error(in Log log)
        {
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
                : $"expected one of `{string.Join(',', expectedTokens.Select(token => token.ToFormat()))}` but got `{lexer.Token.Type.ToFormat()}`";

            Error(new Log(lexer, "unexpected token", new Underline(causeMessage, lexer.Token.Range)));
        }
    }
}


/*

interface Func<T>
{
    public T Call();
}

 

*/