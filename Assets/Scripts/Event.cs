using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event : MonoBehaviour
{
    public string EventFormat; // 이벤트 형식
    public string EventName; // 이벤트 이름
    public int InvokeRate; // 이벤트 발생 수치
    public List<Selection> SelectionList = new List<Selection>(); // 이 이벤트에서 선택할 수 있는 선택지 목록
    public string EventText; // 이벤트가 발생될때 나올 텍스트
    
    [SerializeField]
    int maxCount = 6; // 선택지 최대갯수
    
    public void EventInvoke()
    {
        if(GameManager.GM.StateChangeData != "") // 현재 상태가 변경되었으면
        {
            GameManager.GM.AllResultActionDic["SmallPannelActive"](GameManager.GM.StateChangeData); // 알림창 띄우기
            GameManager.GM.Log += $"<{GameManager.GM.StateChangeData}>{System.Environment.NewLine}"; // 로그에 변경된 상태 업데이트
            GameManager.GM.StateChangeData = ""; // 로그에 업데이트 했으므로 초기화
        }
        // 스크롤뷰 뷰포트 사이즈와 선택지 판넬 사이즈를 선택지 개수에 따라 조정
        int selCount = SelectionList.Count;
        GameManager.GM.ScrollView.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (maxCount-selCount)*120 + 1540);
        GameManager.GM.SelectionPannel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 740 - (maxCount-selCount)*120);
        // 이벤트 발생시 뜨는 텍스트 갱신
        string eventText = EventText;
        string d = GameManager.GM.Day.ToString();
        if(EventFormat == "Death") { eventText = eventText.Replace("#", d); } // 죽었을 때 발동되는 이벤트면 텍스트에서 #을 현재 날짜로 변경한다.
        else if(EventFormat == "Active"){ d += "일차: "; } // 이벤트 형식이 Active면 날이 바뀐것이므로 로그에 n일차 텍스트 추가.
        else d = ""; // 날짜가 바뀌지 않았으므로 d는 없앤다.
        GameManager.GM.Log += $"{d}{eventText}{System.Environment.NewLine}"; // 로그 갱신
        GameManager.GM.EventTextUI.GetComponent<Text>().text = eventText; // 이벤트 텍스트 갱신
        
        int count;
        for (count = maxCount; count > maxCount - SelectionList.Count; count--) // 이벤트의 선택지 갯수만큼 오브젝트 활성화
        {
            GameObject pannel = GameManager.GM.SelectionPannel.transform.GetChild(count-1).gameObject;
            pannel.SetActive(true);
            Selection sel = SelectionList[selCount-1];
            if(GameManager.GM.QuestionAnalyze(sel.IsSelActiveText) == false) // 선택지 활성화 조건이 false면
            {
                pannel.GetComponent<Image>().color = new Color32(255, 196, 196, 255); // 색상 조정(붉은색)
                pannel.transform.GetChild(1).gameObject.SetActive(true); // 자물쇠 이미지 활성화
                pannel.GetComponent<Selection>().SelActionText = "{잠긴 선택지입니다.}SmallPannelActive"; // 선택지 클릭시 잠긴선택지라는 알림이 뜨게함
                pannel.transform.GetChild(0).GetComponent<Text>().text = "잠긴 선택지"; // 선택지에 나오는 텍스트를 "잠긴 선택지" 로 변경
            }
            else
            {
                pannel.GetComponent<Image>().color = new Color32(180, 180, 180, 255); // 색상 조정(회색)
                pannel.transform.GetChild(1).gameObject.SetActive(false); // 자물쇠 이미지 비활성화
                pannel.GetComponent<Selection>().SelectionUpdate(sel); // 선택지 업데이트
            }
            selCount--;
        }
        while(count > 0) // 남는 오브젝트는 비활성화
        {
            GameManager.GM.SelectionPannel.transform.GetChild(count-1).gameObject.SetActive(false);
            count--;
        }
    }
}
