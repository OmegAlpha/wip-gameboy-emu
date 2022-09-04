using System;
using System.Runtime.CompilerServices;
using Random = UnityEngine.Random;

namespace Drongo.GameboyEmulator.Utils
{
    public static class Tools
    {
        public static byte[] GetSubArray(ref byte[] origin, ref int startIndex, int amount)
        {
            byte[] subArray = new byte[amount];
            System.Array.Copy(origin, startIndex, subArray, 0, amount);

            startIndex += amount;
            
            return subArray;
        }

        // used to simulate random initialized memory
        // and debug if the memory write is working well
        public static void RandomFill(ref byte[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = (byte) Random.Range(0x17, 0xF4);
            }
        }
        
        // used to simulate init to 0
        public static void ZeroFill(ref byte[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = 0;
            }
        }
        
        
        public static string HexString(int number, int positions = 4)
        {
            return "0x" + number.ToString($"X{positions}");
        }

        public static byte ClearBit(ushort value, byte bitPos)
        {
            return (byte) (value & (byte)~(1 << bitPos));
        }
        
        public static byte SetBit(ushort value, byte bitPos)
        {
            return (byte) (value | (byte)(1 << bitPos));
        }
        
        public static ushort GetBIT(ushort value, byte bitPos)
        {
            return (ushort) ((value & (1 << bitPos)) == 0 ? 0 : 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Between(ushort value, ushort a, ushort b)
        {
            return value >= a && value <= b;
        }
    }
}