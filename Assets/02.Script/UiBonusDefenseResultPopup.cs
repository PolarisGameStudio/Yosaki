﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UiRewardView;

public class UiBonusDefenseResultPopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI description;

    [SerializeField]
    private Transform rewardParent;

    public void OnClickReturnButton()
    {
        GameManager.Instance.LoadNormalField();
    }

    public void Initialize(int defeatEnemiesNum)
    {
        SoundManager.Instance.PlaySound("BonusEnd");

        description.SetText($"{defeatEnemiesNum} 처치 완료!");

        SetReward(defeatEnemiesNum);
    }

    private void SetReward(int defeatEnemiesNum)
    {
        var prefab = CommonPrefabContainer.Instance.uiRewardViewPrefab;
        int blueStoneRewardNum = ContentsRewardManager.Instance.GetDefenseReward_BlueStone(defeatEnemiesNum);
        var blueStoneReward = Instantiate<UiRewardView>(prefab, rewardParent);


        RewardData rewardData = new RewardData(Item_Type.Jade, blueStoneRewardNum);
        blueStoneReward.Initialize(rewardData);

        RewardManager.Instance.GetReward(Item_Type.Jade, blueStoneRewardNum);

        int prefMaxKillCount = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.bonusDungeonEnterCount).Value;

        if (defeatEnemiesNum > prefMaxKillCount)
        {
            ServerData.userInfoTable.GetTableData(UserInfoTable.bonusDungeonMaxKillCount).Value = defeatEnemiesNum;
            ServerData.userInfoTable.UpData(UserInfoTable.bonusDungeonMaxKillCount, false);
        }
    }
}
