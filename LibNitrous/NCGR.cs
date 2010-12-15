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
    public class NCGR
    {
        public const string
            GHEADER_TAG = "RGCN",
            HEADER_TAG_TTLP = "RAHC",
            HEADER_TAG_PCMP = "SOPC",
            Filter = "*.ncgr";
        private GenericHeader _GHeader;
        private RAHC_Class _RAHC;
        private SOPC_Class _SOPC;
        public GenericHeader GHeader
        {
            get { return _GHeader; }
            set { _GHeader = value; }
        }
        public RAHC_Class RAHC
        {
            get { return _RAHC; }
            set { _RAHC = value; }
        }
        public SOPC_Class SOPC
        {
            get { return _SOPC; }
            set { _SOPC = value; }
        }
        public bool IsValidGHeader
        {
            get { return this._GHeader.IsValid(GHEADER_TAG); }
        }
        public static NCGR FromFile(string path)
        {
            NCGR ncgr;
            if (File.Exists(path))
                ncgr = new NCGR(File.ReadAllBytes(path));
            else throw new Exception("The specified file does not exist.");
            return ncgr;
        }
        public NCGR(byte[] data)
        {
            _GHeader = new GenericHeader(data);
            this._RAHC = new RAHC_Class(ByteOp.CopyBytes(data, Constants.GENERIC_HEADER_SIZE));
            if (this._GHeader.NumberOfSections > 1)
                this._SOPC = new SOPC_Class(ByteOp.CopyBytes(data, this._GHeader.SectionSize));
        }
        public class RAHC_Class
        {
            /*  
                Offset | Length | Name           | Description
                -------+--------+----------------+------------------------------------------------
                   0x0 |    0x4 | Magic ID       | #RAHC (0x52414843)
               //  0x4 |    0x1 | Header Size    | Should always be (0x20)
                   0x4 |    0x4 | Section Size   | Size of this section, including the header.
               //  0x8 |    0x2 | Tile Count     | Multiplied by 1024 gets the total number of pixels in the file.
               //  0xA |    0x2 | Tile Size      | Always (0x20)
                   0x8 |    0x2 | Tile Count Y   | 
                   0xA |    0x2 | Tile Count X   | 
                   0xC |    0x4 | Tile Bit Depth | 3 = 4 Bits
                       |        |                | 4 = 8 Bits
                  0x10 |    0x8 | Padding?       | Always (0x0)
                  0x18 |    0x4 | Tile Data Size | Divided by 1024 should equal Tile Count.
                  0x1C |    0x4 | Unknown        | Always (0x24)
                  DATA |        |                | Tile Data stored as NTFT
             */
            public RAHC_Class(byte[] data)
            { _Data = data; }
            private byte[] _Data;
            public byte[] Data
            { get { return _Data; } }
            /// <summary>
            /// #RAHC (0x52414843)
            /// </summary>
            public string MagicID
            {
                get { return ByteOp.ReadString(_Data, 0x0, 0x4, false); }
                set { ByteOp.WriteString(value.Substring(0, 0x4), ref _Data, 0x0, false); }
            }
            /// <summary>
            /// Size of this section, including the header.
            /// </summary>
            public uint SectionSize
            {
                get { return ByteOp.ReadUInt32(_Data, 0x4, true); }
                set { ByteOp.WriteUInt32(value, ref _Data, 0x4, true); }
            }
            /*
            /// <summary>
            /// Multiplied by 1024 gets the total number of pixels in the file.
            /// </summary>
            public ushort TileCount
            {
                get { return ByteOp.ReadUInt16(_Data, 0x8, true); }
                set { ByteOp.WriteUInt16(value, ref _Data, 0x8, true); }
            }
            /// <summary>
            /// Always (0x20)
            /// </summary>
            public ushort TileSize
            {
                get { return ByteOp.ReadUInt16(_Data, 0xA, true); }
                set { ByteOp.WriteUInt16(value, ref _Data, 0xA, true); }
            }*/
            public ushort TileCountY
            {
                get { return ByteOp.ReadUInt16(_Data, 0x8, true); }
                set { ByteOp.WriteUInt16(value, ref _Data, 0x8, true); }
            }
            public ushort TileCountX
            {
                get { return ByteOp.ReadUInt16(_Data, 0xA, true); }
                set { ByteOp.WriteUInt16(value, ref _Data, 0xA, true); }
            }
            /// <summary>
            /// 3 = 4 Bits
            /// 4 = 8 Bits
            /// </summary>
            public uint TileBitDepth
            {
                get { return ByteOp.ReadUInt32(_Data, 0xC, true); }
                set { ByteOp.WriteUInt32(value, ref _Data, 0xC, true); }
            }
            /// <summary>
            /// Divided by 1024 should equal Tile Count.
            /// </summary>
            public uint TileDataSize
            {
                get { return ByteOp.ReadUInt32(_Data, 0x18, true); }
                set { ByteOp.WriteUInt32(value, ref _Data, 0x18, true); }
            }
            public byte[] GetTile(ushort tile)
            {
                byte[] TileData = new byte[8 * 8];
                for (int i = 0; i < TileData.Length; i++)
                    TileData[i] = (byte)((_Data[0x20 + (TileBitDepth == 3 ? i / 2 : i) + (int)(tile * 8 * 8 * (TileBitDepth == 3 ? 0.5 : 1))] >> (TileBitDepth == 3 && i % 2 == 0 ? 0 : 4)) & (TileBitDepth == 3 ? 0x0F : 0xFF));
                return TileData;
            }
        }
        public class SOPC_Class
        {
            /*
                Offset | Length | Name           | Description
                -------+--------+----------------+------------------------------------------------
                   0x0 |    0x4 | Magic ID       | #SOPC (0x534F5043)
                   0x4 |    0x4 | Section Size   | Should always be (0x10)
                   0x8 |    0x4 | Padding        | Always (0x0)
               //  0xC |    0x2 | Tile Size?     | Always (0x20)
                   0xC |    0x2 | Bytes Per Tile | Always (0x20) It's true for 4-bit depth, need to find an 8-bit depth file to check.
                   0xE |    0x2 | Tile Count     | Is always identical to Tile Count in #1 Section.
             */
            public SOPC_Class(byte[] data)
            { _Data = data; }
            private byte[] _Data;
            public byte[] Data
            { get { return _Data; } }
            /// <summary>
            /// #SOPC (0x534F5043)
            /// </summary>
            public string MagicID
            {
                get { return ByteOp.ReadString(_Data, 0x0, 0x4, false); }
                set { ByteOp.WriteString(value.Substring(0, 0x4), ref _Data, 0x0, false); }
            }
            /// <summary>
            /// Should always be (0x10)
            /// </summary>
            public uint SectionSize
            {
                get { return ByteOp.ReadUInt32(_Data, 0x4, true); }
                set { ByteOp.WriteUInt32(value, ref _Data, 0x4, true); }
            }
            /// <summary>
            /// Always (0x20)
            /// </summary>
            public ushort TileSize
            {
                get { return ByteOp.ReadUInt16(_Data, 0xC, true); }
                set { ByteOp.WriteUInt16(value, ref _Data, 0xC, true); }
            }
            /// <summary>
            /// Is always identical to Tile Count in #1 Section.
            /// </summary>
            public ushort TileCount
            {
                get { return ByteOp.ReadUInt16(_Data, 0xE, true); }
                set { ByteOp.WriteUInt16(value, ref _Data, 0xE, true); }
            }
        }
    }
}