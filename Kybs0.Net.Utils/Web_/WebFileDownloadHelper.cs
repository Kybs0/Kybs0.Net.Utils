﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kybs0.Net.Utils
{
    public class WebFileDownloadHelper
    {
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="resourceUri">eg:"http://ydschool-online.nos.netease.com/account_v0205.mp3"</param>
        /// <param name="downloadPath"></param>
        /// <returns></returns>
        public static bool DownloadWithExtensionUri(string resourceUri, out string downloadPath)
        {
            downloadPath = string.Empty;
            if (string.IsNullOrWhiteSpace(resourceUri))
            {
                return false;
            }
            try
            {
                WebRequest request = WebRequest.Create(resourceUri);
                WebResponse response = request.GetResponse();
                using (Stream reader = response.GetResponseStream())
                {
                    var userDownloadFolder = UtilsCommonPath.GetDownloadFolder();
                    downloadPath = Path.Combine(userDownloadFolder, $"{Guid.NewGuid()}{Path.GetExtension(resourceUri)}");        //图片路径命名 
                    using (FileStream writer = new FileStream(downloadPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        byte[] buff = new byte[512];
                        int c = 0;                                           //实际读取的字节数   
                        while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                        {
                            writer.Write(buff, 0, c);
                        }
                        response.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            //下载成功
            return true;
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="resourceUri">下载地址：/api/downLoadPPT?pptId=3</param>
        /// <param name="downloadPath"></param>
        /// <returns></returns>
        public static bool DownloadWithoutExtension(string resourceUri, out string downloadPath)
        {
            downloadPath = string.Empty;
            if (string.IsNullOrWhiteSpace(resourceUri))
            {
                return false;
            }
            try
            {
                var userDownloadFolder = UtilsCommonPath.GetDownloadFolder();
                var downloadResult = DownloadWithoutExtension(resourceUri, userDownloadFolder, out downloadPath);
                if (downloadResult)
                {
                    //下载成功
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"资源{resourceUri}下载失败", ex);
            }
            return false;
        }
        /// <summary>
        /// 下载资源
        /// </summary>
        /// <param name="resourceUri">下载地址：/api/downLoadPPT?pptId=3</param>
        /// <param name="userDownloadFolder">下载保存目录</param>
        /// <param name="downloadPath">本地路径</param>
        /// <returns></returns>
        public static bool DownloadWithoutExtension(string resourceUri, string userDownloadFolder, out string downloadPath)
        {
            downloadPath = string.Empty;
            if (string.IsNullOrWhiteSpace(resourceUri))
                return false;
            try
            {
                WebResponse response = WebRequest.Create(resourceUri).GetResponse();
                var responseHeader = response.Headers["Content-Disposition"];
                var headers = responseHeader.Split(new string[1] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (headers.Length >= 2 && headers[1].Contains("filename") && headers[1].Contains("."))
                {
                    var lastIndexOfPoint = headers[1].LastIndexOf(".", StringComparison.Ordinal);
                    var fileNameExtension = headers[1].Substring(lastIndexOfPoint, headers[1].LastIndexOf("\"", StringComparison.Ordinal) - lastIndexOfPoint);
                    downloadPath = Path.Combine(userDownloadFolder, Guid.NewGuid() + fileNameExtension);
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (FileStream fileStream = new FileStream(downloadPath, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            byte[] buffer = new byte[512];
                            int count;
                            while (responseStream != null && (count = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                                fileStream.Write(buffer, 0, count);
                            response.Close();
                        }
                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
            return false;
        }
    }
}
