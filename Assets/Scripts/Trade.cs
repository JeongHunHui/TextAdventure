using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trade : MonoBehaviour
{
    [SerializeField]
    private GameObject SellItemsUI, BuyItemsUI, TradeItemUI, SellItemListUI, BuyItemListUI;
    public string TradeName; // 상인or상점 이름
    public List<TradeItem> SellItemList; // 상인이 판매중인 아이템 리스트
    public List<TradeItem> BuyItemList; // 상인이 구매하는 아이템 리스트
    public bool isSell; // 현재 판매중인 아이템이 띄워져있는가
    // public string TraderFormat;
    // public string TraderLevel;
    
    public GameManager.ResultAction GetActiveTradePannel(){ return new GameManager.ResultAction(ActiveTradePannel); } // ActiveTradePannel 반환
    
    void ActiveTradePannel(string tradeName) // 거래 창을 띄우는 함수
    {
        // TraderName|BuyItemListName|SellItemListName
        string TradeData = GameManager.GM.TradeDataDic[tradeName]; // 거래 데이터를 검색
        string [] values = TradeData.Split('|'); // |를 기준으로 나누고 각 데이터를 변수에 입력
        TradeName = values[0]; // 이름
        SellItemList = GameManager.GM.TradeItemListDic[values[1]]; // 판매 아이템 리스트
        BuyItemList = GameManager.GM.TradeItemListDic[values[2]]; // 구매 아이템 리스트

        transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "판매하기"; // BuySellB Text
        transform.GetChild(3).gameObject.SetActive(false); // SmallAlertUI
        transform.GetChild(4).GetComponent<Text>().text = TradeName; // TradeName
        InvenInfoTextUpdate();
        gameObject.SetActive(true);

        for(int i = 0; SellItemsUI.transform.childCount > i; i++)
        {
            Destroy(SellItemsUI.transform.GetChild(i).gameObject);
        }
        for(int i = 0; BuyItemsUI.transform.childCount > i; i++)
        {
            Destroy(BuyItemsUI.transform.GetChild(i).gameObject);
        }
        for(int i = 0; SellItemList.Count > i; i++) // 판매중인 아이템 개수 만큼 UI업데이트
        {
            GameObject ItemPannel = Instantiate(TradeItemUI);
            ItemPannel.transform.SetParent(SellItemsUI.transform, false);
            ItemPannel.GetComponent<TradeItem>().TradeItemUpdate(SellItemList[i], true); // 판매중인 아이템 데이터 갱신
            ItemPannel.GetComponent<TradeItem>().ItemActive("구매"); // 아이템 UI갱신
        }
        SellItemListUI.SetActive(false);
        for(int i = 0; BuyItemList.Count > i; i++) // 상인이 구매하는 아이템 개수 만큼 UI업데이트
        {
            GameObject ItemPannel = Instantiate(TradeItemUI);
            ItemPannel.transform.SetParent(BuyItemsUI.transform, false);
            ItemPannel.GetComponent<TradeItem>().TradeItemUpdate(BuyItemList[i], false); // 상인이 구매하는 아이템 데이터 갱신
            ItemPannel.GetComponent<TradeItem>().ItemActive("판매"); // 아이템 UI갱신
        }
        BuyItemListUI.SetActive(false);
        isSell = false; BuySellChange(); // 처음에는 판매하는 아이템들이 나오도록 함
    }

    void BuySellChange() // 구매와 판매를 바꾸는 함수
    {
        isSell = !isSell;
        if(isSell) // 판매냐 구매냐에 따라 버튼 텍스트와 아이템 리스트 갱신
        {
            SellItemListUI.SetActive(true);
            BuyItemListUI.SetActive(false);
        }
        else
        {
            BuyItemListUI.SetActive(true);
            SellItemListUI.SetActive(false);
        }
    }

    public void BuySellBClick() // 구매/판매 버튼 클릭시 구매버튼이면 물품이 구매로 갱신, 판매버튼이면 판매로 갱신
    {
        transform.Find("BuySellB").GetChild(0).GetComponent<Text>().text = isSell ? "구매하기" : "판매하기";
        BuySellChange();
    }

    public void ExitButtonClick() // 나가기 버튼 클릭시 거래 종료 이벤트와 거래창 비활성화
    {
        GameManager.GM.EventInvokeByName("PaddlerTradeEnd");
        gameObject.SetActive(false);
    }

    public void InvenInfoTextUpdate()
    {
        transform.GetChild(6).GetComponent<Text>().text = "인벤토리:" + GameManager.GM.Weight.ToString() + "/" + GameManager.GM.MaxWeight.ToString();
    }
}