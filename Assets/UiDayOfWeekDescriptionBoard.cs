using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UiDayOfWeekDescriptionBoard : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> rewardCountTxt;

    private void OnEnable()
    {
        Initialize();
    }
    public void Initialize()
    {
        var tableData = TableManager.Instance.dayOfWeekDungeon.dataArray;


        List<float> multipleValue = new List<float>() { 1, 1, 1, 1, 1, 1, 1 };
        
        //i�� ����
        for (int i = 0; i < rewardCountTxt.Count; i++)
        {
            //j�� ������ j��° ����
            for(int j  = 0; j < tableData[i].Score.Length; j++)
            {
                if (tableData[i].Score[j] <= ServerData.userInfoTable.GetTableData(UserInfoTable.topClearStageId).Value)
                {
                    multipleValue[i] = tableData[i].Rewardvalue[j];
                }
                //����
                else
                {
                    break;
                }
            }
            if (ServerData.userInfoTable.GetTableData(UserInfoTable.DayOfWeekClear).Value == 0)
            {
                rewardCountTxt[i].SetText($"���� �̵��");
            }
            else
            {
                rewardCountTxt[i].SetText($"{multipleValue[i] * ServerData.userInfoTable.GetTableData(UserInfoTable.DayOfWeekClear).Value}"); 
            }
        }

    }

}
