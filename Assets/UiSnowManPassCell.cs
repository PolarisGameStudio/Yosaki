using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using BackEnd;
using TMPro;
using System;

public class UiSnowManPassCell : MonoBehaviour
{
    [SerializeField]
    private Image itemIcon_free;

    [SerializeField]
    private TextMeshProUGUI itemName_free;

    [SerializeField]
    private Image itemIcon_ad;

    [SerializeField]
    private TextMeshProUGUI itemName_ad;

    [SerializeField]
    private TextMeshProUGUI itemAmount_free;

    [SerializeField]
    private TextMeshProUGUI itemAmount_ad;

    [SerializeField]
    private GameObject lockIcon_Free;

    [SerializeField]
    private GameObject lockIcon_Ad;

    private PassInfo passInfo;

    [SerializeField]
    private GameObject rewardedObject_Free;

    [SerializeField]
    private GameObject rewardedObject_Ad;

    [SerializeField]
    private GameObject gaugeImage;

    [SerializeField]
    private TextMeshProUGUI descriptionText;

    private CompositeDisposable disposables = new CompositeDisposable();

    private void OnDestroy()
    {
        disposables.Dispose();
    }
    private void Subscribe()
    {
        disposables.Clear();

        //���Ẹ�� ������ �����
        ServerData.oneYearPassServerTable.TableDatas[passInfo.rewardType_Free_Key].Subscribe(e =>
        {
            bool rewarded = HasReward(passInfo.rewardType_Free_Key, passInfo.id);
            rewardedObject_Free.SetActive(rewarded);

        }).AddTo(disposables);

        //������ ������ �����
        ServerData.oneYearPassServerTable.TableDatas[passInfo.rewardType_IAP_Key].Subscribe(e =>
        {
            bool rewarded = HasReward(passInfo.rewardType_IAP_Key, passInfo.id);
            rewardedObject_Ad.SetActive(rewarded);

        }).AddTo(disposables);

        //ųī��Ʈ ����ɶ�
        ServerData.userInfoTable.GetTableData(UserInfoTable.usedSnowManCollectionCount).AsObservable().Subscribe(e =>
        {

        }).AddTo(disposables);
    }



    public void Initialize(PassInfo passInfo)
    {
        this.passInfo = passInfo;

        SetAmount();

        SetItemIcon();

        SetDescriptionText();

        Subscribe();

        RefreshParent();

        RefreshData();
    }

    private void SetAmount()
    {
        itemAmount_free.SetText(Utils.ConvertBigNum(passInfo.rewardTypeValue_Free));
        itemAmount_ad.SetText(Utils.ConvertBigNum(passInfo.rewardTypeValue_IAP));
    }

    private void SetItemIcon()
    {
        itemIcon_free.sprite = CommonUiContainer.Instance.GetItemIcon((Item_Type)(int)passInfo.rewardType_Free);
        itemIcon_ad.sprite = CommonUiContainer.Instance.GetItemIcon((Item_Type)(int)passInfo.rewardType_IAP);

        itemName_free.SetText(CommonString.GetItemName((Item_Type)(int)passInfo.rewardType_Free));
        itemName_ad.SetText(CommonString.GetItemName((Item_Type)(int)passInfo.rewardType_IAP));
    }

    private void SetDescriptionText()
    {
        descriptionText.SetText($"{Utils.ConvertBigNum(passInfo.require)}");
    }

    public List<string> GetSplitData(string key)
    {
        return ServerData.oneYearPassServerTable.TableDatas[key].Value.Split(',').ToList();
    }

    public bool HasReward(string key, int data)
    {
        var splitData = GetSplitData(key);
        return splitData.Contains(data.ToString());
    }

    public void OnClickFreeRewardButton()
    {
        if (CanGetReward() == false)
        {
            PopupManager.Instance.ShowAlarmMessage("�⼮�� �����մϴ�.");
            return;
        }

        if (HasReward(passInfo.rewardType_Free_Key, passInfo.id))
        {
            PopupManager.Instance.ShowAlarmMessage("�̹� ������ �޾ҽ��ϴ�!");
            return;
        }

        PopupManager.Instance.ShowAlarmMessage("������ �����߽��ϴ�!");

        GetFreeReward();

    }

