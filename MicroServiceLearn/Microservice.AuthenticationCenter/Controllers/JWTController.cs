using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microservice.Framework;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Zhaoxi.NET6.AuthenticationCenter.Utility;
using Zhaoxi.NET6.AuthenticationCenter.Utility.RSA;

namespace Zhaoxi.NET6.AuthenticationCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JWTController : ControllerBase
    {
        #region MyRegion
        private ILogger<JWTController> _logger = null;
        private IJWTService _iJWTService = null;
        private readonly IConfiguration _iConfiguration;
        public JWTController(ILoggerFactory factory,
            ILogger<JWTController> logger,
            IConfiguration configuration
            , IJWTService service)
        {
            this._logger = logger;
            this._iConfiguration = configuration;
            this._iJWTService = service;
        }
        #endregion

        [Route("Get")]
        [HttpGet]
        public IEnumerable<int> Get()
        {
            return new List<int>() { 1, 2, 3, 4, 6, 7 };
        }

        [Route("GetKey")]
        [HttpGet]
        public string GetKey()
        {
            string keyDir = Directory.GetCurrentDirectory();
            if (RSAHelper.TryGetKeyParameters(keyDir, false, out RSAParameters keyParams) == false)
            {
                keyParams = RSAHelper.GenerateAndSaveKey(keyDir, false);
            }

            return JsonConvert.SerializeObject(keyParams);
        }

        /// <summary>
        /// 数据库校验
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Route("Login")]
        [HttpPost]
        public string Login([FromForm] string name, [FromForm] string password)
        {
            Console.WriteLine($"This is Login name={name} password={password}");
            if ("Eleven".Equals(name, StringComparison.OrdinalIgnoreCase) && "123456".Equals(password))//应该数据库
            {
                CurrentUserModel currentUser = new CurrentUserModel()
                {
                    Id = 123,
                    Account = "xuyang@zhaoxiEdu.Net",
                    EMail = "57265177@qq.com",
                    Mobile = "18664876671",
                    Sex = 1,
                    Age = 33,
                    Name = "Eleven",
                    Role = "Admin"
                };

                string token = this._iJWTService.GetToken(currentUser);
                if (!string.IsNullOrEmpty(token))
                {
                    return JsonConvert.SerializeObject(new AjaxResult<string>()
                    {
                        Result = true,
                        Message = "Token颁发成功",
                        TValue = token
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(new AjaxResult<string>()
                    {
                        Result = false,
                        Message = "Token获取失败",
                        TValue = ""
                    });
                }
            }
            else
            {
                return JsonConvert.SerializeObject(new AjaxResult<string>()
                {
                    Result = false,
                    Message = "验证失败",
                    TValue = ""
                });
            }
        }


        /// <summary>
        /// 生成Token+RefreshToken
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Route("LoginWithRefresh")]
        [HttpPost]
        public string LoginWithRefresh([FromForm] string name, [FromForm] string password)
        {
            Console.WriteLine($"This is LoginWithRefresh name={name} password={password}");
            if ("Eleven".Equals(name, StringComparison.OrdinalIgnoreCase) && "123456".Equals(password))//应该数据库
            {
                CurrentUserModel currentUser = new CurrentUserModel()
                {
                    Id = 123,
                    Account = "xuyang@zhaoxiEdu.Net",
                    EMail = "57265177@qq.com",
                    Mobile = "18664876671",
                    Sex = 1,
                    Age = 33,
                    Name = "Eleven",
                    Role = "Admin"
                };

                var tokenPair = this._iJWTService.GetTokenWithRefresh(currentUser);
                if (tokenPair != null && !string.IsNullOrEmpty(tokenPair.Item1))
                {
                    return JsonConvert.SerializeObject(new AjaxResult<string>()
                    {
                        Result = true,
                        Message = "验证成功",
                        TValue = tokenPair.Item1,
                        OtherValue = tokenPair.Item2//写在OtherValue
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(new AjaxResult<string>()
                    {
                        Result = false,
                        Message = "颁发token失败",
                        TValue = ""
                    });
                }
            }
            else
            {
                return JsonConvert.SerializeObject(new AjaxResult<string>()
                {
                    Result = false,
                    Message = "验证失败",
                    TValue = ""
                });
            }
        }

        [Route("RefreshToken")]
        [HttpPost]
        public async Task<string> RefreshToken([FromForm] string refreshToken)
        {
            await Task.CompletedTask;
            if (!refreshToken.ValidateRefreshToken())
            {
                return JsonConvert.SerializeObject(new AjaxResult<string>()
                {
                    Result = false,
                    Message = "refreshToken过期了",
                    TValue = ""
                });
            }
            var token = this._iJWTService.GetTokenByRefresh(refreshToken);
            if (!string.IsNullOrEmpty(token))
            {
                return JsonConvert.SerializeObject(new AjaxResult<string>()
                {
                    Result = true,
                    Message = "刷新Token成功",
                    TValue = token,
                    OtherValue = refreshToken//写在OtherValue
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new AjaxResult<string>()
                {
                    Result = false,
                    Message = "刷新token失败",
                    TValue = ""
                });
            }
        }
    }
}