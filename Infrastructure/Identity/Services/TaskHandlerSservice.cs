using Application.Interfaces;
using Application.RequestModel;
using Application.RequestModel.HomePage;
using Application.RequestModel.TaskPage;
using Application.ResponseModel.Task;
using Application.Wrappers;
using EStarGoogleCloud;
using EStarGoogleCloud.Response;
using Google.Cloud.Firestore;
using Infrastructure.Identity.Models;
using Infrastructure.Request.CchGenerator;
using Infrastructure.Request.CchWorkSheet;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Infrastructure.Identity.Services
{
    public class TaskHandlerSservice : ITaskHandlerSservice
    {

        public Task<Response<int>> GetEmployeesCanAddTaskAsync(common_req<string> signup_req)
        {
            string cmdText = string.Format(@"SELECT * FROM Users WHERE Uid IN 
(SELECT Uid FROM User_ServiceDetail usd WHERE id = '{0}' and (OrdinaryEmployees = '{1}' OR ExpertEmployees = '{1}' OR ProfessionalEmployees = '{1}' OR AccountingEmployees = '{1}') AND Status != '10' and 
id not in(select UserServiceDetailId from User_Task where  UserServiceDetailId = usd.id and Status = '0'))", signup_req.Actioninfo, signup_req.User);

            throw new NotImplementedException();
        }
        
        public async Task<Response<int>> AddUserTaskAsync(common_req<Employeetask_res> signup_req)
        {
            string cmdText = " select top 1 id from User_Task where UserServiceDetailId = '"+ signup_req.Actioninfo.UserServiceDetailId + "' and Status = '0'  ";
            int num = 0;
            string taskid = GoogleSqlDBHelper.ExecuteScalar(cmdText);
            if (string.IsNullOrEmpty(taskid)) 
            {
                cmdText = string.Format(@"insert into User_Task(Uid, UserServiceDetailId, CreateTime, UpdateTime, Sid, TaskTitle, TaskContent, Status, Type, SendType,UserContent) 
values((select top 1 UId from User_ServiceDetail where id = @UserServiceDetailId), @UserServiceDetailId, @CreateTime, @UpdateTime, @Sid, @TaskTitle, @TaskContent, @Status, @Type, @SendType,@UserContent)");
                SqlCommand cmd = new SqlCommand(cmdText);
                cmd.Parameters.AddWithValue("@UserServiceDetailId", signup_req.Actioninfo.UserServiceDetailId);
                cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@Sid", signup_req.Actioninfo.Sid);
                cmd.Parameters.AddWithValue("@TaskTitle", signup_req.Actioninfo.TaskTitle);
                cmd.Parameters.AddWithValue("@TaskContent", signup_req.Actioninfo.TaskContent);
                cmd.Parameters.AddWithValue("@Status", "0");
                cmd.Parameters.AddWithValue("@Type", signup_req.Actioninfo.Type);
                cmd.Parameters.AddWithValue("@SendType", "1");
                cmd.Parameters.AddWithValue("@UserContent", "");
                num = GoogleSqlDBHelper.ExecuteNonQuery(cmd);
            }
            else
            {
                return new Response<int>("用户当前服务存在未完成的任务");
            }
            return new Response<int>(num,"");
       
        }
        //更新用户任务状态
        public async Task<Response<int>> UpdateUserTaskStatusAsync(common_req<UpdataTaskStatus_req> signup_req)
        {
            string cmdText = string.Format(@" update user_task set Status = @Status,UserContent=@UserContent where id = @Id ");
            SqlCommand sqlCommand = new SqlCommand(cmdText);
            sqlCommand.Parameters.AddWithValue("@Id", signup_req.Actioninfo.id);
            sqlCommand.Parameters.AddWithValue("@Status", signup_req.Actioninfo.status);
                sqlCommand.Parameters.AddWithValue("@UserContent", signup_req.Actioninfo.content);
            
            int num = GoogleSqlDBHelper.ExecuteNonQuery(sqlCommand);
            return new Response<int>(num, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userservicedetailid"></param>
        public void StartGoogleDocumentAiAsync(string userservicedetailid)
        {
            //获取用户服务详情数据,用户id,服务id,服务年份
            string cmdText = "";
            SqlCommand sqlCommand = new SqlCommand(cmdText);
            sqlCommand.Parameters.AddWithValue("@Id", userservicedetailid);

           


          

        }




        /// <summary>
        /// 谷歌识别文件
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        public async Task GoogleDocumentAIGetFormDataAsync(string userId, string serviceId, string serviceYear, string targetUserServiceDetailId)
        {
            //返回文件数据
            string endpoint = "";
            string sql = "select top 1 projectId,locationId,processorId from Google_ProcessorsConfig where Name = 'FormParser' ";
            DataTable dt = new DataTable();
            GoogleSqlDBHelper.Fill(sql, dt);

            string projects = dt.Rows[0]["projectId"] + "";
            string locations = dt.Rows[0]["locationId"] + "";
            string processors = dt.Rows[0]["processorId"] + "";
            string jsonKeyFilePath = "C:\\work\\EstarOpenAPI\\EstarOpenAPI\\File\\semiotic-art-418621-88496cd79e0d.json";  // 替换为你的密钥文件路径
                                                                                                                           // 获取访问令牌
            var token = await DocumentAIRequest.GetAccessTokenAsync(jsonKeyFilePath);

            //根据详情数据获取识别后数据
            // 初始化 Firestore
            string projectId = "your-project-id";
            string jsonPath = "path/to/your-service-account.json";

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", jsonPath);
            FirestoreDb db = FirestoreDb.Create(projectId);

            // 参数
            // 定位到 file 集合
            CollectionReference fileCollection = db
                .Collection("customers").Document(userId)
                .Collection("services").Document(serviceId)
                .Collection("mamage").Document("download")
                .Collection("year").Document(serviceYear)
                .Collection("file");

            // 查询：UserServiceDetailId == "xx" 且 isocrdata == "xx"
            Query query = fileCollection
                .WhereEqualTo("UserServiceDetailId", targetUserServiceDetailId)
                .WhereEqualTo("isocrdata", "0");

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                // 处理每个文档
                string fileName = doc.GetValue<string>("FileAddress") + "/" + doc.GetValue<string>("FileName");
                string fileType = doc.GetValue<string>("FileType");
                string formType = doc.GetValue<string>("FormType");
                endpoint = $"https://us-documentai.googleapis.com/v1/projects/{projects}/locations/{locations}/processors/{processors}";
                DocumentAIResponse documentAIResponses = await DocumentAIRequest.ProcessDocumentAsync(fileName, fileType, token, endpoint);
                if (documentAIResponses != null && documentAIResponses.document != null && documentAIResponses.document.entities != null && documentAIResponses.document.entities.Count > 0)
                {
                    var documententity = documentAIResponses.document.entities;
                    switch (formType)
                    {
                        case "W2":
                            W2Model w2Form = new W2Model();
                            foreach (var entity in documententity)
                            {
                                SetFieldByType(w2Form, entity);
                            }
                            break;
                         case "1099-A":
                            Interest1099 form1099 = new Interest1099();
                            foreach (var entity in documententity)
                            {
                                SetFieldByType(form1099, entity);
                            }
                            break;
                        default:
                            break;
                    }
                   // string formType = "";
                    //处理识别后数据
                    //foreach (var entity in documententity)
                    //{
                    //    SetFieldByType();
                    //    if (entity.type.Equals("Form"))
                    //    {
                    //        //formType += entity.mentionText + ",";
                    //    }
                    //}
                   // formType = formType.Substring(0, formType.Length - 1);
                    //保存在数据库
                }
            }

        }
        
         void SetFieldByType<T>(T target, Entity entity, string prefix = "") where T : class
        {
            if (!string.IsNullOrWhiteSpace(entity.type))
            {
                string fieldName = string.IsNullOrEmpty(prefix)
                    ? entity.type
                    : entity.type.StartsWith(prefix) ? entity.type.Substring(prefix.Length) : entity.type;

                var prop = typeof(T).GetProperties()
                    .FirstOrDefault(p => string.Equals(p.Name, fieldName, StringComparison.OrdinalIgnoreCase));

                if (prop != null && prop.CanWrite)
                {
                    try
                    {
                        prop.SetValue(target, entity.mentionText);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ 字段赋值失败: {prop.Name} -> {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ 未匹配字段: 实体类型 = {entity.type}，推测属性名 = {fieldName}，但在模型 [{typeof(T).Name}] 中找不到对应属性");
                }
            }
        }

    }
}
