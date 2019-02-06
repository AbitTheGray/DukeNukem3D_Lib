using System.IO;
using System.Text;

namespace DukeNukem3D
{
    public static class CString
    {
        #region Read
        
        public static string ReadCString(BinaryReader reader, int length)
        {
            byte[] bytes = new byte[length];
            reader.Read(bytes, 0, length);

            //Replace all bytes after first '\0' char
            {
                int nullIndex;
                for (nullIndex = 0; nullIndex < length; nullIndex++)
                    if (bytes[nullIndex] == 0)
                        break;

                for (; nullIndex < length; nullIndex++)
                    bytes[nullIndex] = 0;
            }

            return Encoding.ASCII.GetString(bytes);
        }
        
        public static string ReadCStringShort(BinaryReader reader, int length)
        {
            byte[] bytes = new byte[length];
            reader.Read(bytes, 0, length);

            //Get index of first '\0' char
            int nullIndex;
            for (nullIndex = 0; nullIndex < length; nullIndex++)
                if (bytes[nullIndex] == 0)
                    break;

            return Encoding.ASCII.GetString(bytes, 0, nullIndex);
        }
        
        #endregion

        #region Write

        public static void WriteCString(BinaryWriter writer, int length, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < length; i++)
                    writer.Write((byte)0);
            }
            else
            {
                for (int i = 0; i < str.Length - 1; i++)
                    writer.Write((byte)str[i]);

                for (int i = str.Length - 1; i < length; i++)
                    writer.Write((byte)0);
            }
        }

        #endregion
    }
}