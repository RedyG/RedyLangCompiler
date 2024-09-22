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

        private BlockExpr ParseBlock()
        {
            var start = lexer.Token.Range.Start;
            lexer.Consume();
            var statements = new List<IStatement>();
            IExpr? lastExpr = null;
            while (lexer.Token.Type != TokenType.RCurly)
            {
                if (lastExpr != null)
                    Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.RCurly });
                var item = ParseItem();
                switch (item)
                {
                    case Item { Expr: var expr, Statement: null }:
                        if (lexer.Token.Type == TokenType.Semicolon)
                        {
                            statements.Add(new ExprStatement(new TextRange(expr!.Range.Start, lexer.Token.Range.End), expr));
                            lexer.Consume();
                            break;
                        }

                        if (expr!.IsBlock())
                        {
                            statements.Add(new ExprStatement(expr.Range, expr));
                            break;
                        }

                        lastExpr = expr;
                        break;
                    case Item { Expr: null, Statement: var statement }:
                        statements.Add(statement);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            var block = new BlockExpr(new TextRange(start, lexer.Token.Range.End), statements, lastExpr);
            lexer.Consume();
            return block;
        }

        private IfExpr ParseIf()
        {
            var start = lexer.Token.Range.Start;
            lexer.Consume();

            var condition = ParseExpr();

            if (lexer.Token.Type != TokenType.LCurly)
            {
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.LCurly });
                throw new Exception(); // TODO: idk how to handle this
            }

            var then = ParseExpr();

            if (lexer.Token.Type != TokenType.Else)
                return new IfExpr(new TextRange(start, then.Range.End), condition, then);

            lexer.Consume();
            var @else = ParseExpr();

            return new IfExpr(new TextRange(start, @else.Range.End), condition, then, @else);
        }

        private ReturnExpr ParseReturn()
        {
            var returnRange = lexer.Token.Range;
            lexer.Consume();

            if (lexer.Token.Type == TokenType.Semicolon)
                return new ReturnExpr(returnRange, null);

            var expr = ParseExpr();

            return new ReturnExpr(new TextRange(returnRange.Start, expr.Range.End), expr);
        }

        private IExpr ParsePrimary()
        {
            switch (lexer.Token.Type)
            {
                case TokenType.IntLiteral:
                    var intExpr = new IntExpr(lexer.Token.Range, int.Parse(lexer.Token.Content.ToString().Replace("_", "")));
                    lexer.Consume();
                    return intExpr;
                case TokenType.Identifier:
                    return ParseIdentifier();
                case TokenType.LParen:
                {
                    var start = lexer.Token.Range.Start;
                    lexer.Consume();
                    var expr = ParseExpr();
                    if (lexer.Token.Type != TokenType.RParen)
                        Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.RParen });
                    var parenExpr = new ParenExpr(new TextRange(start, lexer.Token.Range.End), expr);
                    lexer.Consume();
                    return parenExpr;
                }
                case TokenType.LCurly:
                    return ParseBlock();
                case TokenType.If:
                    return ParseIf();
                case TokenType.Return:
                    return ParseReturn();
                default:
                    Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.IntLiteral, TokenType.Identifier, TokenType.LParen, TokenType.LCurly, TokenType.If, TokenType.Return });
                    throw new Exception(); // TODO: idk how to handle this
            }
        }

        private CallExpr ParseArgs(IExpr expr)
        {
            lexer.Consume();
            var args = new List<IExpr>();

            while (lexer.Token.Type != TokenType.RParen)
            {
                args.Add(ParseExpr());
                if (lexer.Token.Type is not TokenType.Comma and not TokenType.RParen)
                {
                    Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Comma, TokenType.RParen });
                    break;
                }
            }

            var end = lexer.Token.Range.End;
            lexer.Consume();
            return new CallExpr(new TextRange(expr.Range.Start, end), expr, args);
        }

        private IExpr ParsePostfix(IExpr expr)
        {
            if (lexer.Token.Type == TokenType.LParen)
                return ParsePostfix(ParseArgs(expr));

            return expr;
        }

        private IExpr ParsePostfix()
        {
            return ParsePostfix(ParsePrimary());
        }

        private IExpr ParseUnary()
        {
            return ParsePostfix();
        }

        private VarDeclStatement ParseVarDecl()
        {
            var start = lexer.Token.Range.Start;
            lexer.Consume();
            var identifier = ParseIdentifier();

            ParseTree.Type? type = null;
            if (lexer.Token.Type == TokenType.Colon)
            {
                lexer.Consume();
                type = ParseType();
            }

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

                lexer.Consume();
                var opPrecedence = opNode.Value.Op.GetPrecedence();
                if (opPrecedence < precedence)
                    break;

                var rhs = ParseExpr(precedence);
                expr = new BinOpExpr(new TextRange(expr.Range.Start, rhs.Range.End), expr, opNode.Value, rhs);
            }

            return new Item(expr);
        }

        private IExpr ParseExpr(int precedence = 1) => ParseItem(precedence) switch
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
                if (lexer.Token.Type is not TokenType.Comma and not TokenType.RParen)
                {
                    Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Comma, TokenType.RParen });
                    break;
                }
            }
            lexer.Consume(); // consume rparen

            ParseTree.Type? returnType = null;
            if (lexer.Token.Type == TokenType.RArrow)
            {
                lexer.Consume();
                returnType = ParseType();
            }

            return new Proto(visibility, identifier, @params, returnType);
        }

        private void ParseFunc(ModuleFile moduleFile, VisibilityNode visibility)
        {
            var proto = ParseProto(visibility);
            if (lexer.Token.Type != TokenType.LCurly)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.LCurly });
            var body = ParseExpr();
            moduleFile.Funcs.Add(proto.Identifier.Name, new Func(moduleFile, proto, body));
        }

        public void ParseTypeDecl(ModuleFile moduleFile, VisibilityNode visibility)
        {
            lexer.Consume(); // consume type
            var identifier = ParseIdentifier();
            if (lexer.Token.Type != TokenType.Assign)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Assign });
            lexer.Consume();
            var type = ParseType();
            if (lexer.Token.Type != TokenType.Semicolon)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Semicolon });
            lexer.Consume();

            moduleFile.TypeDecls.Add(identifier.Name, new TypeDecl(moduleFile, visibility, identifier, type));
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

        public Module Parse(Project project, Lexer lexer)
        {

            this.project = project;
            this.lexer = lexer;

            project.Files.Add(lexer.FileName, lexer.Input);

            lexer.Consume();

            if (lexer.Token.Type != TokenType.Mod)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Mod });
            lexer.Consume();

            var identifer = ParseIdentifier();
            if (lexer.Token.Type != TokenType.Semicolon)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Semicolon });
            lexer.Consume();

            if (!project.Modules.TryGetValue(identifer.Name, out var module))
            {
                module = new Module(project, identifer.Name);
                project.Modules.Add(identifer.Name, module);
            }

            var moduleFile = new ModuleFile(module, lexer.FileName);
            module.ModuleFiles.Add(moduleFile);


            while (lexer.Token.Type != TokenType.Unknown)
            {
                var visiblity = ParseVisiblity();
                switch (lexer.Token.Type)
                {
                    case TokenType.Fn:
                        ParseFunc(moduleFile, visiblity);
                        break;
                    case TokenType.Type:
                        ParseTypeDecl(moduleFile, visiblity);
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
            project = new Project();
        }

        private Project project;
        private Lexer lexer;
    }
}
