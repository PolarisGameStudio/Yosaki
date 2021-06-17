﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using System;
using UniRx;

public class IAPServerData
{
    public int id;
    public ReactiveProperty<int> buyCount = new ReactiveProperty<int>();

    public string ConvertToString()
    {
        return $"{id},{buyCount.Value}";
    }
}

public class IAPServerTable
{
    public static string Indate;
    public static string tableName = "IAPHistory";


    private ReactiveDictionary<string, IAPServerData> tableDatas = new ReactiveDictionary<string, IAPServerData>();

    public ReactiveDictionary<string, IAPServerData> TableDatas => tableDatas;


    public void Initialize()
    {
        tableDatas.Clear();

        SendQueue.Enqueue(Backend.GameData.GetMyData, tableName, new Where(), callback =>
        {
            // 이후 처리
            if (callback.IsSuccess() == false)
            {
                Debug.LogError("IAPServerTable");
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, CommonString.DataLoadFailedRetry, Initialize);
                return;
            }

            var rows = callback.Rows();

            //맨처음 초기화
            if (rows.Count <= 0)
            {
                Param defultValues = new Param();

                var table = TableManager.Instance.InAppPurchase.dataArray;

                for (int i = 0; i < table.Length; i++)
                {
                    defultValues.Add(table[i].Productid, $"{table[i].Id},0");

                    var iapData = new IAPServerData();
                    iapData.id = table[i].Id;
                    iapData.buyCount = new ReactiveProperty<int>(0);

                    tableDatas.Add(table[i].Productid, iapData);
                }

                var bro = Backend.GameData.Insert(tableName, defultValues);

                if (bro.IsSuccess() == false)
                {
                    // 이후 처리
                    DatabaseManager.ShowCommonErrorPopup(bro, Initialize);
                    return;
                }
                else
                {
                    var jsonData = bro.GetReturnValuetoJSON();
                    if (jsonData.Keys.Count > 0)
                    {
                        Indate = jsonData[0].ToString();
                    }
                }

                return;
            }
            //나중에 칼럼 추가됐을때 업데이트
            else
            {
                Param defultValues = new Param();
                int paramCount = 0;

                JsonData data = rows[0];

                if (data.Keys.Contains(DatabaseManager.inDate_str))
                {
                    Indate = data[DatabaseManager.inDate_str][DatabaseManager.format_string].ToString();
                }

                var table = TableManager.Instance.InAppPurchase.dataArray;

                for (int i = 0; i < table.Length; i++)
                {
                    if (data.Keys.Contains(table[i].Productid))
                    {
                        //값로드
                        var value = data[table[i].Productid][DatabaseManager.format_string].ToString();

                        var iapData = new IAPServerData();

                        var splitData = value.Split(',');

                        iapData.id = int.Parse(splitData[0]);
                        iapData.buyCount = new ReactiveProperty<int>(int.Parse(splitData[1]));

                        tableDatas.Add(table[i].Productid, iapData);
                    }
                    else
                    {
                        defultValues.Add(table[i].Productid, $"{table[i].Id},0");

                        var iapData = new IAPServerData();
                        iapData.id = table[i].Id;
                        iapData.buyCount = new ReactiveProperty<int>(0);

                        tableDatas.Add(table[i].Productid, iapData);
                        paramCount++;
                    }
                }

                if (paramCount != 0)
                {
                    var bro = Backend.GameData.Update(tableName, Indate, defultValues);

                    if (bro.IsSuccess() == false)
                    {
                        DatabaseManager.ShowCommonErrorPopup(bro, Initialize);
                        return;
                    }
                }

            }
        });
    }

    public void UpData(string key, bool LocalOnly)
    {
        throw new System.NotImplementedException();
    }

    public void UpData(string key, float data, bool LocalOnly)
    {

    }
}
