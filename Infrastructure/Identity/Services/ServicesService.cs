using Application.Interfaces;
using Application.RequestModel;
using Application.RequestModel.HomePage;
using Application.RequestModel.PayPage;
using Application.RequestModel.ServicePage;
using Application.ResponseModel.AccountPage;
using Application.ResponseModel.GoogleDocumentAI;
using Application.ResponseModel.ServicePage;
using Application.Wrappers;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EStarGoogleCloud;
using EStarGoogleCloud.Response;
using Google.Cloud.Firestore;
using Infrastructure.Identity.Models;
using Infrastructure.Shared;
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.StyledXmlParser.Jsoup.Select;
using LightGBMSharp;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Org.BouncyCastle.Crypto;
using Stripe.Radar;
using Stripe.V2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static iText.IO.Image.Jpeg2000ImageData;

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
            string uid = signup_req.User;
            List<ServiceListCount_res> personalAccounting = new List<ServiceListCount_res>();
            List<ServiceListCount_res> personalInsurance = new List<ServiceListCount_res>();
            List<ServiceListCount_res> companyAccounting = new List<ServiceListCount_res>();
            List<ServiceListCount_res> companyInsurance = new List<ServiceListCount_res>();

            string cmdText = string.Format(@" select id,ServiceNameDesc as name,ServiceLevel1,ServiceLevel2,ServiceLevel3,(select count(id) from User_Service where Uid = '{0}' and isnull(Uid,'') != ''  and ServiceId = s.id and Status != '10' ) as userServiceCount from Services s where (ServiceType = '0' OR ServiceType = '2')  and IsSeparateBuy = '1'  and (ServiceLevel1 = '1' or ServiceLevel1 = '2')
 ", signup_req.Actioninfo);
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
                                      id = row.Field<string>("id") ?? "",   // 只选择需要的字段
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
        public static string AddUserService(string uid, string sid, string payamount, string paymentplatform, string oid, string currencycode, List<PayItem> payItems)
        {
            //根据点数,服务状态设置服务员工 - 添加服务基本信息  -  各项服务明细（主服务，附加服务）  -  服务变量信息    -    修改服务员工点数
            string OrdinaryEmployees = "";//普通
            string ExpertEmployees = "";//专家
            string ProfessionalEmployees = "";//专业
            string AccountingEmployees = "";//会计
            string cmdText = "";
            string sql = "";
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            DataTable table = new DataTable();
            List<DbCommand> dbcom = new List<DbCommand>();
            try
            {
                #region 客户购买的服务是否已有验证

                #endregion

                #region 将服务按服务模板点数/最近处理服务时间/最近处理服务状态/创建时间分配给员工，获取3个员工id，会计id
                cmdText = @" WITH MinPoints AS (
    SELECT 
        r.RoleType,
        MIN(s.ServiceModelPoints) AS MinPoints
    FROM staff s
    INNER JOIN User_Identity r ON s.RoleId = r.Id
    WHERE r.RoleType IN (2, 3, 4, 5)
    GROUP BY r.RoleType  -- 按角色类型分组，计算每个角色的最小值
),
RankedStaff AS (
    SELECT 
         s.UId as id, 
        r.RoleType, 
        s.ServiceModelPoints, 
        s.ProcessedServiceCount, 
        s.CreateDate, 
        s.HandleServiceDate,
        s.HandleServiceStatus,
        ROW_NUMBER() OVER (
            PARTITION BY r.RoleType
            ORDER BY 
                s.HandleServiceDate ASC,  -- 移除冗余的ServiceModelPoints排序
                CASE 
                    WHEN s.HandleServiceStatus = 0 THEN 1  -- 状态0优先
                    WHEN s.HandleServiceStatus = 3 THEN 2  -- 状态3其次
                    ELSE 3  -- 其他状态最后
                END,
                s.CreateDate ASC
        ) AS RowNum
    FROM staff s
    INNER JOIN User_Identity r ON s.RoleId = r.Id
    INNER JOIN MinPoints mp ON r.RoleType = mp.RoleType 
        AND s.ServiceModelPoints = mp.MinPoints  -- 关联每个角色的最小值
    WHERE 
        r.RoleType IN (2, 3, 4, 5)
        AND s.Status = '2'  -- 假设状态'2'表示有效员工
)
SELECT Id, RoleType
FROM RankedStaff
WHERE RowNum = 1;  -- 取每个角色的第一条记录 ";
                cmd = new SqlCommand(cmdText);
                table = new DataTable();
                GoogleSqlDBHelper.Fill(cmd, table);
                if (table != null && table.Rows.Count > 0)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        string id = table.Rows[i]["id"] + "";
                        string roleType = table.Rows[i]["RoleType"] + "";
                        switch (roleType)
                        {
                            case "2":
                                OrdinaryEmployees = id;
                                break;
                            case "3":
                                ExpertEmployees = id;
                                break;
                            case "4":
                                ProfessionalEmployees = id;
                                break;
                            case "5":
                                AccountingEmployees = id;
                                break;
                        }
                    }
                }
                #endregion

                #region 添加用户服务基本信息
                UserServiceModel userServiceModel = new UserServiceModel();
                userServiceModel.UId = uid;
                userServiceModel.ServiceId = sid;
                userServiceModel.Status = "0";
                userServiceModel.Descs = "";
                userServiceModel.CreateTime = DateTime.Now;
                userServiceModel.FirstPayAmount = Pub.To<decimal>(payamount);
                userServiceModel.TotalAmount = Pub.To<decimal>(payamount);
                sql = @" insert into User_Service(Id,Uid,ServiceId,Status,Descs,CreateTime,FirstPayAmount,TotalAmount) 
