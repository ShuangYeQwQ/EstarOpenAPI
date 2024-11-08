namespace FireStoreModel
{
    public class FireStoreDataResult
    {
        /// <summary>
        /// 0:成功，1：失败
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 失败信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public FireStoreData fireStoreData { get; set; }
    }
    public class FireStoreData
    {
        /// <summary>
        /// 数据地址名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object fields { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime updateTime { get; set; }
    }
}
