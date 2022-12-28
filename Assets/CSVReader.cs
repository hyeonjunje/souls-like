using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CSVReader
{
    public static void CSVRead(string fileName, Action<List<string>> callback)
    {
        StreamReader sr = new StreamReader(Application.dataPath + "/" + fileName);

        if(sr == null)
        {
            Debug.LogWarning("없는 파일입니다.");
            return;
        }

        bool isFirst = true;

        bool endOfFile = false;

        while (!endOfFile)
        {
            string data_String = sr.ReadLine();

            if (isFirst)
            {
                isFirst = false;
                continue;
            }

            if (data_String == null)
            {
                endOfFile = true;
                break;
            }
            var data_values = data_String.Split(',');
            List<string> datas = new List<string>();
            for (int i = 0; i < data_values.Length; i++)
            {
                datas.Add(data_values[i].ToString());
            }
            callback?.Invoke(datas);
        }

    }
}
