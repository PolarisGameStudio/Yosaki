using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiHellPowerGoodsView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI description;

    private void Start()
    {
        description.SetText($"1���� �����Ҳ� �������� ���ط� ���");
    }
}
