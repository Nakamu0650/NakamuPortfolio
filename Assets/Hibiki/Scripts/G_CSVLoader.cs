using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class CSVLoader
{
    // CSVファイルにデータを保存する
    public static void Save<T>(string filePath, List<T> dataList)
    {
        var lines = new List<string>();

        // ヘッダー行を追加（クラスのフィールド名）
        var properties = typeof(T).GetFields();
        var header = string.Join(",", properties.Select(p => p.Name));
        lines.Add(header);

        // 各データをCSVフォーマットに変換
        foreach (var data in dataList)
        {
            var line = string.Join(",", properties.Select(p => p.GetValue(data)?.ToString() ?? ""));
            lines.Add(line);
        }

        File.WriteAllLines(filePath, lines, Encoding.UTF8);
    }

    // CSVファイルからデータを読み込む
    public static List<T> Load<T>(string filePath) where T : new()
    {
        var dataList = new List<T>();
        string[] lines = new string[0];
        try
        {
            lines = File.ReadAllLines(filePath, Encoding.UTF8);
        }
        catch (UnauthorizedAccessException)
        {
            Debug.LogError($"指定されたファイルパス\"{filePath}\"にはアクセスできません");
            return dataList;
        }

        

        if (lines.Length == 0)
            return dataList;

        var properties = typeof(T).GetFields();
        var headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');
            var data = new T();

            for (int j = 0; j < headers.Length; j++)
            {
                var property = properties.FirstOrDefault(p => p.Name == headers[j]);
                if (property != null)
                {
                    // データ型に変換してフィールドに設定
                    var value = Convert.ChangeType(values[j], property.FieldType);
                    property.SetValue(data, value);
                }
            }

            dataList.Add(data);
        }

        return dataList;
    }

    public static string GetDefaultPath()
    {
        return Application.persistentDataPath;
    }
}