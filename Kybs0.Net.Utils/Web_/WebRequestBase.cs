﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kybs0.Net.Utils
{
    public class WebRequestBase
    {
        protected static async Task<string> RequestUrlAsync(string requestUrl)
        {
            if (string.IsNullOrWhiteSpace(requestUrl))
            {
                return string.Empty;
            }

            try
            {
                return await RequestDataAsync(requestUrl);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static async Task<string> RequestDataAsync(string requestUrl)
        {
            WebRequest translationWebRequest = WebRequest.Create(requestUrl);

            var response = await translationWebRequest.GetResponseAsync();

            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException(),
                    Encoding.GetEncoding("utf-8")))
                {
                    string result = reader.ReadToEnd();
                    var decodeResult = Unicode2String(result);
                    return decodeResult;
                }
            }
        }

        /// <summary>
        /// Unicode转字符串
        /// </summary>
        /// <param name="source">经过Unicode编码的字符串</param>
        /// <returns>正常字符串</returns>
        protected static string Unicode2String(string source)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }
    }
}
