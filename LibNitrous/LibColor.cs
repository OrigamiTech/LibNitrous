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
        public static Color XRGB1555(byte b0, byte b1)
        { return Color.FromArgb(0xFF, (b0 & 0x7C) << 1, ((b0 & 0x03) << 6) | ((b1 & 0xE0) >> 2), (b1 & 0x1F) << 3); }
        public static Color XBGR1555(byte b0, byte b1)
        { return Color.FromArgb(0xFF, (b1 & 0x1F) << 3, ((b0 & 0x03) << 6) | ((b1 & 0xE0) >> 2), (b0 & 0x7C) << 1); }
        public static Color ARGB1555(byte b0, byte b1)
        { return Color.FromArgb((b0 & 0x80), (b0 & 0x7C) << 1, ((b0 & 0x03) << 6) | ((b1 & 0xE0) >> 2), (b1 & 0x1F) << 3); }
        public static Color ABGR1555(byte b0, byte b1)
        { return Color.FromArgb((b0 & 0x80), (b1 & 0x1F) << 3, ((b0 & 0x03) << 6) | ((b1 & 0xE0) >> 2), (b0 & 0x7C) << 1); }
        public static Color RGBA5551(byte b0, byte b1)
        { return Color.FromArgb((b1 & 0x01) << 7, b0 & 0xF8, ((b0 & 0x07) << 5) | ((b1 & 0xC0) >> 3), (b1 & 0x3E) << 2); }
        public static Color BGRA5551(byte b0, byte b1)
        { return Color.FromArgb((b1 & 0x01) << 7, (b1 & 0x3E) << 2, ((b0 & 0x07) << 5) | ((b1 & 0xC0) >> 3), b0 & 0xF8); }
        public static Color RGB565(byte b0, byte b1)
        { return Color.FromArgb(0xFF, b0 & 0xF8, ((b0 & 0x07) << 5) | ((b1 & 0xE0) >> 3), (b1 & 0x1E) << 3); }
        public static Color BGR565(byte b0, byte b1)
        { return Color.FromArgb(0xFF, (b1 & 0x1E) << 3, ((b0 & 0x07) << 5) | ((b1 & 0xE0) >> 3), b0 & 0xF8); }
    }
}
