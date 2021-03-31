using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace SSRSPro.Controllers
{
    [System.Web.Http.Authorize]
    [RoutePrefix("api/SSRSProLogin")]
    public class SSRSProLoginController : ApiController
    {
        SSRSProAuth Auth = null;
        SSRSProLoginController()
        {
            string AuthType = Util.GetAppSetting("SSRSPro.Auth", "None");
            string RootPath = Util.GetAppSetting("SSRSPro.FilePath", "c:/Reports");

            if (AuthType == "SSRSProWindowsAuth")
                Auth = new SSRSProWindowsAuth();
            else if (AuthType == "SSRSProFileAuth")
                Auth = new SSRSProFileAuth(RootPath);


        }

        public class LoginPost
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
        [HttpPost]
        [System.Web.Http.AllowAnonymous]
        [Route("Login")]
        public HttpResponseMessage Login(LoginPost postVal)
        {
            HttpResponseMessage Resp = Request.CreateResponse();
            try
            {
                SSRSProAuthData data = null;
                if (Auth!=null)
                    data = Auth.Login(postVal.UserName, postVal.Password);                
                if (data != null || Auth == null)
                {
                   
                    Util.SetCookie(this.Request, Resp, data);
                    Resp.Content = new StreamContent((new MemoryStream(Encoding.UTF8.GetBytes("{\"status\":\"success\"}")))) ;
                    return Resp;
                }
                else
                {                   
                    Resp.Content = new StreamContent((new MemoryStream(Encoding.UTF8.GetBytes("{\"status\":\"failed\"}"))));
                    return Resp;
                }
                 
            }
            catch (Exception e)
            {
                Resp.Content = new StreamContent((new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e)))));
                return Resp;
            }
        }

        public class ChargePasswordPost
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }
        [HttpPost]
        [Route("ChangePassword")]
        public string ChangePassword(ChargePasswordPost postVal)
        {
            try
            {
                Auth.ChangePassword(Util.GetAuthData().Name, postVal.NewPassword, postVal.OldPassword);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
        [HttpGet]
        [Route("ResetPassword")]
        public string ResetPassword(string userName)
        {
            try
            {
                Auth.ResetPassword(userName);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }

        [HttpGet]
        [Route("Logoff")]
        public HttpResponseMessage Logoff()
        {

            HttpResponseMessage Resp = Request.CreateResponse();
            try
            {
                Util.SetCookie(this.Request, Resp, Util.GetAuthData(),true);
                Resp.StatusCode = HttpStatusCode.OK;
                Resp.Content = new StreamContent((new MemoryStream(Encoding.UTF8.GetBytes("{\"status\":\"success\"}"))));
                return Resp;
            }
            catch (Exception e)
            {
                Resp.Content = new StreamContent((new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e)))));
                return Resp;
            }
        }
        [HttpGet]
        [Route("CreateUser")]
        public string CreateUser(string userName, string email)
        {
            try
            {
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
        [HttpGet]
        [Route("DeleteUser")]
        public string DeleteUser(string userName)
        {
            try
            {
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
    }
}
