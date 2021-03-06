﻿//-----------------------------------------------------------
// 描    述: 字符串编码
// 修改标识: 林建生 1984-02-04
// 修改内容: LJSheng 项目通用类
//-----------------------------------------------------------
using System;
using System.Text;

namespace LJSheng.Common
{
    public class StringTranscoding
    {
        #region Escape编码
        /// <summary>
        /// 进行Javascript的Escape编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Escape(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] ba = System.Text.Encoding.Unicode.GetBytes(str);
            for (int i = 0; i < ba.Length; i += 2)
            {
                sb.Append("%u");
                sb.Append(ba[i + 1].ToString("X2"));
                sb.Append(ba[i].ToString("X2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 进行JavsScript的Escape解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnEscape(string str)
        {
            return System.Web.HttpUtility.UrlDecode(str);
        }
        #endregion

        #region Unicode编码
        /// <summary>
        /// 将单个Unicode字符串转换为普通文字,如\u65e0转换为普通文字时，请这样调用ConvertStr("65e0")
        /// </summary>
        /// <param name="unicodeStr"></param>
        /// <returns></returns>
        public static string ConvertStr(string unicodeStr)
        {
            if (unicodeStr.Length != 4)
            {
                return String.Empty;
            }

            byte byteAfter = Convert.ToByte(unicodeStr.Substring(0, 2), 16);
            byte byteBefore = Convert.ToByte(unicodeStr.Substring(2), 16);

            return System.Text.Encoding.Unicode.GetString(new byte[] { byteBefore, byteAfter });
        }
        /// <summary>
        /// 将单个字符转换为16进制的Unicode字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertUnicode(string str)
        {
            Byte[] arrByte = System.Text.Encoding.Unicode.GetBytes(str);

            string strAfter = Convert.ToString(arrByte[0], 16);
            string strBefore = Convert.ToString(arrByte[1], 16);

            return strBefore + strAfter;
        }


        /// <summary>
        /// Unicode字符串转为正常字符串
        /// </summary>
        /// <param name="srcText"></param>
        /// <returns></returns>
        public static string UnicodeToString(string str)
        {
            string outStr = "";
            if (!string.IsNullOrEmpty(str))
            {
                string[] strlist = str.Replace("/", "").Split('u');
                try
                {
                    for (int i = 1; i < strlist.Length; i++)
                    {
                        //将unicode字符转为10进制整数，然后转为char中文字符  
                        outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
                    }
                }
                catch (FormatException ex)
                {
                    outStr = ex.Message;
                }
            }
            return outStr;
        }
        #endregion
    }
}
