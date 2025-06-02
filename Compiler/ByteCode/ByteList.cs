using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    public class ByteList : ICollection<byte>
    {
        private List<byte> list = new();

        public int Count => list.Count;

        public bool IsReadOnly => false;

        public byte this[int index] { get => list[index]; set => list[index] = value; }

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

        public void Add(char value)
        {
            list.AddRange(Encoding.UTF8.GetBytes([value]));
        }

        public void Add(Int16 value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(UInt16 value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(Int32 value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(UInt32 value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(Int64 value)
        {
            AddLeRange(BitConverter.GetBytes(value));
        }

        public void Add(UInt64 value)
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

        public void AddRString(string value)
        {
            Add((UInt32)value.Length);
            list.AddRange(Encoding.UTF8.GetBytes(value));
            Add('\0');
        }

        public void Add(IEnumerable<byte> value)
        {
            list.AddRange(value);
        }
        #endregion Add

        #region WriteAt
        private void WriteLeRangeAt(int index, byte[] range)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(range);

            WriteAt(index, range);
        }

        public void WriteAt(int index, sbyte value)
        {
            unsafe
            {
                list[index] = *(byte*)&value;
            }
        }

        public void WriteAt(int index, Int16 value)
        {
            WriteLeRangeAt(index, BitConverter.GetBytes(value));
        }

        public void WriteAt(int index, UInt16 value)
        {
            WriteLeRangeAt(index, BitConverter.GetBytes(value));
        }

        public void WriteAt(int index, Int32 value)
        {
            WriteLeRangeAt(index, BitConverter.GetBytes(value));
        }

        public void WriteAt(int index, UInt32 value)
        {
            WriteLeRangeAt(index, BitConverter.GetBytes(value));
        }

        public void WriteAt(int index, Int64 value)
        {
            WriteLeRangeAt(index, BitConverter.GetBytes(value));
        }

        public void WriteAt(int index, UInt64 value)
        {
            WriteLeRangeAt(index, BitConverter.GetBytes(value));
        }

        public void WriteAt(int index, float value)
        {
            WriteLeRangeAt(index, BitConverter.GetBytes(value));
        }

        public void WriteAt(int index, double value)
        {
            WriteLeRangeAt(index, BitConverter.GetBytes(value));
        }

        public void WriteAt(int index, string value) => WriteAt(index, Encoding.UTF8.GetBytes(value));

        public void WriteAt<T>(int index, T value) where T : IEnumerable<byte>
        {
            foreach (var item in value)
                list[index++] = item;
        }
        #endregion WriteAt

        public void AdvanceBy(int count)
        {
            for (int i = 0; i < count; i++)
                list.Add((byte)0);
        }

        public byte[] ToArray() => list.ToArray();

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(byte item)
        {
            return list.Contains(item);
        }

        public void CopyTo(byte[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(byte item)
        {
            return list.Remove(item);
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return ((IEnumerable<byte>)list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }
    }
}
