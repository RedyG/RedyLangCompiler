using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class BinOpExpr : IExpr
    {
        public TextRange Range { get; }
        public IExpr Left { get; }
        public BinOpNode OpNode { get; }
        public IExpr Right { get; }

        public BinOpExpr(TextRange range, IExpr left, BinOpNode opNode, IExpr right)
        {
            Range = range;
            Left = left;
            OpNode = opNode;
            Right = right;
        }

        private static AST.Type GetTypeAST(AST.Type left, BinOp binOp, AST.Type right) => (left, binOp, right) switch {
            (var type, BinOp.Add or BinOp.Sub, _) => type,
            (_, BinOp.Lt, _) => new AST.Type.Bool(),
            (_, BinOp.Assign, var type) => type,
            _ => throw new NotImplementedException()
        };

        public AST.IExpr? ToAST(Func func, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            if (OpNode.Op == BinOp.Access)
            {
                var leftExpr = Left.ToAST(func, globals, scopedSymbols);
                if (leftExpr == null)
                    return null;


                if (Right is Identifier identifier) {
                    if (leftExpr.Type is AST.Type.Struct @struct)
                    {
                        var field = @struct.Fields.FirstOrDefault(f => f.Name == identifier.Name);
                        if (field == null)
                        {
                            Logger.InvalidStructField(func.Proto.ModuleFile, @struct, identifier);
                            return null;
                        }

                        return new AST.AccessExpr(leftExpr, field);
                    }
                }

                return null; // TODO: error this, just lazy rn
            }

            var left = Left.ToAST(func, globals, scopedSymbols);
            var right = Right.ToAST(func, globals, scopedSymbols);

            if (left == null || right == null)
                return null;

            if (left.Type != right.Type)
            {
                Logger.MismatchedTypesOp(func.Proto.ModuleFile, left.Type, right.Type, OpNode);
                return null;
            }

            return new AST.BinOpExpr(GetTypeAST(left.Type, OpNode.Op, right.Type), left, OpNode.Op, right);
        }
    }
}
