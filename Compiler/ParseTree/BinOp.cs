using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public enum BinOp
    {
        Add,
        Sub,
        Lt,
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
            BinOp.Add => 3,
            BinOp.Sub => 3,
            BinOp.Lt => 2,
            BinOp.Assign => 1,
            _ => throw new NotImplementedException(),
        };
    }

    public record struct BinOpNode(BinOp Op, TextRange Range)
    {
        private static BinOp GetBinOp(TokenType type) => type switch
        {
            TokenType.Add => BinOp.Add,
            TokenType.Sub => BinOp.Sub,
            TokenType.Lt => BinOp.Lt,
            TokenType.Assign => BinOp.Assign,
            _ => throw new NotImplementedException()
        };

        public static BinOpNode? FromToken(Token token) => new BinOpNode(GetBinOp(token.Type), token.Range);
    }
}
