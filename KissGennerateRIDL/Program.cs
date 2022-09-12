using System;
using System.IO;
using System.Net;
using System.Text;
using CSharpLike;

namespace KissGennerateRIDL
{
    class Program
    {
        /// <summary>
        /// Normally you just run this program withow args, andd automatic turn all *.ridl files in current folder(include sub folder) into *.cs.
        /// </summary>
        /// <param name="args">[folder path] [debug] [serial number]</param>
        static void Main(string[] args)
        {
            //args [folder path] [debug] [serial number]
            string folder = Environment.CurrentDirectory;
            bool debug = false;
            //Normally don't need serial number, just in case of too much requests make our server overload.
            //We will give priority to user with serial number.
            //When our server overload we limit the request counts each day with same IP and limit the size of the request and limit IP blacklist.
            string serialNumber = "";
            //Load config from args
            for(int i=0; i<args.Length; i++)
            {
                string arg = args[i].Trim();
                Console.WriteLine(arg);
                if (arg == "debug")
                    debug = true;
                else
                {
                    if (string.IsNullOrEmpty(serialNumber))
                    {
                        serialNumber = arg.Replace("-", "");
                        if (serialNumber.Length != 25)//serial number format is 25 letters, like "xxxxx-xxxxx-xxxxx-xxxxx-xxxxx" or "xxxxxxxxxxxxxxxxxxxxxxxxx"
                        {
                            folder = arg;
                            serialNumber = "";
                        }
                    }
                    else
                        folder = arg;
                }
            }
            if (string.IsNullOrEmpty(serialNumber))
                serialNumber = "anonymous";
            Console.WriteLine("folder = "+ folder);
            Console.WriteLine("debug = " + debug);
            Console.WriteLine("serial number = " + serialNumber);

            try
            {
                //Prepare the RIDL file.
                JSONData jsonData = JSONData.NewDictionary();
                jsonData["sn"] = serialNumber;
                jsonData["files"] = LoadFiles(folder);

                //Request the web service.
                Console.WriteLine("start send request.");
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://sn.csharplike.com:11114/GennerateRIDL");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Timeout = 30000;//30 seconds
                httpWebRequest.Method = "POST";
                byte[] buffPost = Encoding.UTF8.GetBytes(jsonData);
                httpWebRequest.ContentLength = buffPost.Length;
                using (Stream stream = httpWebRequest.GetRequestStream())
                    stream.Write(buffPost, 0, buffPost.Length);
                string callback = "";
                using (Stream stream = httpWebRequest.GetResponse().GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        callback = reader.ReadToEnd();
                    }
                }

                //Save file if successes.
                jsonData = KissJson.ToJSONData(callback);
                if (string.IsNullOrEmpty(jsonData["error"]))
                {
                    JSONData jsonFiles = jsonData["files"];
                    Console.WriteLine($"received {jsonFiles.Count} files.");
                    for (int i = 0; i < jsonFiles.Count; i++)
                    {
                        JSONData jsonFile = jsonFiles[i];
                        Console.WriteLine($"path : {jsonFile["path"]}");
                        if (!File.Exists(folder + "\\" + jsonFile["path"]))
                            File.WriteAllBytes(folder + "\\" + jsonFile["path"], CSL_Utils.Decompress(Convert.FromBase64String(jsonFile["content"])));
                        else
                            Console.WriteLine($"Exist {folder + "\\" + jsonFile["path"]}, it won't be replace.");
                        File.WriteAllBytes(folder + "\\" + jsonFile["pathBase"], CSL_Utils.Decompress(Convert.FromBase64String(jsonFile["contentBase"])));
                    }
                    myConfig.Save();//save the config file when success.
                }
                else
                {
                    Console.WriteLine($"callback error : {jsonData["error"]}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //Let user see the log at console in debug mode.
            if (debug)
                Console.ReadKey();
        }
        /// <summary>
        /// Cache the ridl file MD5 value.
        /// </summary>
        static MyConfig myConfig = null;
        /// <summary>
        /// Load all the modified RIDL files into JSONData object.
        /// </summary>
        /// <param name="path">The folder paht of your RIDL file, we will search all RIDL files in it with all directories. 
        /// And we will save file md5 value into 'config.ini' file in that folder for check the RIDL file whether was modified.</param>
        /// <returns>The JSONData about to send to server. We won't save any your file name or your file content in our server.</returns>
        public static JSONData LoadFiles(string path)
        {
            JSONData jsonDatas = JSONData.NewList();//[{"path":"path1","content":"base64"},{"path":"path2","content":"base64"}]
            string[] files = Directory.GetFiles(path, "*.ridl", SearchOption.AllDirectories);
            if (files.Length == 0)
                throw new Exception($"Not exist *.ridl file in {path}.");
            myConfig = new MyConfig(path + "/KissGennerateRIDL.ini");
            foreach (string file in files)
            {
                string str = File.ReadAllText(file, Encoding.UTF8).Replace("\r", "").Replace("\t", " ");
                while (str.Contains("  "))
                    str = str.Replace("  ", " ");
                if (string.IsNullOrEmpty(str))
                    continue;
                string[] lines = str.Split('\n');
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (line.Length == 0)
                        continue;
                    int index = line.IndexOf("//");
                    if (index == 0)
                        continue;
                    if (index > 0)
                        line = line.Substring(0, index);
                    sb.Append(line);
                    sb.Append('\n');
                }
                string strFinal = sb.ToString();
                if (strFinal.Length == 0)
                    continue;
                strFinal = Convert.ToBase64String(CSL_Utils.Compress(Encoding.UTF8.GetBytes(strFinal)));
                string name = file.Replace(path, "");
                while (name[0] == '\\')
                    name = name.Substring(1);
                name = name.Replace("\\", "/");
                string md5Olde = myConfig.GetString(file);
                string md5 = CSL_Utils.GetMD5(strFinal);
                if (md5Olde == md5)
                    continue;
                myConfig.SetString(file, md5);
                JSONData jsonData = JSONData.NewDictionary();//{"path":"path1","content":"base64"}
                jsonDatas.Add(jsonData);
                jsonData["path"] = name;
                jsonData["content"] = strFinal;
            }
            if (jsonDatas.Count == 0)
                throw new Exception($"All *.ridl file in {path} were not be modified since last success request. You can delete '{path}/config.ini' if you want to force rebuild all files.");
            return jsonDatas;
        }
    }
}
