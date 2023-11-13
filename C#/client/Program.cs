using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utility;
using System.Windows.Forms;
using System;
using System.Drawing;
using ScreenCapturerNS;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Collections.Specialized;

namespace myvnc_client
{

    public static class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        const Int32 CURSOR_SHOWING = 0x00000001;
        public static myinclude my = new myinclude();
        static Bitmap pic = null;
        static int step = 0;
        static void OnScreenUpdated(Object sender, OnScreenUpdatedEventArgs e)
        {
            // Process image (e.Bitmap) here            
            //int new_w = 2048;
            //Bitmap eb = ResizeImage((Bitmap)e.Bitmap.Clone(), new_w);
            int new_w = 1560;// Screen.PrimaryScreen.Bounds.Width;

            float r = (float)Screen.PrimaryScreen.Bounds.Height / (float)Screen.PrimaryScreen.Bounds.Width;
            int new_h = (int)(new_w * r);
            Bitmap eb = ResizeImage((Bitmap)e.Bitmap.Clone(), new_w);
            if (pic == null)
            {
                pic = eb;
            }
            else
            {
                //pic = getDifference(pic, eb);
                pic = eb;
            }


            using (Graphics g = Graphics.FromImage(pic))
            {
                //g.CopyFromScreen(0, 0, 0, 0, new Size(pic.Width, pic.Height), CopyPixelOperation.MergeCopy);
                CURSORINFO pci;
                pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));
                if (GetCursorInfo(out pci))
                {
                    if (pci.flags == CURSOR_SHOWING)
                    {
                        double new_x = my.arduino_map((double)pci.ptScreenPos.x, 0.0, (double)Screen.PrimaryScreen.Bounds.Width, 0.0, new_w);
                        double new_y = my.arduino_map((double)pci.ptScreenPos.y, 0.0, (double)Screen.PrimaryScreen.Bounds.Height, 0.0, new_h);

                        DrawIcon(g.GetHdc(), Convert.ToInt32(new_x), Convert.ToInt32(new_y), pci.hCursor);
                        g.ReleaseHdc();
                    }
                }

            }


            using (HttpClient httpClient = new HttpClient())
            using (MultipartFormDataContent form = new MultipartFormDataContent())
            {
                // 添加 computer_id 字段
                form.Add(new StringContent("1"), "computer_id");

                // 添加 pic 字段
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // 将 Bitmap 保存到内存流中
                    pic.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Tiff);

                    // 将内存流的数据添加到表单中
                    form.Add(new ByteArrayContent(memoryStream.ToArray()), "pic", "image.png");
                }

                // 发送 POST 请求
                string url = "http://localhost/myvnc_server/api.aspx?mode=myvnc_setPIC";
                using (MemoryStream stream = new MemoryStream())
                {
                    // 将 Bitmap 保存到 MemoryStream 中
                    pic.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    UploadImage(url, "1", stream.ToArray());
                };
            }
        }
        static void UploadImage(string url, string computerId, byte[] imageData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=----AAA";

            using (Stream requestStream = request.GetRequestStream())
            {
                // 添加 computer_id 字段
                string formData = $"------AAA\r\nContent-Disposition: form-data; name=\"computer_id\"\r\n\r\n{computerId}\r\n";

                byte[] formDataBytes = System.Text.Encoding.UTF8.GetBytes(formData);
                requestStream.Write(formDataBytes, 0, formDataBytes.Length);

                // 添加图像数据
                string fileHeader = $"------AAA\r\nContent-Disposition: form-data; name=\"pic\"; filename=\"image.png\"\r\nContent-Type: image/png\r\n\r\n";
                byte[] fileHeaderBytes = System.Text.Encoding.UTF8.GetBytes(fileHeader);
                requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);

                requestStream.Write(imageData, 0, imageData.Length);

                // 添加结束边界
                string endBoundary = "\r\n------AAA--\r\n";
                byte[] endBoundaryBytes = System.Text.Encoding.UTF8.GetBytes(endBoundary);
                requestStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string responseText = reader.ReadToEnd();
                    Console.WriteLine($"Server Response: {responseText}");
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        static Bitmap ResizeImage(Bitmap originalImage, int targetWidth)
        {
            float aspectRatio = (float)originalImage.Height / originalImage.Width;
            int targetHeight = (int)(targetWidth * aspectRatio);

            Bitmap resizedImage = new Bitmap(targetWidth, targetHeight);

            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalImage, 0, 0, targetWidth, targetHeight);
            }

            return resizedImage;
        }
        static Bitmap getDifference(Bitmap previousFrame, Bitmap currentFrame)
        {
            // 确保两个图像的大小相同
            if (previousFrame.Size != currentFrame.Size)
            {
                //throw new ArgumentException("图像大小不一致");
                return currentFrame;
            }

            Bitmap diff = new Bitmap(previousFrame.Width, previousFrame.Height);

            // 锁定位图数据以便访问像素
            BitmapData previousData = previousFrame.LockBits(new Rectangle(0, 0, previousFrame.Width, previousFrame.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData currentData = currentFrame.LockBits(new Rectangle(0, 0, currentFrame.Width, currentFrame.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData diffData = diff.LockBits(new Rectangle(0, 0, diff.Width, diff.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            // 获取像素数据
            byte[] previousPixels = new byte[previousData.Stride * previousData.Height];
            Marshal.Copy(previousData.Scan0, previousPixels, 0, previousPixels.Length);

            byte[] currentPixels = new byte[currentData.Stride * currentData.Height];
            Marshal.Copy(currentData.Scan0, currentPixels, 0, currentPixels.Length);

            byte[] diffPixels = new byte[diffData.Stride * diffData.Height];

            // 找到不同的像素，以新圖為主
            for (int i = 0; i < previousPixels.Length; i += 4)
            {
                if (previousPixels[i] != currentPixels[i])// || // Blue
                                                          //previousPixels[i + 1] != currentPixels[i + 1] || // Green
                                                          //previousPixels[i + 2] != currentPixels[i + 2] || // Red
                                                          //previousPixels[i + 3] != currentPixels[i + 3]) // Alpha
                {
                    // 将不同的像素设置为新图像的像素值
                    diffPixels[i] = currentPixels[i]; // Blue
                    diffPixels[i + 1] = currentPixels[i + 1]; // Green
                    diffPixels[i + 2] = currentPixels[i + 2]; // Red
                    diffPixels[i + 3] = currentPixels[i + 3]; // Alpha
                }
                else
                {
                    // 將相同的像素設置為白色
                    diffPixels[i] = 255; // Blue
                    diffPixels[i + 1] = 255; // Green
                    diffPixels[i + 2] = 255; // Red
                    diffPixels[i + 3] = 0; // Alpha
                }
            }

            // 将差异数据复制回位图
            Marshal.Copy(diffPixels, 0, diffData.Scan0, diffPixels.Length);

            // 解锁位图数据
            previousFrame.UnlockBits(previousData);
            currentFrame.UnlockBits(currentData);
            diff.UnlockBits(diffData);

            return diff;
        }
        static void Main(string[] args)
        {
            ScreenCapturer.SkipFirstFrame = true;
            ScreenCapturer.SkipFrames = true;
            ScreenCapturer.OnScreenUpdated += OnScreenUpdated;
            ScreenCapturer.StartCapture();
        }

    }
}
