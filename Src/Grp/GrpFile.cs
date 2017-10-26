using System;
using System.IO;
using System.Text;

namespace DukeNukem3D.Grp
{
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
            Grp.WriteCString(writer, 12, Filename);
            writer.Write((int)this.FileSize);
            writer.Flush();
        }

        #region Static

        public static GrpFile ParseFileHeader(BinaryReader reader)
        {
            //first 12 bytes for filename
            string filename = Grp.ReadCStringShort(reader, 12);

            //followed by 4 bytes for size
            int rawSize = reader.ReadInt32();

            return new GrpFile(filename, rawSize);
        }

        #endregion
    }
}