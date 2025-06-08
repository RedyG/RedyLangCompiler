using Compiler.ByteCode;
using Compiler.ParseTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Proto
    {
        public List<string> Attributes { get; set; } = [];
        public ModuleFile? Module { get; set; }
        public IType ReturnType { get; set; }
        public List<Param> Params { get; set; } = new();
        public string Name;


        public IType.FuncPtr GetFuncPtrType()
        {
            return new IType.FuncPtr(Params.Select(param => new VarDeclStatement(param.Type)).ToList(), ReturnType); // todo: default param values
        }

        public bool IsIntrisic => Attributes.Contains("Intrinsic");

        public Intrinsic? Intrinsic => IsIntrisic ? Name switch
        {
            "print" => ByteCode.Intrinsic.Print,
            "println" => ByteCode.Intrinsic.Println,
            "readln" => ByteCode.Intrinsic.ReadLine,
            "read_file" => ByteCode.Intrinsic.ReadFile,
            "write_file" => ByteCode.Intrinsic.WriteFile,
            _ => null
        } : null;

        public Proto(IType returnType, string name)
        {
            ReturnType = returnType;
            Name = name;
        }

        public Proto(IType returnType, List<Param> @params, List<string> attributes, string name, ModuleFile? module)
        {
            Attributes = attributes;
            ReturnType = returnType;
            Name = name;
            Params = @params;
            Module = module;
        }
    }
}
