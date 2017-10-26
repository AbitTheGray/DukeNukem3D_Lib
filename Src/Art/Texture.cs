using System;
using System.Drawing;

namespace DukeNukem3D.Art
{
    public class Texture
    {
        public Texture(int width, int height, Animation animation, int fileID = 0, int textureID = 0, int textureIDInFile = 0)
        {
            this.Width = width;
            this.Height = height;

            this.Animation = animation;

            this.Indexes = new byte[width, height];

            this.FileID = fileID;
            this.TextureID = textureID;
            this.TextureIDInFile = textureIDInFile;
        }

        public int Width
        {
            get;
        }

        public int Height
        {
            get;
        }

        public Animation Animation
        {
            get;
        }

        public int FileID
        {
            get;
        }
        public int TextureID
        {
            get;
        }
        public int TextureIDInFile
        {
            get;
        }

        public readonly byte[,] Indexes;

        public PaletteFile Palette
        {
            get;
            set;
        }

        public Color this[int x, int y]
        {
            get
            {
                if (Palette == null)
                    return PaletteFile.Transparent_color;
                else
                {
                    int index = Indexes[x, y];
                    if (index == PaletteFile.Transparent_index)
                        return PaletteFile.Transparent_color;
                    else
                        return Palette.Colors[index];
                }
            }
            set
            {
                if (Palette == null)
                    Indexes[x, y] = PaletteFile.Transparent_index;
                else
                    Indexes[x, y] = Palette.GetColorIndex(value);
            }
        }

        public Bitmap AsBitmap
        {
            get
            {
                Bitmap bitmap = new Bitmap(Width, Height);

                for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                        bitmap.SetPixel(x, y, this[x, y]);

                return bitmap;
            }
        }
    }
}