**SSRS Pro**

SSRS Pro is a set of JS controls and server WebAPI enabling you to easily embed reporting into your application.  It includes a viewer to view reports and an explorer to navigate and mange reports.  With the controls you can quickly add full reporting to your application or deploy a standalone report server.

By default, the report store is a file system folder, this works well for small deployments or single machine scenarios.  It also supports a SSRS server as the store and a SQL DB. You can also implement your own store.

When not running in SSRS mode, the control support SQL, OLEDB and ODBC data sources.

Authorization is turned on by default for the WebAPI controllers.  Either use the included sample form based auth or use your own.  You may also turn off authorization by removing the Authorize attribute.

We will also be supporting a web-based report designer that you can embed in your application.  You will notice some stubs for this in the API.

Web Config Settings
The config settings are optional and allow you to easily configure the controls.

Store Type
Sets the store type without code.  Set the file path when the type is folder.
<add key="SSRSPro.StoreType" value="Folder" />
<add key="SSRSProViewer.FilePath" value="c:/Reports" />


Values
Folder, SSRS,SQLDB, Custom

Default
Folder

Custom
Specify the fully qualified class name for your custom store
    <add key="SSRSPro.StoreType" value="Custom" />
<add key="SSRSPro.CustomStore" value="MyApp.Reporting.MyStoreReportStore" />

Authorization
Sets the authorization without code.  Specify the admin windows account to set additional access when using windows auth.

<add key="SSRSPro.Auth" value="None" />
<add key="SSRSPro.AdminUser" value="TestAccount" />

Values
SSRSProWindowsAuth, DefaultNetwork, None, Custom

Default
None

SSRS URL
Sets the SSRS server URL when in SSRS store type
<add key="SSRSPro.SSRSURL" value="http://localhost/reportserver" />

Default
http://localhost/reportserver
   
    
ReportStore
Store your reports anywhere by implementing your own report store.  The API is simple, and many methods are optional. Only abstract methods are required.

Definition
public abstract class ReportStore
    {
       

        public abstract RuntimeParameterValues GetParameterValues(string itemPath, string paramName, RuntimeParameter[] values);
        public abstract string GetStoreType();
        public abstract string GetItemString(string itemPath);
        public abstract Stream GetItemStream(string itemPath);
public abstract byte[] GetItemArray(string itemPath);
        public abstract ClientItem GetItem(string itemPath);
        public abstract ClientItem[] GetItems(string parentPath,string type);


        public virtual Report GetCachedReport(string itemPath) {
            return null;
        }
        public virtual void CacheReport(string itemPath, Report rep)
        {

        }
public virtual ReportStore CloneStore(string identity)
        {
            return this;
        }
public virtual void CreateFolder(string folderPath) { }
        public virtual void SaveItemContent(string itemPath,byte[] content)
        {

        }
        public virtual void SaveItemProperties(Item item)
        {

        }
        public virtual string CreateSession(string session, string reportPath, byte[] content)
        {
            return null;
        }
        public virtual string GetItemReferences(string itemPath, string refType)
        {
            return null;
        }
public virtual void MoveItem(string itemPath, string newPath)
        {

        }
        public virtual void DeleteItem(string itemPath, string type = null)
        {

        }
        public virtual ItemRoles[] GetItemPermission(string itemPath)
        {
            return null;
        }
        public virtual void SetItemPermissions(string itemPath, ItemRoles[] permissions)
        {

        }
        public virtual StoreRole[] GetRoles()
        {
            return null;
        }
public virtual Subscription[] GetSubscriptions(SubscriptionFilter filter = null)
        {
            return null;
        }
        public virtual void SaveSubscription(Subscription sub)
        {

        }
        public virtual void SaveSubscriptionRuntimeData(Subscription sub)
        {

        }
        public virtual void DeleteSubscription(string ID)
        {

        }
}

public class SubscriptionFilter
    {
        //All, Owner, Report
        public string Type = "All";
        public string Value = null;
    }

Description
GetCachedReport
Used with CacheReport to enable caching of the report object across API calls.

CacheReport
Cache a Report object

