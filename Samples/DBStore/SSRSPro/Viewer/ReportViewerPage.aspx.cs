using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System.Net;

namespace SSRSPro
{
    public class DrillThrough
    {
        public string ReportPath;
        public IList<ReportParameter> ParamValues;
    }
    public partial class ReportViewerPage : System.Web.UI.Page
    {
        public string ReportPath = "";
        static string AuthType = Util.GetAppSetting("SSRSPro.Auth", "None");
        protected global::Microsoft.Reporting.WebForms.ReportViewer ReportViewer1;


        private ReportStore GetStore()
        {
            //Return store based on config settings
            return Util.GetStore();

        }
        protected override void OnInit(EventArgs e)
        {
            

        }

        protected  void Page_Load(object sender, EventArgs e)
        {
            //Require authentication
            if (AuthType != "None" && !Request.IsAuthenticated)
            {
                Response.StatusCode = 401;
                return;
            }
            ReportStore store = GetStore();

            //Allow Report to be on URL
            ReportPath =  Request.QueryString["ReportPath"];
            if (ReportPath == null)
                ReportPath = ReportPathPost.Value;
            ReportPath = store.GetFullPath(ReportPath);

            if (ReportViewer1.ServerReport.ReportPath != null && ReportViewer1.ServerReport.IsDrillthroughReport)
                ReportPath = ReportViewer1.ServerReport.ReportPath;
            if (ReportViewer1.LocalReport.ReportPath == null && ReportViewer1.LocalReport.IsDrillthroughReport)
                ReportPath = Util.ResolveReference(ReportViewer1.LocalReport.DisplayName, ReportPath);

            ReportViewer1.Drillthrough += new DrillthroughEventHandler(DrillthroughEventHandler);
            ReportViewer1.Back += new BackEventHandler(BackEventHandler);
            ReportViewer1.ReportRefresh += new System.ComponentModel.CancelEventHandler(RefreshEventHandler);

           


            if (ReportPath != "" && store.GetStoreType() == "SSRS" && UseSSRSParams() && OldParameterValuesPost.Value != "SSRSParams")
            {
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
                ReportViewer1.ServerReport.ReportServerUrl = new Uri(((RSServerStore)store).GetURL());
                ReportViewer1.ServerReport.ReportPath = ReportPath;
                ReportViewer1.ServerReport.ReportServerCredentials = new SSRSProReportServerCredentials();
                ReportViewer1.Visible = true;
                ReportViewer1.ServerReport.Refresh();
                OldParameterValuesPost.Value = "SSRSParams";
                HideError();
            }
            else if (ReportPath != "" && ParameterValuesPost.Value != OldParameterValuesPost.Value && !UseSSRSParams())
            {
                try
                {
                    
                    OldParameterValuesPost.Value = ParameterValuesPost.Value;
                    Report rep = new Report(ReportPath, store);
                    //Get post parameters
                    List<ReportParameter> runParam = Util.GetRuntimeParameters(ParameterValuesPost.Value, rep);
                    Util.RaisePreviewParameters(runParam, store, ReportPath, rep);

                    if (Util.HasAllParameters(rep, runParam))
                    {
                        if (store.GetStoreType() == "SSRS")
                        {
                            ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
                            ReportViewer1.ShowParameterPrompts = false;
                            ReportViewer1.ServerReport.ReportServerCredentials = new SSRSProReportServerCredentials();
                            if (!ReportViewer1.ServerReport.IsDrillthroughReport)
                            {
                                ReportViewer1.ServerReport.ReportServerUrl = new Uri(((RSServerStore)store).GetURL());
                                ReportViewer1.ServerReport.ReportPath = ReportPath;
                            }
                            ReportViewer1.ServerReport.SetParameters(runParam);
                            ReportViewer1.Visible = true;
                            DrillThroughParams.Value = "";
                            ReportViewer1.ServerReport.Refresh();
                            HideError();
                        }
                        else
                        {

                            ReportViewer1.LocalReport.EnableExternalImages = true;
                            ReportViewer1.LocalReport.EnableHyperlinks = true;
                            //Try to run fast if configured in web.config <trust legacyCasModel="true" />
                            try
                            {
                                ReportViewer1.EnableTelemetry = false;
                                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                                ReportViewer1.LocalReport.ExecuteReportInCurrentAppDomain(System.Reflection.Assembly.GetExecutingAssembly().Evidence);
                                System.Security.PermissionSet permissions = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
                            }
                            catch { }

                            
                            ReportViewer1.LocalReport.LoadReportDefinition(rep.Stream);
                            rep.AddSubreports(ReportViewer1.LocalReport, ReportPath);
                            ReportViewer1.LocalReport.DisplayName = Path.GetFileName(ReportPath);
                            rep.SetDataSources(ReportViewer1.LocalReport.DataSources, runParam);
                            ReportViewer1.LocalReport.SetParameters(runParam);
                            ReportViewer1.Visible = true;
                            DrillThroughParams.Value = "";

                            ReportViewer1.LocalReport.Refresh();
                           
                            HideError();

                        }
                    }
                    else
                    {
                        ReportViewer1.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    string err = ex.Message;

                    if (ex.InnerException != null)
                        err = ex.InnerException.Message;

                    SetErrorText(err);
                }

            }
        }

        private bool UseSSRSParams()
        {

            if (OldParameterValuesPost.Value == "UseSSRSParam" || OldParameterValuesPost.Value == "SSRSParams")
                return true;
            return false;

        }
        private void  SetErrorText(string errorStr)
        {
            string script = "var err = document.getElementById('ErrorText'); err.style.display = 'block' ; err.innerHTML = \"" + errorStr.Replace("\"", "'").Replace("\\","/") + "\";";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "open", script, true);
        }
        private void HideError()
        {
            string script = "var err = document.getElementById('ErrorText'); err.style.display = 'none' ; ";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "open", script, true);
        }

