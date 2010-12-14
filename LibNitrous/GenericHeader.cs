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
    public class GenericHeader
    {
        /* 
            Offset | Length | Name               | Description
            -------+--------+--------------------+------------------------------------------------
               0x0 |    0x4 | Magic ID	         | Identifies the file format.
               0x4 |    0x4 | Constant	         | Always (0xFFFE0001)
               0x8 |    0x4 | Section Size	     | Size of this section, including the header.
               0xC |    0x2 | Header Size	     | Size of this header. (Should always equal 0x10)
               0xE |    0x2 | Number of Sections | The number of sub-sections in this section.
         */
        public GenericHeader(byte[] fileData)
        {
            _Data = new byte[Constants.GENERIC_HEADER_SIZE];
            for (int i = 0; i < Constants.GENERIC_HEADER_SIZE; i++)
                _Data[i] = fileData[i];
        }
        private byte[] _Data;
        public byte[] Data
        { get { return _Data; } }
        /// <summary>
        /// The 4-byte tag at the start of the file.
        /// </summary>
        public string MagicID
        {
            get { return ByteOp.ReadString(_Data, 0x0, 0x4, false); }
            set { ByteOp.WriteString(value.Substring(0, 0x4), ref _Data, 0x0, false); }
        }
        /// <summary>
        /// Constant 0xFFFE0001.
        /// </summary>
        public uint Constant
        {
            get { return ByteOp.ReadUInt32(_Data, 0x4, false); }
            set { ByteOp.WriteUInt32(value, ref _Data, 0x4, false); }
        }
        public uint SectionSize
        {
            get { return ByteOp.ReadUInt32(_Data, 0x8, true); }
            set { ByteOp.WriteUInt32(value, ref _Data, 0x8, true); }
        }
        /// <summary>
        /// Constant 0x0010.
        /// </summary>
        public ushort HeaderSize
        {
            get { return ByteOp.ReadUInt16(_Data, 0xC, true); }
            set { ByteOp.WriteUInt16(value, ref _Data, 0xC, true); }
        }
        public ushort NumberOfSections
        {
            get { return ByteOp.ReadUInt16(_Data, 0xE, true); }
            set { ByteOp.WriteUInt16(value, ref _Data, 0xE, true); }
        }
        public bool IsValid(string type)
        {
            return (this.MagicID == type && this.Constant == Constants.GENERIC_HEADER_CONSTANT && this.HeaderSize == Constants.GENERIC_HEADER_SIZE);
        }
    }
}
