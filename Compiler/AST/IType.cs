using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public interface IType : IEquatable<IType>
    {
        public bool IsEmpty { get; }
        public uint Size();
        public IType ToConcrete();

        public class Struct : IType
        {
            public List<Field> Fields { get; set; } = new List<Field>();
            public bool IsEmpty => false;

            public bool Equals(IType? other)
            {
                if (other == null)
                    return false;

                if (other is Struct s)
                    return Fields.SequenceEqual(s.Fields);

                return false;
            }

            public override bool Equals(object? obj) => Equals(obj as IType);

            public override int GetHashCode() => Fields.GetHashCode();

            public uint Size() => (uint)Fields.Sum(field => field.Type.Size());
            public IType ToConcrete() => this;
            public override string ToString() => $"struct {{\n{string.Join(",\n", Fields.Select(field => $"{field.Name}: {field.Type}"))}}}";
        }

        public class FuncPtr : IType
        {
            public List<VarDeclStatement> Params { get; }
            public IType ReturnType { get; }

            public FuncPtr(List<VarDeclStatement> @params, IType returnType)
            {
                Params = @params;
                ReturnType = returnType;
            }

            public bool Equals(IType? other)
            {
                if (other == null)
                    return false;

                if (other is FuncPtr f)
                    return Params.SequenceEqual(f.Params) && ReturnType == f.ReturnType;

                return false;
            }

            public override bool Equals(object? obj) => Equals(obj as IType);

            // TODO: will stack overflow
            public override int GetHashCode() => HashCode.Combine(Params, ReturnType);

            public bool IsEmpty => false;
            public uint Size() => 8;
            public IType ToConcrete() => this;
            public override string ToString() => $"fn({string.Join(", ", Params.Select(param => param.Type.ToString()))}) -> {ReturnType}";
        }
        public class Trait : IType
        {
            public List<Func> Funcs { get; set; } = new List<Func>();
            public List<Proto> Protos { get; set; } = new List<Proto>();

            public bool Equals(IType? other)
            {
                if (other == null)
                    return false;

                if (other is Trait t)
                    return Funcs.SequenceEqual(t.Funcs) && Protos.SequenceEqual(t.Protos);

                return false;
            }

            public override bool Equals(object? obj) => Equals(obj as IType);

            public override int GetHashCode() => HashCode.Combine(Funcs, Protos);

            public bool IsEmpty => false;
            public uint Size() => 16;
            public IType ToConcrete() => this;
            public override string ToString() => $"trait {{\n{string.Join(",\n", Funcs.Select(func => func.ToString()))}}}";
        }

        public class Identifier : IType
        {
            public string Name { get; set; }
            public IType Type { get; set; }

            public bool Equals(IType? other) => other is Identifier identifier && Type.Equals(identifier.Type) && Name == identifier.Name;

            public bool IsEmpty => Type.IsEmpty;
            public uint Size() => Type.Size();
            public IType ToConcrete() => Type;
            public override string ToString() => Name;
        }

        public class Ref : IType
        {
            public IType Type { get; set; }

            public bool Equals(IType? other) => other is Ref r && Type.Equals(r.Type);

            public Ref(IType type)
            {
                Type = type;
            }

            public bool IsEmpty => false;
            public uint Size() => 8;
            public IType ToConcrete() => this;
            public override string ToString() => $"&{Type}";
        }
        public class Void : IType
        {
            public bool Equals(IType? other) => other is Void;

            public bool IsEmpty => true;
            public uint Size() => 0;
            public IType ToConcrete() => this;
            public override string ToString() => "void";
        }

        public class Never : IType
        {
            public bool Equals(IType? other) => other is Never;

            public bool IsEmpty => true;
            public uint Size() => 0;
            public IType ToConcrete() => this;
            public override string ToString() => "never";
        }

        public class I32 : IType
        {
            public bool Equals(IType? other) => other is I32;

            public bool IsEmpty => false;
            public uint Size() => 4;
            public IType ToConcrete() => this;
            public override string ToString() => "i32";
        }

        public class Bool : IType
        {
            public bool Equals(IType? other) => other is Bool;

            public bool IsEmpty => false;
            public uint Size() => 1;
            public IType ToConcrete() => this;
            public override string ToString() => "bool";
        }
    }


}
