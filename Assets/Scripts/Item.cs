using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public enum ItemFormat{기타, 소비, 무기, 투구, 갑옷, 신발, 장신구, 가방};
    public ItemFormat ThisItemFormat; // 아이템의 종류
    public string ItemName; // 아이템의 이름
    public int ItemCost; // 아이템의 가격
    public string ItemExplain; // 아이템 설명
    public string ItemInfo; // 잡화템 경우 특산물, 주산물 인가에 대한 정보
    public int ItemCount; // 아이템 보유 개수 -> 장비템은 강화한 횟수
    public int ItemWeight; // 한 칸에 최대로 가지고 있을 수 있는 개수
    public string StatusData; // 장비템스테이터스
    public bool IsWearing; // 장비템인 경우 착용중인가?
    public int ItemIndex; // 장비 고유의 번호
    /* -> 무기류 ex) 0,1,196,100,10
    int AtkForm(0 = 물리, 1 = 마법)
    int Type(0 = 무, 1 = 냉기, 2 = 불꽃, 3 = 대지, 4 = 빛, 5 = 어둠)
    int Atk;         // 공격력:      공격을 가할 때 입히는 데미지
    int AtkDelay;    // 공격주기:    공격을 하는 주기, 110 이면 1.10초 마다 공격
    int CriRate;     // 치명확률:    공격시 치명타가 발생할 확률, 10 이면 10% 확률로 치명타를 가함. 치명타는 데미지가 2배로 증가함. 100을 넘을 수 없음.
    
    -> 방어구 ex) 5,50,50
    int HP;          // 체력:        데미지를 받으면 체력이 닳고, 0이되면 전투에서 패배함
    int Def;         // 방어력:      물리공격을 받으면 상대의 공격력과 비교해서 방어할 확률이 정해짐
    int MagicDef;    // 항마력:      마법공격을 받으면 상대의 공격력과 비교해서 방어할 확률이 정해짐

    -> 장신구 ex) 10,10,10,기품|1 -> 가방은 예외적으로 int invenCount, Dictionary<string, int> AbilityDic
    int AtkPer;      // 공격력%:     x% 만큼 공격력이 상승
    int DefPer;      // 방어력%:     x% 만큼 방어력이 상승
    int MagicDefPer; // 항마력%:     x% 만큼 항마력이 상승
    Dictionary<string, int> AbilityDic; // 능력 들을 저장하는 딕셔너리 */

    public void ItemUpdate(int itemFormat, string itemName, int itemCost, string itemExplain, string statusData, int itemWeight, string itemInfo)
    {
        ThisItemFormat = (ItemFormat)itemFormat;
        StatusData = statusData;
        if(itemFormat >= 2)
        {
            string explain = "";
            string [] values = statusData.Split(',');
            // 아이템 종류에 따라 설명 작성
            switch (itemFormat)
            {
                case 2:
                    explain += values[0] + ", " + values[1] + " , 공격력:" + values[2] + 
                        " , 공격주기:" + (0.01f * int.Parse(values[3])).ToString() + "초 , 치명확률:" + values[4] + "% , " + itemExplain;
                    // 공격형태, 속성, 공격력, 공격주기, 치명확률
                    break;
                case 3:
                case 4:
                case 5:
                    explain += "HP:" + values[0] + " , 방어력:" + values[1] + " , 항마력:" + values[2] + " , " + itemExplain;
                    // HP, 방어력, 항마력
                    break;
                case 6:
                    if(values[0] != ""){ explain += "공격력:+" + values[0] + "% , "; }
                    if(values[1] != ""){ explain += "방어력:+" + values[1] + "% , "; }
                    if(values[2] != ""){ explain += "항마력:+" + values[2] + "% , "; }
                    if(values[3] != "")
                    {
                        string [] abilitys = values[3].Split(',');
                        foreach(string ability in abilitys)
                        {
                            string [] datas = ability.Split('_');
                            explain += datas[0] + " +" + datas[1] + " , ";
                        }
                    }
                    explain += itemExplain;
                    // 공격력%, 방어력%, 항마력%, 능력
                    break;
                case 7:
                    explain = "인벤토리 증가:" + values[0] + "칸 , " + itemExplain;
                    // 가방 칸수
                    break;
                default:
                    break;
            }
            ItemExplain = explain;
        }
        else {ItemExplain = itemExplain; ItemIndex = 0;}

        ItemName = itemName;
        ItemCost = itemCost;
        ItemInfo = itemInfo;
        ItemWeight = itemWeight;
        ItemCount = 1;
        IsWearing = false;
    }
    public void ItemUpdate(Item item)
    {
        ThisItemFormat = item.ThisItemFormat;
        ItemExplain = item.ItemExplain;
        ItemName = item.ItemName;
        ItemCost = item.ItemCost;
        ItemInfo = item.ItemInfo;
        ItemWeight = item.ItemWeight;
        StatusData = item.StatusData;
        ItemCount = item.ItemCount;
        IsWearing = item.IsWearing;
        ItemIndex = item.ItemIndex;
    }
    public string WearingEquip()
    {
        if(IsWearing)
        {
            IsWearing = false;
            for(int i = 0; i < GameManager.GM.HoldItemList.Count; i++)
            {
                if(GameManager.GM.HoldItemList[i].ItemIndex == ItemIndex)
                {
                    GameManager.GM.HoldItemList[i].IsWearing = false;
                    IsWearing = false;
                    GameManager.GM.PlayerStatus.StatusUpdate(-1, (int)ThisItemFormat, StatusData);
                    return ItemName + ":장착 해제";
                }
            }
        }
        else
        {
            foreach(Item it in GameManager.GM.HoldItemList)
            {
                if(it.ThisItemFormat == ThisItemFormat && it.IsWearing) return "이미 다른 장비를 장착 중입니다.";
            }
            for(int i = 0; i < GameManager.GM.HoldItemList.Count; i++)
            {
                if(GameManager.GM.HoldItemList[i].ItemIndex == ItemIndex)
                {
                    GameManager.GM.HoldItemList[i].IsWearing = true;
                    IsWearing = true;
                    GameManager.GM.PlayerStatus.StatusUpdate(1, (int)ThisItemFormat, StatusData);
                    return ItemName + ":장착";
                }
            }
        }
        return "?";
    }
    // 클릭시 사용가능 아이템 -> 사용하겠습니까, 사용하고 갯수 최신화
    // 클릭시 사용불가능 아이템 -> 사용이 불가능한 아이템 입니다.
    // 클릭시 장착 중인 장비 -> 해제됨
    // 클릭시 장착 중이지 않은 장비 -> 원래 장비 해제되고 새 장비 장착
}
