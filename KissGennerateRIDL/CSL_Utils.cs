/*
 *           C#Like
 * Copyright © 2022 RongRong. All right reserved.
 */
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.IO.Compression;

namespace CSharpLike
{
    public class CSL_Utils
    {
        #region MD5 Utils
        public static string GetMD5(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            return GetMD5(Encoding.UTF8.GetBytes(str));
        }
        static MD5 md5 = new MD5CryptoServiceProvider();
        public static string GetMD5(byte[] data, int offset = 0, int count = 0)
        {
            if (count <= 0)
                count = data.Length;
            byte[] targetData = md5.ComputeHash(data, offset, count);
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < targetData.Length; i++)
                strBuilder.AppendFormat("{0:x2}", targetData[i]);
            return strBuilder.ToString();
        }
        public static string GetMD5(FileStream stream)
        {
            byte[] targetData = md5.ComputeHash(stream);
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < targetData.Length; i++)
                strBuilder.AppendFormat("{0:x2}", targetData[i]);
            return strBuilder.ToString();
        }
        public static string GetMD5ByFileName(string str)
        {
            using (FileStream fs = new FileStream(str, FileMode.Open, FileAccess.Read))
            {
                return GetMD5(fs);
            }
        }
        #endregion
        #region Compress/Decompress final binary file
        public static byte[] Compress(byte[] buff)
        {
            ////No compress,direct return.
            //return buff;

            //Compress with zip
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(buff, 0, buff.Length);
                    zipStream.Close();
                    return outStream.ToArray();
                }
            }
        }
        public static byte[] Decompress(byte[] buff)
        {
            ////No decompress,direct return.
            //return buff;

            //Decompress with zip
            using (MemoryStream inputStream = new MemoryStream(buff))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zipStream.CopyTo(outStream);
                        zipStream.Close();
                        return outStream.ToArray();
                    }
                }
            }
        }
        #endregion
    }
}
