using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using BackEnd;
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

    [SerializeField]
    private TextMeshProUGUI adTicketDescription;

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
        ServerData.userInfoTable.TableDatas[UserInfoTable.receivedPartyTowerTicket].AsObservable().Subscribe(e =>
        {
            adTicketDescription.SetText(e == 0 ? $"��û�� ȹ��\n(1�� 1ȸ)" : $"ȹ�� �Ϸ�");
        }).AddTo(this);

    }

    void OnEnable()
    {
        SetStageText();
        SetReward();
    }
    public void OnClickAdButton()
    {
        bool received = ServerData.userInfoTable.GetTableData(UserInfoTable.receivedPartyTowerTicket).Value == 1;
        if (received)
        {
            PopupManager.Instance.ShowAlarmMessage("�����Ͽ� �� �� ȹ�� �����մϴ�.");
            return;
        }

        AdManager.Instance.ShowRewardedReward(RewardAdFinished);
    }

    private void RewardAdFinished()
    {

        ServerData.userInfoTable.TableDatas[UserInfoTable.partyTowerRecommend].Value++;
        ServerData.userInfoTable.GetTableData(UserInfoTable.receivedPartyTowerTicket).Value = 1f;

        List<TransactionValue> transactionList = new List<TransactionValue>();
     
        Param userInfoParam = new Param();
        userInfoParam.Add(UserInfoTable.receivedPartyTowerTicket, ServerData.userInfoTable.GetTableData(UserInfoTable.receivedPartyTowerTicket).Value);
        userInfoParam.Add(UserInfoTable.partyTowerRecommend, ServerData.userInfoTable.GetTableData(UserInfoTable.partyTowerRecommend).Value);
        transactionList.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));

        ServerData.SendTransaction(transactionList);
    }

    private bool IsAllClear()
    {
        int currentFloor = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.partyTowerFloor).Value;

        return currentFloor >= TableManager.Instance.towerTableMulti.dataArray.Length;
    }

    private void SetStageText()
    {
        if (IsAllClear() == false)
        {
            int currentFloor = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.partyTowerFloor).Value;
            currentStageText.SetText($"{currentFloor + 1}��");
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

            if (currentFloor >= TableManager.Instance.towerTableMulti.dataArray.Length)
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"�߸��� ������ idx : {currentFloor}", null);
                return;
            }

            var towerTableData = TableManager.Instance.towerTableMulti.dataArray[currentFloor];

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
