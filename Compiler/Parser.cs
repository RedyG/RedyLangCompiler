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

        private BlockExpr ParseBlock(ModuleFile moduleFile)
        {
            var start = lexer.Token.Range.Start;
            lexer.Consume();
            var statements = new List<IStatement>();
            IExpr? lastExpr = null;
            while (lexer.Token.Type != TokenType.RCurly)
            {
                if (lastExpr != null)
                {
                    Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.RCurly });
                    return new BlockExpr(new TextRange(start, lexer.Token.Range.End), statements, lastExpr);
                }
                var item = ParseItem(moduleFile);
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

        private IfExpr ParseIf(ModuleFile moduleFile)
        {
            var start = lexer.Token.Range.Start;
            lexer.Consume();

            var condition = ParseExpr(moduleFile, 1, true);

            if (lexer.Token.Type != TokenType.LCurly)
            {
                Logger.UnexpectedToken(lexer, [TokenType.LCurly]);
                throw new Exception(); // TODO: idk how to handle this
            }

            var then = ParseExpr(moduleFile);

            if (lexer.Token.Type != TokenType.Else)
                return new IfExpr(new TextRange(start, then.Range.End), condition, then);

            lexer.Consume();
            var @else = ParseExpr(moduleFile);

            return new IfExpr(new TextRange(start, @else.Range.End), condition, then, @else);
        }

        private IExpr ParseWhile(ModuleFile moduleFile)
        {
            var start = lexer.Token.Range.Start;
            lexer.Consume();

            var condition = ParseExpr(moduleFile, 1, true);

            if (lexer.Token.Type != TokenType.LCurly)
            {
                Logger.UnexpectedToken(lexer, [TokenType.LCurly]);
                throw new Exception(); // TODO: idk how to handle this
            }

            var body = ParseExpr(moduleFile);

            return new WhileExpr(new TextRange(start, body.Range.End), condition, body);
        }

        private ReturnExpr ParseReturn(ModuleFile moduleFile)
        {
            var returnRange = lexer.Token.Range;
            lexer.Consume();

            if (lexer.Token.Type == TokenType.Semicolon)
                return new ReturnExpr(returnRange, null);

            var expr = ParseExpr(moduleFile);

            return new ReturnExpr(new TextRange(returnRange.Start, expr.Range.End), expr);
        }

        private IExpr ParsePrimary(ModuleFile moduleFile, bool isCond = false)
        {
            switch (lexer.Token.Type)
            {
                case TokenType.IntLiteral:
                    var intExpr = new IntExpr(lexer.Token.Range, int.Parse(lexer.Token.Content.ToString().Replace("_", "")));
                    lexer.Consume();
                    return intExpr;
                case TokenType.Identifier:
                    {
                        var identifier = ParseIdentifier();
                        if (lexer.Token.Type != TokenType.LCurly || isCond)
                            return identifier;

                        lexer.Consume();

                        var namedArgs = new List<NamedArg>();
                        while (lexer.Token.Type != TokenType.RCurly)
                        {
                            var argIdentifier = ParseIdentifier();
                            if (lexer.Token.Type != TokenType.Colon)
                                Logger.UnexpectedToken(lexer, [TokenType.Colon]);
                            lexer.Consume();
                            var argExpr = ParseExpr(moduleFile);
                            namedArgs.Add(new NamedArg(argIdentifier, argExpr));
                            
                            if (lexer.Token.Type == TokenType.Comma)
                            {
                                lexer.Consume();
                                continue;
                            }

                            if (lexer.Token.Type != TokenType.RCurly)
                                Logger.UnexpectedToken(lexer, [TokenType.Comma, TokenType.RCurly]);
                        }

                        var end = lexer.Token.Range.End;
                        lexer.Consume();
                        return new StructConstructorExpr(new ParseTree.Type.Identifier(identifier), namedArgs, new TextRange(identifier.Range.Start, end));

                    }
                case TokenType.StringLiteral:
                    var stringExpr = new StringExpr(lexer.Token.Range, lexer.Token.Content);
                    lexer.Consume();
                    return stringExpr;
                case TokenType.LParen:
                {
                    var start = lexer.Token.Range.Start;
                        lexer.Consume();
                    var expr = ParseExpr(moduleFile);
                    if (lexer.Token.Type != TokenType.RParen)
                        Logger.UnexpectedToken(lexer, [TokenType.RParen]);
                    var parenExpr = new ParenExpr(new TextRange(start, lexer.Token.Range.End), expr);
                    lexer.Consume();
                    return parenExpr;
                }
                case TokenType.LCurly:
                    return ParseBlock(moduleFile);
                case TokenType.If:
                    return ParseIf(moduleFile);
                case TokenType.While:
                    return ParseWhile(moduleFile);
                case TokenType.Return:
                    return ParseReturn(moduleFile);
                default:
                    Logger.UnexpectedToken(lexer, [TokenType.IntLiteral, TokenType.Identifier, TokenType.LParen, TokenType.LCurly, TokenType.If, TokenType.Return]);
                    throw new Exception(); // TODO: idk how to handle this
            }
        }

        private CallExpr ParseArgs(ModuleFile moduleFile, IExpr expr)
        {
            lexer.Consume();
            var args = new List<IExpr>();

            while (lexer.Token.Type != TokenType.RParen)
            {
                args.Add(ParseExpr(moduleFile));
                if (lexer.Token.Type is not TokenType.Comma and not TokenType.RParen)
                {
                    Logger.UnexpectedToken(lexer, [TokenType.Comma, TokenType.RParen]);
                    break;
                }

                if (lexer.Token.Type is TokenType.Comma)
                    lexer.Consume();
            }

            var end = lexer.Token.Range.End;
            lexer.Consume();
            return new CallExpr(new TextRange(expr.Range.Start, end), expr, args);
        }

        private IExpr ParseUnary(ModuleFile moduleFile, bool isCond = false)
        {
            if (lexer.Token.Type == TokenType.Amp)
            {
                var start = lexer.Token.Range.Start;
                lexer.Consume();
                var expr = ParseExpr(moduleFile, 5);
                return new RefExpr(new TextRange(start, expr.Range.End), expr);
            }

            if (lexer.Token.Type == TokenType.Mul)
            {
                var start = lexer.Token.Range.Start;
                lexer.Consume();
                var expr = ParseExpr(moduleFile, 5);
                return new DeRefExpr(new TextRange(start, expr.Range.End), expr);
            }

            return ParsePrimary(moduleFile, isCond);
        }

        private VarDeclStatement ParseVarDecl(ModuleFile moduleFile)
        {
            var start = lexer.Token.Range.Start;
            lexer.Consume();
            var identifier = ParseIdentifier();

            ParseTree.Type? type = null;
            if (lexer.Token.Type == TokenType.Colon)
            {
                lexer.Consume();
                type = ParseType(moduleFile);
            }

            IExpr? value = null;
            if (lexer.Token.Type == TokenType.Assign)
            {
                lexer.Consume();
                value = ParseExpr(moduleFile);
            }

            if (lexer.Token.Type != TokenType.Semicolon)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Semicolon });
            var end = lexer.Token.Range.End;
            lexer.Consume();

            return new VarDeclStatement(new TextRange(start, end), type, identifier, value);
        }

        private Item ParseItem(ModuleFile moduleFile, int precedence = 1, bool isCond = false)
        {
            if (lexer.Token.Type == TokenType.Var)
                return new Item(ParseVarDecl(moduleFile));

            var expr = ParseUnary(moduleFile, isCond);
            while (true)
            {
                if (lexer.Token.Type == TokenType.LParen) // todo abstract other postfix
                {
                    var opPrecedence = 6;
                    if (opPrecedence < precedence)
                        break;

                    expr = ParseArgs(moduleFile, expr);
                    continue;
                }

                var binOpNode = BinOpNode.FromToken(lexer.Token);
                if (binOpNode != null)
                {
                    var opPrecedence = binOpNode.Value.Op.GetPrecedence();
                    if (opPrecedence < precedence)
                        break;

                    lexer.Consume();
                    var rhs = ParseExpr(moduleFile, opPrecedence + (binOpNode.Value.Op.LeftAssociative() ? 1 : 0));
                    expr = new BinOpExpr(new TextRange(expr.Range.Start, rhs.Range.End), expr, binOpNode.Value, rhs);
                    continue;
                }

                break;
            }

            return new Item(expr);
        }

        private IExpr ParseExpr(ModuleFile moduleFile, int precedence = 1, bool isCond = false) => ParseItem(moduleFile, precedence, isCond) switch
        {
            Item { Expr: var expr, Statement: null } => expr!,
            _ => throw new NotImplementedException()
        };

        private ParseTree.Type.Struct ParseStruct(ModuleFile moduleFile)
        {
            lexer.Consume();
            if (lexer.Token.Type != TokenType.LCurly)
                Logger.UnexpectedToken(lexer, [TokenType.LCurly]);

            var start = lexer.Token.Range.Start;
            lexer.Consume();

            List<Field> fields = new List<Field>();
            while (lexer.Token.Type != TokenType.RCurly)
            {
                var visiblity = ParseVisiblity();
                var identifier = ParseIdentifier();
                if (lexer.Token.Type != TokenType.Colon)
                    Logger.UnexpectedToken(lexer, [TokenType.Colon]);
                lexer.Consume();
                var type = ParseType(moduleFile);
                fields.Add(new Field(visiblity, new VarDeclStatement(new TextRange(identifier.Range.Start, type.GetRange().End), type, identifier)));
                if (lexer.Token.Type == TokenType.Comma)
                {
                    lexer.Consume();
                    continue;
                }
                if (lexer.Token.Type != TokenType.RCurly)
                    Logger.UnexpectedToken(lexer, [TokenType.Comma, TokenType.RCurly]);
            }

            var @struct = new ParseTree.Type.Struct(fields, new TextRange(start, lexer.Token.Range.End));
            lexer.Consume();
            return @struct;
        }

        private ParseTree.Type.Trait ParseTrait(ModuleFile moduleFile)
        {
            lexer.Consume();
            if (lexer.Token.Type != TokenType.LCurly)
                Logger.UnexpectedToken(lexer, [TokenType.LCurly]);

            var start = lexer.Token.Range.Start;
            lexer.Consume();

            List<Func> funcs = new List<Func>();
            List<Proto> protos = new List<Proto>();
            while (lexer.Token.Type != TokenType.RCurly)
            {
                var proto = ParseProto(moduleFile, new VisibilityNode(new TextRange(), Visibility.Pub), null); // TODO: add attributes
                if (lexer.Token.Type == TokenType.Semicolon)
                {
                     protos.Add(proto);
                     lexer.Consume();
                     continue;
                }

                if (lexer.Token.Type != TokenType.LCurly)
                    Logger.UnexpectedToken(lexer, [TokenType.LCurly]);

                var body = ParseExpr(moduleFile);
                funcs.Add(new Func(proto, body));
            }

            var end = lexer.Token.Range.End;
            lexer.Consume();
            return new ParseTree.Type.Trait(funcs, protos, new TextRange(start, end));
        }

        private ParseTree.Type ParseType(ModuleFile moduleFile)
        {
            if (lexer.Token.Type == TokenType.Amp)
            {
                var start = lexer.Token.Range.Start;
                lexer.Consume();
                var type = ParseType(moduleFile);
                return new ParseTree.Type.Ref(type, new TextRange(start, type.GetRange().End));
            }

            if (lexer.Token.Type == TokenType.Struct)
                return ParseStruct(moduleFile);

            if (lexer.Token.Type == TokenType.Trait)
                return ParseTrait(moduleFile);

            var identifier = ParseIdentifier();

            if (lexer.Token.Type == TokenType.Lt)
            {

            }

        }

        private Param ParseParam(ModuleFile moduleFile)
        {
            var identifer = ParseIdentifier();
            if (lexer.Token.Type != TokenType.Colon)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Colon });
            lexer.Consume();
            var type = ParseType(moduleFile);
            var value = lexer.Token.Type == TokenType.Assign ? ParseExpr(moduleFile) : null;
            return new Param(new TextRange(identifer.Range.Start, type.GetRange().End), type, identifer, value);
        }

        private Identifier ParseIdentifier()
        {
            if (lexer.Token.Type != TokenType.Identifier)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Identifier });

            var identifier = new Identifier(lexer.Token.Range, lexer.Token.Content);
            lexer.Consume();
            return identifier;
        }

        private Proto ParseProto(ModuleFile moduleFile, VisibilityNode visibility, Attributes? attributes)
        {
            lexer.Consume(); // consume fn
            var identifier = ParseIdentifier();

            var @params = new List<Param>();
            if (lexer.Token.Type != TokenType.LParen)
                Logger.UnexpectedToken(lexer, [TokenType.LParen]);
            lexer.Consume();
            while (lexer.Token.Type != TokenType.RParen)
            {
                @params.Add(ParseParam(moduleFile));
                if (lexer.Token.Type is not TokenType.Comma and not TokenType.RParen)
                {
                    Logger.UnexpectedToken(lexer, [TokenType.Comma, TokenType.RParen]);
                    break;
                }

                if (lexer.Token.Type is TokenType.Comma)
                    lexer.Consume();
            }
            lexer.Consume(); // consume rparen

            ParseTree.Type? returnType = null;
            if (lexer.Token.Type == TokenType.RArrow)
            {
                lexer.Consume();
                returnType = ParseType(moduleFile);
            }

            return new Proto(visibility, attributes, identifier, @params, returnType, moduleFile);
        }

        private Func ParseFunc(ModuleFile moduleFile, VisibilityNode visibility, Attributes? attributes)
        {
            var proto = ParseProto(moduleFile, visibility, attributes);
            if (lexer.Token.Type == TokenType.Semicolon)
            {
                lexer.Consume();
                return new Func(proto, null); // function without body
            }
            if (lexer.Token.Type != TokenType.LCurly)
                Logger.UnexpectedToken(lexer, [TokenType.LCurly]);
            var body = ParseExpr(moduleFile);
            return new Func(proto, body);
        }

        public void ParseTypeDecl(ModuleFile moduleFile, VisibilityNode visibility, Attributes? attributes)
        {
            bool isAlias = lexer.Token.Type == TokenType.Alias;
            lexer.Consume(); // consume type
            var identifier = ParseIdentifier();
            List<GenericParam> genericParams = [];
            if (lexer.Token.Type == TokenType.Lt)
            {
                lexer.Consume();

                while (lexer.Token.Type != TokenType.Gt)
                {
                    var paramIdentifier = ParseIdentifier();
                    genericParams.Add(new GenericParam { Identifier = paramIdentifier });
                    if (lexer.Token.Type != TokenType.Colon)
                        Logger.UnexpectedToken(lexer, [TokenType.Colon]);
                    lexer.Consume();
                }

                lexer.Consume();
            }

            if (lexer.Token.Type != TokenType.Assign)
                Logger.UnexpectedToken(lexer, [TokenType.Assign]);
            lexer.Consume();
            var type = ParseType(moduleFile);
            if (lexer.Token.Type != TokenType.Semicolon)
                Logger.UnexpectedToken(lexer, [TokenType.Semicolon]);
            lexer.Consume();

            moduleFile.TypeDecls.Add(identifier.Name, new TypeDecl(moduleFile, visibility, attributes, identifier, type, isAlias, genericParams));
        }

        public void ParseUse(ModuleFile moduleFile, VisibilityNode visibilityNode)
        {
            lexer.Consume(); // consume use
            var start = lexer.Token.Range.Start;
            var path = new List<Identifier>();
            var imported = new List<Identifier>();
            while (true)
            {
                if (lexer.Token.Type == TokenType.LCurly)
                    break;
                path.Add(ParseIdentifier());
                if (lexer.Token.Type == TokenType.DoubleColon)
                    lexer.Consume();
            }

            if (lexer.Token.Type == TokenType.LCurly)
            {
                lexer.Consume();
                while (lexer.Token.Type != TokenType.RCurly)
                {
                    imported.Add(ParseIdentifier());
                    if (lexer.Token.Type == TokenType.Comma)
                        lexer.Consume();
                    else if (lexer.Token.Type != TokenType.RCurly)
                    {
                        Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Comma, TokenType.RCurly });
                        break;
                    }
                }
                lexer.Consume();
            }
            else
            {
                imported.Add(path.Pop());
            }

            if (lexer.Token.Type != TokenType.Semicolon)
                Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.Semicolon });
            lexer.Consume();

            moduleFile.UseDecls.Add(new UseDecl(visibilityNode, moduleFile, path, imported));
        }

        private ImplDecl ParseImpl(ModuleFile moduleFile)
        {
            lexer.Consume();
            var trait = ParseType(moduleFile);
            ParseTree.Type? type = null;
            if (lexer.Token.Type != TokenType.LCurly)
            {
                if (lexer.Token.Type != TokenType.For)
                    Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.For });
                lexer.Consume();
                type = ParseType(moduleFile);
                if (lexer.Token.Type != TokenType.LCurly)
                    Logger.UnexpectedToken(lexer, new TokenType[] { TokenType.LCurly });
            }
            lexer.Consume();

            var funcs = new List<Func>();
            while (lexer.Token.Type != TokenType.RCurly)
            {
                var attributes = ParseAttributes();
                var visiblity = ParseVisiblity();
                funcs.Add(ParseFunc(moduleFile, visiblity, attributes));
            }

            lexer.Consume();
            return new ImplDecl(moduleFile, type != null ? trait : null, type != null ? type : trait, funcs);
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

        private Attributes? ParseAttributes()
        {
            var start = lexer.Token.Range.Start;
            if (lexer.Token.Type != TokenType.LSquare)
                return null;

            lexer.Consume();
            var identifiers = new List<Identifier>();
            while (lexer.Token.Type != TokenType.RSquare)
            {
                if (lexer.Token.Type != TokenType.Identifier)
                    Logger.UnexpectedToken(lexer, [TokenType.Identifier]);

                identifiers.Add(ParseIdentifier());

                if (lexer.Token.Type == TokenType.Comma)
                {
                    lexer.Consume();
                    continue;
                }

                if (lexer.Token.Type != TokenType.RSquare)
                    Logger.UnexpectedToken(lexer, [TokenType.Comma, TokenType.RSquare]);
            }

            var attributes = new Attributes(new TextRange(start, lexer.Token.Range.End), identifiers);
            lexer.Consume(); // consume rsquare
            return attributes;
        }

        public Module Parse(Project project, Lexer lexer)
        {
            this.project = project;
            this.lexer = lexer;

            project.Files.Add(lexer.FileName, lexer.Input);

            lexer.Consume();

            if (lexer.Token.Type != TokenType.Mod)
                Logger.UnexpectedToken(lexer, [TokenType.Mod]);
            lexer.Consume();

            List<Identifier> identifiers = [];
            while (true)
            {
                var identifer = ParseIdentifier();
                identifiers.Add(identifer);
                if (lexer.Token.Type == TokenType.Semicolon)
                {
                    lexer.Consume();
                    break;
                }
                else if (lexer.Token.Type == TokenType.DoubleColon)
                {
                    lexer.Consume();
                    continue;
                }
                Logger.UnexpectedToken(lexer, [TokenType.Semicolon, TokenType.DoubleColon]);
            }

            var module = project.Modules.CreateModule(project, identifiers.Select(identifier => identifier.Name).ToList());
            var moduleFile = new ModuleFile(module, lexer.FileName);
            module.ModuleFiles.Add(moduleFile);


            while (lexer.Token.Type != TokenType.Unknown)
            {
                var attributes = ParseAttributes();
                var visiblity = ParseVisiblity();
                switch (lexer.Token.Type)
                {
                    case TokenType.Fn:
                        var func = ParseFunc(moduleFile, visiblity, attributes);
                        moduleFile.Funcs.Add(func.Proto.Identifier.Name, func);
                        break;
                    case TokenType.Type or TokenType.Alias:
                        ParseTypeDecl(moduleFile, visiblity, attributes);
                        break;
                    case TokenType.Use:
                        ParseUse(moduleFile, visiblity);
                        break;
                    case TokenType.Impl:
                        var impl = ParseImpl(moduleFile);
                        moduleFile.Impls.Add(impl);
                        break;
                    default:
                        Logger.UnexpectedToken(lexer, [ TokenType.Fn ]);
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
