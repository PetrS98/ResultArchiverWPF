using System;
using System.Collections.Generic;
using System.Text;

namespace WPFUtilsLib.Helpers
{
    public static class PLCDataTypeHelper
    {
        public static bool PLCGetBool(byte[] databuffer, int position, int bit)
        {
            return Sharp7.S7.GetBitAt(databuffer, position, bit);
        }

        public static ushort PLCGetWord(byte[] databuffer, int position)
        {
            return Sharp7.S7.GetWordAt(databuffer, position);
        }

        public static byte PLCGetByte(byte[] databuffer, int position)
        {
            return Sharp7.S7.GetByteAt(databuffer, position);
        }

        public static short PLCGetInt(byte[] databuffer, int position)
        {
            return Sharp7.S7.GetIntAt(databuffer, position);
        }

        public static int PLCGetDInt(byte[] databuffer, int position)
        {
            return Sharp7.S7.GetDIntAt(databuffer, position);
        }

        public static float PLCGetReal(byte[] databuffer, int position)
        {
            return Sharp7.S7.GetRealAt(databuffer, position);
        }

        public static string PLCGetStringFromChars(byte[] databuffer, int position, int charArrayLen)
        {
            return Sharp7.S7.GetCharsAt(databuffer, position, charArrayLen);
        }

        public static string PLCGetString(byte[] databuffer, int position, int stringLen)
        {
            byte[] bytes = new byte[stringLen];

            for (int i = 0; i < stringLen; i++)
            {
                bytes[i] = databuffer[position + i + 2];
            }

            return Encoding.Default.GetString(bytes);
        }

        public static string PLCGetString(byte[] databuffer, int position)
        {
            return Sharp7.S7.GetStringAt(databuffer, position);
        }

        public static ushort PLCGetUInt(byte[] databuffer, int position)
        {
            return Sharp7.S7.GetUIntAt(databuffer, position);
        }

        public static byte PLCGetUSInt(byte[] databuffer, int position)
        {
            return Sharp7.S7.GetUSIntAt(databuffer, position);
        }

        public static void PLCSetBool(byte[] databuffer, int position, int bit, bool value)
        {
            Sharp7.S7.SetBitAt(databuffer, position, bit, value);
        }

        public static void PLCSetWord(byte[] databuffer, int position, ushort value)
        {
            Sharp7.S7.SetWordAt(databuffer, position, value);
        }

        public static void PLCSetInt(byte[] databuffer, int position, short value)
        {
            Sharp7.S7.SetIntAt(databuffer, position, value);
        }

        public static void PLCSetUInt(byte[] databuffer, int position, ushort value)
        {
            Sharp7.S7.SetUIntAt(databuffer, position, value);
        }

        public static void PLCSetByte(byte[] databuffer, int position, byte value)
        {
            Sharp7.S7.SetByteAt(databuffer, position, value);
        }

        public static void PLCSetString(byte[] databuffer, int position, int maxLen, string value)
        {
            Sharp7.S7.SetStringAt(databuffer, position, maxLen, value);
        }
    }
}
