using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeItem : MonoBehaviour
{
    int TradeCount; // 거래 가능 횟수
    bool IsSell; // 지금 상인이 판매를 하고있는가
    private Item ThisItem; // 거래할 아이템

    public void TradeItemUpdate(Item thisItem, int tradeCount) // 처음에 텍스트 파일을 읽어올 때 사용
    {
        TradeCount = tradeCount;
        ThisItem = thisItem;
    }
    public void TradeItemUpdate(TradeItem tradeItem, bool isSell) // Trade에서 TradeItem을 갱신할 때 사용
    {
        ThisItem = tradeItem.ThisItem;
        IsSell = isSell;
        TradeCount = tradeItem.TradeCount;
    }

    public void ItemActive(string buySell) // 각각의 아이템 UI를 구매냐 판매냐 에 따라 활성화
    {   // 아이템 이름, 아이템 설명, 아이템 구매/판매 버튼 텍스트들 갱신
        transform.GetChild(0).GetComponent<Text>().text = "[" + ThisItem.ThisItemFormat + "]" + ThisItem.ItemName;
        transform.GetChild(3).GetChild(0).GetComponent<Text>().text = ThisItem.ItemExplain;
        transform.GetChild(4).GetChild(0).GetComponent<Text>().text = buySell;
        transform.GetChild(5).GetComponent<Text>().text = "무게:" + ThisItem.ItemWeight.ToString();
        if(IsSell) transform.GetChild(2).GetComponent<Text>().text = $"남은 수량: {TradeCount}"; // 상인이 판매 중이면 남은 수량 표시
        else transform.GetChild(2).GetComponent<Text>().text = "보유 수량:" + GameManager.GM.GetItemCount(ThisItem.ItemName).ToString();
        // 상인이 구매 중이면 보유 수량 표시
        transform.GetChild(1).GetComponent<Text>().text = buySell + " 가격:"+ThisItem.ItemCost; // 판매,구매 가격 표시
    }

    public void TradeBClick() // 구매 버튼이나 판매 버튼을 눌렀을 때
    {
        string alertMsg = "";
        if(IsSell) // 현재 판매중인 아이템이 띄워져 있으면
        {
            if(TradeCount <= 0) alertMsg = "남은 수량이 없습니다."; // 거래 가능 횟수가 0이하면
            else if(GameManager.GM.CheckInvenMax()) alertMsg = "인벤토리가 부족합니다."; // 인벤토리가 꽉 찼으면
            else if(GameManager.GM.GoldO(ThisItem.ItemCost.ToString())) // 가격에 맞는 골드가 있으면
            {
                TradeCount--; // 거래 가능 횟수 -1
                GameManager.GM.Gold -= ThisItem.ItemCost; // 가격 만큼 골드 감소
                GameManager.GM.AddItem(ThisItem.ItemName); // 구매한 아이템을 인벤토리에 추가
                alertMsg = $"-{ThisItem.ItemCost}G, +{ThisItem.ItemName}"; // 알림 메세지 갱신
                transform.GetChild(2).GetComponent<Text>().text = $"남은 수량: {TradeCount}"; // 남은 수량 갱신
            }
            else alertMsg = "골드가 부족합니다."; // 골드가 없으면
        }
        else // 현재 상인에게 판매중 이면
        {
            if(!GameManager.GM.ItemO(ThisItem.ItemName)) alertMsg = "판매할 아이템이 없습니다."; // 판매하려는 아이템이 없으면
            else
            {
                GameManager.GM.Gold += ThisItem.ItemCost; // 판매할 아이템이 있으면 아이템 가격만큼 골드 추가
                GameManager.GM.RemoveItem(ThisItem.ItemName); // 판매한 아이템 제거
                alertMsg = $"+{ThisItem.ItemCost}G, -{ThisItem.ItemName}"; // 알림 메세지 갱신
                transform.GetChild(2).GetComponent<Text>().text = "보유 수량: " + GameManager.GM.GetItemCount(ThisItem.ItemName).ToString(); // 보유 수량 갱신
            }
        }
        transform.parent.parent.parent.parent.GetComponent<Trade>().InvenInfoTextUpdate();
        GameManager.GM.StateChangeData = "";
        GameManager.GM.AllResultActionDic["SmallPannelInTradeActive"](alertMsg); // 알림 메세지 띄우기
    }
}
