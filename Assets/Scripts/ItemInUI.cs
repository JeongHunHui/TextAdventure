using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInUI : MonoBehaviour
{
    bool IsStuff; // 지금 잡화 아이템을 보는 중인가?
    private Item ThisItem; // 이 UI가 보여주는 아이템

    public void StuffDataUpdate(Item itemData) // 잡화 아이템 정보 전달 및 UI업데이트
    {
        Destroy(ThisItem);
        ThisItem = gameObject.AddComponent<Item>();
        ThisItem.ItemUpdate(itemData);
        IsStuff = true;

        transform.GetChild(0).GetComponent<Text>().text = "[" + ThisItem.ThisItemFormat + "]" + ThisItem.ItemName;
        transform.GetChild(1).GetComponent<Text>().text = "x" + ThisItem.ItemCount;
        transform.GetChild(2).GetChild(0).GetComponent<Text>().text = ThisItem.ItemExplain;
        transform.GetChild(3).GetComponent<Text>().text = "무게:" + ThisItem.ItemWeight.ToString();
    }

    public void EquipDataUpdate(Item itemData) // 장비 아이템 정보 전달 및 UI업데이트
    {
        Destroy(ThisItem);
        ThisItem = gameObject.AddComponent<Item>();
        ThisItem.ItemUpdate(itemData);
        IsStuff = false;

        transform.GetChild(0).GetComponent<Text>().text = "[" + ThisItem.ThisItemFormat + "]" + ThisItem.ItemName;
        transform.GetChild(1).GetComponent<Text>().text = "무게:" + ThisItem.ItemWeight.ToString();
        transform.GetChild(2).GetComponent<Text>().text = ThisItem.IsWearing ? "착용중" : "미착용";
        transform.GetChild(3).GetChild(0).GetComponent<Text>().text = ThisItem.ItemExplain;
        transform.parent.parent.parent.parent.GetComponent<Inventory>().InvenInfoTextUpdate();
    }

    public void OnClick()
    {
        if(IsStuff)
        {
            //if(ThisItem.ThisItemFormat == 1) Yes No 선택창 띄우기
        }
        else
        {
            GameManager.GM.AllResultActionDic["SmallPannelInInventoryActive"](ThisItem.WearingEquip()); // 알림 메세지 띄우기
            EquipDataUpdate(ThisItem);
        }
    }
}
