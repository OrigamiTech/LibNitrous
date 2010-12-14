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
    public static class LZ77
    {
        // All of this code is taken from Treeki's NSMBEditor, available under the GPL at http://nsmb-editor.googlecode.com/
        // I have been through and modified sections of this code.
        private static unsafe int[] Search(byte* source, int position, int length) //Function taken from Nintenlord's compressor. All credits for this code goes to Nintenlord!
        {
            int SlidingWindowSize = 4096, ReadAheadBufferSize = 18;
            if (position >= length)
                return new int[2] { -1, 0 };
            if ((position < 2) || ((length - position) < 2))
                return new int[2] { 0, 0 };
            List<int> results = new List<int>();
            for (int i = 1; (i < SlidingWindowSize) && (i < position); i++)
                if (source[position - (i + 1)] == source[position])
                    results.Add(i + 1);
            if (results.Count == 0)
                return new int[2] { 0, 0 };
            int amountOfBytes = 0;
            bool Continue = true;
            while (amountOfBytes < ReadAheadBufferSize && Continue)
            {
                amountOfBytes++;
                for (int i = results.Count - 1; i >= 0; i--)
                    if (source[position + amountOfBytes] != source[position - results[i] + (amountOfBytes % results[i])])
                    {
                        if (results.Count > 1)
                            results.RemoveAt(i);
                        else
                            Continue = false;
                    }
            }
            return new int[2] { amountOfBytes, results[0] }; //length of data is first, then position
        }
        public static unsafe byte[] Compress(byte[] source)//Function taken from Nintenlord's compressor. All credits for this code goes to Nintenlord!
        {
            fixed (byte* pointer = &source[0])
                return Compress(pointer, source.Length);
        }
        static public unsafe byte[] Compress(byte* source, int length) //Function taken from Nintenlord's compressor. All credits for this code goes to Nintenlord!
        {
            int position = 0, BlockSize = 8;
            List<byte> CompressedData = new List<byte>();
            CompressedData.Add(0x10);
            {
                byte* pointer = (byte*)&length;
                for (int i = 0; i < 3; i++)
                    CompressedData.Add(*(pointer++));
            }
            while (position < length)
            {
                byte isCompressed = 0;
                List<byte> tempList = new List<byte>();
                for (int i = 0; i < BlockSize; i++)
                {
                    int[] searchResult = Search(source, position, length);
                    if (searchResult[0] > 2)
                    {
                        byte add = (byte)((((searchResult[0] - 3) & 0xF) << 4) + (((searchResult[1] - 1) >> 8) & 0xF));
                        tempList.Add(add);
                        add = (byte)((searchResult[1] - 1) & 0xFF);
                        tempList.Add(add);
                        position += searchResult[0];
                        isCompressed |= (byte)(1 << (BlockSize - (i + 1)));
                    }
                    else if (searchResult[0] >= 0)
                        tempList.Add(source[position++]);
                    else
                        break;
                }
                CompressedData.Add(isCompressed);
                CompressedData.AddRange(tempList);
            }
            while (CompressedData.Count % 4 != 0)
                CompressedData.Add(0);
            return CompressedData.ToArray();
        }
        public static byte[] FastCompress(byte[] source)
        {
            byte[] dest = new byte[4 + source.Length + (int)Math.Ceiling((double)source.Length / 8)];
            dest[0] = 0;
            dest[1] = (byte)(source.Length & 0xFF);
            dest[2] = (byte)((source.Length >> 8) & 0xFF);
            dest[3] = (byte)((source.Length >> 16) & 0xFF);
            int FilePos = 4, UntilNext = 0;
            for (int SrcPos = 0; SrcPos < source.Length; SrcPos++)
            {
                if (UntilNext == 0)
                {
                    dest[FilePos] = 0;
                    FilePos++;
                    UntilNext = 8;
                }
                dest[FilePos] = source[SrcPos];
                FilePos++;
                UntilNext -= 1;
            }
            return dest;
        }
        public static byte[] Decompress(byte[] source)
        {
            // This code converted from Elitemap
            int DataLen = source[1] | (source[2] << 8) | (source[3] << 16);
            byte[] dest = new byte[DataLen];
            int i, j, xin = 4, xout = 0, length, offset, windowOffset, data;
            byte d;
            while (DataLen > 0)
            {
                d = source[xin++];
                if (d != 0)
                {
                    for (i = 0; i < 8; i++)
                    {
                        if ((d & 0x80) != 0)
                        {
                            data = ((source[xin] << 8) | source[xin + 1]);
                            xin += 2;
                            length = (data >> 12) + 3;
                            offset = data & 0xFFF;
                            windowOffset = xout - offset - 1;
                            for (j = 0; j < length; j++)
                            {
                                dest[xout++] = dest[windowOffset++];
                                DataLen--;
                                if (DataLen == 0)
                                    return dest;
                            }
                        }
                        else
                        {
                            dest[xout++] = source[xin++];
                            DataLen--;
                            if (DataLen == 0)
                                return dest;
                        }
                        d <<= 1;
                    }
                }
                else
                {
                    for (i = 0; i < 8; i++)
                    {
                        dest[xout++] = source[xin++];
                        DataLen--;
                        if (DataLen == 0)
                            return dest;
                    }
                }
            }
            return dest;
        }
    }
}