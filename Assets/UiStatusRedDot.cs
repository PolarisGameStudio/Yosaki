﻿using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UiStatusRedDot : UiRedDotBase
{
    private ReactiveProperty<bool> hasRedDot = new ReactiveProperty<bool>();

    protected override void Subscribe()
    {
        DatabaseManager.statusTable.GetTableData(StatusTable.StatPoint).AsObservable().Subscribe(e =>
        {
            hasRedDot.Value |= e > 0;
        }).AddTo(this);

        DatabaseManager.statusTable.GetTableData(StatusTable.Memory).AsObservable().Subscribe(e =>
        {
            hasRedDot.Value |= e > 0;
        }).AddTo(this);

        hasRedDot.AsObservable().Subscribe(e=> 
        {
            rootObject.SetActive(e);
        }).AddTo(this);
    }
}
