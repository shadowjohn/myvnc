<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Import Namespace="utility" %>
<% 
    myinclude my = new myinclude();
%>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <script type="text/javascript" src="js/jquery.min.js"></script>
    <script type="text/javascript" src="js/include.js?_t=<% my.echo(my.time()); %>"></script>
    <link rel="stylesheet" href="css/reset.css" />
    <link rel="stylesheet" href="css/style.css" />
    <title>相當神祕的後台管理</title>
</head>
<body>
    <canvas reqc="canvas"></canvas>
    <script>
        var App = {
            item: {
                w: null,
                h: null,
                canvas: null,
                ctx: null
            },
            computer_id: 1,
            method: {
                loadPic: function () {
                    var URL = "http://localhost/myvnc_server/api.aspx?mode=myvnc_getNewestPIC&computer_id=" + App.computer_id + "&_t=" + microtime();
                    App.item.canvas = $("canvas[reqc='canvas']")[0];
                    var img = new Image();
                    
                    img.onload = function () {
                        App.item.w = img.width;
                        App.item.h = img.height;
                        if (App.item.ctx == null) {
                            App.item.canvas.width = App.item.w;
                            App.item.canvas.height = App.item.h;
                            App.item.ctx = App.item.canvas.getContext('2d');
                            App.item.ctx.drawImage(img, 0, 0, App.item.canvas.width, App.item.canvas.height);
                        }
                        else {
                            var tempCanvas = document.createElement('canvas');
                            tempCanvas.width = img.width;
                            tempCanvas.height = img.height;
                            var tempContext = tempCanvas.getContext('2d');

                            // 在临时 canvas 上绘制 img2，用于找到差异
                            App.item.ctx.drawImage(img, 0, 0, tempCanvas.width, tempCanvas.height);

                        }
                        setTimeout(function () {
                            App.method.loadPic();
                        }, 50)
                    }
                    img.src = URL;
                }
            }
        };
        App.method.loadPic();
    </script>
</body>
</html>
