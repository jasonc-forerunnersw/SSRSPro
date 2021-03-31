using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.Net.Http.Headers;

namespace SSRSPro.Controllers
{
    
    [Authorize]
    [RoutePrefix("api/SSRSPro")]
    public class SSRSProController : ApiController
    {
        ReportStore Store;
        static bool Init= false;
        static object Lock = new object();
        static SubscriptionEngine Sub;//
        SSRSProController()
        {

            //Get store based on config settings
            //Replace as necessery 
            Store = Util.GetStore();
            lock (Lock)
            {
                if (!Init)
                {
                    Init = true;

                    //This should be moved to a seperate service so that it is always running
                    //Included as sample.
                    Sub = new SubscriptionEngine(Store); 
                    
                    Util.OnResolveDataSource += OnResolveDataSourceHandler;
                    Util.OnPreviewParameters += OnPreviewParametersHandler;
                }
            }            
            
        }

        //Modify Datasource information if needed.  This is an example, shoul dbe moved to application code.
        private static void OnResolveDataSourceHandler(object sender, ResolveDataSourceEventArgs args)
        {
           

        }
        //Modify parameters if needed. This is an example, shoul dbe moved to application code.
        private static void OnPreviewParametersHandler(object sender, PreviewParametersEventArgs args)
        {


        }