CloneStore
Return a instance of this store, usually with a different user context


GetStoreType
Return a string of the unique store type.

GetItemStream
Return a stream for the given path.
GetItemArray
Return an array for the given path.


SaveItemContent
Save the given content

GetItemString
Return a string of the content from the given path

SaveItemProperties
Save the properties of the given item.

GetItem
Return an item at the given path,

GetItems
Return an array of Items for the given path, filter based on type.  If type is null or empty string return all types.

CreateFolder
Create a new folder

MoveItem
Change the path of the specified item

CreateSession
This method is needed when using the designer. Return a session id (path) to the given content RDL and path.  The report path is used to dereference links in the report.

GetItemReferences
Return replacement references to the ones specified in the RDL for data sets and data sources.

DeleteItem
Delete the item at the given path.

GetItemPermission
Get the user roles for an item.

SetItemPermissions
Set the user roles for an item.

GetRoles
Return the available roles in the store

Project Files
ReportViewerPage.aspx
This page is a webform for the Microsoft Report Viewer control.  It is a separate page to isolate the webform from the rest of your application.  The Microsoft control does not always play well with others.  The page contains hidden elements to communicate with the SSRS Report Viewer Pro control.  You should not need to modify the page.

ReportViewerPage.aspx.cs
This code behind page for additional customization. You should not need to modify the page.

SSRSProViewer.css
The class definitions allow you to select and control the style of all elements in SSRS Report Viewer Pro and Explorer Pro

SSRSProViewer.min.js
Includes both SSRSPro Viewer and the SSRSPro Explorer.  Create the controls with a pointer to element on your page you want to display in.  You can override all strings by providing your own localization object.  

ExplorerSample.html
Sample explorer page.

ViewerSample.html
Sample viewer only page

SSRSProControler.cs
The server API for the report store.

SSRSProLoginControler.cs
Example login controller for samples.

SSRSPro.dll
Code for the controller and sample stores

SSRSPro.Uitl
Utility class with static methods and events

Events
OnResolveDataSource
public static event EventHandler<ResolveDataSourceEventArgs> OnResolveDataSource;
Allows you to modify any data source before it is used
Set the ResolvedConnectionInfo in event agrs to change the data source at runtime.


SSRSProViewer

