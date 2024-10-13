using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public record struct NamedArg(Identifier identifier, IExpr expr) : INode
    {
        public TextRange Range => new TextRange(identifier.Range.Start, expr.Range.End);
    }

    public class StructConstructorExpr : IExpr
    {
        public TextRange Range { get; }

        public Type.Identifier Type { get; }
        public List<NamedArg> NamedArgs { get; }

        public StructConstructorExpr(Type.Identifier type, List<NamedArg> namedArgs, TextRange range)
        {
            Type = type;
            NamedArgs = namedArgs;
            Range = range;
        }

        public AST.IExpr? ToAST(Func func, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            var type = Type.ToAST(func.Proto.ModuleFile.Module);
            if (type == null)
                return null;

            var args = new List<(AST.Field Field, AST.IExpr Value)>();
            foreach (var namedArg in NamedArgs)
            {
                if (type is AST.Type.Struct @struct)
                {
                    var field = @struct.Fields.FirstOrDefault(f => f.Name == namedArg.identifier.Name);
                    if (field == null)
                    {
                        Logger.InvalidStructField(func.Proto.ModuleFile, type, namedArg.identifier);
                        return null;
                    }

                    var value = namedArg.expr.ToAST(func, globals, scopedSymbols);
                    if (value == null)
                        return null;

                    args.Add((field, value));
                }
                else
                    return null;
            }

            return new AST.StructConstructorExpr(type, args);
        }
    }
}
