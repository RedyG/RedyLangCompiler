﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public enum BinOp
    {
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

        public static AST.Type GetType(this BinOp binOp) => binOp switch
        {
            BinOp.Mul => new AST.Type.I32(),
            BinOp.Div => new AST.Type.I32(),
            BinOp.Add => new AST.Type.I32(),
            BinOp.Sub => new AST.Type.I32(),
            BinOp.Lt => new AST.Type.Bool(),
            BinOp.Le => new AST.Type.Bool(),
            BinOp.Gt => new AST.Type.Bool(),
            BinOp.Ge => new AST.Type.Bool(),
            BinOp.Assign => new AST.Type.Void(),
            _ => throw new NotImplementedException(),
        };

        public static int GetPrecedence(this BinOp binOp) => binOp switch
        {
            BinOp.Mul => 4,
            BinOp.Div => 4,
            BinOp.Add => 3,
            BinOp.Sub => 3,
            BinOp.Lt => 2,
            BinOp.Le => 2,
            BinOp.Gt => 2,
            BinOp.Ge => 2,
            BinOp.Assign => 1,
            _ => throw new NotImplementedException(),
        };

        public static string ToSentenceFormat(this BinOp binOp) => binOp switch
        {
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
            TokenType.Add => BinOp.Add,
            TokenType.Sub => BinOp.Sub,
            TokenType.Lt => BinOp.Lt,
            TokenType.Assign => BinOp.Assign,
            _ => null,
        };

        public static BinOpNode? FromToken(Token token) => GetBinOp(token.Type) == null ? null : new BinOpNode(GetBinOp(token.Type)!.Value, token.Range);
    }
}