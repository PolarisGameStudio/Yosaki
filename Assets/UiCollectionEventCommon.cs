using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UiCollectionEventCommon : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI eventDescription;

    [SerializeField]
    private string eventGoodsName;

    private void Start()
    {
        SetDescriptionText();
    }

    private void SetDescriptionText()
    {
        string description = string.Empty;

        description = "-";
        //description += $"<color=red>���� ȹ�� 3�� 31�ϱ���</color>\n";
        //description += $"<color=red>������ ���� 3�� 31�ϱ���</color>\n";
        //description += $"<color=red>��ǰ �Ǹ� 3�� 31�ϱ���</color>";

        eventDescription.SetText(description);
    }

#if UNITY_EDITOR
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ServerData.goodsTable.GetTableData(eventGoodsName).Value += 1000000;
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ServerData.goodsTable.GetTableData(eventGoodsName).Value += 10;
        }
    }
#endif
}
