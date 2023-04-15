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
        /// Normally you just run this program withow args, and automatic turn all *.ridl files in current folder(include sub folder) into *.cs.
        /// You can set the 'KissGennerateRIDL.ini' in current folder.
        /// If your are using Full C#Like version, you can set 'KissGennerateRIDL.ini' like this 'folderClientFull = D:/MyProject/Assets/C#Like/HotUpdateScripts/Sample/NetObjects'.
        /// If your are using Free C#Like version, you can set 'KissGennerateRIDL.ini' like this 'folderClientFree = D:/MyProject/Assets/C#Like/HotUpdateScripts/Sample/NetObjects'.
        /// </summary>
        /// <param name="args">[folder path] [debug] [serial number]</param>
        static void Main(string[] args)
        {
            //args [folder path] [debug] [serial number]
            string folder = Environment.CurrentDirectory;
            if (folder.EndsWith("\\OldTools"))
                folder = folder.Substring(0, folder.Length - 9);
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
            bool bFree = string.IsNullOrEmpty(serialNumber);
            if (bFree)
                serialNumber = "anonymous";
            Console.WriteLine("folder = "+ folder);
            Console.WriteLine("debug = " + debug);
            Console.WriteLine("serial number = " + serialNumber);

            try
            {
                //Prepare the RIDL file.
                JSONData jsonData = JSONData.NewDictionary();
                jsonData["sn"] = serialNumber;
                jsonData["files"] = LoadFiles(folder, bFree ? 20 : 200);//Don't modify the value of maxCount, the real limit is at server, not here.

                //Request the web service.
                Console.WriteLine("start send request.");
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(bFree
                    ? "http://sn.csharplike.com:11114/GennerateRIDL" : "http://snVIP.csharplike.com:11114/GennerateRIDL");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Timeout = 30000;//30 seconds
                httpWebRequest.Method = "POST";
                byte[] buffPost = Encoding.UTF8.GetBytes(jsonData.ToJson());
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

                //Save file if success.
                jsonData = KissJson.ToJSONData(callback);
                if (string.IsNullOrEmpty(jsonData["error"]))
                {
                    JSONData jsonFiles = jsonData["files"];
                    Console.WriteLine($"received {jsonFiles.Count} files.");
                    string folderServerBackup = folder + "\\Export\\Server";
                    string folderClientFullBackup = folder + "\\Export\\Client\\FullVersion";
                    string folderClientFreeBackup = folder + "\\Export\\Client\\FreeVersion";
                    string folderClientFull = myConfig.GetString("folderClientFull");//Absolute path for Unity with Full C#Like version, such as set 'KissGennerateRIDL.ini' like this 'folderClientFull = D:/MyProject/Assets/C#Like/HotUpdateScripts/Sample/NetObjects'
                    string folderClientFree = myConfig.GetString("folderClientFree");//Absolute path for Unity with Free C#Like version, such as set 'KissGennerateRIDL.ini' like this 'folderClientFree = D:/MyProject/Assets/C#Like/HotUpdateScripts/Sample/NetObjects'
                    if (!Directory.Exists(folderServerBackup))
                        Directory.CreateDirectory(folderServerBackup);
                    if (!Directory.Exists(folderClientFullBackup))
                        Directory.CreateDirectory(folderClientFullBackup);
                    if (!Directory.Exists(folderClientFreeBackup))
                        Directory.CreateDirectory(folderClientFreeBackup);
                    if (!string.IsNullOrEmpty(folderClientFull) && !Directory.Exists(folderClientFull))
                        Directory.CreateDirectory(folderClientFull);
                    if (!string.IsNullOrEmpty(folderClientFree) && !Directory.Exists(folderClientFree))
                        Directory.CreateDirectory(folderClientFree);
                    if (!File.Exists(folder + "\\Export\\ReadMe.txt"))
                        File.WriteAllText(folder + "\\Export\\ReadMe.txt", "Folder '/Export/Server/' is for server code backup (We had copy '*.cs' to the path that same with the '*.ridl'), all files in that folder name suffix with '.cs.backup'.\n\nFolder '/Export/Client/FullVersion' is for the C#Like FULL version, and folder '/Export/Client/FreeVersion' is for the C#Like FREE version. You choose that according to your C#Like version . All files in their folder name suffix with '.backup', please rename all the '*.cs.backup' to '*.cs' after you copy to your Unity project, because we don't want that code be identified as server code by KissServerFramework project.\n\nYou can copy that client code to your C#Like HotUpdate script folder in your Unity project, such as 'Assets\\C#Like\\HotUpdateScripts\\NetObjects\\'.\n\nStrong recommend that set absolute path in KissGennerateRIDL.ini for copy the '*.cs' to your project. \nIf your are using Full C#Like version, you can set 'config.ini' like this 'folderClientFull = D:/MyProject/Assets/C#Like/HotUpdateScripts/NetObjects' \nIf your are using Free C#Like version, you can set 'KissGennerateRIDL.ini' like this 'folderClientFree = D:/MyProject/Assets/C#Like/HotUpdateScripts/Sample/NetObjects'");

                    for (int i = 0; i < jsonFiles.Count; i++)
                    {
                        JSONData jsonFile = jsonFiles[i];
                        string fileName = jsonFile["fileName"];
                        Console.WriteLine($"fileName : {fileName}");

                        byte[] buff = CSL_Utils.Decompress(Convert.FromBase64String(jsonFile["content"]));
                        if (!File.Exists(folder + "\\" + jsonFile["path"]))
                            File.WriteAllBytes(folder + "\\" + jsonFile["path"], buff);
                        else
                            Console.WriteLine($"Exist {folder + "\\" + jsonFile["path"]}, it won't be replace.");
                        File.WriteAllBytes(folderServerBackup + "\\" + fileName + ".cs.txt", buff);

                        buff = CSL_Utils.Decompress(Convert.FromBase64String(jsonFile["contentBase"]));
                        File.WriteAllBytes(folder + "\\" + jsonFile["pathBase"], buff);
                        File.WriteAllBytes(folderServerBackup + "\\" + fileName + "_Base.cs.txt", buff);

                        buff = CSL_Utils.Decompress(Convert.FromBase64String(jsonFile["contentFull"]));
                        File.WriteAllBytes(folderClientFullBackup + "\\" + fileName + ".cs.txt", buff);
                        if (!string.IsNullOrEmpty(folderClientFull))
                        {
                            if (!File.Exists(folderClientFull+"\\"+ fileName+".cs"))
                                File.WriteAllBytes(folderClientFull + "\\" + fileName + ".cs", buff);
                            else
                                Console.WriteLine($"Exist {folderClientFull + "\\" + fileName + ".cs"}, it won't be replace.");
                        }

                        buff = CSL_Utils.Decompress(Convert.FromBase64String(jsonFile["contentFullBase"]));
                        File.WriteAllBytes(folderClientFullBackup + "\\" + fileName + "_Base.cs.txt", buff);
                        if (!string.IsNullOrEmpty(folderClientFull))
                            File.WriteAllBytes(folderClientFull + "\\" + fileName + "_Base.cs", buff);

                        buff = CSL_Utils.Decompress(Convert.FromBase64String(jsonFile["contentFree"]));
                        File.WriteAllBytes(folderClientFreeBackup + "\\" + fileName + ".cs.txt", buff);
                        if (!string.IsNullOrEmpty(folderClientFree))
                        {
                            if (!File.Exists(folderClientFree + "\\" + fileName + ".cs"))
                                File.WriteAllBytes(folderClientFree + "\\" + fileName + ".cs", buff);
                            else
                                Console.WriteLine($"Exist {folderClientFree + "\\" + fileName + ".cs"}, it won't be replace. You should merge it yourself!!!!!");
                        }
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
                Console.WriteLine(e.Message);
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
        /// And we will save file md5 value into 'KissGennerateRIDL.ini' file in that folder for check the RIDL file whether was modified.</param>
        /// <param name="maxCount">Limit the file count process each time. </param>
        /// <returns>The JSONData about to send to server. We won't save any your file name or your file content in our server.</returns>
        public static JSONData LoadFiles(string path, int maxCount)
        {
            JSONData jsonDatas = JSONData.NewList();//[{"path":"path1","content":"base64"},{"path":"path2","content":"base64"}]
            string[] files = Directory.GetFiles(path, "*.ridl", SearchOption.AllDirectories);
            if (files.Length == 0)
                throw new Exception($"Not exist *.ridl file in {path}.");
            myConfig = new MyConfig(path + "/KissGennerateRIDL.ini");
            if (string.IsNullOrEmpty(myConfig.GetString("folderClientFull")))
                myConfig.SetString("folderClientFull","");
            if (string.IsNullOrEmpty(myConfig.GetString("folderClientFree")))
                myConfig.SetString("folderClientFree", "");
            foreach (string file in files)
            {
                if (jsonDatas.Count > maxCount)
                {
                    Console.WriteLine($"Had reach max file count of each time, you need run the 'KissGennerateRIDL.exe' again. Free max count is 20 and VIP is 200. e.g. If you are free user and have 25 *.ridl files want to process, you first time run the 'KissGennerateRIDL.exe' will process 20 files, and the second time run the 'KissGennerateRIDL.exe' will process the next 5 files.");
                    break;
                }
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
                throw new Exception($"All *.ridl files in {path} were not be modified since last success request. You can delete '{path}/KissGennerateRIDL.ini' if you want to force rebuild all files.");
            return jsonDatas;
        }
    }
}
