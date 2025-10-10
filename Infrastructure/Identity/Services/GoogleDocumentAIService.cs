using Application.Interfaces;
using Application.RequestModel;
using Application.ResponseModel.GoogleDocumentAI;
using Application.Wrappers;
using EStarGoogleCloud;
using EStarGoogleCloud.Response;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.DocumentAI.V1;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Location;
using Grpc.Core;
using System.Data;

namespace Infrastructure.Identity.Services
{
    public class GoogleDocumentAIService
    {
        /// <summary>
        /// ocr识别文件中表格类型 w-2,1099
        /// </summary>
        /// <returns></returns>
        public async Task<Response<List<GoogleDocumentAIFormName_res>>> GoogleDocumentAIGetFormNameAsync(common_req<string> signup_req)
        {
            // 返回文件数据
            List<GoogleDocumentAIFormName_res> googleDocumentAIFormName_Res = new List<GoogleDocumentAIFormName_res>();
            string endpoint = "";
            string sql = $"select top 1 projectId,locationId,processorId,(select top 1 ServiceBeginYear from User_Service where id = '{signup_req.Actioninfo}') as year,(select top 1 serviceid from User_Service where id = '{signup_req.Actioninfo}') as serviceid from Google_ProcessorsConfig where Name = 'FormName' ";
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
                var collectionReference = firestoreDb.Collection($"customers/{signup_req.User}/services/{serviceid}/manage/upload/year/{year}/file");
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
                                    if (!string.IsNullOrEmpty(formtype))
                                    {
                                        formtype = formtype.Substring(0, formtype.Length - 1);
                                        // 更新 Firestore 中的 filetype 字段
                                        var documentReference = firestoreDb.Collection($"customers/{signup_req.User}/services/{serviceid}/manage/upload/year/{year}/file").Document(doc.Id);
                                        await documentReference.UpdateAsync("formtype", formtype); // 更新 formtype
                                        await documentReference.UpdateAsync("isocrname", "1"); // 更新 isocrname
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

        // sql = $"select id,FileAddress,FileName,FileType,BucketName,FileURL,FileSize from User_Service_File where UId = '{signup_req.user}' and UserServiceId = '{signup_req.actioninfo}' and isnull(FormType,'') = '' ";
        //DataTable dt2 = new DataTable();
        //GoogleSqlDBHelper.Fill(sql, dt2);
        //if (dt != null && dt.Rows.Count > 0 && dt2 != null && dt2.Rows.Count > 0)
        //{
        //  string projects = dt.Rows[0]["projectId"] + "";
        //    string locations = dt.Rows[0]["locationId"] + "";
        //    string processors = dt.Rows[0]["processorId"] + "";
        //    

        // 获取访问令牌
        //    var token = await DocumentAIRequest.GetAccessTokenAsync(jsonKeyFilePath);
        //for (int i = 0; i < dt2.Rows.Count; i++)
        //{
        //    documentAIFormName_Res = new GoogleDocumentAIFormName_res();
        //    documentAIFormName_Res.Id = dt2.Rows[i]["id"] + "";
        //    documentAIFormName_Res.BucketName = dt2.Rows[i]["BucketName"] + "";
        //    documentAIFormName_Res.FileName = dt2.Rows[i]["FileName"] + "";
        //    documentAIFormName_Res.FileURL = dt2.Rows[i]["FileURL"] + "";
        //    documentAIFormName_Res.FileSize = dt2.Rows[i]["FileSize"] + "";
        //    documentAIFormName_Res.FileAddress = dt2.Rows[i]["FileAddress"] + "";
        //    documentAIFormName_Res.FormType = "";
        //    documentAIFormName_Res.FileType = dt2.Rows[i]["FileType"] + "";
        //    //string bucketName = dt2.Rows[i]["BucketName"] + "";
        //    string fileName = dt2.Rows[i]["FileAddress"] + "/" + dt2.Rows[i]["FileName"];
        //    string fileType = dt2.Rows[i]["FileType"] + "";
        //    endpoint = $"https://us-documentai.googleapis.com/v1/projects/{projects}/locations/{locations}/processors/{processors}";
        //    DocumentAIResponse documentAIResponses = await DocumentAIRequest.ProcessDocumentAsync(fileName, fileType, token, endpoint);
        //    if (documentAIResponses != null && documentAIResponses.document != null && documentAIResponses.document.entities != null && documentAIResponses.document.entities.Count > 0)
        //    {
        //        var documententity = documentAIResponses.document.entities;
        //        string formType = "";
        //        foreach (var entity in documententity)
        //        {
        //            if (entity.type.Equals("Form"))
        //            {
        //                formType +=  entity.mentionText + ",";
        //            }
        //        }
        //        formType = formType.Substring(0, formType.Length - 1);
        //        sql = $" update User_Service_File set FormType = '{formType}' where id = '{documentAIFormName_Res.Id}' ";
        //        string msg = "";
        //        GoogleSqlDBHelper.ExecuteNonQuery(sql,ref msg);
        //        documentAIFormName_Res.FormType = formType;
        //        googleDocumentAIFormName_Res.Add(documentAIFormName_Res);
        //    }
        //}
        //}

    


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
            sql = $"select id,FileAddress,FileName,FileType,BucketName,FileURL,FileSize from User_Service_File where UId = '{signup_req.User}' and UserServiceId = '{signup_req.Actioninfo}' and isocr = '0' ";
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
