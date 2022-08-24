using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private GameObject StuffItems, EquipItems, StuffItemListUI, EquipItemListUI, StuffUI, EquipUI;
    List<Item> StuffList; // 가지고 있는 잡화 리스트
    List<Item> EquipList; // 가지고 있는 장비 리스트
    public bool isStuff; // 현재 잡화 아이템이 띄워져있는가
    bool isRest;
    
    public GameManager.ResultAction GetActiveInventoryPannel(){ return new GameManager.ResultAction(ActiveInventoryPannel); } // ActiveInventoryPannel 반환
    
    void ActiveInventoryPannel(string isRestS)
    {
        StuffList = new List<Item>(); EquipList = new List<Item>();
        foreach(Item it in GameManager.GM.HoldItemList)
        {
            if((int)it.ThisItemFormat <= 1) StuffList.Add(it);
            else EquipList.Add(it);
        }
        transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "장비"; //  EquipStuffB Text
        transform.GetChild(3).gameObject.SetActive(false); // SmallAlertUI
        transform.GetChild(4).GetComponent<Text>().text = "잡화 아이템"; // InventoryText
        InvenInfoTextUpdate();
        gameObject.SetActive(true); // InventoryUI활성화

        for(int i = 0; StuffItems.transform.childCount > i; i++)
        {
            Destroy(StuffItems.transform.GetChild(i).gameObject);
        }
        for(int i = 0; EquipItems.transform.childCount > i; i++)
        {
            Destroy(EquipItems.transform.GetChild(i).gameObject);
        }
        for(int i = 0; StuffList.Count > i; i++) // 잡화 아이템 개수 만큼 UI업데이트
        {
            GameObject ItemPannel = Instantiate(StuffUI);
            ItemPannel.transform.SetParent(StuffItems.transform, false);
            ItemPannel.GetComponent<ItemInUI>().StuffDataUpdate(StuffList[i]);
        }
        StuffItemListUI.SetActive(false);
        for(int i = 0; EquipList.Count > i; i++) // 장비 아이템 개수 만큼 UI업데이트
        {
            GameObject ItemPannel = Instantiate(EquipUI);
            ItemPannel.transform.SetParent(EquipItems.transform, false);
            ItemPannel.GetComponent<ItemInUI>().EquipDataUpdate(EquipList[i]);
        }
        EquipItemListUI.SetActive(false);
        isStuff = false; EquipStuffChange(); // 처음에는 잡화 아이템들이 나오도록 함
    }
    
    void EquipStuffChange() // 장비와 잡화를 바꾸는 함수
    {
        isStuff = !isStuff;
        if(isStuff) // 잡화냐 장비냐 에 따라 버튼 텍스트와 아이템 리스트 갱신
        {
            StuffItemListUI.SetActive(true);
            EquipItemListUI.SetActive(false);
        }
        else
        {
            EquipItemListUI.SetActive(true);
            StuffItemListUI.SetActive(false);
        }
    }

    public void EquipStuffBClick() // 장비버튼 클릭시 장비아이템으로, 잡화버튼 클릭시 잡화아이템으로 갱신
    {
        string str = isStuff ? "장비" : "잡화";
        transform.GetChild(0).GetChild(0).GetComponent<Text>().text = isStuff ? "잡화" : "장비"; //  EquipStuffB Text
        transform.GetChild(4).GetComponent<Text>().text = str + " 아이템"; // InventoryText
        EquipStuffChange();
    }

    public void ExitButtonClick() // 나가기 버튼 클릭시 인벤토리창 비활성화
    {
        // 인벤토리 종료 이벤트 실행 GameManager.GM.EventInvokeByName(인벤토리 종료 이벤트);
        gameObject.SetActive(false);
    }

    public void InvenInfoTextUpdate()
    {
        transform.GetChild(6).GetComponent<Text>().text = "인벤토리:" + GameManager.GM.Weight.ToString() + "/" + GameManager.GM.MaxWeight.ToString();
    }
}
