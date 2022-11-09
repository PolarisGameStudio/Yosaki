using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UiRewardView;

public class UiDokebiTowerBoard : MonoBehaviour
{
    [SerializeField]
    private UiTower3RewardView uiTower3RewardView;

    [SerializeField]
    private TextMeshProUGUI currentStageText;

    [SerializeField]
    private GameObject normalRoot;

    [SerializeField]
    private GameObject allClearRoot;

    void OnEnable()
    {
        SetStageText();
        SetReward();
    }

    private bool IsAllClear()
    {
        int currentFloor = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.currentFloorIdx3).Value;

        return currentFloor >= TableManager.Instance.towerTable3.dataArray.Length;
    }

    private void SetStageText()
    {
        if (IsAllClear() == false)
        {
            int currentFloor = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.currentFloorIdx3).Value;
            currentStageText.SetText($"{currentFloor + 1}�� ����");
        }
        else
        {
            currentStageText.SetText($"������Ʈ ���� �Դϴ�");
        }

    }

    private void SetReward()
    {
        bool isAllClear = IsAllClear();

        normalRoot.SetActive(isAllClear == false);
        allClearRoot.SetActive(isAllClear == true);

        if (isAllClear == false)
        {
            int currentFloor = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.currentFloorIdx3).Value;

            if (currentFloor >= TableManager.Instance.towerTable3.dataArray.Length)
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"�߸��� ������ idx : {currentFloor}", null);
                return;
            }

            var towerTableData = TableManager.Instance.towerTable3.dataArray[currentFloor];

            uiTower3RewardView.UpdateRewardView(towerTableData.Id);
        }


    }
}