    //����ƴ�
    public void OnClickAdRewardButton()
    {
        if (CanGetReward() == false)
        {
            PopupManager.Instance.ShowAlarmMessage("�⼮�� �����մϴ�.");
            return;
        }

        if (HasReward(passInfo.rewardType_IAP_Key, passInfo.id))
        {
            PopupManager.Instance.ShowAlarmMessage("�̹� ������ �޾ҽ��ϴ�!");
            return;
        }


        if (HasPassItem())
        {
            GetAdReward();
        }
        else
        {
            PopupManager.Instance.ShowAlarmMessage($"����� �н����� �ʿ��մϴ�.");
            return;
        }
        PopupManager.Instance.ShowAlarmMessage("������ �����߽��ϴ�!");
    }

    static public bool HasPassItem()
    {
        bool hasIapProduct = ServerData.iapServerTable.TableDatas[UiSnowManEventBuyButton.fallPassKey].buyCount.Value > 0;

        return hasIapProduct;
    }
    private void GetFreeReward()
    {
        //����
        ServerData.oneYearPassServerTable.TableDatas[passInfo.rewardType_Free_Key].Value += $",{passInfo.id}";
        ServerData.AddLocalValue((Item_Type)(int)passInfo.rewardType_Free, passInfo.rewardTypeValue_Free);

        List<TransactionValue> transactionList = new List<TransactionValue>();

        //�н� ����
        Param passParam = new Param();
        passParam.Add(passInfo.rewardType_Free_Key, ServerData.oneYearPassServerTable.TableDatas[passInfo.rewardType_Free_Key].Value);
        transactionList.Add(TransactionValue.SetUpdate(OneYearPassServerTable.tableName, OneYearPassServerTable.Indate, passParam));

        var rewardTransactionValue = ServerData.GetItemTypeTransactionValue((Item_Type)(int)passInfo.rewardType_Free);
        transactionList.Add(rewardTransactionValue);

        //ųī��Ʈ
        Param userInfoParam = new Param();
        //userInfoParam.Add(UserInfoTable.attenCountOne, ServerData.userInfoTable.GetTableData(UserInfoTable.attenCountOne).Value);
        transactionList.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));

        ServerData.SendTransaction(transactionList, successCallBack: () =>
        {
            //  LogManager.Instance.SendLogType("����", "����", $"{passInfo.id}");
        });
    }
    private void GetAdReward()
    {
        //����
        ServerData.oneYearPassServerTable.TableDatas[passInfo.rewardType_IAP_Key].Value += $",{passInfo.id}";
        ServerData.AddLocalValue((Item_Type)(int)passInfo.rewardType_IAP, passInfo.rewardTypeValue_IAP);

        List<TransactionValue> transactionList = new List<TransactionValue>();

        //�н� ����
        Param passParam = new Param();
        passParam.Add(passInfo.rewardType_IAP_Key, ServerData.oneYearPassServerTable.TableDatas[passInfo.rewardType_IAP_Key].Value);
        transactionList.Add(TransactionValue.SetUpdate(OneYearPassServerTable.tableName, OneYearPassServerTable.Indate, passParam));

        var rewardTransactionValue = ServerData.GetItemTypeTransactionValue((Item_Type)(int)passInfo.rewardType_IAP);
        transactionList.Add(rewardTransactionValue);

        //ųī��Ʈ
        Param userInfoParam = new Param();
        //userInfoParam.Add(UserInfoTable.attenCountOne, ServerData.userInfoTable.GetTableData(UserInfoTable.attenCountOne).Value);
        transactionList.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));

        ServerData.SendTransaction(transactionList, successCallBack: () =>
        {
            //   LogManager.Instance.SendLogType("����", "����", $"{passInfo.id}");
        });

        PopupManager.Instance.ShowAlarmMessage("������ �����߽��ϴ�!");
    }

    private bool CanGetReward()
    {
        int killCountTotalBok = (int)ServerData.userInfoTable.GetTableData(UserInfoTable.usedSnowManCollectionCount).Value;
        return killCountTotalBok >= passInfo.require;
    }
    private void OnEnable()
    {
        RefreshParent();
        RefreshData();

    }

    private void RefreshData()
    {
        if (passInfo == null) return;
        lockIcon_Free.SetActive(!CanGetReward());
        lockIcon_Ad.SetActive(!CanGetReward());
        gaugeImage.SetActive(CanGetReward());
    }

    public void RefreshParent()
    {
        if (passInfo == null) return;

        if (CanGetReward() == true && HasReward(passInfo.rewardType_Free_Key, passInfo.id) == false)
        {
            this.transform.SetAsFirstSibling();
        }
    }

}
