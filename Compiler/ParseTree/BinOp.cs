using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public enum BinOp
    {
        Access,
        Mul,
        Div,
        Add,
        Sub,
        Lt,
        Le,
        Gt,
        Ge,
        Assign,
    }

    public static class BinOpExtensions
    {
        public static bool LeftAssociative(this BinOp binOp) => binOp switch
        {
            BinOp.Assign => false,
            _ => true,
        };

        public static int GetPrecedence(this BinOp binOp) => binOp switch
        {
            BinOp.Access => 6,
            BinOp.Mul or BinOp.Div => 4,
            BinOp.Add or BinOp.Sub => 3,
            BinOp.Lt or BinOp.Le or BinOp.Gt or BinOp.Ge => 2,
            BinOp.Assign => 1,
            _ => throw new NotImplementedException(),
        };

        public static string ToSentenceFormat(this BinOp binOp) => binOp switch
        {
            BinOp.Access => "access",
            BinOp.Mul => "multiply",
            BinOp.Div => "divide",
            BinOp.Add => "add",
            BinOp.Sub => "subtract",
            BinOp.Lt => "compare",
            BinOp.Le => "compare",
            BinOp.Gt => "compare",
            BinOp.Ge => "compare",
            BinOp.Assign => "assign"
        };
    }

    public record struct BinOpNode(BinOp Op, TextRange Range)
    {
        private static BinOp? GetBinOp(TokenType type) => type switch
        {
            TokenType.Dot => BinOp.Access,
            TokenType.Add => BinOp.Add,
            TokenType.Sub => BinOp.Sub,
            TokenType.Mul => BinOp.Mul,
            TokenType.Div => BinOp.Div,
            TokenType.Gt => BinOp.Gt,
            TokenType.Ge => BinOp.Ge,
            TokenType.Lt => BinOp.Lt,
            TokenType.Le => BinOp.Le,
            TokenType.Assign => BinOp.Assign,
            _ => null,
        };

        public static BinOpNode? FromToken(Token token) => GetBinOp(token.Type) == null ? null : new BinOpNode(GetBinOp(token.Type)!.Value, token.Range);
    }
}
