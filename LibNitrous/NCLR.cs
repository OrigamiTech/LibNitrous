/* 
 * LibNitrous is a simple but powerful hacking library for files commonly used in the Nintendo DS ROM filesystem.
 * Copyright (C) 2010  Will Kirkby
 * Read LicenseInformation.txt for more information.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace LibNitrous
{
    public class NCLR
    {
        public const string
            GHEADER_TAG = "RLCN",
            HEADER_TAG_TTLP = "TTLP",
            HEADER_TAG_PCMP = "PCMP";
        private GenericHeader _GHeader;
        private TTLP_Class _TTLP;
        private PCMP_Class _PCMP;
        public GenericHeader GHeader
        {
            get { return _GHeader; }
            set { _GHeader = value; }
        }
        public TTLP_Class TTLP
        {
            get { return _TTLP; }
            set { _TTLP = value; }
        }
        public PCMP_Class PCMP
        {
            get { return _PCMP; }
            set { _PCMP = value; }
        }
        public bool HasPCMP
        {
            get { return this._GHeader.NumberOfSections > 1; }
        }
        public bool IsValidGHeader
        {
            get { return this._GHeader.IsValid(GHEADER_TAG); }
        }
        public static NCLR FromFile(string path)
        {
            NCLR nclr;
            if (File.Exists(path))
                nclr = new NCLR(File.ReadAllBytes(path));
            else throw new Exception("The specified file does not exist.");
            return nclr;
        }
        public NCLR(byte[] data)
        {
            _GHeader = new GenericHeader(data);
            this._TTLP = new TTLP_Class(ByteOp.CopyBytes(data, Constants.GENERIC_HEADER_SIZE));
            if (this._GHeader.NumberOfSections > 1)
                this._PCMP = new PCMP_Class(ByteOp.CopyBytes(data, this._GHeader.SectionSize));
        }
        public class TTLP_Class
        {
            /*  
                Offset | Length | Name               | Description
                -------+--------+--------------------+------------------------------------------------
                   0x0 |    0x4 | Magic ID	         | #TTLP (0x54544C50)
                   0x4 |    0x4 | Section Size	     | Size of this section, including the header.
                   0x8 |    0x4 | Palette Bit Depth	 | 3 = 4 Bits
                       |        |                    | 4 = 8 Bits
                   0xC |    0x4	| Padding?	         | Always 0x000000
                  0x10 |    0x4	| Palette Data Size	 | Size of Palette Data in bytes.
                       |        |                    | if (0x200-size > 0) then size = 0x200-size
                       |        |                    | also if the bit depth is 8 palette is 0x200
                  0x14 |    0x4	| Colors Per Palette | Probably always 16 colors.
                  DATA |        |                    | Palette Data stored as NTFP
             */
            public TTLP_Class(byte[] data)
            { _Data = data; }
            private byte[] _Data;
            public byte[] Data
            { get { return _Data; } }
            public string MagicID
            {
                get { return ByteOp.ReadString(_Data, 0x0, 0x4, false); }
                set { ByteOp.WriteString(value.Substring(0, 0x4), ref _Data, 0x0, false); }
            }
            public uint SectionSize
            {
                get { return ByteOp.ReadUInt32(_Data, 0x4, true); }
                set { ByteOp.WriteUInt32(value, ref _Data, 0x4, true); }
            }
            public uint PaletteBitDepth
            {
                get { return ByteOp.ReadUInt32(_Data, 0x8, true); }
                set { ByteOp.WriteUInt32(value, ref _Data, 0x8, true); }
            }
            public uint PaletteDataSize
            {
                get
                {
                    uint val = ByteOp.ReadUInt32(_Data, 0x10, true);
                    return 0x200 - val > 0 ? 0x200 - val : val;
                }
                set { ByteOp.WriteUInt32(value, ref _Data, 0x10, true); }
            }
            public uint ColorsPerPalette
            {
                get { return ByteOp.ReadUInt32(_Data, 0x14, true); }
                set { ByteOp.WriteUInt32(value, ref _Data, 0x14, true); }
            }
            public uint PaletteCount
            {
                get { return PaletteDataSize / (ColorsPerPalette * 2); }
            }
            public bool IsValid
            {
                get { return MagicID == HEADER_TAG_TTLP && (PaletteBitDepth == 3 || PaletteBitDepth == 4); }
            }
            public Color GetColor(uint palette, uint color)
            {
                return LibColor.XBGR1555(_Data[0x18 + ((palette * ColorsPerPalette + color) * 2) + 1], _Data[0x18 + ((palette * ColorsPerPalette + color) * 2)]);
            }

            public Color[] GetPalette(uint palette)
            {
                Color[] paletteData = new Color[ColorsPerPalette];
                for (uint i = 0; i < paletteData.Length; i++)
                    paletteData[i] = GetColor(palette, i);
                return paletteData;
            }
        }
        public class PCMP_Class
        {
            /*
                Offset | Length | Name          | Description
                -------+--------+---------------+------------------------------------------------
                   0x0 |    0x4 | Magic ID	    | #PMCP (0x504D4350)
                   0x4 |    0x4 | Section Size  | Should always be (0x12).
                   0x8 |    0x2 | Palette Count	| Number of palettes in file.
                   0xA |    0x6 | Unknown       | Always (0xEFBE080000)
                  DATA |    0x2 | Palette ID    | Simple ID number for each palette (starting from 0x0).
             */
            public PCMP_Class(byte[] data)
            { _Data = data; }
            private byte[] _Data;
            public byte[] Data
            { get { return _Data; } }
            public string MagicID
            {
                get { return ByteOp.ReadString(_Data, 0x0, 0x4, false); }
                set { ByteOp.WriteString(value.Substring(0, 0x4), ref _Data, 0x0, false); }
            }
            public uint SectionSize
            {
                get { return ByteOp.ReadUInt32(_Data, 0x4, true); }
                set { ByteOp.WriteUInt32(value, ref _Data, 0x4, true); }
            }
            public ushort PaletteCount
            {
                get { return ByteOp.ReadUInt16(_Data, 0x8, true); }
                set { ByteOp.WriteUInt16(value, ref _Data, 0x8, true); }
            }
            public ushort UnknownConstant1
            {
                get { return ByteOp.ReadUInt16(_Data, 0xA, true); }
                set { ByteOp.WriteUInt16(value, ref _Data, 0xA, true); }
            }
            public uint UnknownConstant2
            {
                get { return ByteOp.ReadUInt32(_Data, 0xC, true); }
                set { ByteOp.WriteUInt32(value, ref _Data, 0xC, true); }
            }
            public bool IsValid
            {
                get { return MagicID == HEADER_TAG_PCMP && SectionSize == 0x12 && UnknownConstant1 == 0xEFBE && UnknownConstant2 == 0x080000; }
            }
            public ushort GetPaletteID(ushort palette)
            {
                return ByteOp.ReadUInt16(_Data, 0x10 + palette * 2, true);
            }
        }
    }
}