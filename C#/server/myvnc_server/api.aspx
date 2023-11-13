<%@ Page Language="C#" AutoEventWireup="true" CodeFile="api.aspx.cs" Inherits="_Default" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="utility" %>
<% 
    myinclude my = new myinclude();
    string GETS_STRING = "mode";
    var GETS = my.getGET_POST(GETS_STRING, "GET");
    switch (GETS["mode"].ToString())
    {
        case "myvnc_getNewestPIC":
            {
                GETS_STRING = "computer_id";
                GETS = my.getGET_POST(GETS_STRING, "GET");
                GETS["computer_id"] = Convert.ToInt32(GETS["computer_id"].ToString());
                var dn = my.pwd() + "\\cache\\my_vnc\\" + GETS["computer_id"].ToString();
                if (!my.is_dir(dn))
                {
                    my.mkdir(dn);
                }
                var fp = my.glob(dn, "*.png");
                if (fp.Length >= 2)
                {
                    Response.Clear();
                    Response.AppendHeader("Content-Length", my.filesize(fp[1]).ToString());
                    Response.AppendHeader("Content-Type", "image/png");
                    Response.TransmitFile(fp[1]);
                    //Response.WriteFile(fp[1]);
                }
                my.exit();
            }
            break;
        case "myvnc_setPIC":
            {
                var POSTS_STRING = "computer_id";
                var POSTS = my.getGET_POST(POSTS_STRING, "POST_UNVALID");
                POSTS["computer_id"] = Convert.ToInt32(POSTS["computer_id"].ToString());
                var op = my.pwd() + "\\cache\\my_vnc\\" + POSTS["computer_id"].ToString() + "\\" + my.microtime() + ".png";
                var dn = my.dirname(op);
                if (!my.is_dir(dn))
                {
                    my.mkdir(dn);
                }
                HttpPostedFile f = Request.Files["pic"];
                f.SaveAs(op);
                var fp = my.glob(dn, "*.png");
                var orderedFiles = fp.Select(file => new FileInfo(file))
                          .OrderByDescending(fileInfo => fileInfo.CreationTime)
                          .ToArray();
                int numberOfFilesToKeep = 5;
                var filesToKeep = orderedFiles.Take(numberOfFilesToKeep).ToArray();
                foreach (var file in orderedFiles.Skip(numberOfFilesToKeep))
                {
                    File.Delete(file.FullName);
                }
                my.exit();
            }
            break;
    }
%>