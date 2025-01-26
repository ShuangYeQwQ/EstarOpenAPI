using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseModel.AccountPage
{
    public class UserService_res
    {
        //用户个人所得税未处理服务数
        public int PersonalTaxDeclarationCount { get; set; }
        //用户公司所得税未处理服务数
        public int EnterpriseTaxDeclarationCount { get; set; }
    }
}
