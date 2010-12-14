/* 
 * LibNitrous is a simple but powerful hacking library for files commonly used in the Nintendo DS ROM filesystem.
 * Copyright (C) 2010  Will Kirkby
 * Read LicenseInformation.txt for more information.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibNitrous
{
    public static class ByteOp
    {
        public static byte[] CopyBytes(byte[] source, long offset, long length)
        {
            byte[] dest = new byte[length];
            for (int i = 0; i < dest.Length; i++)
                dest[i] = source[offset + i];
            return dest;
        }
        public static byte[] CopyBytes(byte[] source, long offset)
        {
            return CopyBytes(source, offset, source.Length - offset);
        }
        public static uint ReadUInt32(byte[] data, int offset, bool littleEndian)
        { return (((uint)data[offset + (littleEndian ? 3 : 0)] << 24) | ((uint)data[offset + (littleEndian ? 2 : 1)] << 16) | ((uint)data[offset + (littleEndian ? 1 : 2)] << 8) | ((uint)data[offset + (littleEndian ? 0 : 3)])); }
        public static ushort ReadUInt16(byte[] data, int offset, bool littleEndian)
        { return (ushort)((ushort)(data[offset + (littleEndian ? 1 : 0)] << 8) | (ushort)(data[offset + (littleEndian ? 0 : 1)])); }
        public static string ReadString(byte[] data, int offset, int length, bool littleEndian)
        {
            string s = "";
            for (int i = offset; i < offset + length; i++)
                s = littleEndian ? (char)data[i] + s : s + (char)data[i];
            return s;
        }
        public static void WriteUInt32(uint value, ref byte[] data, int offset, bool littleEndian)
        {
            data[offset + 0] = littleEndian ? (byte)((value & 0xFF) >> 0) : (byte)((value & 0xFF000000) >> 24);
            data[offset + 1] = littleEndian ? (byte)((value & 0xFF00) >> 8) : (byte)((value & 0xFF0000) >> 16);
            data[offset + 2] = littleEndian ? (byte)((value & 0xFF0000) >> 16) : (byte)((value & 0xFF00) >> 8);
            data[offset + 3] = littleEndian ? (byte)((value & 0xFF000000) >> 24) : (byte)((value & 0xFF) >> 0);
            // Clean this up with a for loop.
        }
        public static void WriteUInt16(ushort value, ref byte[] data, int offset, bool littleEndian)
        {
            data[offset + 0] = littleEndian ? (byte)((value & 0xFF) >> 0) : (byte)((value & 0xFF00) >> 8);
            data[offset + 1] = littleEndian ? (byte)((value & 0xFF00) >> 8) : (byte)((value & 0xFF) >> 0);
            // Clean this up with a for loop.
        }
        public static void WriteString(string value, ref byte[] data, int offset, bool littleEndian)
        {
            throw new NotImplementedException();
        }
    }
}
