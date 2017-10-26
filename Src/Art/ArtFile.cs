using System;
using System.Collections.Generic;
using System.IO;

namespace DukeNukem3D.Art
{
    /// <summary>
    /// http://www.shikadi.net/moddingwiki/ART_Format_(Build)
    /// </summary>
    public class ArtFile
    {

        #region Parsing

        public static ArtFile[] ParseAllFilesInDirectory(string path)
        {
            if (!Directory.Exists(path))
                return null;

            List<ArtFile> files = new List<ArtFile>();

            foreach (var filePath in Directory.GetFiles(path))
            {
                var fileName = Path.GetFileName(filePath);
                if (!fileName.EndsWith(".art", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                int fileId;
                if (fileName.Length < 4 + 3 || !int.TryParse(fileName.Substring(fileName.Length - 4 - 3, 3), out fileId))
                    fileId = 0;

                using (var reader = new BinaryReader(File.OpenRead(filePath)))
                {
                    var art = Parse(reader, fileId);
                    if (art != null)
                        files.Add(art);
                }
            }

            return files.ToArray();
        }
        public static ArtFile Parse(string path)
        {
            if (!File.Exists(path))
                return null;

            int id;
            if (!int.TryParse(path.Substring(path.Length - 4 - 3, 3), out id))
                return null;

            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                return Parse(reader);
            }
        }
        public static ArtFile Parse(string path, int fileID)
        {
            if (!File.Exists(path))
                return null;

            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                return Parse(reader, fileID);
            }
        }
        public static ArtFile Parse(BinaryReader reader, int fileID = 0)
        {
            int version = reader.ReadInt32();

            switch (version)
            {
                case 1:
                    {
                        int numTiles = reader.ReadInt32();
                        int localTileStart = reader.ReadInt32();
                        int localTileEnd = reader.ReadInt32();
                        numTiles = (localTileEnd - localTileStart) + 1;

                        var file = new ArtFile(version, numTiles, localTileStart);

                        short[] tileWidth = new short[numTiles];
                        for (int i = 0; i < numTiles; i++)
                            tileWidth[i] = reader.ReadInt16();

                        short[] tileHeight = new short[numTiles];
                        for (int i = 0; i < numTiles; i++)
                            tileHeight[i] = reader.ReadInt16();

                        Animation[] animations = new Animation[numTiles];
                        for (int i = 0; i < numTiles; i++)
                            animations[i] = new Animation(reader.ReadInt32());

                        //Load image body
                        for (int i = 0; i < numTiles; i++)
                        {
                            if (tileWidth[i] == 0 || tileHeight[i] == 0)
                                continue;

                            var texture = new Texture(tileWidth[i], tileHeight[i], animations[i], textureID: i + localTileStart, textureIDInFile: i, fileID: fileID);

#if DEBUG_TO_ERROR
                            Console.Error.WriteLine("{0} ({1}) - {2}x{3}, frames: {4}", i+localTileStart, i, texture.Width, texture.Height, texture.Animation.Stored);
                            //Console.Error.WriteLine("{0}x{1}", texture.Width, texture.Height);
#endif

                            for (int x = 0; x < texture.Width; x++)
                                for (int y = 0; y < texture.Height; y++)
                                    texture.Indexes[x, y] = reader.ReadByte();

                            file.Textures[i] = texture;
                        }

                        return file;
                    }
                default:
                    return null;
            }
        }

        public const int MaxTextureSize = short.MaxValue;

        #endregion

        #region Instance

        public ArtFile(int artVersion, int texturesCount, int startTextureIndex)
        {
            this.ArtVersion = artVersion;
            this.TexturesCount = texturesCount;
            this.StartTextureIndex = startTextureIndex;

            this.Textures = new Texture[TexturesCount];
        }

        /// <summary>
        /// Version number<br>
        /// should be 1
        /// </summary>
        public int ArtVersion
        {
            get;
        }

        public int TexturesCount
        {
            get;
        }

        public int StartTextureIndex
        {
            get;
        }

        public Texture[] Textures
        {
            get;
        }

        public void Save(string path)
        {
            if (File.Exists(path))
                throw new IOException("File already exists");
            using (var writer = new BinaryWriter(File.OpenWrite(path)))
            {
                Save(writer);
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(ArtVersion);

            switch (ArtVersion)
            {
                case 1:
                    {
                        writer.Write((int)TexturesCount);
                        writer.Write((int)StartTextureIndex);
                        writer.Write((int)(StartTextureIndex + TexturesCount - 1));

                        writer.Flush();

                        for (int i = 0; i < TexturesCount; i++)
                            writer.Write((short)Textures[i].Width);

                        writer.Flush();

                        for (int i = 0; i < TexturesCount; i++)
                            writer.Write((short)Textures[i].Height);

                        writer.Flush();

                        for (int i = 0; i < TexturesCount; i++)
                            writer.Write((int)Textures[i].Animation.Stored);

                        writer.Flush();

                        for (int i = 0; i < TexturesCount; i++)
                        {
                            var texture = Textures[i];

                            if (texture == null)
                                continue;

                            for (int x = 0; x < texture.Width; x++)
                                for (int y = 0; y < texture.Height; y++)
                                    writer.Write((byte)texture.Indexes[x, y]);

                            writer.Flush();
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}