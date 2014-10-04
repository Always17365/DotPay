<%@ WebHandler Language="C#" Class="UEditorHandler" %>

using System;
using System.Web;
using System.IO;
using System.Collections;
using Newtonsoft.Json;

public class UEditorHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        Handler action = null;
        switch (context.Request["action"])
        {
            case "config":
                action = new ConfigHandler(context);
                break;
            case "uploadimage":
                action = new UploadHandler(context, new UploadConfig()
                {
                    AllowExtensions = UEditorConfig.GetStringList("imageAllowFiles"),
                    PathFormat = UEditorConfig.GetString("imagePathFormat"),
                    SizeLimit = UEditorConfig.GetInt("imageMaxSize"),
                    UploadFieldName = UEditorConfig.GetString("imageFieldName")
                });
                break;
            case "uploadscrawl":
                action = new UploadHandler(context, new UploadConfig()
                {
                    AllowExtensions = new string[] { ".png" },
                    PathFormat = UEditorConfig.GetString("scrawlPathFormat"),
                    SizeLimit = UEditorConfig.GetInt("scrawlMaxSize"),
                    UploadFieldName = UEditorConfig.GetString("scrawlFieldName"),
                    Base64 = true,
                    Base64Filename = "scrawl.png"
                });
                break;
            case "uploadvideo":
                action = new UploadHandler(context, new UploadConfig()
                {
                    AllowExtensions = UEditorConfig.GetStringList("videoAllowFiles"),
                    PathFormat = UEditorConfig.GetString("videoPathFormat"),
                    SizeLimit = UEditorConfig.GetInt("videoMaxSize"),
                    UploadFieldName = UEditorConfig.GetString("videoFieldName")
                });
                break;
            case "uploadfile":
                action = new UploadHandler(context, new UploadConfig()
                {
                    AllowExtensions = UEditorConfig.GetStringList("fileAllowFiles"),
                    PathFormat = UEditorConfig.GetString("filePathFormat"),
                    SizeLimit = UEditorConfig.GetInt("fileMaxSize"),
                    UploadFieldName = UEditorConfig.GetString("fileFieldName")
                });
                break;
            case "listimage":
                action = new ListFileManager(context, UEditorConfig.GetString("imageManagerListPath"), UEditorConfig.GetStringList("imageManagerAllowFiles"));
                break;
            case "listfile":
                action = new ListFileManager(context, UEditorConfig.GetString("fileManagerListPath"), UEditorConfig.GetStringList("fileManagerAllowFiles"));
                break;
            case "catchimage":
                action = new CrawlerHandler(context);
                break;
            default:
                action = new NotSupportedHandler(context);
                break;
        }
        action.Process();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}