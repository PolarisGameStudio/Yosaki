using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;
using BackEnd;
using UnityEngine.UI;

public class UiSumisanFireBoard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI dokebiLevelText;

    [SerializeField]
    private TextMeshProUGUI dokebiAbilText1;


    public Button registerButton;


    public TextMeshProUGUI getButtonDesc;

    public TMP_InputField inputField;

    private void Start()
    {
        Initialize();
        Subscribe();
        SetFlowerReward();


    }

    //��� ����
    private void SetFlowerReward()
    {
        //chunFlowerReward.Initialize(TableManager.Instance.TwelveBossTable.dataArray[65]);
    }
    private void OnEnable()
    {
        UpdateAbilText1((int)ServerData.goodsTable.GetTableData(GoodsTable.SumiFire).Value);

        if (inputField != null)
        {
            inputField.text = $"���� Ƚ�� �Է�";
        }
    }
    private void Subscribe()
    {
        ServerData.goodsTable.GetTableData(GoodsTable.SumiFire).AsObservable().Subscribe(level =>
        {
            dokebiLevelText.SetText($"LV : {level}");
            UpdateAbilText1((int)level);

        }).AddTo(this);

        ServerData.userInfoTable.TableDatas[UserInfoTable.getSumiFire].AsObservable().Subscribe(e =>
        {
            registerButton.interactable = e == 0;

            getButtonDesc.SetText(e == 0 ? "ȹ��" : "���� ȹ����");
        }).AddTo(this);
    }

    private void UpdateAbilText1(int currentLevel)
    {
        var tableData = TableManager.Instance.sumiAbilBase.dataArray;

        string abilDesc = string.Empty;

        for (int i = 0; i < tableData.Length; i++)
        {
            StatusType type = (StatusType)tableData[i].Abiltype;

            if (type == StatusType.AttackAddPer)
            {
                abilDesc += $"{CommonString.GetStatusName(type)} {Utils.ConvertBigNum(PlayerStats.GetSumiFireAbilHasEffect(type))}\n";
            }
            else
            {
                abilDesc += $"{CommonString.GetStatusName(type)} {PlayerStats.GetSumiFireAbilHasEffect(type) * 100f}\n";
            }
        }

        abilDesc.Remove(abilDesc.Length - 2, 2);

        dokebiAbilText1.SetText(abilDesc);
    }

    private void Initialize()
    {
        scoreText.SetText($"�ְ� ���� : {Utils.ConvertBigNum(ServerData.userInfoTable.TableDatas[UserInfoTable.sumiFireClear].Value)}");

    }

    public void OnClickDokebiEnterButton()
    {
        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, "���� �Ͻðڽ��ϱ�?", () =>
        {
            GameManager.Instance.LoadContents(GameManager.ContentsType.SumiFire);
        }, () => { });
    }


#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ServerData.goodsTable.GetTableData(GoodsTable.SumiFire).Value += 2000;
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ServerData.goodsTable.GetTableData(GoodsTable.SumiFireKey).Value += 1;
        }
    }