values(@Id,@Uid,@ServiceId,@Status,@Descs,@CreateTime,@FirstPayAmount,@TotalAmount) ";
                cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@Id", userServiceModel.Id);
                cmd.Parameters.AddWithValue("@Uid", userServiceModel.UId);
                cmd.Parameters.AddWithValue("@ServiceId", userServiceModel.ServiceId);
                cmd.Parameters.AddWithValue("@Status", userServiceModel.Status);
                cmd.Parameters.AddWithValue("@Descs", userServiceModel.Descs);
                cmd.Parameters.AddWithValue("@CreateTime", userServiceModel.CreateTime);
                cmd.Parameters.AddWithValue("@FirstPayAmount", userServiceModel.FirstPayAmount);
                cmd.Parameters.AddWithValue("@TotalAmount", userServiceModel.TotalAmount);
                dbcom.Add(cmd);
                #endregion

                #region 创建订单基本信息
                UserOrderModel orderModel = new UserOrderModel();
                if (!payamount.Equals("0"))
                {
                    orderModel.Uid = uid;
                    orderModel.Createtime = DateTime.Now;
                    orderModel.UpdateTime = DateTime.Now;
                    orderModel.UserServiceId = userServiceModel.Id;
                    orderModel.PayAmount = Pub.To<decimal>(payamount);
                    orderModel.DiscountAmount = 0;
                    orderModel.OrderId = oid;
                    orderModel.OrderTime = DateTime.Now;
                    orderModel.OrderNote = "";
                    orderModel.OrderSource = paymentplatform;
                    orderModel.CurrencyCode = currencycode;
                    sql = @" insert into user_orders(Id, Uid, CreateTime, UpdateTime, UserServiceId, OrderId, OrderTime, PayAmount, DiscountAmount, OrderNote, OrderSource,CurrencyCode) values(
@Id, @Uid, @CreateTime, @UpdateTime, @UserServiceId, @OrderId, @OrderTime, @PayAmount, @DiscountAmount, @OrderNote, @OrderSource,@CurrencyCode) ";
                    cmd = new SqlCommand(sql);
                    cmd.Parameters.AddWithValue("@Id", orderModel.Id);
                    cmd.Parameters.AddWithValue("@Uid", orderModel.Uid);
                    cmd.Parameters.AddWithValue("@Createtime", orderModel.Createtime);
                    cmd.Parameters.AddWithValue("@UpdateTime", orderModel.UpdateTime);
                    cmd.Parameters.AddWithValue("@UserServiceId", orderModel.UserServiceId);
                    cmd.Parameters.AddWithValue("@OrderId", orderModel.OrderId);
                    cmd.Parameters.AddWithValue("@OrderTime", orderModel.OrderTime);
                    cmd.Parameters.AddWithValue("@PayAmount", orderModel.PayAmount);
                    cmd.Parameters.AddWithValue("@DiscountAmount", orderModel.DiscountAmount);
                    cmd.Parameters.AddWithValue("@OrderNote", orderModel.OrderNote);
                    cmd.Parameters.AddWithValue("@OrderSource", orderModel.OrderSource);
                    cmd.Parameters.AddWithValue("@CurrencyCode", orderModel.CurrencyCode);
                    dbcom.Add(cmd);
                }
                #endregion

                #region 创建服务详情，订单详情，服务任务
                for (int i = 0; i < payItems.Count; i++)
                {
                    var serviceid = payItems[i].Id;
                    var servicemoney = Pub.To<decimal>(payItems[i].Money);
                    var servicenum = payItems[i].Num;
                    var servicetype = payItems[i].Type;//0:基础服务   1: 服务变量  2:附加服务
                    if (payamount.Equals("0"))
                    {
                        servicemoney = 0;
                    }
                    if (!servicetype.Equals("1"))
                    {
                        //添加服务详情
                        UserServiceDetailModel userServiceDetailModel = new UserServiceDetailModel();
                        userServiceDetailModel.UId = uid;
                        userServiceDetailModel.UserServiceId = userServiceModel.Id;
                        userServiceDetailModel.ServiceId = serviceid;
                        userServiceDetailModel.CreateTime = DateTime.Now;
                        userServiceDetailModel.TotalAmount = servicemoney;
                        userServiceDetailModel.PayAmount = servicemoney;
                        userServiceDetailModel.BeginDate = DateTime.Now;
                        userServiceDetailModel.EndDate = DateTime.Now;
                        userServiceDetailModel.Status = "0";
                        userServiceDetailModel.BeginServiceDate = DateTime.Now;
                        userServiceDetailModel.EndServiceDate = DateTime.Now;
                        userServiceDetailModel.ServiceNumber = 1;
                        userServiceDetailModel.IsEnd = "1";
                        userServiceDetailModel.OrdinaryEmployees = OrdinaryEmployees;
                        userServiceDetailModel.ExpertEmployees = ExpertEmployees;
                        userServiceDetailModel.ProfessionalEmployees = ProfessionalEmployees;
                        userServiceDetailModel.AccountingEmployees = AccountingEmployees;
                        userServiceDetailModel.Descs = "";
                        sql = @" insert into User_ServiceDetail(Id,Uid,UserServiceId, ServiceId, CreateTime, TotalAmount, PayAmount, Begindate, Enddate, Status, BeginServiceDate, EndServiceDate, ServiceNumber, IsEnd, OrdinaryEmployees, ExpertEmployees, ProfessionalEmployees, AccountingEmployees, Descs) 
values(@Id,@Uid,@UserServiceId, @ServiceId, @CreateTime, @TotalAmount, @PayAmount, @Begindate, @Enddate, @Status, @BeginServiceDate, @EndServiceDate, @ServiceNumber, @IsEnd, @OrdinaryEmployees, @ExpertEmployees, @ProfessionalEmployees, @AccountingEmployees, @Descs) ";
                        cmd = new SqlCommand(sql);
                        cmd.Parameters.AddWithValue("@Id", userServiceDetailModel.Id);
                        cmd.Parameters.AddWithValue("@Uid", userServiceDetailModel.UId);
                        cmd.Parameters.AddWithValue("@UserServiceId", userServiceDetailModel.UserServiceId);
                        cmd.Parameters.AddWithValue("@ServiceId", userServiceDetailModel.ServiceId);
                        cmd.Parameters.AddWithValue("@CreateTime", userServiceDetailModel.CreateTime);
                        cmd.Parameters.AddWithValue("@TotalAmount", userServiceDetailModel.TotalAmount);
                        cmd.Parameters.AddWithValue("@PayAmount", userServiceDetailModel.PayAmount);
                        cmd.Parameters.AddWithValue("@Begindate", userServiceDetailModel.BeginDate);
                        cmd.Parameters.AddWithValue("@Enddate", userServiceDetailModel.EndDate);
                        cmd.Parameters.AddWithValue("@Status", userServiceDetailModel.Status);
                        cmd.Parameters.AddWithValue("@BeginServiceDate", userServiceDetailModel.BeginServiceDate);
                        cmd.Parameters.AddWithValue("@EndServiceDate", userServiceDetailModel.EndServiceDate);
                        cmd.Parameters.AddWithValue("@ServiceNumber", userServiceDetailModel.ServiceNumber);
                        cmd.Parameters.AddWithValue("@IsEnd", userServiceDetailModel.IsEnd);
                        cmd.Parameters.AddWithValue("@OrdinaryEmployees", userServiceDetailModel.OrdinaryEmployees);
                        cmd.Parameters.AddWithValue("@ExpertEmployees", userServiceDetailModel.ExpertEmployees);
                        cmd.Parameters.AddWithValue("@ProfessionalEmployees", userServiceDetailModel.ProfessionalEmployees);
                        cmd.Parameters.AddWithValue("@AccountingEmployees", userServiceDetailModel.AccountingEmployees);
                        cmd.Parameters.AddWithValue("@Descs", userServiceDetailModel.Descs);
                        dbcom.Add(cmd);


                        cmdText = @" SELECT ID FROM Service_Items WHERE ServiceId = '" + serviceid + "' AND Isrequired = '1' ";
                        cmd = new SqlCommand(cmdText);
                        DataTable dataTable = new DataTable();
                        GoogleSqlDBHelper.Fill(cmd, dataTable);
                        //添加服务变量
                        for (int j = 0; j < dataTable.Rows.Count; j++)
                        {
                            var filteredItems =
   (from item in payItems
    where item.Type == "1" && item.Id == dataTable.Rows[j]["ID"] + ""
    select item).First();
                            if (filteredItems != null && !string.IsNullOrEmpty(filteredItems.Id))
                            {
                                servicemoney += Pub.To<decimal>(filteredItems.Money);
                                UserServiceItems userServiceItems = new UserServiceItems();
                                userServiceItems.UserServiceDeatilId = userServiceDetailModel.Id;
                                userServiceItems.ServiceItemId = filteredItems.Id;
                                userServiceItems.UserServiceItemValue = filteredItems.Num;
                                userServiceItems.UserServiceItemAmount = filteredItems.Money;
                                userServiceItems.StaffServiceItemValue = "0";
                                userServiceItems.StaffServiceItemAmount = "0";
                                sql = @" insert into User_ServiceItems(UserServiceDeatilId, ServiceItemId, UserServiceItemValue, UserServiceItemAmount, StaffServiceItemValue, StaffServiceItemAmount) 
                            values(@UserServiceDeatilId, @ServiceNumber, @ServiceItemId, @UserServiceItemValue, @UserServiceItemAmount, @StaffServiceItemValue, @StaffServiceItemAmount) ";
                                cmd = new SqlCommand(sql);
                                cmd.Parameters.AddWithValue("@UserServiceDeatilId", userServiceItems.UserServiceDeatilId);
                                cmd.Parameters.AddWithValue("@ServiceItemId", userServiceItems.ServiceItemId);
                                cmd.Parameters.AddWithValue("@UserServiceItemValue", userServiceItems.UserServiceItemValue);
                                cmd.Parameters.AddWithValue("@UserServiceItemAmount", userServiceItems.UserServiceItemAmount);
                                cmd.Parameters.AddWithValue("@StaffServiceItemValue", userServiceItems.StaffServiceItemValue);
                                cmd.Parameters.AddWithValue("@StaffServiceItemAmount", userServiceItems.StaffServiceItemAmount);
                                dbcom.Add(cmd);
                            }
                        }
                        //订单明细
                        if (!payamount.Equals("0"))
                        {
                            sql = @" insert into User_OrdersDetail(Oid, UserServiceDetailId, Createtime, PaymentPlatform, PayTime, OrderId, Amount, CurrencyCode, OrderNote) values(
 @Oid, @UserServiceDetailId, @Createtime, @PaymentPlatform, @PayTime, @OrderId, @Amount, @CurrencyCode, @OrderNote) ";
                            cmd = new SqlCommand(sql);
                            cmd.Parameters.AddWithValue("@OId", orderModel.Id);
                            cmd.Parameters.AddWithValue("@UserServiceDetailId", userServiceDetailModel.Id);
                            cmd.Parameters.AddWithValue("@Createtime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@PaymentPlatform", paymentplatform);
                            cmd.Parameters.AddWithValue("@PayTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@OrderId", oid);
                            cmd.Parameters.AddWithValue("@CurrencyCode", currencycode);
                            cmd.Parameters.AddWithValue("@OrderNote", "");
                            cmd.Parameters.AddWithValue("@Amount", servicemoney);
                            dbcom.Add(cmd);
                        }
                        sql = @" insert into user_task(Uid, UserServiceDetailId, CreateTime, UpdateTime, Sid, TaskTitle, TaskContent, Status, Type, SendType) 
values(@Uid, @UserServiceDetailId, @CreateTime, @UpdateTime, @Sid, @TaskTitle, @TaskContent, @Status, @Type, @SendType) ";
                        cmd = new SqlCommand(sql);
                        cmd.Parameters.AddWithValue("@Uid", userServiceDetailModel.UId);
                        cmd.Parameters.AddWithValue("@UserServiceDetailId", userServiceDetailModel.Id);
                        cmd.Parameters.AddWithValue("@Createtime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Sid", "");
                        cmd.Parameters.AddWithValue("@TaskTitle", "服务处理");
                        cmd.Parameters.AddWithValue("@TaskContent", "发送服务所需的文件或输入服务所需的内容");
                        cmd.Parameters.AddWithValue("@Status", "0");
                        cmd.Parameters.AddWithValue("@Type", "0");
                        cmd.Parameters.AddWithValue("@SendType", "0");
                        dbcom.Add(cmd);

                    }
                }
                #endregion



                int num = GoogleSqlDBHelper.ExecuteNonQueryTransaction(dbcom, ref msg);
                if (num <= 0)
                {
                    Pub.SaveLog(nameof(AccountService), $"添加用户购买的服务时发生异常: " + msg);
                }
                return msg;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Pub.SaveLog(nameof(AccountService), $"添加用户购买的服务时发生异常: " + ex.Message);
                return msg;
            }
        }

        /// <summary>
        /// 添加用户订单
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static void AddAccountOrder(UserOrderModel orderModel, List<PayItem> payItems)
        {
            int num = 0;
            string sql = "";
            string msg = "";
            List<DbCommand> dbcom = new List<DbCommand>();
            sql = @" insert into user_orders(uid,ServiceId,createtime,OrderTime,amount,currencycode,oid) values(
@Uid,@ServiceId,@Createtime,@OrderTime,@Amount,@currencycode,@Oid) ";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@Createtime", orderModel.Createtime);
            cmd.Parameters.AddWithValue("@OrderTime", orderModel.OrderTime);
            num = GoogleSqlDBHelper.ExecuteNonQuery(cmd);
            if (num > 0)
            {
                foreach (PayItem payItem in payItems)
                {
                    sql = @" insert into User_OrdersDetail(Oid,ServiceId,createtime,ServiceNumber,PaymentPlatform,PayTime,OrderId,Amount,currencycode,ServiceType) values(
(select top 1 id from User_Orders where uid = '" + orderModel.Uid + "' and  oid = '" + orderModel.OrderId + "' order by CreateTime desc),@ServiceId,@Createtime,@ServiceNumber,@PaymentPlatform,@PayTime,@OrderId,@Amount,@currencycode,@ServiceType) ";
                    cmd = new SqlCommand(sql);
                    cmd.Parameters.AddWithValue("@ServiceId", payItem.Id);
                    cmd.Parameters.AddWithValue("@createtime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ServiceNumber", "1");
                    cmd.Parameters.AddWithValue("@PaymentPlatform", orderModel.OrderSource);
                    cmd.Parameters.AddWithValue("@PayTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@OrderId", orderModel.OrderId);
                    cmd.Parameters.AddWithValue("@Amount", payItem.Money);
                    cmd.Parameters.AddWithValue("@ServiceType", payItem.Type);
                    dbcom.Add(cmd);
                }
                try
                {
                    num = GoogleSqlDBHelper.ExecuteNonQueryTransaction(dbcom, ref msg);
                    // num = GoogleSqlDBHelper.ExecuteNonQuery(cmd);
                    if (num <= 0)
                    {
                        //string logSql = Pub.ReplaceSqlParameters(sql, cmd);
                        Pub.SaveLog(nameof(AccountService), $"新增用户订单详情失败");
                    }
                }
                catch (Exception ex)
                {
                    Pub.SaveLog(nameof(AccountService), $"插入用户订单时发生异常:{ex.Message}");
                }
            }
            else
            {
                Pub.SaveLog(nameof(AccountService), $"新增用户订单失败");
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
            string cmdText = string.Format(@" SELECT id,taskTitle,taskContent,type as taskType FROM user_task where uid = '{0}' and status = '0' order by CreateTime DESC ", signup_req.Actioninfo);
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
            string sqltxt = "  select ut.UserServiceDetailId, ut.CreateTime, ut.TaskTitle, ut.TaskContent, ut.Type,isnull(u.nickname,'系统发布') as sendUser,s.ServiceName,usd.Begindate from User_Task ut left join Users u on ut.Sid = u.Uid inner join User_ServiceDetail usd on ut.UserServiceDetailId = usd.Id inner join Services s on usd.ServiceId = s.Id where ut.uid = @Uid and ut.id = @Id  ";
            SqlCommand sqlCommand = new SqlCommand(sqltxt);
            sqlCommand.Parameters.AddWithValue("@Uid", signup_req.User);
            sqlCommand.Parameters.AddWithValue("@Id", signup_req.Actioninfo);
            DataTable table = new DataTable();
            GoogleSqlDBHelper.Fill(sqlCommand, table);
            if (table != null && table.Rows.Count > 0)
            {
                userDetailTask_Res.sendUser = table.Rows[0]["sendUser"] + "";
                userDetailTask_Res.taskContent = table.Rows[0]["taskContent"] + "";
                userDetailTask_Res.type = table.Rows[0]["type"] + "";
                userDetailTask_Res.createTime = table.Rows[0]["createTime"] + "";
                userDetailTask_Res.userServiceDetailId = table.Rows[0]["userServiceDetailId"] + "";
                userDetailTask_Res.taskTitle = table.Rows[0]["taskTitle"] + "";
                userDetailTask_Res.serviceName = table.Rows[0]["ServiceName"] + "";
                userDetailTask_Res.begindate = table.Rows[0]["Begindate"] + "";
            }
            return new Response<UserTaskDetail_res>(userDetailTask_Res, "");
        }
        /// <summary>
        /// 修改用户任务状态
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        public async Task<Response<string>> UpdateUserTaskStatusAsync(common_req<string> signup_req)
        {
            string sqltxt = @" Update User_Task SET Status = '" + signup_req.Type + "',UpdateTime = getdate() WHERE id = '" + signup_req.Actioninfo + "' ";
            string msg = "";
            int num = GoogleSqlDBHelper.ExecuteNonQuery(sqltxt, ref msg);
            if (num > 0)
            {
                if (signup_req.Type.Equals("1"))
                {
                    sqltxt = @" SELECT UserServiceId User_Task WHERE id = '" + signup_req.Actioninfo + "' ";
                    string userServiceId = GoogleSqlDBHelper.ExecuteScalar(sqltxt);
                    if (!string.IsNullOrEmpty(userServiceId))
                    {
                        sqltxt = @" Update User_Service SET Status = '" + signup_req.Type + "' WHERE id = '" + userServiceId + "' ";
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
                Pub.SaveLog(nameof(ServicesService), $"修改任务用户任务状态出错！任务id:{signup_req.Actioninfo}，错误信息:{msg}");
                return new Response<string>("修改任务出错！" + msg);
            }


        }
        /// <summary>
        /// 用户服务列表
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<UserServiceList_res>> GetUserServiceListAsync(common_req<UserService_req> signup_req)
        {
            string role = signup_req.Actioninfo.Role;
            UserServiceList_res serviceList_Res = new UserServiceList_res();
            serviceList_Res.totalCount = 0;
            serviceList_Res.totalPages = 0;
            serviceList_Res.unfinishedServiceList = new List<UnfinishedServiceList_res>();
            string cmdText = "";
            string servicecountcmdText = "";
            int page = signup_req.Actioninfo.Page;
            int pageSize = 10;
            page = (page - 1) * pageSize;
            string serviceName = signup_req.Actioninfo.ServiceName;
            string status = signup_req.Actioninfo.Status;
            string wheretxt = "";
            if (status.Equals("2"))
            {
                wheretxt += " AND us.Status = '" + status + "'";
            }
            if (!status.Equals("1"))
            {
                wheretxt += " AND us.Status != '10'";
            }

            if (!string.IsNullOrEmpty(serviceName))
            {
                wheretxt += " AND  s.ServiceName like '%" + serviceName + "%' ";
            }
            switch (role)
            {
                case "0":
                    break;
                case "1":
                    break;
                case "2":
                    //普通员工
                    cmdText = string.Format(@" select us.id,s.ServiceName as serviceName,u.NickName as nickName,us.CreateTime as beginDate,us.Status as serviceStatus from User_ServiceDetail us  inner join Services s on us.ServiceId = s.Id inner join Users u on us.Uid = u.Uid
where 1 = 1 AND OrdinaryEmployees = '{0}' {3}
ORDER BY us.CreateTime DESC
OFFSET {1} ROWS 
FETCH NEXT {2} ROWS ONLY ", signup_req.User, page, pageSize, wheretxt);
                    servicecountcmdText = string.Format(@" select COUNT(*) AS TotalCount from User_ServiceDetail us  inner join Services s on us.ServiceId = s.Id inner join Users u on us.Uid = u.Uid
where 1 = 1 AND OrdinaryEmployees = '{0}' {1} ", signup_req.User, wheretxt);
                    break;
                case "3":
                    break;
                case "4":
                    break;
                case "5":
                    break;
                case "6":
                    //普通用户
                    cmdText = string.Format(@" SELECT 
    us.id,
    s.ServiceName AS serviceName,
    u.NickName AS nickName,
    us.CreateTime AS beginDate,
    us.Status AS serviceStatus 
FROM User_ServiceDetail us
INNER JOIN Services s ON us.ServiceId = s.Id
INNER JOIN Users u ON us.Uid = u.Uid
WHERE 1 = 1 AND us.uid = '{0}' {3}
ORDER BY us.CreateTime DESC
OFFSET {1} ROWS 
FETCH NEXT {2} ROWS ONLY ", signup_req.User, page, pageSize, wheretxt);
                    servicecountcmdText = string.Format(@" SELECT COUNT(*) AS TotalCount
FROM User_ServiceDetail us
INNER JOIN Services s ON us.ServiceId = s.Id
INNER JOIN Users u ON us.Uid = u.Uid
WHERE us.uid = '{0}' {1} ", signup_req.User, wheretxt);
                    break;
                case "7":
                    break;
                case "8":
                    break;
                default:
                    break;
            }

            //数据库处理
            DataTable table = new DataTable();
            GoogleSqlDBHelper.Fill(cmdText, table);
            if (table != null && table.Rows.Count > 0)
            {
                serviceList_Res.unfinishedServiceList = Pub.ToList<UnfinishedServiceList_res>(table);
            }
            table = new DataTable();
            GoogleSqlDBHelper.Fill(servicecountcmdText, table);
            if (table != null && table.Rows.Count > 0)
            {
                serviceList_Res.totalCount = Pub.To<int>(table.Rows[0]["totalCount"] + "");
                serviceList_Res.totalPages = (int)Math.Ceiling(serviceList_Res.totalCount / (double)pageSize);
            }
            return new Response<UserServiceList_res>(serviceList_Res, "");
        }


        /// <summary>
        /// 用户服务详情
        /// </summary>
        /// <param name="signup_req"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<UserServiceDetail_res>> GetUserServiceDetailAsync(common_req<string> signup_req)
        {

            //            WITH RankedData AS(
            //  SELECT *,
            //    ROW_NUMBER() OVER(
            //      PARTITION BY ServiceId
            //      ORDER BY ServiceNumber DESC
            //    ) AS rn
            //  FROM User_ServiceDetail
            //  WHERE UserServiceId = @UserServiceId
            //)
            //SELECT rd.UserServiceId, rd.ServiceId,rd.CreateTime,CASE rd.Status
            //    WHEN 10 THEN '已完成'
            //    ELSE '未完成'
            //  END AS Status,s.ServiceNameDesc as ServiceName
            //FROM RankedData rd
            //LEFT JOIN Services s ON rd.ServiceId = s.Id
            //WHERE rd.rn = 1
            //ORDER BY rd.ServiceNumber DESC 
            UserServiceDetail_res userServiceDetail_Res = new UserServiceDetail_res();
            string cmdText = string.Format(@" SELECT s.ServiceName,us.CreateTime,
CASE us.Status   WHEN 10 THEN '已完成'  WHEN 0 THEN '待处理' ELSE '审核中' END as Status
,us.PayAmount,us.Begindate,us.Enddate,us.OrdinaryEmployees,us.ExpertEmployees,us.ProfessionalEmployees,
isnull((select top 1 Name from Staff where uid = us.OrdinaryEmployees),'') as OrdinaryEmployeesName,
isnull((select top 1 Name from Staff where uid = us.ExpertEmployees),'') as ExpertEmployeesName,
isnull((select top 1 Name from Staff where uid = us.ProfessionalEmployees),'') as ProfessionalEmployeesName,
isnull((select top 1 id from User_Task where UserServiceDetailId = us.id and Status = '0' and uid = @uid),'') as taskid
FROM User_ServiceDetail us left join Services s on us.ServiceId = s.Id 
WHERE us.Id = @UserServiceDetailId ");
            SqlCommand sqlCommand = new SqlCommand(cmdText);
            sqlCommand.Parameters.AddWithValue("@UserServiceDetailId", signup_req.Actioninfo);
            sqlCommand.Parameters.AddWithValue("@uid", signup_req.User);
            DataTable table = new DataTable();
            GoogleSqlDBHelper.Fill(sqlCommand, table);
            if (table != null && table.Rows.Count > 0)
            {
                userServiceDetail_Res = Pub.ToList<UserServiceDetail_res>(table).FirstOrDefault();
                return new Response<UserServiceDetail_res>(userServiceDetail_Res, "");
            }
            return new Response<UserServiceDetail_res>("未找到数据");
        }



        /// <summary>
        /// 服务下变量列表
        /// </summary>
        /// <param name="signup_req.actioninfo">服务id</param>
        /// <returns></returns>
        public async Task<Response<List<ServiceItem_res>>> GetServiceItemListAsync(common_req<string> signup_req)
        {
            List<ServiceItem_res> serviceItemList = GetServiceItem(signup_req.Actioninfo);
            if (serviceItemList.Count > 0)
            {


                return new Response<List<ServiceItem_res>>(serviceItemList, "");
            }
            return new Response<List<ServiceItem_res>>("未找到该服务下变量设置");
        }

        public List<ServiceItem_res> GetServiceItem(string id)
        {
            List<ServiceItem_res> serviceItemList = new List<ServiceItem_res>();
            string cmdText = string.Format(@"select Id,ItemsName,ItemsType,ItemMinInterval,ItemMaxInterval,IsRequired,Descs from Service_Items si 
where  ServiceId = '{0}' ", id);
            //数据库处理
            DataTable table = new DataTable();
            GoogleSqlDBHelper.Fill(cmdText, table);
            if (table != null && table.Rows.Count > 0)
            {

                cmdText = string.Format(@"select Id,ItemsId, ItemsMinNumber, ItemsMaxNumber, BaseValue, ItemsMax, AdditionalValue from Service_ItemsValue siv where siv.ItemsId in (select si.id from Service_Items si 
where si.ServiceId = '{0}') ", id);
                //数据库处理
                DataTable itemvaluetable = new DataTable();
                GoogleSqlDBHelper.Fill(cmdText, itemvaluetable);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    ServiceItem_res serviceItem_Res = new ServiceItem_res();
                    serviceItem_Res.Id = table.Rows[i]["Id"] + "";
                    serviceItem_Res.ItemsName = table.Rows[i]["ItemsName"] + "";
                    serviceItem_Res.ItemsType = table.Rows[i]["ItemsType"] + "";
                    serviceItem_Res.IsRequired = table.Rows[i]["IsRequired"] + "";
                    serviceItem_Res.Descs = table.Rows[i]["Descs"] + "";
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
            }
            return serviceItemList;
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
                                    // 更新 Firestore 中的 filetype 字段
                                    var documentReference = firestoreDb.Collection($"customers/{signup_req.User}/services/{serviceid}/manage/upload/year/{year}/file").Document(doc.Id);
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


        /// <summary>
        /// 获取服务信息
        /// </summary>
        /// <param name="signup_req.Actioninfo">选择购买的服务</param>
        /// <returns></returns>
        public async Task<Response<ServiceDeatil_res>> GetServiceDetail(common_req<string> signup_req)
        {
            string cmdText = "";
            string servicetype = signup_req.Actioninfo;
            ServiceDeatil_res serviceDetail = new ServiceDeatil_res();
            switch (servicetype)
            {
                case "1":
                    //个人所得税申报
                    cmdText = string.Format(@" SELECT top 1 id,ServiceName,ServiceNameDesc,Amount,ServiceLevel1,ServiceLevel2,ServiceLevel3,Descs FROM Services s WHERE ServiceLevel1 = '1' AND  ServiceLevel2 = '1' AND ServiceLevel3 = '1' ");
                    break;
                case "8":
                    //新公司成立
                    cmdText = string.Format(@" SELECT top 1 id,ServiceName,ServiceNameDesc,Amount,ServiceLevel1,ServiceLevel2,ServiceLevel3,Descs FROM Services s WHERE ServiceLevel1 = '2' AND  ServiceLevel2 = '1' AND ServiceLevel3 = '1' ");
                    break;
                case "9":
                    //代发薪资
                    cmdText = string.Format(@" SELECT top 1 id,ServiceName,ServiceNameDesc,Amount,ServiceLevel1,ServiceLevel2,ServiceLevel3,Descs FROM Services s WHERE ServiceLevel1 = '2' AND  ServiceLevel2 = '1' AND ServiceLevel3 = '2' ");
                    break;
                case "10":
                    //代理记账
                    cmdText = string.Format(@" SELECT top 1 id,ServiceName,ServiceNameDesc,Amount,ServiceLevel1,ServiceLevel2,ServiceLevel3,Descs FROM Services s WHERE ServiceLevel1 = '2' AND  ServiceLevel2 = '1' AND ServiceLevel3 = '3' ");
                    break;
                case "11":
                    //报表申报

                    break;
                case "12":
                    //报销售
                    cmdText = string.Format(@" SELECT top 1 id,ServiceName,ServiceNameDesc,Amount,ServiceLevel1,ServiceLevel2,ServiceLevel3,Descs FROM Services s WHERE ServiceLevel1 = '2' AND  ServiceLevel2 = '1' AND ServiceLevel3 = '5' ");
                    break;
                default:
                    //服务id
                    cmdText = string.Format(@" SELECT top 1 id,ServiceName,ServiceNameDesc,Amount,ServiceLevel1,ServiceLevel2,ServiceLevel3,Descs FROM Services s WHERE id = '{0}' ", servicetype);
                    break;
            }
            //数据库处理
            DataTable table = new DataTable();
            GoogleSqlDBHelper.Fill(cmdText, table);
            if (table != null && table.Rows.Count > 0)
            {
                serviceDetail.Id = table.Rows[0]["id"] + "";
                serviceDetail.ServiceName = table.Rows[0]["ServiceName"] + "";
                serviceDetail.ServiceNameDesc = table.Rows[0]["ServiceNameDesc"] + "";
                serviceDetail.Amount = table.Rows[0]["Amount"] + "";
                serviceDetail.ServiceLevel1 = table.Rows[0]["ServiceLevel1"] + "";
                serviceDetail.ServiceLevel2 = table.Rows[0]["ServiceLevel2"] + "";
                serviceDetail.ServiceLevel3 = table.Rows[0]["ServiceLevel3"] + "";
                serviceDetail.Descs = table.Rows[0]["Descs"] + "";
                serviceDetail.AdditionalList = new List<ServiceDeatil_res>();
                serviceDetail.ServicePackage = new List<ServiceDeatil_res>();
                //获取服务相关联的包
                cmdText = string.Format(@" SELECT 
    s.id, s.ServiceName, s.ServiceNameDesc, s.Amount, 
    s.ServiceLevel1, s.ServiceLevel2, s.ServiceLevel3, s.Descs
FROM Services s
JOIN Service_Additional sal ON s.id = sal.ServiceId
WHERE sal.AdditionalId = '{0}'
order by s.ServiceLevel3 asc ", serviceDetail.Id);
                table = new DataTable();
                GoogleSqlDBHelper.Fill(cmdText, table);
                if (table != null && table.Rows.Count > 0)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        ServiceDeatil_res servicepackage = new ServiceDeatil_res();
                        servicepackage.Id = table.Rows[i]["id"] + "";
                        servicepackage.ServiceName = table.Rows[i]["ServiceName"] + "";
                        servicepackage.ServiceNameDesc = table.Rows[i]["ServiceNameDesc"] + "";
                        servicepackage.Amount = table.Rows[i]["Amount"] + "";
                        servicepackage.ServiceLevel1 = table.Rows[i]["ServiceLevel1"] + "";
                        servicepackage.ServiceLevel2 = table.Rows[i]["ServiceLevel2"] + "";
                        servicepackage.ServiceLevel3 = table.Rows[i]["ServiceLevel3"] + "";
                        servicepackage.Descs = table.Rows[i]["Descs"] + "";
                        servicepackage.AdditionalList = new List<ServiceDeatil_res>();
                        servicepackage.ServicePackage = new List<ServiceDeatil_res>();
                        serviceDetail.ServicePackage.Add(servicepackage);
                    }
                }
                //获取服务相关联附加服务
                cmdText = string.Format(@" SELECT 
    s.id, s.ServiceName, s.ServiceNameDesc, s.Amount, 
    s.ServiceLevel1, s.ServiceLevel2, s.ServiceLevel3, s.Descs
FROM Services s
JOIN Service_Additional sal ON s.id = sal.AdditionalId
WHERE sal.ServiceId = '{0}'
	 order by s.ServiceLevel3 asc ", serviceDetail.Id);
                table = new DataTable();
                GoogleSqlDBHelper.Fill(cmdText, table);
                if (table != null && table.Rows.Count > 0)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        ServiceDeatil_res serviceAdditional = new ServiceDeatil_res();
                        serviceAdditional.Id = table.Rows[i]["id"] + "";
                        serviceAdditional.ServiceName = table.Rows[i]["ServiceName"] + "";
                        serviceAdditional.ServiceNameDesc = table.Rows[i]["ServiceNameDesc"] + "";
                        serviceAdditional.Amount = table.Rows[i]["Amount"] + "";
                        serviceAdditional.ServiceLevel1 = table.Rows[i]["ServiceLevel1"] + "";
                        serviceAdditional.ServiceLevel2 = table.Rows[i]["ServiceLevel2"] + "";
                        serviceAdditional.ServiceLevel3 = table.Rows[i]["ServiceLevel3"] + "";
                        serviceAdditional.Descs = table.Rows[i]["Descs"] + "";
                        serviceAdditional.AdditionalList = new List<ServiceDeatil_res>();
                        serviceAdditional.ServicePackage = new List<ServiceDeatil_res>();
                        serviceAdditional.serviceItems = GetServiceItem(table.Rows[i]["id"] + "");
                        serviceDetail.AdditionalList.Add(serviceAdditional);
                    }
                }
                //获取服务必填变量信息
                serviceDetail.serviceItems = GetServiceItem(serviceDetail.Id);
                return new Response<ServiceDeatil_res>(serviceDetail);
            }
            return new Response<ServiceDeatil_res>("错误！获取服务信息出错");

        }


        /// <summary>
        /// 获取包信息
        /// </summary>
        /// <param name="signup_req.Actioninfo">选择购买的服务</param>
        /// <returns></returns>
        public async Task<Response<ServicePackageDeatil_res>> GetServicePackageDetail(common_req<string> signup_req)
        {
            string cmdText = "";
            string servicetype = signup_req.Actioninfo;
            ServicePackageDeatil_res serviceDetail = new ServicePackageDeatil_res();
            serviceDetail.PackageServices = new List<ServicePackageDeatil_res.PackageService>();
            cmdText = string.Format(@" SELECT top 1 id,ServiceName,ServiceNameDesc,Amount,ServiceLevel1,ServiceLevel2,ServiceLevel3,Descs FROM Services s WHERE id = '{0}' ", servicetype);

            //数据库处理
            DataTable table = new DataTable();
            GoogleSqlDBHelper.Fill(cmdText, table);
            if (table != null && table.Rows.Count > 0)
            {
                serviceDetail.Id = table.Rows[0]["id"] + "";
                serviceDetail.ServiceName = table.Rows[0]["ServiceName"] + "";
                serviceDetail.ServiceNameDesc = table.Rows[0]["ServiceNameDesc"] + "";
                serviceDetail.Discount = table.Rows[0]["Amount"] + "";
                serviceDetail.ServiceLevel1 = table.Rows[0]["ServiceLevel1"] + "";
                serviceDetail.ServiceLevel2 = table.Rows[0]["ServiceLevel2"] + "";
                serviceDetail.ServiceLevel3 = table.Rows[0]["ServiceLevel3"] + "";
                serviceDetail.Descs = table.Rows[0]["Descs"] + "";
                cmdText = string.Format(@" SELECT id,ServiceName,ServiceNameDesc,Amount,ServiceLevel1,ServiceLevel2,ServiceLevel3,Descs FROM Services WHERE  id in ( select sa.additionalid from service_additional sa  where sa.serviceid = '{0}') order by ServiceLevel1,ServiceLevel2,ServiceLevel3 ", servicetype);

                //数据库处理
                table = new DataTable();
                GoogleSqlDBHelper.Fill(cmdText, table);
                if (table != null && table.Rows.Count > 0)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        ServicePackageDeatil_res.PackageService packageService = new ServicePackageDeatil_res.PackageService();
                        packageService.ServiceId = table.Rows[i]["id"] + "";
                        packageService.ServiceName = table.Rows[i]["ServiceName"] + "";
                        packageService.ServiceNameDesc = table.Rows[i]["ServiceNameDesc"] + "";
                        packageService.Amount = table.Rows[i]["Amount"] + "";
                        packageService.ServiceLevel1 = table.Rows[i]["ServiceLevel1"] + "";
                        packageService.ServiceLevel2 = table.Rows[i]["ServiceLevel2"] + "";
                        packageService.ServiceLevel3 = table.Rows[i]["ServiceLevel3"] + "";
                        packageService.Descs = table.Rows[i]["Descs"] + "";
                        packageService.AdditionalList = new List<ServiceDeatil_res>();
                        //获取服务相关联附加服务
                        cmdText = string.Format(@" SELECT 
    s.id, s.ServiceName, s.ServiceNameDesc, s.Amount, 
    s.ServiceLevel1, s.ServiceLevel2, s.ServiceLevel3, s.Descs
FROM Services s
JOIN Service_Additional sal ON s.id = sal.AdditionalId
WHERE sal.ServiceId = '{0}'
	 order by s.ServiceLevel3 asc ", packageService.ServiceId);
                        DataTable table2 = new DataTable();
                        GoogleSqlDBHelper.Fill(cmdText, table2);
                        if (table2 != null && table2.Rows.Count > 0)
                        {
                            for (int j = 0; j < table2.Rows.Count; j++)
                            {
                                ServiceDeatil_res serviceAdditional = new ServiceDeatil_res();
                                serviceAdditional.Id = table2.Rows[j]["id"] + "";
                                serviceAdditional.ServiceName = table2.Rows[j]["ServiceName"] + "";
                                serviceAdditional.ServiceNameDesc = table2.Rows[j]["ServiceNameDesc"] + "";
                                serviceAdditional.Amount = table2.Rows[j]["Amount"] + "";
                                serviceAdditional.ServiceLevel1 = table2.Rows[j]["ServiceLevel1"] + "";
                                serviceAdditional.ServiceLevel2 = table2.Rows[j]["ServiceLevel2"] + "";
                                serviceAdditional.ServiceLevel3 = table2.Rows[j]["ServiceLevel3"] + "";
                                serviceAdditional.Descs = table2.Rows[j]["Descs"] + "";
                                serviceAdditional.AdditionalList = new List<ServiceDeatil_res>();
                                serviceAdditional.ServicePackage = new List<ServiceDeatil_res>();
                                packageService.AdditionalList.Add(serviceAdditional);
                            }
                        }
                        //获取服务必填变量信息
                        packageService.serviceItems = GetServiceItem(packageService.ServiceId);
                        serviceDetail.PackageServices.Add(packageService);
                    }
                }
                return new Response<ServicePackageDeatil_res>(serviceDetail);
            }
            return new Response<ServicePackageDeatil_res>("错误！获取服务包信息出错");

        }




        /// <summary>
        /// 店保险
        /// </summary>
        /// <param name="input"></param>
        public void SmallCommercModel(SmallCommercData small)
        {
            var predictor = new InsurancePredictor(
    "insurance_model.onnx",
    "model_features.txt",
    "feature_importance.csv"
);

            float[] input = { 941, 5, 250000, 12, 800000, 1, 0.15f, 2000, 2, 1, 1, 60, 1000000, 500000, 1000000, 2000000, 5000, 10000, 1, 3 };

            var result = predictor.Predict(input);

            //float[] features = new float[]
            //{
            //    small.zip3, small.state, small.industry_naics, small.years_in_business,
            //    small.gross_revenue_usd, small.num_employees, small.payroll_usd,
            //    small.prior_claims_5yr, small.loss_ratio_5yr, small.location_sqft,
            //    small.num_locations, small.sprinklers, small.burglar_alarm,
            //    small.hours_of_operation_per_week, small.coverage_property_limit,
            //    small.coverage_bi_limit, small.coverage_gl_each_occur,
            //    small.coverage_gl_aggregate, small.deductible_property,
            //    small.deductible_gl
            //};
            //using var session = new InferenceSession("smallcommerc_model.onnx");
            //var tensor = new DenseTensor<float>(features, new[] { 1, features.Length });
            //var inputName = session.InputMetadata.Keys.First();
            //var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(inputName, tensor) };
            //// 执行推理
            //using var results = session.Run(inputs);
            //var prediction = results.First().AsEnumerable<float>().First();
        }
        /// <summary>
        /// 劳工保险
        /// </summary>
        /// <param name="input"></param>
        public void CommercialWorkersCompModel(CommercialWorkersComp commercialWorkersComp)
        {
            float[] features = new float[]
            {

            };
            using var session = new InferenceSession("commercialworkerscomp_model.onnx");
            var tensor = new DenseTensor<float>(features, new[] { 1, features.Length });
            var inputName = session.InputMetadata.Keys.First();
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(inputName, tensor) };
            // 执行推理
            using var results = session.Run(inputs);
            var prediction = results.First().AsEnumerable<float>().First();
        }
    }
}
