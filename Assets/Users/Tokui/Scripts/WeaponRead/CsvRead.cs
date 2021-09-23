using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class CsvRead : MonoBehaviour ,IManagedMethod
{
    TextAsset csvFile; // CSVファイル
    List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト;

    Encoding encoding;

    public void ManagedStart()
    {
        this.encoding = Encoding.GetEncoding("utf-8");
        csvFile = Resources.Load("Data/WeaponData") as TextAsset; // Resouces下のCSV読み込み
        StringReader reader = new StringReader(csvFile.text);

        // , で分割しつつ一行ずつ読み込み
        // リストに追加していく
        reader.ReadLine(); // 1行読み捨てる。
        while (reader.Peek() != -1) // reader.Peaekが-1になるまで
        {
            string line = reader.ReadLine(); // 一行ずつ読み込み
            csvDatas.Add(line.Split(',')); // , 区切りでリストに追加
        }

        foreach (string[] Row in csvDatas)
        {
            WeaponManager.weaponDatas.Add(new WeaponDatas
            {
                WeaponName = Row[0],
                PrefabsName = Row[1],
                BulletSpeed = float.Parse(Row[2]),
                GaugeMax = float.Parse(Row[3]),
                GaugeRecovery = float.Parse(Row[4]),
                Weaponinfo = (Row[5])
            });
        }

        // csvDatas[行][列]を指定して値を自由に取り出せる
        // Debug.Log(csvDatas/*行*/[0]/*列*/[0]);
        // Debug.Log(csvDatas/*行*/[1]/*列*/[0]);
        // Debug.Log(csvDatas/*行*/[2]/*列*/[0]);
    }

    public void ManagedUpdate()
    {

    }
}
