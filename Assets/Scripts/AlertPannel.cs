using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertPannel : MonoBehaviour
{
    private bool _isYes;
    public bool IsYes
    {
        get{ return _isYes; }
        set
        {
            _isYes = value;
            gameObject.SetActive(false);
            // 버튼 클릭시 액션 여기다 코딩
        }
    }

    public void ActiveAlertPannel(string question)
    {
        gameObject.SetActive(true);
        transform.GetChild(0).GetChild(0).GetComponent<Text>().text = question;
    }
}
