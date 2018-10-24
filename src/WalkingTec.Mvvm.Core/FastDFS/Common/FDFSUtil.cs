/****************************************************************************************************************
*                                                                                                               *
* Copyright (C)                                                                                                 *
*                                                                                                               *
*                                                                                                               *
*                                                                                                               *
* Author:  
* add   md5，GetToken Function by G小星星                                                                       *
*                                                                                                               *
****************************************************************************************************************/
using System;
using System.IO;
using System.Security.Cryptography;

namespace WalkingTec.Mvvm.Core.FDFS
{
    public class FDFSUtil
    {
        /// <summary>
        /// Convert Long to byte[]
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static byte[] LongToBuffer(long l)
        {
            byte[] buffer = new byte[8];
            buffer[0] = (byte)((l >> 56) & 0xFF);
            buffer[1] = (byte)((l >> 48) & 0xFF);
            buffer[2] = (byte)((l >> 40) & 0xFF);
            buffer[3] = (byte)((l >> 32) & 0xFF);
            buffer[4] = (byte)((l >> 24) & 0xFF);
            buffer[5] = (byte)((l >> 16) & 0xFF);
            buffer[6] = (byte)((l >> 8) & 0xFF);
            buffer[7] = (byte)(l & 0xFF);

            return buffer;
        }
        /// <summary>
        /// Convert byte[] to Long
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static long BufferToLong(byte[] buffer, int offset)
        {
            return (long)(((long)(buffer[offset] >= 0 ? buffer[offset] : 256 + buffer[offset])) << 56) |
                   (long)(((long)(buffer[offset + 1] >= 0 ? buffer[offset + 1] : 256 + buffer[offset + 1])) << 48) |
                   (long)(((long)(buffer[offset + 2] >= 0 ? buffer[offset + 2] : 256 + buffer[offset + 2])) << 40) |
                   (long)(((long)(buffer[offset + 3] >= 0 ? buffer[offset + 3] : 256 + buffer[offset + 3])) << 32) |
                   (long)(((long)(buffer[offset + 4] >= 0 ? buffer[offset + 4] : 256 + buffer[offset + 4])) << 24) |
                   (long)(((long)(buffer[offset + 5] >= 0 ? buffer[offset + 5] : 256 + buffer[offset + 5])) << 16) |
                   (long)(((long)(buffer[offset + 6] >= 0 ? buffer[offset + 6] : 256 + buffer[offset + 6])) << 8) |
                   (long)((buffer[offset + 7] >= 0 ? buffer[offset + 7] : 256 + buffer[offset + 7]));
        }

        public static string ByteToString(byte[] input)
        {
            char[] chars = FDFSConfig.Charset.GetChars(input);
            string result = new string(chars, 0, input.Length);
            return result;
        }

        public static byte[] StringToByte(string input)
        {
            return FDFSConfig.Charset.GetBytes(input);
        }

        /// <summary>
        /// 返回指定路径字符串的扩展名（不带.）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetExtension(string fileName)
        {
            return Path.GetExtension(fileName).TrimStart('.');
        }

        /// <summary>
        /// get token for file URL
        /// </summary>
        /// <param name="file_id">file_id the file id return by FastDFS server</param>
        /// <param name="ts">ts unix timestamp, unit: second</param>
        /// <param name="secret_key">secret_key the secret key</param>
        /// <returns>token string</returns>

        public static string GetToken(string file_id, int ts, string secret_key)
        {
            byte[] bsFileId = StringToByte(file_id);
            byte[] bsKey = StringToByte(secret_key);
            byte[] bsTimestamp = StringToByte(ts.ToString());

            byte[] buff = new byte[bsFileId.Length + bsKey.Length + bsTimestamp.Length];
            Array.Copy(bsFileId, 0, buff, 0, bsFileId.Length);
            Array.Copy(bsKey, 0, buff, bsFileId.Length, bsKey.Length);
            Array.Copy(bsTimestamp, 0, buff, bsFileId.Length + bsKey.Length, bsTimestamp.Length);

            return Md5(buff);
        }
        /// <summary>
        /// md5 function
        /// </summary>
        /// <param name="source">source the input buffer </param>
        /// <returns>md5 string </returns>
        public static string Md5(byte[] source)
        {

            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(source);
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X2");
                int len = pwd.Length;

            }
            return pwd.ToLower();
        }



    }
}

