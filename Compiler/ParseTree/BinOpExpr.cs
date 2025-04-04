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

        private static AST.IType GetTypeAST(AST.IType left, BinOp binOp, AST.IType right) => (left, binOp, right) switch {
            (var type, BinOp.Add or BinOp.Sub or BinOp.Mul or BinOp.Div, _) => type,
            (_, BinOp.Lt or BinOp.Le or BinOp.Gt or BinOp.Ge, _) => new AST.IType.Bool(),
            (_, BinOp.Assign, _) => new AST.IType.Void(),
        };

        public AST.IExpr? ToAST(Decl decl, GlobalSymbols globals, ScopedSymbols scopedSymbols, bool ignored = false)
        {
            if (OpNode.Op == BinOp.Access)
            {
                var leftExpr = Left.ToAST(decl, globals, scopedSymbols);
                if (leftExpr == null)
                    return null;


                if (Right is Identifier identifier)
                {
                    if (leftExpr.Type.DeRef().ToConcrete() is AST.IType.Struct @struct)
                    {
                        var field = @struct.Fields.FirstOrDefault(f => f.Name == identifier.Name);
                        if (field == null)
                        {
                            Logger.InvalidStructField(decl.ModuleFile, @struct, identifier);
                            return null;
                        }

                        return new AST.AccessExpr(leftExpr, field);
                    }
                }

                Logger.InvalidMemberAccess(decl.ModuleFile, this);
                return null;
            }

            if (OpNode.Op == BinOp.StaticAccess)
            {
                if (Left is Identifier identifier)
                {
                    var type = decl.ModuleFile.Module.GetType(identifier);
                    if (type is not null)
                    {
                        if (Right is Identifier field)
                        {
                            var func = globals.Project.GetMethod(type.GetType(globals, scopedSymbols), field.Name.ToString());
                            if (func == null)
                            {
                                Logger.MethodNotFound(decl.ModuleFile,type.GetType(globals, scopedSymbols), field);
                                return null;
                            }
                            return new AST.FuncExpr(func.Proto.GetFuncPtrType(), func);
                        }

                        return null; // TODO: error
                    }
                }

                return null; // TODO: modules
            }
                 
            var left = Left.ToAST(decl, globals, scopedSymbols);
            var right = Right.ToAST(decl, globals, scopedSymbols);

            if (left == null || right == null)
                return null;

            if (!left.Type.Equals(right.Type))
            {
                Logger.MismatchedTypesOp(decl.ModuleFile, left.Type, right.Type, OpNode);
                return null;
            }

            return new AST.BinOpExpr(GetTypeAST(left.Type, OpNode.Op, right.Type), left, OpNode.Op, right);
        }
    }
}
