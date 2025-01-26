using Application.Interfaces;
using Application.RequestModel;
using Application.ResponseModel.AccountPage;
using Application.ResponseModel.GoogleDocumentAI;
using Application.ResponseModel.ServicePage;
using Application.Wrappers;
using EStarGoogleCloud;
using EStarGoogleCloud.Response;
using Google.Cloud.Firestore;
using Infrastructure.Identity.Models;
using Infrastructure.Shared;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Services
{
    public class ServicesService : IServicesService
    {
        /// <summary>
        /// 用户服务显示模板，每个服务未完成个数
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        public async Task<Response<ServiceList_res>> GetServiceCountAsync(common_req<string> signup_req)
        {
            ServiceList_res serviceList_Res = new ServiceList_res();
            string uid = signup_req.user;
            List<ServiceListCount_res> personalAccounting = new List<ServiceListCount_res>();
            List<ServiceListCount_res> personalInsurance = new List<ServiceListCount_res>();
            List<ServiceListCount_res> companyAccounting = new List<ServiceListCount_res>();
            List<ServiceListCount_res> companyInsurance = new List<ServiceListCount_res>();
            
                string cmdText = string.Format(@" select id,ServiceNameDesc as name,ServiceLevel1,ServiceLevel2,ServiceLevel3,(select count(id) from User_Service where Uid = '{0}' and isnull(Uid,'') != ''  and ServiceId = s.id and Status != '10' ) as userServiceCount from Services s where (ServiceType = '0' OR ServiceType = '2')  and IsSeparateBuy = '1'  and (ServiceLevel1 = '1' or ServiceLevel1 = '2')
 ", signup_req.actioninfo);
                //数据库处理 
                DataTable table = new DataTable();
                GoogleSqlDBHelper.Fill(cmdText, table);
                if (table != null && table.Rows.Count > 0)
                {
                    // 个人会计
                    var sortedQuery = from row in table.AsEnumerable()
                                      where row.Field<int>("ServiceLevel1") == 1
                                      where row.Field<int>("ServiceLevel2") == 1
                                      orderby row.Field<int>("ServiceLevel3") ascending
                                      select new ServiceListCount_res
                                      {
                    id = row.Field<string>("id")??"",   // 只选择需要的字段
                    name = row.Field<string>("name") ?? "",  // 服务名称
                    serviceCount = row.Field<int>("userServiceCount") // 计算服务数量，确保是字符串
                }; 
                    personalAccounting = sortedQuery.ToList();                
               
                //个人保险
                sortedQuery = from row in table.AsEnumerable()
                                      where row.Field<int>("ServiceLevel1") == 1
                                      where row.Field<int>("ServiceLevel2") == 2
                                      orderby row.Field<int>("ServiceLevel3") ascending
                                   select new ServiceListCount_res
                                   {
                                       id = row.Field<string>("id") ?? "",   // 只选择需要的字段
                                       name = row.Field<string>("name") ?? "",  // 服务名称
                                       serviceCount = row.Field<int>("userServiceCount") // 计算服务数量，确保是字符串
                                   };
                    personalInsurance = sortedQuery.ToList();
                // 公司会计
                sortedQuery = from row in table.AsEnumerable()
                              where row.Field<int>("ServiceLevel1") == 2
                              where row.Field<int>("ServiceLevel2") == 1
                              orderby row.Field<int>("ServiceLevel3") ascending
                              select new ServiceListCount_res
                                   {
                                       id = row.Field<string>("id") ?? "",   // 只选择需要的字段
                                       name = row.Field<string>("name") ?? "",  // 服务名称
                                       serviceCount = row.Field<int>("userServiceCount") // 计算服务数量，确保是字符串
                                   };
                    companyAccounting = sortedQuery.ToList();
                //公司保险
                sortedQuery = from row in table.AsEnumerable()
                              where row.Field<int>("ServiceLevel1") == 2
                              where row.Field<int>("ServiceLevel2") == 2
                              orderby row.Field<int>("ServiceLevel3") ascending
                              select new ServiceListCount_res
                                  {
                                      id = row.Field<string>("id") ?? "",   // 只选择需要的字段
                                      name = row.Field<string>("name") ?? "",  // 服务名称
                                      serviceCount = row.Field<int>("userServiceCount") // 计算服务数量，确保是字符串
                                  };
                    companyInsurance = sortedQuery.ToList();
            }
            serviceList_Res.personalAccounting = personalAccounting;
            serviceList_Res.personalInsurance = personalInsurance;
            serviceList_Res.companyAccounting = companyAccounting;
            serviceList_Res.companyInsurance = companyInsurance;
            return new Response<ServiceList_res>(serviceList_Res, "");
        }
        /// <summary>
        /// 添加用户购买的服务
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static int AddUserService(UserServiceModel userProjectModel)
        {
            int num = 0;
            string sql = "";
            sql = "SELECT top 1 ProjectName FROM Project_Service WHERE id = '" + userProjectModel.ServiceId + "';";
            string servicename = userProjectModel.Year + GoogleSqlDBHelper.ExecuteScalar(sql);
            sql = @" insert into User_Service(CreateTime, Uid, Year, ServiceId, Status, Begindate, Enddate, Sid, Sid1, Sid2, Descs,Name) 
values(@CreateTime, @Uid, @Year, @ServiceId, @Status, @Begindate, @Enddate, @Sid, @Sid1, @Sid2, @Descs,@Name) ";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@Uid", userProjectModel.UId);
            cmd.Parameters.AddWithValue("@CreateTime", userProjectModel.CreateTime);
            cmd.Parameters.AddWithValue("@Year", userProjectModel.Year);
            cmd.Parameters.AddWithValue("@ServiceId", userProjectModel.ServiceId);
            cmd.Parameters.AddWithValue("@Status", userProjectModel.Status);
            cmd.Parameters.AddWithValue("@Begindate", userProjectModel.BeginDate);
            cmd.Parameters.AddWithValue("@Enddate", userProjectModel.EndDate);
            cmd.Parameters.AddWithValue("@Sid", userProjectModel.SId);
            cmd.Parameters.AddWithValue("@Sid1", userProjectModel.SId1);
            cmd.Parameters.AddWithValue("@Sid2", userProjectModel.SId2);
            cmd.Parameters.AddWithValue("@Descs", userProjectModel.Descs);
            cmd.Parameters.AddWithValue("@Name", servicename);
            try
            {
                num = GoogleSqlDBHelper.ExecuteNonQuery(cmd);
                if (num <= 0)
                {
                    string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                    Pub.SaveLog(nameof(AccountService), $"新增用户购买的服务失败, SQL: {logSql}");
                }
                return num;
            }
            catch (Exception ex)
            {
                string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                Pub.SaveLog(nameof(AccountService), $"插入用户购买的服务时发生异常:{ex.Message} , SQL: {logSql}");
                return num;
            }
        }

        /// <summary>
        /// 添加用户任务
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static int AddUserTask(UserTaskModel userTaskModel)
        {
            int num = 0;
            string sql = "";
            sql = @" insert into User_Task(Uid, UserServiceId, CreateTime, UpdateTime, Sid, TaskTitle, TaskContent, Status, Type, SendType) 
values(@Uid, @UserServiceId, @CreateTime, @UpdateTime, @Sid, @TaskTitle, @TaskContent, @Status, @Type, @SendType) ";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@Uid", userTaskModel.Uid);
            cmd.Parameters.AddWithValue("@UserServiceId", userTaskModel.UserServiceId);
            cmd.Parameters.AddWithValue("@CreateTime", userTaskModel.CreateTime);
            cmd.Parameters.AddWithValue("@UpdateTime", userTaskModel.UpdateTime);
            cmd.Parameters.AddWithValue("@Sid", userTaskModel.Sid);
            cmd.Parameters.AddWithValue("@TaskTitle", userTaskModel.TaskTitle);
            cmd.Parameters.AddWithValue("@TaskContent", userTaskModel.TaskContent);
            cmd.Parameters.AddWithValue("@Status", userTaskModel.Status);
            cmd.Parameters.AddWithValue("@Type", userTaskModel.Type);
            cmd.Parameters.AddWithValue("@SendType", userTaskModel.SendType);
            try
            {
                num = GoogleSqlDBHelper.ExecuteNonQuery(cmd);
                if (num <= 0)
                {
                    string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                    Pub.SaveLog(nameof(AccountService), $"新增用户任务, SQL: {logSql}");
                }
                return num;
            }
            catch (Exception ex)
            {
                string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                Pub.SaveLog(nameof(AccountService), $"插入用户任务时发生异常:{ex.Message} , SQL: {logSql}");
                return num;
            }
        }

        /// <summary>
        /// 获取用户任务信息
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<UserTask_res>> GetUserTaskListAsync(common_req<string> signup_req)
        {
            UserTask_res userTask_Res = new UserTask_res();
            List<UserUnfinishedTask> userUnfinishedTasks = new List<UserUnfinishedTask>();
            List<UserCompleteServices> userCompleteServices = new List<UserCompleteServices>();
            string cmdText = string.Format(@" SELECT id,taskTitle,taskContent,type as taskType FROM user_task where uid = '{0}' and status = '0' order by CreateTime DESC ", signup_req.actioninfo);
            //数据库处理
            DataTable table = new DataTable();
            GoogleSqlDBHelper.Fill(cmdText, table);
            if (table != null && table.Rows.Count > 0)
            {
                userUnfinishedTasks = Pub.ToList<UserUnfinishedTask>(table);
            }
            userTask_Res.userUnfinishedTasks = userUnfinishedTasks;
            userTask_Res.userCompleteServices = userCompleteServices;
            return new Response<UserTask_res>(userTask_Res, "");
        }
        /// <summary>
        /// 获取用户任务详情
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<UserTaskDetail_res>> GetUserTaskDetailAsync(common_req<string> signup_req)
        {

            UserTaskDetail_res userDetailTask_Res = new UserTaskDetail_res();

            return new Response<UserTaskDetail_res>(userDetailTask_Res, "");
        }
        /// <summary>
        /// 修改用户任务状态
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        public async Task<Response<string>> UpdateUserTaskStatusAsync(common_req<string> signup_req)
        {
            string sqltxt = @" Update User_Task SET Status = '" + signup_req.type + "',UpdateTime = getdate() WHERE id = '" + signup_req.actioninfo + "' ";
            string msg = "";
            int num = GoogleSqlDBHelper.ExecuteNonQuery(sqltxt, ref msg);
            if (num > 0)
            {
                if (signup_req.type.Equals("1"))
                {
                    sqltxt = @" SELECT UserServiceId User_Task WHERE id = '" + signup_req.actioninfo + "' ";
                    string userServiceId = GoogleSqlDBHelper.ExecuteScalar(sqltxt);
                    if (!string.IsNullOrEmpty(userServiceId))
                    {
                        sqltxt = @" Update User_Service SET Status = '" + signup_req.type + "' WHERE id = '" + userServiceId + "' ";
                        num = GoogleSqlDBHelper.ExecuteNonQuery(sqltxt, ref msg);
                        if (num <= 0)
                        {
                            Pub.SaveLog(nameof(ServicesService), $"修改任务用户服务状态出错！服务id:{userServiceId}，错误信息:{msg}");
                            return new Response<string>("修改任务出错！" + msg);
                        }
                    }
                }
                return new Response<string>("", "");
            }
            else
            {
                Pub.SaveLog(nameof(ServicesService), $"修改任务用户任务状态出错！任务id:{signup_req.actioninfo}，错误信息:{msg}");
                return new Response<string>("修改任务出错！" + msg);
            }


        }
        /// <summary>
        /// 用户已完成，未完成服务列表
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<UserServiceList_res>> GetServiceListAsync(common_req<string> signup_req)
        {
            UserServiceList_res serviceList_Res = new UserServiceList_res();
            List<CompleteServiceList_res> completeServiceList_Res = new List<CompleteServiceList_res>();
            List<UnfinishedServiceList_res> unfinishedServiceList_Res = new List<UnfinishedServiceList_res>();
            string cmdText = string.Format(@"select id,name as serviceName,(select top 1 UserName from Users where id = us.Uid) as userName,FORMAT(Begindate, 'yyyy-MM-dd') AS serviceDate,Status as serviceStatus,FORMAT(Begindate, 'yyyy-MM-dd') AS beginDate from User_Service us 
where Status != '10' and uid = '{0}' ", signup_req.actioninfo);
            //数据库处理
            DataTable table = new DataTable();
            GoogleSqlDBHelper.Fill(cmdText, table);
            if (table != null && table.Rows.Count > 0)
            {
                unfinishedServiceList_Res = Pub.ToList<UnfinishedServiceList_res>(table);
            }
            serviceList_Res.unfinishedServiceList = unfinishedServiceList_Res;
            //serviceList_Res.completeServiceList_Res = completeServiceList_Res;
            return new Response<UserServiceList_res>(serviceList_Res, "");
        }
        /// <summary>
        /// 服务下变量列表
        /// </summary>
        /// <param name="signup_req.actioninfo">服务id</param>
        /// <returns></returns>
        public async Task<Response<List<ServiceItem_res>>> GetServiceItemListAsync(common_req<string> signup_req)
        {
            List<ServiceItem_res> serviceItemList = new List<ServiceItem_res>();
            string cmdText = string.Format(@"select Id,ItemsName,ItemsType,ItemMinInterval,ItemMaxInterval from Service_Items si 
where  ServiceId = '{0}' ", signup_req.actioninfo);
            //数据库处理
            DataTable table = new DataTable();
            GoogleSqlDBHelper.Fill(cmdText, table); 
            if (table != null && table.Rows.Count > 0)
            {
                cmdText = string.Format(@"select Id,ItemsId, ItemsMinNumber, ItemsMaxNumber, BaseValue, ItemsMax, AdditionalValue from Service_ItemsValue siv where siv.ItemsId in (select si.id from Service_Items si 
where si.ServiceId = '{0}') ", signup_req.actioninfo);
                //数据库处理
                DataTable itemvaluetable = new DataTable();
                GoogleSqlDBHelper.Fill(cmdText, itemvaluetable);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    ServiceItem_res serviceItem_Res = new ServiceItem_res();
                    serviceItem_Res.Id = table.Rows[i]["Id"] + "";
                    serviceItem_Res.ItemsName = table.Rows[i]["ItemsName"] + "";
                    serviceItem_Res.ItemsType = table.Rows[i]["ItemsType"] + "";
                    serviceItem_Res.ItemMinInterval = table.Rows[i]["ItemMinInterval"] + "";
                    serviceItem_Res.ItemMaxInterval = table.Rows[i]["ItemMaxInterval"] + "";
                    //获取每项变量下设置的条件，金额
                    if (itemvaluetable != null && itemvaluetable.Rows.Count > 0)
                    {
                        string ItemsId = serviceItem_Res.Id;
                        // 使用 LINQ 查询和排序
                        var sortedQuery = from row in itemvaluetable.AsEnumerable()
                                          where row.Field<string>("ItemsId") == ItemsId
                                          orderby row.Field<string>("ItemsMinNumber") ascending
                                          select row;
                        DataTable sortedTable = sortedQuery.CopyToDataTable();
                        serviceItem_Res.serviceitem = Pub.ToList<ServiceItemValueList_res>(sortedTable);
                    }
                    serviceItemList.Add(serviceItem_Res);
                }
                return new Response<List<ServiceItem_res>>(serviceItemList, "");
            }
            return new Response<List<ServiceItem_res>>("未找到该服务下变量设置");
        }

        /// <summary>
        /// ocr识别文件中表格类型 w-2,1099
        /// </summary>
        /// <returns></returns>
        public async Task<Response<List<GoogleDocumentAIFormName_res>>> GoogleDocumentAIGetFormNameAsync(common_req<string> signup_req)
        {
            // 返回文件数据
            List<GoogleDocumentAIFormName_res> googleDocumentAIFormName_Res = new List<GoogleDocumentAIFormName_res>();
            string endpoint = "";
            string sql = $"select top 1 projectId,locationId,processorId,(select top 1 Year from User_Service where id = '{signup_req.actioninfo}') as year,(select top 1 serviceid from User_Service where id = '{signup_req.actioninfo}') as serviceid from Google_ProcessorsConfig where Name = 'FormName' ";
            DataTable dt = new DataTable();
            GoogleSqlDBHelper.Fill(sql, dt);
            string jsonKeyPath = "C:\\work\\EstarOpenAPI\\EstarOpenAPI\\File\\semiotic-art-418621-88496cd79e0d.json";  // 替换为你的密钥文件路径

            if (dt.Rows.Count > 0)
            {
                string token = await DocumentAIRequest.GetAccessTokenAsync(jsonKeyPath);
                string projects = dt.Rows[0]["projectId"] + "";
                string locations = dt.Rows[0]["locationId"] + "";
                string processors = dt.Rows[0]["processorId"] + "";
                string year = dt.Rows[0]["year"] + "";
                string serviceid = dt.Rows[0]["serviceid"] + "";
                endpoint = $"https://us-documentai.googleapis.com/v1/projects/{projects}/locations/{locations}/processors/{processors}"; // 处理器

                // 初始化 Firestore 客户端
                FirestoreDb firestoreDb = FirestoreDb.Create("semiotic-art-418621");

                // 获取 Firestore 集合
                var collectionReference = firestoreDb.Collection($"customers/{signup_req.user}/services/{serviceid}/manage/upload/year/{year}/file");
                // 构建查询，筛选 `isocrname` 为 "0" 的文档
                Query query = collectionReference.WhereEqualTo("isocrname", "0");
                // 执行查询并获取结果
                QuerySnapshot snapshot = await query.GetSnapshotAsync();

                // 设置并发任务的最大数量，例如限制为 5 个并发任务
                int maxConcurrentTasks = 5;
                SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrentTasks);

                // 创建任务列表
                List<Task> tasks = new List<Task>();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        // 等待信号量，确保并发数量不超过最大限制
                        await semaphore.WaitAsync();

                        try
                        {
                            string mediaLink = doc.GetValue<string>("medialink");
                            string filetype = doc.GetValue<string>("filetype");
                            // 1. 使用 HttpClient 从 MediaLink 获取 PDF 文件内容
                            byte[] pdfBytes = await DocumentAIRequest.DownloadPdfFromMediaLink(mediaLink);

                            // 2. 使用 iTextSharp 拆分 PDF 文件，并获取 byte[] 数组
                            List<byte[]> pdfPages = DocumentAIRequest.SplitPdfByPageToByteArray(pdfBytes);
                            string formtype = "";

                            for (int i = 0; i < pdfPages.Count; i++)
                            {
                                // 处理每个文档
                                DocumentAIResponse documentAIResponses = await DocumentAIRequest.ProcessDocumentNameAsync(pdfPages[i], filetype, token, endpoint);
                                if (documentAIResponses != null && documentAIResponses.document != null && documentAIResponses.document.entities != null && documentAIResponses.document.entities.Count > 0)
                                {
                                    var documententity = documentAIResponses.document.entities;
                                    foreach (var entity in documententity)
                                    {
                                        string mentionText = entity.mentionText.ToUpper();
                                        string entitytype = entity.type.ToUpper();
                                        if (entitytype.Equals("FORM"))
                                        {
                                            switch (mentionText)
                                            {
                                                case "W-2":
                                                case "W2":
                                                    formtype += i + "/W-2,";
                                                    break;
                                                case "1099-A":
                                                case "1099A":
                                                    formtype += i + "/1099-A,";
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                    // 更新 Firestore 中的 filetype 字段
                                    var documentReference = firestoreDb.Collection($"customers/{signup_req.user}/services/{serviceid}/manage/upload/year/{year}/file").Document(doc.Id);
                                    await documentReference.UpdateAsync("isocrname", "1"); // 更新 isocrname
                                    if (!string.IsNullOrEmpty(formtype))
                                    {
                                        formtype = formtype.Substring(0, formtype.Length - 1);
                                        await documentReference.UpdateAsync("formtype", formtype); // 更新 formtype
                                    }
                                }
                            }
                        }
                        finally
                        {
                            // 释放信号量，允许其他任务继续执行
                            semaphore.Release();
                        }
                    }));
                }
                // 等待所有任务完成
                await Task.WhenAll(tasks);
            }

            return new Response<List<GoogleDocumentAIFormName_res>>(googleDocumentAIFormName_Res, "");
        }
        /// <summary>
        /// 谷歌识别文件
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        public async Task GoogleDocumentAIGetFormDataAsync(common_req<string> signup_req)
        {
            //返回文件数据
            string endpoint = "";
            string sql = "select top 1 projectId,locationId,processorId from Google_ProcessorsConfig where Name = 'FormParser' ";
            DataTable dt = new DataTable();
            GoogleSqlDBHelper.Fill(sql, dt);
            sql = $"select id,FileAddress,FileName,FileType,BucketName,FileURL,FileSize from User_Service_File where UId = '{signup_req.user}' and UserServiceId = '{signup_req.actioninfo}' and isocr = '0' ";
            DataTable dt2 = new DataTable();
            GoogleSqlDBHelper.Fill(sql, dt2);
            if (dt != null && dt.Rows.Count > 0 && dt2 != null && dt2.Rows.Count > 0)
            {
                string projects = dt.Rows[0]["projectId"] + "";
                string locations = dt.Rows[0]["locationId"] + "";
                string processors = dt.Rows[0]["processorId"] + "";
                string jsonKeyFilePath = "C:\\work\\EstarOpenAPI\\EstarOpenAPI\\File\\semiotic-art-418621-88496cd79e0d.json";  // 替换为你的密钥文件路径
                // 获取访问令牌
                var token = await DocumentAIRequest.GetAccessTokenAsync(jsonKeyFilePath);
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    //string bucketName = dt2.Rows[i]["BucketName"] + "";
                    string fileName = dt2.Rows[i]["FileAddress"] + "/" + dt2.Rows[i]["FileName"];
                    string fileType = dt2.Rows[i]["FileType"] + "";
                    endpoint = $"https://us-documentai.googleapis.com/v1/projects/{projects}/locations/{locations}/processors/{processors}";
                    DocumentAIResponse documentAIResponses = await DocumentAIRequest.ProcessDocumentAsync(fileName, fileType, token, endpoint);
                    if (documentAIResponses != null && documentAIResponses.document != null && documentAIResponses.document.entities != null && documentAIResponses.document.entities.Count > 0)
                    {
                        var documententity = documentAIResponses.document.entities;
                        string formType = "";
                        foreach (var entity in documententity)
                        {
                            if (entity.type.Equals("Form"))
                            {
                                formType += entity.mentionText + ",";
                            }
                        }
                        formType = formType.Substring(0, formType.Length - 1);
                        sql = $" update User_Service_File set IsOCR = '1' where id = '{dt2.Rows[i]["id"]}' ";
                        string msg = "";
                        GoogleSqlDBHelper.ExecuteNonQuery(sql, ref msg);
                    }
                }
            }
        }
    
    }
}