#endif

    public void OnClickGetDokebiFireButton()
    {
        if (ServerData.userInfoTable.GetTableData(UserInfoTable.getSumiFire).Value == 1)
        {
            PopupManager.Instance.ShowAlarmMessage($"{CommonString.GetItemName(Item_Type.SumiFire)}�� �Ϸ翡 �ѹ��� ȹ�� �����մϴ�!");
            return;
        }

        int score = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.sumiFireClear].Value;

        if (score == 0)
        {
            PopupManager.Instance.ShowAlarmMessage("������ ��ϵ��� �ʾҽ��ϴ�.");
            return;
        }

        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{score}�� ȹ�� �մϱ�?\n{CommonString.GetItemName(Item_Type.DokebiTreasure)}�� �߰�ȹ�� : {Utils.GetDokebiTreasureAddValue()}", () =>
        {
            ServerData.userInfoTable.GetTableData(UserInfoTable.getSumiFire).Value = 1;
            ServerData.goodsTable.GetTableData(GoodsTable.SumiFire).Value += score + Utils.GetDokebiTreasureAddValue();

            List<TransactionValue> transactions = new List<TransactionValue>();

            Param userInfoParam = new Param();
            userInfoParam.Add(UserInfoTable.getSumiFire, ServerData.userInfoTable.TableDatas[UserInfoTable.getSumiFire].Value);

            Param goodsParam = new Param();
            goodsParam.Add(GoodsTable.SumiFire, ServerData.goodsTable.GetTableData(GoodsTable.SumiFire).Value);

            transactions.Add(TransactionValue.SetUpdate(UserInfoTable.tableName, UserInfoTable.Indate, userInfoParam));
            transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));
            
            EventMissionManager.UpdateEventMissionClear(EventMissionKey.ClearSumiFire, 1);

            ServerData.SendTransaction(transactions, successCallBack: () =>
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{CommonString.GetItemName(Item_Type.SumiFire)} {score + Utils.GetDokebiTreasureAddValue()}�� ȹ��!", null);
            });
        }, null);
    }

    public void OnClickGetAllDokebiFireButton()
    {
        if (ServerData.goodsTable.GetTableData(GoodsTable.SumiFireKey).Value < 1)
        {
            PopupManager.Instance.ShowAlarmMessage($"{CommonString.GetItemName(Item_Type.SumiFireKey)}�� �����մϴ�!");
            return;
        }

        int score = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.sumiFireClear].Value + (int)Utils.GetDokebiTreasureAddValue();

        if (score == 0)
        {
            PopupManager.Instance.ShowAlarmMessage("������ ��ϵ��� �ʾҽ��ϴ�.");
            return;
        }

        PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{score * ServerData.goodsTable.GetTableData(GoodsTable.SumiFireKey).Value}�� ȹ�� �մϱ�?\n<color=red>({score} * {ServerData.goodsTable.GetTableData(GoodsTable.SumiFireKey).Value} ȹ�� ����)</color>", () =>
        {
            int clearCount = (int)ServerData.goodsTable.GetTableData(GoodsTable.SumiFireKey).Value;
            ServerData.goodsTable.GetTableData(GoodsTable.SumiFire).Value += score * clearCount;
            ServerData.goodsTable.GetTableData(GoodsTable.SumiFireKey).Value -= clearCount;

            List<TransactionValue> transactions = new List<TransactionValue>();


            Param goodsParam = new Param();
            goodsParam.Add(GoodsTable.SumiFire, ServerData.goodsTable.GetTableData(GoodsTable.SumiFire).Value);
            goodsParam.Add(GoodsTable.SumiFireKey, ServerData.goodsTable.GetTableData(GoodsTable.SumiFireKey).Value);

            transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));

            ServerData.SendTransaction(transactions, successCallBack: () =>
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{CommonString.GetItemName(Item_Type.SumiFire)} {score * clearCount}�� ȹ��!", null);
            });
        }, null);
    }
    public void OnClickGetManyDokebiFireButton()
    {

        if (!string.IsNullOrEmpty(inputField.text))
        {
            if (int.TryParse(inputField.text, out int result))
            {
                if (result < 1)
                {
                    PopupManager.Instance.ShowAlarmMessage("�ùٸ� ������ �ƴմϴ�.");
                    return;
                }
                if (ServerData.goodsTable.GetTableData(GoodsTable.SumiFireKey).Value < result)
                {
                    PopupManager.Instance.ShowAlarmMessage($"{CommonString.GetItemName(Item_Type.SumiFireKey)}�� �����մϴ�!");
                    return;
                }

                int score = (int)ServerData.userInfoTable.TableDatas[UserInfoTable.sumiFireClear].Value;

                if (score == 0)
                {
                    PopupManager.Instance.ShowAlarmMessage("������ ��ϵ��� �ʾҽ��ϴ�.");
                    return;
                }


                PopupManager.Instance.ShowYesNoPopup(CommonString.Notice, $"{score * result}�� ȹ�� �մϱ�?\n<color=red>({score} x {result} ȹ�� ����)</color>", () =>
                {
                    if (result < 1)
                    {
                        PopupManager.Instance.ShowAlarmMessage("�ùٸ� ������ �ƴմϴ�.");
                        return;
                    }
                    if (ServerData.goodsTable.GetTableData(GoodsTable.SumiFireKey).Value < result)
                    {
                        PopupManager.Instance.ShowAlarmMessage($"{CommonString.GetItemName(Item_Type.SumiFireKey)}�� �����մϴ�!");
                        return;
                    }


                    ServerData.goodsTable.GetTableData(GoodsTable.SumiFire).Value += score * result;
                    ServerData.goodsTable.GetTableData(GoodsTable.SumiFireKey).Value -= result;

                    List<TransactionValue> transactions = new List<TransactionValue>();


                    Param goodsParam = new Param();
                    goodsParam.Add(GoodsTable.SumiFire, ServerData.goodsTable.GetTableData(GoodsTable.SumiFire).Value);
                    goodsParam.Add(GoodsTable.SumiFireKey, ServerData.goodsTable.GetTableData(GoodsTable.SumiFireKey).Value);

                    transactions.Add(TransactionValue.SetUpdate(GoodsTable.tableName, GoodsTable.Indate, goodsParam));

                    ServerData.SendTransaction(transactions, successCallBack: () =>
                    {
                        PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, $"{CommonString.GetItemName(Item_Type.SumiFire)} {score * result}�� ȹ��!", null);
                    });
                }, null);
            }
            else
            {
                PopupManager.Instance.ShowAlarmMessage("���� Ƚ���� �Է����ּ���.");
            }
        }


    }
}
