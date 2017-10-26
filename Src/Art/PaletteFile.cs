using System;
using System.Drawing;
using System.IO;

namespace DukeNukem3D.Art
{
    /// <summary>
    /// http://wiki.eduke32.com/wiki/Palette_data_files#PALETTE.DAT
    /// </summary>
    public class PaletteFile
    {
        public static PaletteFile Parse(string path)
        {
            if (!File.Exists(path))
                return null;

            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                return Parse(reader);
            }
        }
        public static PaletteFile Parse(BinaryReader reader)
        {
            var file = new PaletteFile();

            for (int i = byte.MinValue; i <= byte.MaxValue; i++)
            {
                //The colors range from 0 to 63, so scaling will be required for acceptable display on a modern operating system.
                //This is because of DOS-based memory constraints that don't exist in modern systems.
                //Scale up by 4x.
                byte r = (byte)(reader.ReadByte() * 4);
                byte g = (byte)(reader.ReadByte() * 4);
                byte b = (byte)(reader.ReadByte() * 4);

                file.Colors[i] = Color.FromArgb(r, g, b);
            }

            //TODO add more parts of file

            return file;
        }

        #region Color

        public Color[] Colors
        {
            get;
        } = new Color[256];

        public static readonly Color Transparent_color = Color.Transparent;
        public const byte Transparent_index = byte.MaxValue;

        public byte GetColorIndex(Color color, byte @default = Transparent_index)
        {
            for (int i = byte.MinValue; i <= byte.MaxValue; i++)
                if (Colors[i] == color)
                    return (byte)i;
            return @default;
        }

        public byte GetColorIndexNearest(Color color)
        {
            throw new NotImplementedException();
        }

        public Bitmap ColorsAsBitmap
        {
            get
            {
                var bitmap = new Bitmap(16, 16);

                for (int i = byte.MinValue; i <= byte.MaxValue; i++)
                    bitmap.SetPixel(i % 16, i / 16, Colors[i]);

                return bitmap;
            }
        }

        #endregion
    }
}