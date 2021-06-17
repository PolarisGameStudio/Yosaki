﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using BackEnd;
using LitJson;
using System.Linq;

[System.Serializable]
public class PassiveServerData 
{
    public int idx;
    public ReactiveProperty<int> level;

    public string ConvertToString()
    {
        return $"{idx},{level.Value}";
    }
}

public class PassiveServerTable
{
    public static string Indate;
    public static string tableName = "PassiveSkill";

    private ReactiveDictionary<string, PassiveServerData> tableDatas = new ReactiveDictionary<string, PassiveServerData>();

    public ReactiveDictionary<string, PassiveServerData> TableDatas => tableDatas;


    public void Initialize()
    {
        tableDatas.Clear();

        SendQueue.Enqueue(Backend.GameData.GetMyData, tableName, new Where(), callback =>
        {
            // 이후 처리
            if (callback.IsSuccess() == false)
            {
                Debug.LogError("LoadStatusFailed");
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, CommonString.DataLoadFailedRetry, Initialize);
                return;
            }

            var rows = callback.Rows();

            //맨처음 초기화
            if (rows.Count <= 0)
            {
                Param defultValues = new Param();

                var table = TableManager.Instance.PassiveSkill.dataArray;

                for (int i = 0; i < table.Length; i++)
                {
                    var passiveData = new PassiveServerData();
                    passiveData.idx = table[i].Id;
                    passiveData.level = new ReactiveProperty<int>(0);

                    tableDatas.Add(table[i].Stringid, passiveData);
                    defultValues.Add(table[i].Stringid, passiveData.ConvertToString());
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

                var table = TableManager.Instance.PassiveSkill.dataArray;

                for (int i = 0; i < table.Length; i++)
                {
                    if (data.Keys.Contains(table[i].Stringid))
                    {
                        //값로드
                        var value = data[table[i].Stringid][DatabaseManager.format_string].ToString();

                        var passiveSkill = new PassiveServerData();

                        var splitData = value.Split(',');

                        passiveSkill.idx = int.Parse(splitData[0]);
                        passiveSkill.level = new ReactiveProperty<int>(int.Parse(splitData[1]));

                        tableDatas.Add(table[i].Stringid, passiveSkill);
                    }
                    else
                    {

                        var passiveData = new PassiveServerData();
                        passiveData.idx = table[i].Id;
                        passiveData.level = new ReactiveProperty<int>(0);

                        tableDatas.Add(table[i].Stringid, passiveData);
                        defultValues.Add(table[i].Stringid, passiveData.ConvertToString());
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
}
