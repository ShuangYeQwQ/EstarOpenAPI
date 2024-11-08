namespace EstarOpenAPI.Application.Wrappers
{
    public class Request
    {
        /// <summary>
        /// 简单的请求包
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Simply_Req<T>
        {
            /// <summary>
            /// 请求包名称
            /// </summary>
            public string ACTION_NAME { get; set; }
            /// <summary>
            /// 请求包的数据对象
            /// </summary>
            public T ACTION_INFO { get; set; }
            /// <summary>
            /// 请求的代码
            /// </summary>
            public string ACTION_RETURN_CODE { get; set; }
            /// <summary>
            /// 请求的提示信息
            /// </summary>
            public string ACTION_RETURN_MESSAGE { get; set; }
        }
        /// <summary>
        /// 通用的请求包
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Common_Req<T> : Simply_Req<ACTION_INFO_Req<T>>
        {
            //public string ACTION_NAME { get; set; }

            //public ACTION_INFO_Req<T> ACTION_INFO { get; set; }

            //public string ACTION_RETURN_CODE { get; set; }

            //public string ACTION_RETURN_MESSAGE { get; set; }
        }
        /// <summary>
        /// 请求包的数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ACTION_INFO_Req<T>
        {
            /// <summary>
            /// 调用的API名称
            /// </summary>
            public string callname { get; set; }
            /// <summary>
            /// 参数数量
            /// </summary>
            public int inCount { get; set; }
            /// <summary>
            /// 返回的参数数量
            /// </summary>
            public int outCount { get; set; }
            /// <summary>
            /// 请求的数据对象
            /// </summary>
            public T inCount1 { get; set; }  
            /// <summary>
            /// 请求的附加数据
            /// </summary>
            public string inCount2 { get; set; } 
            // 请求的数据列表
            public List<T> RESULT_LIST { get; set; }
            /// <summary>
            /// 总数
            /// </summary>
            public int TOTAL { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<T> flag { get; set; }
        }
    }
}
