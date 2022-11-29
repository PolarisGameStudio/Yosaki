using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static UiRewardView;
public class UiPartyTowerBoard : MonoBehaviour
{
    [SerializeField]
    private UiTower4RewardView uiTower4RewardView;

    [SerializeField]
    private TextMeshProUGUI currentStageText;

    [SerializeField]
    private GameObject normalRoot;

    [SerializeField]
    private GameObject allClearRoot;

    [SerializeField]
    private TextMeshProUGUI helpDescription;

    private void Start()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        ServerData.userInfoTable.TableDatas[UserInfoTable.partyTowerRecommend].AsObservable().Subscribe(e =>
        {
            helpDescription.SetText($"���� ���� ��û�� : {e}��");
        }).AddTo(this);
    }

    void OnEnable()
    {
        SetStageText();
        SetReward();
    }

    private bool IsAllClear()
    {
        int currentFloor = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.partyTowerFloor).Value;

        return currentFloor >= TableManager.Instance.towerTable4.dataArray.Length;
    }

    private void SetStageText()
    {
        if (IsAllClear() == false)
        {
            int currentFloor = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.partyTowerFloor).Value;
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
            int currentFloor = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.partyTowerFloor).Value;

            if (currentFloor >= TableManager.Instance.towerTable4.dataArray.Length)
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"�߸��� ������ idx : {currentFloor}", null);
                return;
            }

            var towerTableData = TableManager.Instance.towerTable4.dataArray[currentFloor];

            uiTower4RewardView.UpdateRewardView(towerTableData.Id);
        }


    }

    public void OnClickEnterButton()
    {
        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, "���� �ұ��?", () =>
        {

            GameManager.Instance.LoadContents(GameManager.ContentsType.DokebiTower);

        }, () => { });
    }
}
