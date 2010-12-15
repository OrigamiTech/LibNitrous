/* 
 * LibNitrous is a simple but powerful hacking library for files commonly used in the Nintendo DS ROM filesystem.
 * Copyright (C) 2010  Will Kirkby
 * Read LicenseInformation.txt for more information.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace LibNitrous
{
    public class ENPG
    {
        public const string Filter = "*.enpg";
        private const int paletteOffset = 0x10000;
        public static ENPG FromFile(string path)
        {
            ENPG enpg;
            if (File.Exists(path))
                enpg = new ENPG(File.ReadAllBytes(path), true);
            else throw new Exception("The specified file does not exist.");
            return enpg;
        }
        public ENPG(byte[] data, bool compressed)
        { _Data = compressed ? LZ77.Decompress(data) : data; }
        private byte[] _Data;
        public byte[] Data
        { get { return _Data; } }
        public byte GetPixel(int index)
        { return _Data[index]; }
        public void SetPixel(int index, byte value)
        { _Data[index] = value; }
        public Color GetColor(int index)
        { return LibColor.XBGR1555(_Data[paletteOffset + index * 2 + 1], _Data[paletteOffset + index * 2]); }
        public void SetColor(int index, Color color)
        { ByteOp.WriteUInt16(LibColor.XBGR1555(color), ref _Data, paletteOffset + index * 2, true); }
        public Bitmap GetAsBitmap(int width)
        {
            Bitmap b = new Bitmap(width, (int)Math.Ceiling((double)paletteOffset / (double)width));
            for (int i = 0; i < paletteOffset; i++)
                b.SetPixel(i % width, i / width, GetColor(GetPixel(i)));
            return b;
        }
    }
}