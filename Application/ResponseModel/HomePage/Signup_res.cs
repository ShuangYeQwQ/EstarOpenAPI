namespace EstarOpenAPI.Application.ResponseModel.HomePage
{
    public class Signup_res
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string user { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 用户token
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 用户token过期时间
        /// </summary>
        public string accessTokenExpiration { get; set; }
        
        /// <summary>
        /// 用户刷新token
        /// </summary>
        public string refiesh_token { get; set; }

    }
}
