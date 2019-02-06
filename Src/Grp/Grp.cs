using System;
using System.IO;
using System.Text;

namespace DukeNukem3D.Grp
{
    /// <summary>
    /// https://github.com/jonof/jfbuild/blob/master/doc/buildinf.txt
    /// </summary>
    public sealed class Grp
    {
        public Grp(string filename, int fileCount)
        {
            if (Encoding.ASCII.GetByteCount(filename) > 12)
                throw new ArgumentOutOfRangeException(nameof(filename));
            this.Filename = filename;

            this.FileCount = fileCount;
            this.Files = new GrpFile[fileCount];
        }

        /// <summary>
        /// Fixed length: 12
        /// </summary>
        public string Filename
        {
            get;
        }

        /// <summary>
        /// Number of files inside
        /// </summary>
        public int FileCount
        {
            get;
        }

        public GrpFile[] Files
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
            CString.WriteCString(writer, 12, Filename);
            writer.Write((int)FileCount);
            writer.Flush();

            for (int i = 0; i < FileCount; i++)
            {
                var file = Files[i] ?? new GrpFile("", 0);
                file.SaveFileHeader(writer);
            }
            for (int i = 0; i < FileCount; i++)
            {
                var file = Files[i] ?? new GrpFile("", 0);
                writer.Write(file.FileRawData);
                writer.Flush();
            }
        }

        #region Parsing

        public static Grp Parse(string path)
        {
            if (!File.Exists(path))
                return null;

            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                return Parse(reader);
            }
        }

        public static Grp Parse(BinaryReader reader)
        {
            //first 12 bytes for name (author?)
            string filename = CString.ReadCStringShort(reader, 12);
            //followed by 4 bytes for count
            int count = reader.ReadInt32();

            Grp grp = new Grp(filename, count);

            //followed by header for files (name+size)
            for (int i = 0; i < grp.FileCount; i++)
                grp.Files[i] = GrpFile.ParseFileHeader(reader);

            //followed by content of files
            for (uint i = 0; i < grp.FileCount; i++)
            {
                var file = grp.Files[i];
                reader.Read(file.FileRawData, 0, file.FileSize);
            }

            return grp;
        }

        #endregion

        public sealed class GrpFile
        {
            /// <summary>
            /// Fixed length: 12
            /// </summary>
            public string Filename
            {
                get;
            }

            /// <summary>
            /// Size of file in bytes
            /// </summary>
            public int FileSize
            {
                get;
            }

            /// <summary>
            /// Raw file data
            /// </summary>
            public byte[] FileRawData
            {
                get;
            }

            public GrpFile(string filename, int rawCount)
            {
                if (Encoding.ASCII.GetByteCount(filename) > 12)
                    throw new ArgumentOutOfRangeException(nameof(filename));
                this.Filename = filename;

                this.FileSize = rawCount;
                this.FileRawData = new byte[rawCount];
            }

            public void SaveFileHeader(BinaryWriter writer)
            {
                CString.WriteCString(writer, 12, Filename);
                writer.Write((int)this.FileSize);
                writer.Flush();
            }

            #region Static

            public static GrpFile ParseFileHeader(BinaryReader reader)
            {
                //first 12 bytes for filename
                string filename = CString.ReadCStringShort(reader, 12);

                //followed by 4 bytes for size
                int rawSize = reader.ReadInt32();

                return new GrpFile(filename, rawSize);
            }

            #endregion
        }
    }
}