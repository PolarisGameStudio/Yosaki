using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BackEnd;
public class OldDokebiRewardBoard : MonoBehaviour
{
    [SerializeField]
    private UiOldDokebi2RewardCell cellPrefab;

    [SerializeField]
    private Transform cellParent;

    [SerializeField]
    private TextMeshProUGUI lastClearStageDesc;

    private List<UiOldDokebi2RewardCell> cellLists = new List<UiOldDokebi2RewardCell>();

    void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        var tableDatas = TableManager.Instance.OldDokebiTable.dataArray;

        for (int i = 0; i < tableDatas.Length; i++)
        {
            if (tableDatas[i].Rewardtype == -1) continue;

            var cell = Instantiate<UiOldDokebi2RewardCell>(cellPrefab, cellParent);

            cell.Initialize(tableDatas[i]);

            cellLists.Add(cell);
        }

        lastClearStageDesc.SetText($"�ְ� �ܰ� : {(int)(ServerData.userInfoTable.TableDatas[UserInfoTable.oldDokebi2LastClear].Value)}");
    }

    public void OnClickAllReceiveButton()
    {

        int lastClearStageId = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.oldDokebi2LastClear].Value;

        var tableDatas = TableManager.Instance.OldDokebiTable.dataArray;

        var GetOldDokebi2RewardedList = ServerData.etcServerTable.GetOldDokebi2RewardedList();

        int rewardReceiveCount = 0;

        string addStringValue = string.Empty;


        for (int i = 0; i < tableDatas.Length; i++)
        {
            if (tableDatas[i].Rewardtype == -1) continue;

            if (lastClearStageId < tableDatas[i].Stage) break;

            if (GetOldDokebi2RewardedList.Contains(tableDatas[i].Stage) == true) continue;

            ServerData.AddLocalValue((Item_Type)tableDatas[i].Rewardtype, tableDatas[i].Rewardvalue);

            addStringValue += $"{BossServerTable.rewardSplit}{tableDatas[i].Stage}";

            rewardReceiveCount++;
        }


        if (rewardReceiveCount > 0)
        {
            ServerData.etcServerTable.TableDatas[EtcServerTable.oldDokebi2Reward].Value += addStringValue;

            List<TransactionValue> transactions = new List<TransactionValue>();

            Param rewardParam = new Param();

            rewardParam.Add(EtcServerTable.oldDokebi2Reward, ServerData.etcServerTable.TableDatas[EtcServerTable.oldDokebi2Reward].Value);

            transactions.Add(TransactionValue.SetUpdate(EtcServerTable.tableName, EtcServerTable.Indate, rewardParam));

            Param goodsParam = new Param();

            goodsParam.Add(GoodsTable.DokebiBundle, ServerData.goodsTable.GetTableData(GoodsTable.DokebiBundle).Value);
            

            transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));

            ServerData.SendTransaction(transactions, successCallBack: () =>
            {
                PopupManager.Instance.ShowAlarmMessage("������ �޾ҽ��ϴ�!");
                SoundManager.Instance.PlaySound("Reward");
            });
        }
        else
        {
            PopupManager.Instance.ShowAlarmMessage("���� �� �ִ� ������ �����ϴ�.");
        }
    }
}
