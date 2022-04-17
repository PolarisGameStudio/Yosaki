﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiGumGiBoard : MonoBehaviour
{
    [SerializeField]
    private UiGumGiCell gumGiCellPrefab;

    [SerializeField]
    private Transform cellParent;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        var tableData = TableManager.Instance.gumGiTable.dataArray;

        for (int i = 0; i < tableData.Length; i++)
        {
            var cell = Instantiate<UiGumGiCell>(gumGiCellPrefab, cellParent);

            cell.Initialize(tableData[i]);
        }
    }

}
