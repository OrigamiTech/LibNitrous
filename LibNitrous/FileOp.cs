using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LibNitrous
{
    public static class FileOp
    {
        public static Bitmap GetScreenResource(NCLR nclr, NCGR ncgr, NSCR nscr)
        {
            if (ncgr != null)
            {
                if (nclr != null)
                {
                    if (nscr != null)
                    {
                        Bitmap b = new Bitmap(nscr.NRCS.ScreenWidth, nscr.NRCS.ScreenHeight);
                        Graphics g = Graphics.FromImage(b);
                        for (ushort ty = 0; ty < b.Height / 8; ty++)
                        {
                            for (ushort tx = 0; tx < b.Width / 8; tx++)
                            {
                                NSCR.NTFS _NTFS = nscr.NRCS.GetTile(tx, ty);
                                try
                                {
                                    if (_NTFS.TileNumber > 0)
                                    {
                                        byte[] tileData = ncgr.RAHC.GetTile((ushort)(_NTFS.TileNumber - 1));
                                        Color[] palette = nclr.TTLP.GetPalette(_NTFS.PaletteNumber);
                                        for (byte y = 0; y < 8; y++)
                                            for (byte x = 0; x < 8; x++)
                                                g.FillRectangle(new SolidBrush(palette[tileData[x + y * 8]]), new Rectangle(tx * 8 + x, ty * 8 + y, 1, 1));
                                    }
                                }
                                catch { }
                            }
                        }
                        return b;
                    }
                    throw new Exception("NSCR was null");
                }
                throw new Exception("NCLR was null");
            }
            throw new Exception("NCGR was null");
        }
    }
}