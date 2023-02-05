using KissFramework;
using System;
using System.Collections.Generic;

namespace KissServerFramework
{
	public sealed class Mail : Mail_Base
	{
        /// <summary>
        /// Convert appendix as items.
        /// Format: itemId1,itemCount1 itemId2,itemCount2...
        /// </summary>
        public Dictionary<int, int> AppendixItems
        {
			get
            {
                Dictionary<int, int> items = new Dictionary<int, int>();
                if (!string.IsNullOrEmpty(appendix))
                {
                    try
                    {
                        string[] strs = appendix.Split(" ");
                        foreach (string str in strs)
                        {
                            string[] strs2 = str.Split(",");
                            if (strs2.Length == 2)
                                items[Convert.ToInt32(strs2[0])] = Convert.ToInt32(strs2[1]);
                        }
                    }
                    catch(Exception e)
                    {
                        Logger.LogError($"AppendixItem {appendix} error : {e}");
                    }
                }
                return items;
            }
        }
	}
}
