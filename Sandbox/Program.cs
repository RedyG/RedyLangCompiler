using Compiler;
using Compiler.AST;
using System;
/*Module module = new();
var main = new Func(0, 0, new List<Block> {
    new Block(
        new List<Instruction> {
            new(OpCode.I8Const, (sbyte)10),
            new(OpCode.Call, (short)1)
        },
        BrInstruction.CreateExit()
    )
});

var thirdBlock = new Block(
    new List<Instruction> {
        new(OpCode.LocalGet, (ushort)0),
        new(OpCode.I8Const, (sbyte)1),
        new(OpCode.I32Sub),
        new(OpCode.Call, (short)1),
        new(OpCode.LocalGet, (ushort)0),
        new(OpCode.I8Const, (sbyte)2),
        new(OpCode.I32Sub),
        new(OpCode.Call, (short)1),
        new(OpCode.I32Add),
    },
    BrInstruction.CreateRet()
);

var secondBlock = new Block(
    new List<Instruction> {
        new(OpCode.LocalGet, (ushort)0),
    },
    BrInstruction.CreateRet()
);

var firstBlock = new Block(
    new List<Instruction> {
        new(OpCode.LocalGet, (ushort)0),
        new(OpCode.I8Const, (sbyte)2),
        new(OpCode.I32Lt),
    },
    BrInstruction.CreateBrIf(secondBlock, thirdBlock)
);

var fib = new Func(1, 0, new List<Block>
{
    firstBlock,
    secondBlock,
    thirdBlock
});
module.Funcs.Add(main);
module.Funcs.Add(fib);
var list = new ByteList();
module.WriteTo(list);


var mainList = new ByteList();
main.WriteTo(mainList);
File.WriteAllBytes("main.redy", mainList.ToArray());

var fibList = new ByteList();
//fib.WriteTo(fibList);
File.WriteAllBytes("fib.redy", fibList.ToArray());*/


/*

fn fib(n: i32) -> i32 {
    if n < 2 {
        return n;
    }
    return fib(n - 1) + fib(n - 2);
}
 
 */

/*var i32 = new Compiler.AST.Type.I32();
var never = new Compiler.AST.Type.Never();

var module = new Module();

var param = new VarDeclStatement(i32);

var fib = new Func(i32, new IntExpr(999), new List<VarDeclStatement> { param });
fib.Body = new IfExpr
(
    never,
    new BinOpExpr(i32, new VarUse(param), BinOp.Lt, new IntExpr(2)),
    new ReturnExpr(new VarUse(param)),
    new ReturnExpr(new BinOpExpr
    (
        i32,
        new CallExpr(fib, new List<IExpr> { new BinOpExpr(i32, new VarUse(param), BinOp.Sub, new IntExpr(1)) }),
        BinOp.Add,
        new CallExpr(fib, new List<IExpr> { new BinOpExpr(i32, new VarUse(param), BinOp.Sub, new IntExpr(2)) })
    ))
);

module.Funcs.Add(new Func(never, new CallExpr(fib, new List<IExpr> { new IntExpr(10) })));
module.Funcs.Add(fib);

var byteModule = module.CodeGen();

var list = new Compiler.ByteCode.ByteList();
byteModule.WriteTo(list);

File.WriteAllBytes("testAST.redy", list.ToArray());*/


var parser = new Parser();
var project = new Compiler.ParseTree.Project();
parser.Parse(project, new Lexer("C:\\Users\\minio\\source\\repos\\RedyLangCompiler\\Sandbox\\program.redy"));
parser.Parse(project, new Lexer("C:\\Users\\minio\\source\\repos\\RedyLangCompiler\\Sandbox\\std\\io.redy"));
var projectAST = project.ToAST();
if (Logger.CompilationFailed || projectAST == null)
    return;

var funcSymbols = new Dictionary<Func, Compiler.ByteCode.Func>();
var byteModules = projectAST.Modules.Select(
    module =>
    module.CodeGen(funcSymbols)
    ).ToList();
foreach (var byteModule in byteModules)
{
    var root = "C:\\Users\\minio\\source\\repos\\RedyLangCompiler\\Sandbox";
    var list = new Compiler.ByteCode.ByteList();
    byteModule.WriteTo(list, root);

    var path = $@"C:\Users\minio\OneDrive\Bureau\redy_test\{Path.GetRelativePath(root, byteModule.FileName).Split(".")[0]}.rasm";
    var directory = Path.GetDirectoryName(path);
    if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);
 
    File.WriteAllBytes(path, list.ToArray());
}