using CSharpLike;
using KissFramework;
using SAEA.Http.Model;
using System;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using KissServerFramework;
using System.Collections.Generic;

public class ValidCode
{
    public string token;
    public string code;
    public DateTime expireTime;

    static Dictionary<string, ValidCode> codeDic = new Dictionary<string, ValidCode>();
    static List<ValidCode> codeList = new List<ValidCode>();
    static Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
    {
        Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
        // 将位图背景填充为白色
        Graphics graph = Graphics.FromImage(destBmp);
        graph.FillRectangle(new SolidBrush(Color.LightGray), 0, 0, destBmp.Width, destBmp.Height);
        graph.Dispose();
        double dBaseAxisLen = bXDir ? destBmp.Height : destBmp.Width;
        for (int i = 0; i < destBmp.Width; i++)
        {
            for (int j = 0; j < destBmp.Height; j++)
            {
                double dx = bXDir ? (6.283185307179586476925286766559 * j) / dBaseAxisLen : (6.283185307179586476925286766559 * i) / dBaseAxisLen;
                dx += dPhase;
                double dy = Math.Sin(dx);
                // 取得当前点的颜色
                int nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                int nOldY = bXDir ? j : j + (int)(dy * dMultValue);
                Color color = srcBmp.GetPixel(i, j);
                if (nOldX >= 0 && nOldX < destBmp.Width
                 && nOldY >= 0 && nOldY < destBmp.Height)
                {
                    destBmp.SetPixel(nOldX, nOldY, color);
                }
            }
        }
        return destBmp;
    }
    static byte[] CreateCheckCodeImage(string code)
    {
        MemoryStream ms = null;
        Bitmap image = new Bitmap(72, 24);
        Graphics g = Graphics.FromImage(image);
        try
        {
            Random random = new Random();
            g.Clear(Color.White);
            for (int i = 0; i < 18; i++)
            {
                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);
                g.DrawLine(new Pen(Color.FromArgb(random.Next()), 1), x1, y1, x2, y2);
            }
            Font font = new Font("Times New Roman", 14, FontStyle.Bold);
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
            for (int i = 0; i < 4; i++)
            {
                g.DrawString(code[i].ToString(), font, brush, i * 18 + random.Next(5), random.Next(5));
            }
            for (int i = 0; i < 150; i++)
            {
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);
                image.SetPixel(x, y, Color.FromArgb(random.Next()));
            }
            image = TwistImage(image, true, 3, 1);
            g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
            ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
        }
        finally
        {
            g.Dispose();
            image.Dispose();
        }
        return ms.ToArray();
    }
    [WebMethod]
    static string GetValidCode(JSONData data, string ip, IHttpContext context, Action<string> action)
    {
        byte[] buff = null;
        string randomCodes = "ABCEFGHJKLMNPQRSTWXY123456789";
        string code = "";
        for (int i = 0; i < 4; i++)
        {
            code += randomCodes[Framework.GetRand(randomCodes.Length)];
        }
        new ThreadPoolEvent(() =>
        {
            buff = CreateCheckCodeImage(code);
        },
        () =>
        {
            if (buff == null)
            {
                action("");
                return;
            }
            string token = data["r"];
            ValidCode old = new ValidCode() { code = code, expireTime = DateTime.Now.AddMinutes(10), token = token };
            codeDic[token] = old;
            codeList.Add(old);
            context.Response.Status = System.Net.HttpStatusCode.OK;
            context.Response.SetHeader(ResponseHeaderType.CacheControl, "no-cache");
            context.Response.Body = buff;
            context.Response.ContentLength = buff.Length;
            context.Response.End();
            action("");
        });
        return "";
    }
    public static bool CheckCode(string token, string code)
    {
        if (token.Length < 10 || token.Length > 64)
            return false;
        bool ret = false;
        if (codeDic.TryGetValue(token, out ValidCode validCode))
        {
            ret = validCode.code == code;
            codeDic.Remove(token);
            codeList.Remove(validCode);
        }
        return ret;
    }
    [EventMethod(IntervalTime = 60f)]
    static void UpdateCode()
    {
        DateTime now = DateTime.Now;
        while (codeList.Count > 0)
        {
            ValidCode code = codeList[0];
            if (code.expireTime > now)
            {
                codeList.RemoveAt(0);
                codeDic.Remove(code.code);
            }
            else
                break;
        }
    }
}

