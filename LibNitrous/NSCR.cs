using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace LibNitrous
{
    public class NSCR
    {
        public const string
            GHEADER_TAG = "RCSN",
            HEADER_TAG_NRCS = "NRCS";
        private GenericHeader _GHeader;
        private NRCS_Class _NRCS;
        public GenericHeader GHeader
        {
            get { return _GHeader; }
            set { _GHeader = value; }
        }
        public NRCS_Class NRCS
        {
            get { return _NRCS; }
            set { _NRCS = value; }
        }
        public bool IsValidGHeader
        {
            get { return this._GHeader.IsValid(GHEADER_TAG); }
        }
        public static NSCR FromFile(string path)
        {
            NSCR nscr;
            if (File.Exists(path))
                nscr = new NSCR(File.ReadAllBytes(path));
            else throw new Exception("The specified file does not exist.");
            return nscr;
        }
        public NSCR(byte[] data)
        {
            _GHeader = new GenericHeader(data);
            this._NRCS = new NRCS_Class(ByteOp.CopyBytes(data, Constants.GENERIC_HEADER_SIZE));
        }
        public class NRCS_Class
        {
            /*  
                Offset | Length | Name               | Description
                -------+--------+--------------------+------------------------------------------------
                   0x0 |    0x4 | Magic ID           | #NRCS (0x4E524353)
               //  0x4 |    0x1 | Header Size        | Should always be (0x14)
                   0x4 |    0x4 | Section Size       | Size of this section, including the header.
                   0x8 |    0x2 | Screen Width       | Value is in pixels.
                   0xA |    0x2 | Screen Height      | Value is in pixels.
                   0xC |    0x4 | Padding            | Always (0x0)
                  0x10 |    0x4 | Screen Data Size   | 
                  DATA |        |                    | Screen Data stored as NTFS
             */
            public NRCS_Class(byte[] data)
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
            public ushort ScreenWidth
            {
                get { return ByteOp.ReadUInt16(_Data, 0x8, true); }
                set { ByteOp.WriteUInt16(value, ref _Data, 0x8, true); }
            }
            public ushort ScreenHeight
            {
                get { return ByteOp.ReadUInt16(_Data, 0xA, true); }
                set { ByteOp.WriteUInt16(value, ref _Data, 0xA, true); }
            }
            public uint ScreenDataSize
            {
                get { return ByteOp.ReadUInt32(_Data, 0x10, true); }
                set { ByteOp.WriteUInt32(value, ref _Data, 0x10, true); }
            }
            public bool IsValid
            {
                get { return MagicID == HEADER_TAG_NRCS; }
            }
            public NTFS GetTile(ushort x, ushort y)
            {
                return new NTFS(_Data[0x14 + (x + ((y * ScreenWidth) / 8)) * 2 + 1], _Data[0x14 + (x + ((y * ScreenWidth) / 8)) * 2]);
            }
        }
        public class NTFS
        {
            ushort _Data = 0;
            public NTFS(byte b0, byte b1)
            { _Data = (ushort)((((ushort)b0) << 8) | (ushort)b1); }
            public NTFS(ushort u)
            { _Data = u; }
            public byte PaletteNumber
            {
                get { return (byte)((_Data >> 12) & 0x0F); }
                set { _Data &= 0x0FFF; _Data |= (ushort)((ushort)value << 12); }
            }
            public bool Xflip
            {
                get { return (_Data & 0x0800) == 0x0800; }
                set { if (value)_Data |= 0x0800; else _Data &= 0xF7FF; }
            }
            public bool Yflip
            {
                get { return (_Data & 0x0400) == 0x0400; }
                set { if (value)_Data |= 0x0400; else _Data &= 0xFBFF; }
            }
            public ushort TileNumber
            {
                get { return (ushort)(_Data & 0x03FF); }
                set { _Data &= 0xFC00; _Data |= (ushort)(value & 0x03FF); }
            }
        }
    }
}
