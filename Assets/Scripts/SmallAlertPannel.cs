using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallAlertPannel : MonoBehaviour
{
    Text PannelText;

    public GameManager.ResultAction GetSmallPannelActive() // SmallPannelActive를 ResultAction델리게이트 형으로 가져오는 함수
    {
        PannelText = transform.GetChild(0).GetComponent<Text>();
        gameObject.SetActive(false);
        return new GameManager.ResultAction(SmallPannelActive);
    }

    public void SmallPannelActive(string alertMsg) // 알림창을 띄우는 함수
    {
        PannelText.text = alertMsg;
        gameObject.SetActive(true);
        StopCoroutine ("SmallPannelActiveFalse"); // 진행중이던 코루틴 삭제
        StartCoroutine ("SmallPannelActiveFalse"); // 다시 시작
    }

    IEnumerator SmallPannelActiveFalse() // UI를 활성화하고 점차 투명해지다가 비활성화 시키는 코루틴
    {
        GetComponent<Image>().color = new Color(0.7f,0.7f,0.7f,1);
        yield return new WaitForSeconds(0.1f); // 여러번 클릭해도 새로운 UI가 뜨는것 처럼 보이게 색을 바꿈
        GetComponent<Image>().color = new Color(0.86f,0.86f,0.86f,1);
        yield return new WaitForSeconds(1.4f);
        for(float i = 1; i>0; i-=0.05f)
        {
            Color imgColor = GetComponent<Image>().color;
            imgColor.a = i;
            GetComponent<Image>().color = imgColor;
            yield return new WaitForSeconds(0.03f);
        }
        gameObject.SetActive(false);
        yield break;
    }
}
