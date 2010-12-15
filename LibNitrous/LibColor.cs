/* 
 * LibNitrous is a simple but powerful hacking library for files commonly used in the Nintendo DS ROM filesystem.
 * Copyright (C) 2010  Will Kirkby
 * Read LicenseInformation.txt for more information.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LibNitrous
{
    /// <summary>
    /// Library for Color conversions. 
    /// These all use big-endian encoding, so remember to swap the bits for little-endian!
    /// </summary>
    public static class LibColor
    {
        static byte[] lookup5bpp = new byte[]
        {
            0x00, 0x08, 0x10, 0x18, 0x21, 0x29, 0x31, 0x39,
            0x42, 0x4A, 0x52, 0x5A, 0x63, 0x6B, 0x73, 0x7B,
            0x84, 0x8C, 0x94, 0x9C, 0xA5, 0xAD, 0xB5, 0xBD,
            0xC6, 0xCE, 0xD6, 0xDE, 0xE7, 0xEF, 0xF7, 0xFF
        };
        public static Color XRGB1555(byte b0, byte b1)
        { return XRGB1555((ushort)(((ushort)b0 << 8) | (ushort)b1)); }
        public static Color XRGB1555(ushort u)
        { return Color.FromArgb(0xFF, lookup5bpp[(u >> 10) & 0x1F], lookup5bpp[(u >> 5) & 0x1F], lookup5bpp[u & 0x1F]); }
        public static ushort XRGB1555(Color c)
        { return (ushort)(0x8000 | (((c.R >> 3) & 0x1F) << 10) | (((c.G >> 3) & 0x1F) << 5) | ((c.B >> 3) & 0x1F)); }
        public static Color XBGR1555(byte b0, byte b1)
        { return XBGR1555((ushort)(((ushort)b0 << 8) | (ushort)b1)); }
        public static Color XBGR1555(ushort u)
        { return Color.FromArgb(0xFF, lookup5bpp[u & 0x1F], lookup5bpp[(u >> 5) & 0x1F], lookup5bpp[(u >> 10) & 0x1F]); }
        public static ushort XBGR1555(Color c)
        { return (ushort)(0x8000 | (((c.B >> 3) & 0x1F) << 10) | (((c.G >> 3) & 0x1F) << 5) | ((c.R >> 3) & 0x1F)); }
        public static Color ARGB1555(byte b0, byte b1)
        { return ARGB1555((ushort)(((ushort)b0 << 8) | (ushort)b1)); }
        public static Color ARGB1555(ushort u)
        { return Color.FromArgb((u & 0x8000) != 0 ? 0xFF : 0, lookup5bpp[(u >> 10) & 0x1F], lookup5bpp[(u >> 5) & 0x1F], lookup5bpp[u & 0x1F]); }
        public static ushort ARGB1555(Color c)
        { return (ushort)((c.A == 0 ? 0x0000 : 0x8000) | (((c.R >> 3) & 0x1F) << 10) | (((c.G >> 3) & 0x1F) << 5) | ((c.B >> 3) & 0x1F)); }
        public static Color ABGR1555(byte b0, byte b1)
        { return ABGR1555((ushort)(((ushort)b0 << 8) | (ushort)b1)); }
        public static Color ABGR1555(ushort u)
        { return Color.FromArgb((u & 0x8000) != 0 ? 0xFF : 0, lookup5bpp[u & 0x1F], lookup5bpp[(u >> 5) & 0x1F], lookup5bpp[(u >> 10) & 0x1F]); }
        public static ushort ABGR1555(Color c)
        { return (ushort)((c.A == 0 ? 0x0000 : 0x8000) | (((c.B >> 3) & 0x1F) << 10) | (((c.G >> 3) & 0x1F) << 5) | ((c.R >> 3) & 0x1F)); }
        public static Color RGBA5551(byte b0, byte b1)
        { return RGBA5551((ushort)(((ushort)b0 << 8) | (ushort)b1)); }
        public static Color RGBA5551(ushort u)
        { return Color.FromArgb((u & 0x0001) != 0 ? 0xFF : 0, lookup5bpp[(u >> 11) & 0x1F], lookup5bpp[(u >> 6) & 0x1F], lookup5bpp[(u >> 1) & 0x1F]); }
        public static Color BGRA5551(byte b0, byte b1)
        { return BGRA5551((ushort)(((ushort)b0 << 8) | (ushort)b1)); }
        public static Color BGRA5551(ushort u)
        { return Color.FromArgb((u & 0x0001) != 0 ? 0xFF : 0, lookup5bpp[(u >> 1) & 0x1F], lookup5bpp[(u >> 6) & 0x1F], lookup5bpp[(u >> 11) & 0x1F]); }
        public static Color RGB565(byte b0, byte b1)
        { return RGB565((ushort)(((ushort)b0 << 8) | (ushort)b1)); }
        public static Color RGB565(ushort u)
        { return Color.FromArgb(0xFF, lookup5bpp[(u >> 11) & 0x1F], ((u >> 3) & 0xFC) | ((u >> 9) & 0x03), lookup5bpp[u & 0x1F]); }
        public static Color BGR565(byte b0, byte b1)
        { return BGR565((ushort)(((ushort)b0 << 8) | (ushort)b1)); }
        public static Color BGR565(ushort u)
        { return Color.FromArgb(0xFF, lookup5bpp[u & 0x1F], ((u >> 3) & 0xFC) | ((u >> 9) & 0x03), lookup5bpp[(u >> 11) & 0x1F]); }
    }
}