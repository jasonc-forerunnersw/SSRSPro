The Forerunner SSRS Pro controls are an extension to the Microsoft Report Viewer control.  
The control removes most limitations of the Microsoft Report Viewer control allowing you to run almost any RDL with full parameter and data access support and no coding.
The report Explorer control allows you to browse and manage your report library.  
By default, the Viewer control has a 20 row limit on valid value parameters and 1000 rows on report data.  To remove, simply register the control 
by sending mail to Sales@forerunnersw.com  After registration, you will receive a license key in email to add 
to your web.config.  This will unlock the control for 30 days.  If you wish to receive priority support 
and unlock it permanently, please purchase a subscription starting at just $299 per year.  Please contact sales@forerunnersw.com 
if you are an ISV and need to redistribute.

 
Full documentation can be found at https://forerunnersw.com/home/ReportViewerPro 

After installation, your project will contain:
	o   SSRSPro/Viewer/SSRSProControler.cs  -  The source code to the controller 
	o   SSRSPro/Viewer/SSRSProViewer.min.js – The control js code
	o   SSRSPro/Viewer/ViewerSample.html – Sample page
	o   SSRSPro/Viewer/ExplorerSample.html – Sample page
	o   SSRSPro/Viewer/SSRSProViewer.css – The css not minified
	o   SSRSPro/Viewer/ReportViewerPage.aspx - The aspx page for the Microsoft Report Viewer control.  
	o   SSRSPro/Viewer/ReportViewerPage.aspx.cs - Code behind to manage the Microsoft Report Viewer Control
	o   SSRSPro/Viewer/ReportViewerPage.aspx.designer.cs


Start with an existing WebAPI and WebForms project or create an empty one with WebAPI and WebForms. If your existing project does not include both please refer to the links below.
https://forums.asp.net/t/2090133.aspx?How+do+I+add+Web+Forms+support+to+an+MVC+project+

The default report store is the file system.  By default the report store location is c:\reports, you can change this either in your web.confg or in the SSRSProController.  
You will need to create this folder and place your RDL files there.  You can also write your own ReportStore and store your RDLs anywhere.

To use the control, you will need to edit a couple of files.

1.  ViewerSample.html / ExplorerSample.html
	o   Depending on which version of jQuery you are using, edit the sample.html to the version you have installed.  There are two references, a script in the head and a path in requirejs.
 	o   Change the reportPath variable to point to a rdl file

2. By defult the SSRSProController requires authentication.  If you do not want to require authentication you can comment out the Authorize attribute. The default authentication
provider is "None", which means that your application is responibile for athentication.  SSRSPro comes with a sample Login controler and authentication support for Windows, Default Network,
None and File Store.  You can also write a custom auth provider.  To use any of the built in authentication providers you need to configure authetication in your web.config.
For example:	
	<authentication mode="Forms">
		<forms name=".SSRSPro" slidingExpiration="true"/>
	</authentication>

3.  If your project does not have a reference to system.web.extensions you will need to add it.  This will remove the script manager error.

4.  Register you control by sending email to sales@ForerunnerSW.com, you will receive a license key as a reply.
	o   An unregistered control will only return 1000 data rows to a report and 20 rows for a parameter query
	o   Place the license key in your web.config
	o   You can also change the default folder for your report store
		<add key="SSRSProViewer.FilePath" value="c:/Reports" />

5.  If using a spatial datatype you may need to add the following to your web.config
    <dependentAssembly>
      <assemblyIdentity name="Microsoft.SqlServer.Types" publicKeyToken="89845dcd8080cc91" culture="neutral" />
      <bindingRedirect oldVersion="0.0.0.0-14.0.0.0" newVersion="14.0.0.0" />
    </dependentAssembly>



 
 
