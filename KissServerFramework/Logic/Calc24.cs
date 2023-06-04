using CSharpLike;
using KissFramework;
using SAEA.Http.Model;
using System;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using KissServerFramework;
using System.Collections.Generic;
using System.Text;

public class Calc24
{
    static Dictionary<string, JSONData> datas;
    static List<string> keys;
    static Calc24()
    {
        datas = new Dictionary<string, JSONData>();
        keys = new List<string>();
        string[] strs = File.ReadAllText(Environment.CurrentDirectory + "/24.txt", Encoding.UTF8).Split('\n');
        foreach(string str in strs)
        {
            string[] strss = str.Split(':');
            if (strss.Length == 2)
            {
                string key = strss[0];
                JSONData dataDic = JSONData.NewDictionary();
                JSONData dataList = JSONData.NewList();
                string[] values = key.Split(',');
                dataDic["v1"] = values[0];
                dataDic["v2"] = values[1];
                dataDic["v3"] = values[2];
                dataDic["v4"] = values[3];
                dataDic["msg"] = "";
                string[] strsss = strss[1].Split('\t');
                foreach (string str2 in strsss)
                    dataList.Add(str2);
                dataDic["result"] = dataList;
                datas[key] = dataDic;
                keys.Add(key);
            }
        }
    }
    [WebMethod]
    static string HttpRefreshCalc24(int v1 = 0, int v2 = 0, int v3 = 0, int v4 = 0)
    {
        string keyOld = "";
        if (v1 > 0 && v2 > 0 && v3 > 0 && v4 > 0)
        {
            List<int> sorts = new List<int>();
            sorts.Add(v1);
            sorts.Add(v2);
            sorts.Add(v3);
            sorts.Add(v4);
            sorts.Sort();
            foreach (int v in sorts)
            {
                keyOld += v + ",";
            }
            keyOld = keyOld.Substring(0, keyOld.Length - 1);
        }
        string key;
        do
        {
            key = keys[Framework.GetRand(keys.Count)];
        }
        while (key == keyOld);
        return datas[key].ToJson();
    }
}

