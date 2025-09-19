using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Application.RequestModel.GoogleFile;
using Application.ResponseModel.GoogleFile;
using Application.Wrappers;
using Infrastructure.Shared;
using Application.RequestModel;
using Application.Interfaces;
namespace Infrastructure.Identity.Services
{
    public class GoogleFileService: IGoogleFileService
    {
        public Response<string> GenerateSignedUploadUrl(common_req<UploadGoogleFile_req> signup_req)
        {
            try
            {
                // 1. 创建 URL 签名器
                var credential = GoogleCredential.FromFile("C:\\work\\EstarOpenAPI\\EstarOpenAPI\\semiotic-art-418621-aebebd631510.json");
                var urlSigner = UrlSigner.FromCredential(credential);

                // 2. 生成唯一对象路径
                //var objectName = GenerateUniqueObjectName(request.FileName);

                // 3. 设置内容头
                var contentHeaders = new Dictionary<string, IEnumerable<string>>
                {
                    ["Content-Type"] = new[] { signup_req.Actioninfo.ContentType },
                    ["Content-Length"] = new[] { signup_req.Actioninfo.FileSize }
                };

                // 4. 创建请求模板
                var template = UrlSigner.RequestTemplate
                    .FromBucket("estar_bucket")
                    .WithObjectName(signup_req.Actioninfo.FileName)
                    .WithHttpMethod(HttpMethod.Put)
                    .WithContentHeaders(contentHeaders);

                // 5. 创建选项（5分钟有效期）
                var options = UrlSigner.Options.FromDuration(TimeSpan.FromMinutes(5));

                // 6. 生成签名 URL
                string signedUrl = urlSigner.Sign(template, options);
                return new Response<string>(signedUrl, "");
            }
            catch (Exception ex)
            {
                Pub.SaveLog(nameof(GoogleFileService), "用户："+ signup_req.User+ "生成GoogleCloud签名URL失败:" + ex.Message);
                return new Response<string>("生成签名URL失败!错误信息");
            }
        }
        private string GenerateUniqueObjectName(string fileName)
        {
            return $"uploads/{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}/{fileName}";
        }
    }
}