         private void RefreshEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (((ReportViewer)sender).ServerReport.ReportPath != "")
                return;

            LocalReport localReport = ((ReportViewer)sender).LocalReport;
            string thisPath = Util.ResolveReference(localReport.DisplayName, ReportPath);
            ReportStore store = GetStore();

            Report rep = new Report(thisPath, store);           
            rep.AddSubreports(localReport, rep.ReportPath);
            rep.SetDataSources(ReportViewer1.LocalReport.DataSources, ReportViewer1.LocalReport.GetParameters());
            localReport.Refresh();
            
        }
       
        private void BackEventHandler(object sender, BackEventArgs e)
        {
            if (UseSSRSParams())
                return;
            ReportStore store = GetStore();
            
            try
            {
                //Keep track of depth
                string script = @"document.getElementById('DrillThroughParams').value = 'parent';";
                if (e.ParentReport.IsDrillthroughReport)
                {
                    Report rep = new Report(Util.ResolveReference(e.ParentReport.DisplayName, ReportPath), store);
                    DrillThrough dt = new DrillThrough();
                    dt.ReportPath = Util.ResolveReference(e.ParentReport.DisplayName, ReportPath);
                    if (e.ParentReport.GetType() == typeof(ServerReport))
                    {
                        dt.ParamValues = Util.GetRuntimeParameters( e.ParentReport.GetParameters());
                    }
                    else 
                        dt.ParamValues = ((LocalReport)e.ParentReport).OriginalParametersToDrillthrough;

                    //Store params for client
                    script = @"document.getElementById('DrillThroughParams').value = '" + JsonConvert.SerializeObject(dt) + "';";
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "open", script, true);
            }
            catch (Exception ex)
            {
                string err = ex.Message;

                if (ex.InnerException != null)
                    err = ex.InnerException.Message;

                SetErrorText(err);
            }

        }

        private void DrillthroughEventHandler(object sender, DrillthroughEventArgs e)
        {
            
            if (UseSSRSParams())
                return;

            ReportStore store = GetStore();
            try
            {
                e.Report.DisplayName = e.ReportPath;
                DrillThrough dt = new DrillThrough();
                dt.ReportPath = Util.ResolveReference(e.ReportPath, ReportPath);

                if (e.Report.GetType() != typeof(ServerReport))
                {
                    LocalReport localReport =(LocalReport) e.Report;
                    dt.ParamValues = localReport.OriginalParametersToDrillthrough;

                    Report rep = new Report(Util.ResolveReference(e.ReportPath, ReportPath), store);
                    e.Report.LoadReportDefinition(rep.Stream);
                    rep.AddSubreports(localReport, rep.ReportPath);

                    //Moify Data Source and Data Sets if needed
                    rep.SetDataSources(localReport.DataSources, new List<ReportParameter>(localReport.OriginalParametersToDrillthrough));
                    localReport.SetParameters(localReport.OriginalParametersToDrillthrough);

                    localReport.EnableExternalImages = true;
                    localReport.EnableHyperlinks = true;

                    localReport.Refresh();
                }
                else
                {
                    dt.ParamValues = Util.GetRuntimeParameters(e.Report.GetParameters());
                }
                //Store params for client
                string script = @"document.getElementById('DrillThroughParams').value = '" + JsonConvert.SerializeObject(dt) + "';";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "open", script, true);
            }
            catch (Exception ex)
            {
                string err = ex.Message;

                if (ex.InnerException != null)
                    err = ex.InnerException.Message;

                SetErrorText(err);
            }

        }
    }
}