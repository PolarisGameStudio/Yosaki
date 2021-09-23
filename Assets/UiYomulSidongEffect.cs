﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UiYomulSidongEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject rootObject;

    private List<ReactiveProperty<int>> buffRemainTimes = new List<ReactiveProperty<int>>();

    void Start()
    {
        Subscribe();
    }

    private bool HasActivatedYomulBuff()
    {
        for (int i = 0; i < buffRemainTimes.Count; i++)
        {
            if (buffRemainTimes[i].Value > 0)
            {
                return true;
            }
        }
        return false;
    }

    private void Subscribe()
    {
        buffRemainTimes.Clear();

        var tableData = TableManager.Instance.BuffTable.dataArray;

        for (int i = 0; i < tableData.Length; i++)
        {
            if (tableData[i].Isyomulabil)
            {
                buffRemainTimes.Add(ServerData.buffServerTable.TableDatas[tableData[i].Stringid].remainSec);
            }
        }

        for (int i = 0; i < tableData.Length; i++)
        {
            if (tableData[i].Isyomulabil)
            {
                ServerData.buffServerTable.TableDatas[tableData[i].Stringid].remainSec.AsObservable().Subscribe(e =>
                {
                    rootObject.SetActive(HasActivatedYomulBuff());
                }).AddTo(this);
            }
        }
    }
}