        private string GetPostBodyString()
        {
            return System.Text.Encoding.UTF8.GetString(GetPostBody());
        }
        private byte[] GetPostBody()
        {
            Stream result = new MemoryStream();

            System.Threading.Tasks.Task.Run(async () =>
            {
                result = await Request.Content.ReadAsStreamAsync();
            }).Wait();
            result.Position = 0;
           
            //remove BOM if present
            byte[] bom = new byte[3];
            if (result.Length > 3)
            {
                result.Read(bom, 0, 3);
                if (!bom.SequenceEqual(new byte[] { 0xef, 0xbb, 0xbf }))
                    result.Position = 0;
            }
            MemoryStream mStream = new MemoryStream();
            result.CopyTo(mStream);
            return mStream.ToArray();
        }

        
        [HttpGet]
        [Route("GetParameters")]
        public string GetParameters(string reportPath)
        {
            try
            {                
                Report rep = new Report(reportPath, Store);
                return JsonConvert.SerializeObject(rep.GetParameterDef());
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
        [HttpPost]        
        [Route("GetParameterValue")]
        public string GetParameterValue(string reportPath,string paramName)
        {
            try
            {
                string paramVals = GetPostBodyString();
                RuntimeParameter[] pars = JsonConvert.DeserializeObject<RuntimeParameter[]>(paramVals);
                return JsonConvert.SerializeObject(Store.GetParameterValues(reportPath, paramName, pars));
   
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }

        [HttpPost]
        [Route("CreateSession")]
        public string CreateSession (string reportPath,string editSession = null)
        {
            try
            {
                byte[] raw = GetPostBody();
                string Session = Store.CreateSession(editSession, reportPath,raw);
                return "{\"EditSession\":\"" + Session + "\"}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }

        }
        [HttpPost]
        [Route("SetItemPermissions")]
        public string SetItemPermissions(string reportPath)
        {
          
            try
            {
               string p = GetPostBodyString();
                ItemRoles[] roles = JsonConvert.DeserializeObject<ItemRoles[]>(p);              
            
                Store.SetItemPermissions(reportPath, roles);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }

        }
        [HttpGet]
        [Route("GetRoles")]
        public string GetRoles()
        {   
            try 
            {
                return JsonConvert.SerializeObject(Store.GetRoles());
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }

        }
        [HttpGet]
        [Route("GetItemContent")]
        public string GetItemContent(string path)
        {
            try
            {
                return Store.GetItemString(path);
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }

        }
        [HttpGet]
        [Route("Download")]
        public HttpResponseMessage Download(string path,bool toFile = false)
        {

            HttpResponseMessage resp = new HttpResponseMessage();

            Item item = Store.GetItem(path);
            Stream content = Store.GetItemStream(path);
            content.Position = 0;
            resp.Content = new StreamContent(content);
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue(item.MimeType);

            if (toFile)
                resp.Content.Headers.Add("Content-Disposition", "attachment;filename=\"" + HttpUtility.UrlEncode(Util.GetNameWithExtension(item.Name,item.MimeType)) + "\"");
            return resp;

        }

        [HttpGet]      
        [Route("GetItem")]
        public string GetItem(string path)
        {
            try
            {
                return JsonConvert.SerializeObject(Store.GetItem(path));
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }

        }
        [HttpGet]
        [Route("GetItemReferences")]
        public string GetItemReferences(string reportPath,string refType)
        {
            try
            {
                return Store.GetItemReferences(reportPath, refType);
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
        [HttpPost]
        [Route("SaveItemContent")]
        public string SaveItemContent(string path,bool overwrite = false)
        {
            try
            {
                Store.SaveItemContent(path, GetPostBody(),overwrite);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }

        }
        [HttpPost]
        [Route("ValidateQuery")]
        public string ValidateQuery(string reportPath = "/")
        {
            try
            {
                return Util.ValidateQuery( GetPostBodyString(),Store, reportPath);
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }

        }
        [HttpPost]
        [Route("ValidateDataSource")]
        public string ValidateDataSource()
        {
            try
            {
                Util.ValidateDS(GetPostBodyString());
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }

        }

        [HttpPost]
        [Route("SaveItemProperties")]
        public string SaveItemProperties()
        {
            try
            {
                string post = GetPostBodyString();
                Item item = JsonConvert.DeserializeObject<Item>(post);
                Store.SaveItemProperties(item);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }

        }
        [HttpGet]
        [Route("GetItems")]
        public string GetItems(string path,string type =null)
        {
            try
            {
                return JsonConvert.SerializeObject(Store.GetItems(path, type));                             
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }

        [HttpGet]
        [Route("GetItemPermission")]
        public string GetItemPermission(string path)
        {
            try
            { 
                return JsonConvert.SerializeObject(Store.GetItemPermission(path));
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
        [HttpGet]
        [Route("MoveItem")]
        public string MoveItem(string path, string newPath)
        {
            try
            {
                Store.MoveItem(path, newPath);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
        [HttpGet]
        [Route("CreateFolder")]
        public string CreateFolder(string path)
        {
            try
            {
                Store.CreateFolder(path);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
        [HttpGet]
        [Route("DeleteItem")]
        public string DeleteItem(string path)
        {
            try
            {
                Store.DeleteItem(path);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
        [HttpGet]
        [Route("ListRenderFormats")]
        public string ListRenderFormats()
        {
            try
            {
                return JsonConvert.SerializeObject(Util.ListRenderformats(Store));
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
        
        
        [HttpPost]
        [Route("SetItemPermission")]
        public string SetItemPermission(string path)
        {
            try
            {
                Store.SetItemPermissions(path, JsonConvert.DeserializeObject<ItemRoles[]>(GetPostBodyString()));
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
        [HttpPost]
        [Route("SaveSubscription")]
        public string SaveSubscription()
        {
            try
            {
                Store.SaveSubscription( JsonConvert.DeserializeObject<Subscription>(GetPostBodyString()));
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
        [HttpGet]
        [Route("DeleteSubscription")]
        public string DeleteSubscription(string ID)
        {
            try
            {
                Store.DeleteSubscription(ID);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }  



        [HttpPost]
        [Route("ListSubscriptions")]
        public string ListSubscriptions()
        {
            try
            {
                return JsonConvert.SerializeObject(Store.GetSubscriptions(JsonConvert.DeserializeObject<SubscriptionFilter>(GetPostBodyString())));
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
        [HttpPost]
        [Route("GetVisualization")]
        public string GetVisualization(string Type)
        {
            try
            {                
                return JsonConvert.SerializeObject(Util.GetVisualization(Type, GetPostBodyString()));
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }
    }
}
