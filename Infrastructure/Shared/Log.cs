namespace Infrastructure.Shared
{
    public static class LogEx
    {
        //测试
        public static string logDir = "D:/EstarOpenAPI/";
        //正式
        // public static string logDir = qkHelper.GetConfigByA("qk_UploadDate");

        private static void InitDir()
        {
        }

        public static object locker = new object();//添加一个对象作为锁

        /// <summary>
        /// 存储错误日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static void WiretLog(string content, string filename)
        {
            try
            {
                string fileurl = logDir;//qkHelper.GetNodeValue("OpenLog", logDir);
                if (string.IsNullOrEmpty(fileurl))
                {
                    return;
                }

                string str = string.Concat(new object[] { filename, "_", DateTime.Now.Year, "-", DateTime.Now.Month, "-", DateTime.Now.Day, ".log" });
                string str2 = logDir; // HttpContext.Current.Server.MapPath("~/logs");
                string logPath = str2 + @"\" + str;
                DirectoryInfo info = new DirectoryInfo(str2 + @"\");
                if (!info.Exists)
                {
                    info.Create();
                }

                lock (locker)
                {
                    using (var fs = new StreamWriter(logPath, true))
                    {
                        fs.WriteLine("当前时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        fs.WriteLine("记录：" + content);
                        fs.WriteLine();
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        ///// <summary>
        ///// 将日志记录在数据库中
        ///// </summary>
        ///// <param name="table">数据表</param>
        ///// <param name="data"></param>
        //public static void WriteLogToDataBase(string tablename,string desc,string title,string connectring)
        //{
        //    string sql = string.Format("insert into {0}(title,desc,createdate) values('{1}','{2}','{3}')",tablename,title,desc,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //}
    }

}
