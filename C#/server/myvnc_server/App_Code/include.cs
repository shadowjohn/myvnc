using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

//using System.Reflection;
/*
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="webservice.aspx.cs" Inherits="WaterRegion.Search.webservice.webservice"  %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="utility" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ import Namespace="Newtonsoft.Json.Linq" %>
<%@ Import Namespace="System.Linq" %>
*/

namespace utility
{
    public class myinclude : System.Web.Services.WebService
    {
        private Random rnd = new Random(DateTime.Now.Millisecond);
        public myinclude()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        }
        public void echo(double value)
        {
            echo(value.ToString());
        }
        public void echo(long value)
        {
            echo(value.ToString());
        }
        public void echo(int value)
        {
            echo(value.ToString());
        }
        public void echo(byte[] value)
        {
            echo(value.ToString());
        }
        public void echo(string value)
        {
            //System.Web.HttpContext.Current.Response.Write(value);
            HttpContext.Current.Response.Write(value);
        }
        public void echoBinary(string value)
        {
            HttpContext.Current.Response.BinaryWrite(s2b(value));
        }
        public void echoBinary(byte[] value)
        {
            HttpContext.Current.Response.BinaryWrite(value);
        }
        public string b2s(byte[] input)
        {
            return System.Text.Encoding.UTF8.GetString(input);
        }
        public byte[] s2b(string input)
        {
            return System.Text.Encoding.UTF8.GetBytes(input);
        }
        public string microtime()
        {
            System.DateTime dt = DateTime.Now;
            System.DateTime UnixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan span = dt - UnixEpoch;
            long microseconds = span.Ticks / (TimeSpan.TicksPerMillisecond / 1000);
            return microseconds.ToString();
        }
        public DateTime UnixTimeToDateTime(string text)
        {
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            // Add the number of seconds in UNIX timestamp to be converted.            
            dateTime = dateTime.AddSeconds(Convert.ToDouble(text));
            return dateTime;
        }
        //仿php的date
        public string time()
        {
            return strtotime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        //strtotime 轉換成 Unix time
        public string strtotime(string value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (Convert.ToDateTime(value) - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            if (is_string_like(value, "."))
            {
                //有小數點               
                double sec = span.Ticks / (TimeSpan.TicksPerMillisecond / 1000.0) / 1000000.0;
                return sec.ToString();
            }
            else
            {
                return span.TotalSeconds.ToString();
            }
        }
        public string strtotime(DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            return span.TotalSeconds.ToString();
        }
        public bool is_string_like(string data, string find_string)
        {
            return (data.IndexOf(find_string) == -1) ? false : true;
        }
        public bool is_istring_like(string data, string find_string)
        {
            return (data.ToUpper().IndexOf(find_string.ToUpper()) == -1) ? false : true;
        }
        public string[] glob(string path, string patten)
        {
            //string[] test = my.glob("c:\\tmp");
            //my.echo(my.pre_print_r(test));
            return Directory.GetFiles(path, patten);
        }
        public void mkdir(string path)
        {
            Directory.CreateDirectory(path);
        }
        public bool is_dir(string path)
        {
            return Directory.Exists(path);
        }
        public bool is_file(string filepath)
        {
            return File.Exists(filepath);
        }
        public Dictionary<string, object> getGET_POST(string inputs, string method)
        {
            /*
                string GETS_STRING="mode,id";
                Dictionary<string, object> get = x.getGET_POST(GETS_STRING, "GET");
                string vv=x.print_r(get,0);
                foreach (string a in get.Keys)
                {
                    Response.Write(a+":"+get[a]+"<br>");
                }
               sample:
                string GETS_STRING="mode,id";
                Dictionary<string, object> get = x.getGET_POST(GETS_STRING, "GET");
                
                string POSTS_STRING ="abc,b,s_a,s_b[],ddd";

                Dictionary<string, object> post = x.getGET_POST(POSTS_STRING, "POST");
                string q = x.print_r(get, 0);
                string p = x.print_r(post, 0);
                Response.Write("<pre>" + q + "<br>" + p + "</pre>");
                Response.Write("aaaaaaa->" + post["s_a"]+"<br>");
                Response.Write("aaaaaab->" + post["s_b[]"] + "<br>");             
             * 
            */
            method = method.ToUpper();
            Dictionary<string, object> get_post = new Dictionary<string, object>();
            switch (method)
            {
                case "GET":
                    foreach (string k in inputs.Split(','))
                    {
                        if (this.Context.Request.QueryString[k] != null)
                        {
                            get_post[k] = this.Context.Request.QueryString[k];
                        }
                        else
                        {
                            get_post[k] = "";
                        }

                    }
                    break;
                case "POST":
                    foreach (string k in inputs.Split(','))
                    {
                        if (this.Context.Request.Form[k] != null)
                        {
                            if (this.Context.Request.Form.GetValues(k).Length != 1)
                            {
                                //暫時先這樣，以後再修= =
                                //alert(this.Context.Request.Form.GetValues(k).Length.ToString());
                                get_post[k] = implode("┃", this.Context.Request.Form.GetValues(k));
                            }
                            else
                            {
                                get_post[k] = this.Context.Request.Form[k];
                            }
                        }
                        else
                        {
                            get_post[k] = "";
                        }
                    }
                    break;
                case "POST_UNVALID":
                    foreach (string k in inputs.Split(','))
                    {
                        if (this.Context.Request.Unvalidated[k] != null)
                        {
                            if (this.Context.Request.Unvalidated.Form.GetValues(k).Length > 0)
                            {
                                //暫時先這樣，以後再修= =
                                //alert(this.Context.Request.Form.GetValues(k).Length.ToString());
                                get_post[k] = implode("┃", this.Context.Request.Unvalidated.Form.GetValues(k));
                            }
                            else
                            {
                                //get_post[k] = this.Context.Request.Form[k];
                                get_post[k] = this.Context.Request.Unvalidated.Form[k];
                            }

                        }
                        else
                        {
                            get_post[k] = "";
                        }
                    }
                    break;
            }
            return get_post;
        }
        public string implode(string keyword, string[] arrays)
        {
            return string.Join(keyword, arrays);
        }
        public string implode(string keyword, List<string> arrays)
        {
            return string.Join<string>(keyword, arrays);
        }
        public string implode(string keyword, Dictionary<int, string> arrays)
        {
            string[] tmp = new String[arrays.Keys.Count];
            int i = 0;
            foreach (int k in arrays.Keys)
            {
                tmp[i++] = arrays[k];
            }
            return string.Join(keyword, tmp);
        }
        public string implode(string keyword, Dictionary<string, string> arrays)
        {
            string[] tmp = new String[arrays.Keys.Count];
            int i = 0;
            foreach (string k in arrays.Keys)
            {
                tmp[i++] = arrays[k];
            }
            return string.Join(keyword, tmp);
        }
        public string implode(string keyword, ArrayList arrays)
        {
            string[] tmp = new String[arrays.Count];
            for (int i = 0; i < arrays.Count; i++)
            {
                tmp[i] = arrays[i].ToString();
            }
            return string.Join(keyword, tmp);
        }
        public string[] explode(string keyword, string data)
        {
            return data.Split(new string[] { keyword }, StringSplitOptions.None);
        }
        public string[] explode(string keyword, object data)
        {
            return data.ToString().Split(new string[] { keyword }, StringSplitOptions.None);
        }
        public string[] explode(string[] keyword, string data)
        {
            return data.Split(keyword, StringSplitOptions.None);
        }
        public void exit()
        {
            System.Web.HttpContext.Current.Response.Flush(); //強制輸出緩衝區資料
            System.Web.HttpContext.Current.Response.Clear(); //清除緩衝區的資料
            System.Web.HttpContext.Current.Response.End(); //結束資料輸出
            //System.Web.HttpContext.Current.Response.StatusCode = 200;
        }
        public void file_put_contents(string filepath, string input)
        {
            file_put_contents(filepath, s2b(input), false);
        }
        public void file_put_contents(string filepath, byte[] input)
        {
            file_put_contents(filepath, input, false);
        }
        public void file_put_contents(string filepath, string input, bool isFileAppend)
        {
            file_put_contents(filepath, s2b(input), isFileAppend);
        }
        public string dirname(string path)
        {
            return Directory.GetParent(path).FullName;
        }
        public string pwd()
        {
            return dirname(System.Web.HttpContext.Current.Request.PhysicalPath);
            //return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }
        public void file_put_contents(string filepath, byte[] input, bool isFileAppend)
        {

            switch (isFileAppend)
            {
                case true:
                    {
                        FileMode FM = new FileMode();
                        if (!is_file(filepath))
                        {
                            FM = FileMode.Create;
                            using (FileStream myFile = File.Open(@filepath, FM, FileAccess.Write, FileShare.Read))
                            {
                                myFile.Seek(myFile.Length, SeekOrigin.Begin);
                                myFile.Write(input, 0, input.Length);
                                myFile.Dispose();
                            }
                        }
                        else
                        {
                            FM = FileMode.Append;
                            using (FileStream myFile = File.Open(@filepath, FM, FileAccess.Write, FileShare.Read))
                            {
                                myFile.Seek(myFile.Length, SeekOrigin.Begin);
                                myFile.Write(input, 0, input.Length);
                                myFile.Dispose();
                            }
                        }
                    }
                    break;
                case false:
                    {
                        using (FileStream myFile = File.Open(@filepath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                        {
                            myFile.Write(input, 0, input.Length);
                            myFile.Dispose();
                        };
                    }
                    break;
            }
        }
        public long filesize(string path)
        {
            FileInfo f = new FileInfo(path);
            return f.Length;
        }
    }
}