Methods
constructor(private container: HTMLElement, private options?: ViewerOptions, private serverOptions?: ServerOptions, private loc?: Localization) {

//Display the specified Report with optional parameters
public showReport(reportPath: string, parameters?: RunParameter[], runOptions?: RunReportOptions)

Events
validateParameter
Triggered when a parameter changes
Event details
definition
The parameter definition
value
The current value of the parameter

showViewerToolbar
Triggered when the viewer toolbar is loaded

Event details
reportPath
the path to the report being loaded.
menu
An array of menu objects that you can add, remove or modify to customize the manage menu.



SSRSProExplorer 
Methods
constructor(private container: HTMLElement, private options?: SSRSPro
Options, private loc?: Localization)

// Load a specific item, or root by default
public async loadItem(path?: string)

Events
preLoadItem
Triggered when a new item is loaded, before the explorer body is loaded with the item viewer
Event details
item  
The item definition
container
The element the item view will be loaded into.  For a report this is the element the report view will be loaded and fire events.

loadItem
Triggered after a new item is loaded, the item object is supplied.
Event details
item  
The item definition
container
The element the item view will be loaded into.  For a report this is the element the report view will be loaded and fire events.


showManage
Triggered when the mange menu is clicked, the item and menu are supplied
Event details
item  
The item definition
menu
An array of menu objects that you can add, remove or modify to customize the manage menu.

OpenDesigner
Triggered when a report is loaded into the SSRSPro Designer.
Event details

designer
The SSRSPro designer object
container
The element the designer loaded into.  This is the element the events will be fired on.




Structures
export class ViewerLoc {
        submit: string = "View Report";
        null: string = "Null";
        selectAll: string = "Select all";
        selectValue: string = "Select a value";
        true: string = "True";
        false: string = "False";
        hide: string = "Hide";
        show: string = "Show";
        login: string = "Login";
        password: string = "Password";
        userName: string = "User Name";
        logoff: string = "Logoff";
        dateFormat: string = "mm/dd/yyyy";
        dateLoc = {
            previousMonth: 'Previous Month',
            nextMonth: 'Next Month',
            months: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
            weekdays: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
            weekdaysShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']
        };
    }
    export class ExplorerLoc {
        home: string = "Home";
        reports: string = "Reports";
        folders: string = "Folders";
        dataSets: string = "Data Sets";
        dataSources: string = "Data Sources";
        resources: string = "Resources";
        search: string = "Search";

        default: string = "Default";
        subscribe: string = "Subscribe";
        edit: string = "Edit";
        delete: string = "Delete";
        rename: string = "Rename";
        security: string = "Security";
        properties: string = "Properties";

        new: string = "New";
        folder: string = "Folder";
        report: string = "Report";
        upload: string = "Upload";
        manage: string = "Manage";

        title: string = "Title";
        description: string = "Description";

        ok: string = "OK";
        cancel: string = "Cancel";

    }
    export class Localization {
        viewer: ViewerLoc = new ViewerLoc();
        explorer: ExplorerLoc = new ExplorerLoc();    
        designer?: DesignerLocalization = undefined;
}
export class Item {
        public Name: string;
        public Path: string;
        public TypeName: string;
        public Properties: ItemProperties;
        public Permission: ItemPermission;
    }
    export class ItemProperties{
        public Description?: string;
        public Title?: string;
        public IsHidden: boolean;
    }
    export class ItemPermission {
        public View: boolean = true;
        public FullControl: boolean = true;
        public Delete: boolean = true;
        public Subscribe: boolean = true;
        public Update: boolean = true;
        public Create: boolean = true;
        public Print: boolean = true;
        public Export: boolean = true;
}

    export class SSRSProOptions {
        viewer: ViewerOptions = new ViewerOptions();
        explorer: ExplorerOptions = new ExplorerOptions();
        designer?: DesignerOptions = undefined;
        server?: ServerOptions = new ServerOptions();
        designerProxy?: ReportDesignerProxy = undefined;
    }
    export class ViewerOptions {
        validateParameters?: boolean = true;
        hideParameters?: boolean = false;       
        useSSRSParamaters?: boolean = false;
        reportViewerPagePath?: string = "/SSRSPro/Viewer/ReportViewerPage.aspx";
}

    export class ServerOptions {
        reportViewerControlerPath: string = "/api/SSRSPro";
        loginControlerPath?: string = "/api/SSRSProLogin";
        onUnathorized?: (retry: () => any) => boolean = null;
    }

export class ExplorerOptions {
        showDescription?: boolean = true;
        showMove?: boolean = true;
 showSubscriptions?: boolean = true;
        showDownload?: boolean = true;
        showProperties?: boolean = true;
}
export class RunReportOptions {
        allowPrint?: boolean = true;
        allowExport?: boolean = true;
    }



 
Samples
Explorer
Setup the explorer with default options.  Keep track of the path in a URL hash and load the url path on page load.  Resize on widow resize.  

var explorerEl = $(".reportArea");
var opt = new SSRSPro.SSRSProOptions();          
var proExplorer = new SSRSPro.Explorer(explorerEl[0],opt);

let path = "/";
if (window.location.hash.length > 1) {
     path = window.location.hash.substr(1);
     path = decodeURI(path);
}

proExplorer.loadItem(path);
$(window).resize(function (e) {
      setTimeout(function (e) {
       //buffer
       explorerEl.height($(window).height() - 20);
       explorerEl.width($(window).width()-10);
        }, 1);
  }).trigger("resize");

   explorerEl.on("loadItem", function (e) {
        if (e.originalEvent.detail.item.Path)
             window.location.hash = e.originalEvent.detail.item.Path;
   });


Viewer
Create the viewer in the report area DIV and show the specified report
var reportPath = "/Employee Sales Summary 2008.rdl";

var proViewer = new SSRSPro.Viewer(document.getElementsByClassName("reportArea")[0]);
        proViewer.showReport(reportPath);

