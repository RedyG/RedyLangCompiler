using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    internal class ByteList
    {
        private List<byte> list = new();

        #region Add
        private void AddLeRange(byte[] range)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(range);

            list.AddRange(range);
        }

        public void Add(sbyte value)
        {
            unsafe
            {
                list.Add(*(byte*)&value);
            }
        }

        public void Add(byte value)
        {
            list.Add(value);
        }

        public void Add(short value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(ushort value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(int value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(uint value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(long value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(ulong value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(float value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(double value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(string value)
        {
            list.AddRange(Encoding.UTF8.GetBytes(value));
        }

        public void Add(IEnumerable<byte> value)
        {
            list.AddRange(value);
        }
        #endregion Add

        #region Insert

        private void InsertLeRange(int index, byte[] range)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(range);

            list.InsertRange(index, range);
        }

        public void Insert(int index, sbyte value)
        {
            unsafe
            {
                list.Insert(index, *(byte*)&value);
            }
        }

        public void Insert(int index, byte value)
        {
            list.Insert(index, value);
        }

        public void Insert(int index, short value)
        {
            InsertLeRange(index, BitConverter.GetBytes(value));
        }

        public void Insert(int index, ushort value)
        {
            InsertLeRange(index, BitConverter.GetBytes(value));
        }

        public void Insert(int index, int value)
        {
            InsertLeRange(index, BitConverter.GetBytes(value));
        }

        public void Insert(int index, uint value)
        {
            InsertLeRange(index, BitConverter.GetBytes(value));
        }

        public void Insert(int index, long value)
        {
            InsertLeRange(index, BitConverter.GetBytes(value));
        }

        public void Insert(int index, ulong value)
        {
            InsertLeRange(index, BitConverter.GetBytes(value));
        }

        public void Insert(int index, float value)
        {
            InsertLeRange(index, BitConverter.GetBytes(value));
        }

        public void Insert(int index, double value)
        {
            InsertLeRange(index, BitConverter.GetBytes(value));
        }

        public void Insert(int index, string value)
        {
            list.InsertRange(index, Encoding.UTF8.GetBytes(value));
        }

        public void Insert(int index, IEnumerable<byte> value)
        {
            list.InsertRange(index, value);
        }
        #endregion Insert

        public void AdvanceBy(int count)
        {
            for (int i = 0; i < count; i++)
                list.Add(0);
        }
        public byte[] ToArray() => list.ToArray();
    }
}
