using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ParseTree;

namespace Compiler
{
    public class Parser
    {
        private readonly struct Item
        {
            public IExpr? Expr { get; } = null;
            public IStatement? Statement { get; } = null;

            public Item(IExpr expr)
            {
                Expr = expr;
            }

            public Item(IStatement statement)
            {
                Statement = statement;
            }
        }

        private Item ParsePrimary()
        {
            switch (lexer.Token.Type)
            {
                case TokenType.IntLiteral:
                    var intExpr = new IntExpr(lexer.Token.Range, int.Parse(lexer.Token.Content.ToString().Replace("_", "")));
                    lexer.Consume();
                    return new Item(intExpr);
                case TokenType.Identifier:
                    return new Item(ParseIdentifier());
                case TokenType.LParen:
                    lexer.Consume();
                    var expr = ParseExpr();
                    if (lexer.Token.Type != TokenType.RParen)
                        Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.RParen });
                    lexer.Consume();
                    return new Item(expr);
                default:
                    Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.IntLiteral, TokenType.Identifier, TokenType.LParen });
                    return new Item(new IntLiteral(new TextRange(), 0));
            }
        }

        private Item ParsePostfix()
        {
            return ParsePrimary();
        }

        private Item ParseUnary()
        {
            return ParsePostfix();
        }

        private VarDeclStatement ParseVarDecl()
        {
            var start = lexer.Token.Range.Start;
            lexer.Consume();
            var identifier = ParseIdentifier();

            if (lexer.Token.Type != TokenType.Colon)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Colon });
            lexer.Consume();

            var type = ParseType();
            IExpr? value = null;
            if (lexer.Token.Type == TokenType.Assign)
            {
                lexer.Consume();
                value = ParseExpr();
            }

            if (lexer.Token.Type != TokenType.Semicolon)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Semicolon });
            var end = lexer.Token.Range.End;
            lexer.Consume();

            return new VarDeclStatement(new TextRange(start, end), type, identifier, value);
        }

        private Item ParseItem(int precedence = 1)
        {
            if (lexer.Token.Type == TokenType.Var)
                return new Item(ParseVarDecl());

            var expr = ParseUnary();
            while (true)
            {
                var opNode = BinOpNode.FromToken(lexer.Token);
                if (opNode == null)
                    break;
            }

            return new Item(expr);
        }

        private IExpr ParseExpr() => ParseItem() switch
        {
            Item { Expr: var expr, Statement: null } => expr!,
            _ => throw new NotImplementedException()
        };

        private ParseTree.Type ParseType()
        {
            return new ParseTree.Type.Identifier(ParseIdentifier());
        }

        private VarDeclStatement ParseParam()
        {
            var identifer = ParseIdentifier();
            if (lexer.Token.Type != TokenType.Colon)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Colon });
            lexer.Consume();
            var type = ParseType();
            var value = lexer.Token.Type == TokenType.Assign ? ParseExpr() : null;
            return new VarDeclStatement(new TextRange(identifer.Range.Start, type.GetRange().End), type, identifer, value);
        }

        private Identifier ParseIdentifier()
        {
            if (lexer.Token.Type != TokenType.Identifier)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Identifier });

            var identifier = new Identifier(lexer.Token.Range, lexer.Token.Content);
            lexer.Consume();
            return identifier;
        }

        private Proto ParseProto(VisibilityNode visibility)
        {
            lexer.Consume(); // consume fn
            var identifier = ParseIdentifier();

            var @params = new List<VarDeclStatement>();
            if (lexer.Token.Type != TokenType.LParen)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.LParen });
            lexer.Consume();
            while (lexer.Token.Type != TokenType.RParen)
            {
                @params.Add(ParseParam());
                if (lexer.Token.Type is not TokenType.Comma or TokenType.RParen)
                {
                    Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Comma, TokenType.RParen });
                    break;
                }
            }
            lexer.Consume(); // consume rparen

            ParseTree.Type returnType = BuiltInTypes.Void;
            if (lexer.Token.Type == TokenType.RArrow)
            {
                lexer.Consume();
                returnType = ParseType();
            }

            return new Proto(visibility, identifier, @params, returnType);
        }

        private Func ParseFunc(VisibilityNode visibility)
        {
            var proto = ParseProto(visibility);
            if (lexer.Token.Type != TokenType.LCurly)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.LCurly });
            var body = ParseExpr();
            return new Func(proto, body);
        }

        private VisibilityNode ParseVisiblity()
        {
            if (lexer.Token.Type == TokenType.Pub)
            {
                var visibility = new VisibilityNode(lexer.Token.Range, Visibility.Pub);
                lexer.Consume();
                return visibility;
            }

            return new VisibilityNode(new TextRange(), Visibility.Priv);
        }

        public Module Parse(Lexer lexer)
        {
            this.lexer = lexer;
            lexer.Consume();

            var module = new Module();

            while (lexer.Token.Type != TokenType.Unknown)
            {
                var visiblity = ParseVisiblity();
                switch (lexer.Token.Type)
                {
                    case TokenType.Fn:
                        module.Funcs.Add(ParseFunc(visiblity));
                        break;
                    default:
                        Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Fn });
                        while (!lexer.Token.Type.IsModuleItem())
                            lexer.Consume();
                        break;
                }
            }

            return module;

        }

        public Parser()
        {
            lexer = new Lexer("", "");
        }

        private Lexer lexer;
    }
}
