using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selection : MonoBehaviour
{
    public string SelName; // 선택지 이름, 딕셔너리의 키값으로 사용
    public string SelectionText; // 선택지에 나올 텍스트
    public string SelActionText;
    public string IsSelActiveText;

    public void SelectionUpdate(Selection selection)
    {
        SelectionText = selection.SelectionText;
        SelName = selection.SelName;
        SelActionText = selection.SelActionText;
        transform.GetChild(0).GetComponent<Text>().text = SelectionText;
        IsSelActiveText = selection.IsSelActiveText;
    }

    public void OnClick()
    {
        GameManager.GM.ActionAnalyze(SelActionText);
    }
}
