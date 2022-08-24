using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    public void OnClick()
    {
        switch(gameObject.name)
        {
            case "LogButton":
                GameManager.GM.LogPannel.gameObject.SetActive(true);
                GameManager.GM.LogText.text = GameManager.GM.Log;
                break;
            case "StatusButton":
                string HaveItemListStr = "";
                foreach(Item it in GameManager.GM.HoldItemList)
                {
                    if(HaveItemListStr == "") HaveItemListStr += $"{it.ItemName} x{it.ItemCount}";
                    else HaveItemListStr += $", {it.ItemName} x{it.ItemCount}";
                }
                GameManager.GM.LogPannel.gameObject.SetActive(true);
                GameManager.GM.LogText.text = "현재 상태    인벤토리:" + GameManager.GM.Weight + "/" + GameManager.GM.MaxWeight + System.Environment.NewLine +
                    GameManager.GM.PlayerStatus.StatusText() + System.Environment.NewLine + HaveItemListStr;
                break;
            case "InventoryButton":
                GameManager.GM.AllResultActionDic["ActiveInventoryPannel"]("");
                break;
            case "SettingButton":
                GameManager.GM.AllResultActionDic["NewGame"]("");
                break;
            case "CloseLogPannelB":
                transform.parent.gameObject.SetActive(false);
                break;
            case "YesB":
                transform.parent.parent.GetComponent<AlertPannel>().IsYes = true;
                break;
            case "NoB":
                transform.parent.parent.GetComponent<AlertPannel>().IsYes = false;
                break;
            default:
                break;
        }
    }
}
