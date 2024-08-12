using Compiler.ByteCode;

Module module = new();
var main = new Func(0, 0, new List<Block> {
    new Block(
        new List<Instruction> {
            new(OpCode.I8Const, (sbyte)10),
            new(OpCode.Call, (short)1)
        },
        BranchInstruction.CreateExit()
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
    BranchInstruction.CreateRet()
);

var secondBlock = new Block(
    new List<Instruction> {
        new(OpCode.LocalGet, (ushort)0),
    },
    BranchInstruction.CreateRet()
);

var firstBlock = new Block(
    new List<Instruction> {
        new(OpCode.LocalGet, (ushort)0),
        new(OpCode.I8Const, (sbyte)2),
        new(OpCode.I32Lt),
    },
    BranchInstruction.CreateBr(secondBlock, thirdBlock)
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


/*var mainList = new ByteList();
main.WriteTo(mainList);
File.WriteAllBytes("main.redy", mainList.ToArray());

var fibList = new ByteList();
//fib.WriteTo(fibList);
File.WriteAllBytes("fib.redy", fibList.ToArray());*/

File.WriteAllBytes("test.redy", list.ToArray());