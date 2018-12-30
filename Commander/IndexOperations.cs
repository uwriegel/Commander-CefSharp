using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    static class IndexOperations
    {
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
        public static int CombineIndexes(byte hi, int lo) => (int)(((uint)hi << 24) | lo);
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
        public static int GetLoIndex(this int value) => (int)((uint)value & 0xFFFFFF);
        public static byte GetHiIndex(this int value) => (byte)(value >> 24);
    }
}
