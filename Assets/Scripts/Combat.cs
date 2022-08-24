using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{
    Status PlayerStatus, EnemyStatus;
    Vector2 PlayerHPSize, EnemyHPSize;
    int LeftTimeData, PlayerHP, EnemyHP, objCount;
    string VicEvent, DefeatEvent, DrawEvent, CombatResult;
    float passSec = 0.1f;
    bool isPaused;
    GameObject RecentObj_P, RecentObj_E;
    [SerializeField]
    Color PlayerColor, EnemyColor;
    [SerializeField]
    Image PlayPauseBttonImg;
    [SerializeField]
    Sprite PlayImg, PauseImg;
    [SerializeField]
    int MaxTime;
    [SerializeField]
    private GameObject PlayerHPBG, EnemyHPBG, Content, PlayerActionUI, EnemyActionUI, CombatEndBtton;
    [SerializeField]
    private Text PlayerNameText, EnemyNameText, LeftTime, PlayerCombetStat, EnemyCombetStat;

    public GameManager.ResultAction GetActiveCombatPannel(){ return new GameManager.ResultAction(ActiveCombatPannel); }
    
    public void ActiveCombatPannel(string CombatData)
    {
        //적이름,승리시이벤트,패배시이벤트,무승부시이벤트
        string [] values = CombatData.Split(',');
        isPaused = false;
        VicEvent = values[1]; DefeatEvent = values[2]; DrawEvent = values[3];
        PlayerStatus = GameManager.GM.PlayerStatus;
        EnemyStatus = GameManager.GM.EnemyDataDic[values[0]];
        PlayerNameText.text = PlayerStatus.StatusName;
        EnemyNameText.text = EnemyStatus.StatusName;
        LeftTimeData = MaxTime;
        for(int i = objCount-1; i >= 0; i--)
        {
            Destroy(Content.transform.GetChild(i).gameObject);
        }
        objCount = 0;
        passSec = 0.1f;
        LeftTime.text = ((float)LeftTimeData/10f).ToString()+"초";
        PlayerHPBG.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(500, 70);
        EnemyHPBG.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(500, 70);
        PlayerHP = PlayerStatus.HP; EnemyHP = EnemyStatus.HP;
        PlayerHPSize = PlayerHPBG.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        EnemyHPSize = EnemyHPBG.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        PlayerCombetStat.text = "전투력: " + PlayerStatus.GetCombetStat().ToString();
        EnemyCombetStat.text = "전투력: " + EnemyStatus.GetCombetStat().ToString();
        HPUpdate(true, 0);
        HPUpdate(false, 0);
        CombatEndBtton.gameObject.SetActive(false);
        gameObject.SetActive(true);
        StartCoroutine ("InCombat");
    }
    void HPUpdate(bool isPlayerHit, int dmg)
    {
        if(isPlayerHit)
        {
            PlayerHP -= dmg;
            RectTransform PlayerHPTransform = PlayerHPBG.transform.GetChild(0).GetComponent<RectTransform>();
            float size = (float)PlayerHP/(float)PlayerStatus.HP;
            PlayerHPTransform.sizeDelta = new Vector2(PlayerHPSize.x*size, PlayerHPSize.y);
            PlayerHPBG.transform.GetChild(1).GetComponent<Text>().text = $"HP: {PlayerHP}";
        }
        else
        {
            EnemyHP -= dmg;
            RectTransform EnemyHPTransform = EnemyHPBG.transform.GetChild(0).GetComponent<RectTransform>();
            float size = (float)EnemyHP/(float)EnemyStatus.HP;
            EnemyHPTransform.sizeDelta = new Vector2(EnemyHPSize.x*size, EnemyHPSize.y);
            EnemyHPBG.transform.GetChild(1).GetComponent<Text>().text = $"HP: {EnemyHP}";
        }
    }
    IEnumerator InCombat()
    {
        LeftTimeData = MaxTime;
        int playerDelay = PlayerStatus.AtkDelay;
        int enemyDelay = EnemyStatus.AtkDelay;
        int currentPlayerDelay = 0;
        int currentEnemyDelay = 0;
        while(true)
        {
            do{ yield return new WaitForSeconds(passSec); } while(isPaused);
            LeftTimeData--; currentPlayerDelay++; currentEnemyDelay++;
            LeftTime.text = ((float)LeftTimeData/10f).ToString()+"초";
            if(currentPlayerDelay >= playerDelay) // 플레이어의 공격 주기가 되면 플레이어가 공격을 시도함.
            {
                InAtk(true);
                currentPlayerDelay = 0;
            }
            if(currentEnemyDelay >= enemyDelay) // 적의 공격 주기가 되면 적이 공격을 시도함.
            {
                InAtk(false);
                currentEnemyDelay = 0;
            }
            if(PlayerHP <= 0 || EnemyHP <= 0) // 둘중 한명 사망
            {
                if(PlayerHP <= 0 && EnemyHP <= 0) CombatResult = "무승부";
                else if(PlayerHP <= 0) CombatResult = "패배";
                else if(EnemyHP <= 0) CombatResult = "승리";
                CombatEndBtton.transform.GetChild(0).GetComponent<Text>().text = CombatResult;
                CombatEndBtton.gameObject.SetActive(true);
                yield break;
            }
            if(LeftTimeData <= 0) // 시간이 다됨 -> 무승부
            {
                CombatResult = "무승부";
                CombatEndBtton.transform.GetChild(0).GetComponent<Text>().text = CombatResult;
                CombatEndBtton.gameObject.SetActive(true);
                yield break;
            }
        }
    }
    void InAtk(bool isPlayerTurn)
    {
        int dmg = 1;
        float typeValue; 
        float ranNum = Random.Range(0,10000);
        if(isPlayerTurn)
        {
            typeValue = TypeCompare((int)PlayerStatus.ThisType, (int)EnemyStatus.ThisType);
            float DefOrMagicDef = PlayerStatus.ThisAtkForm == Status.AtkForm.물리 ? EnemyStatus.Def : EnemyStatus.MagicDef;
            float percent = 10000f*(1f-1f/((Mathf.Pow((float)PlayerStatus.Atk*typeValue/DefOrMagicDef, 2)+1f)));
            Debug.Log($"플레이어인가: {isPlayerTurn} | 공격 확률: {percent/100}%");
            if(ranNum < percent)
            {
                if(Random.Range(0,100) < PlayerStatus.CriRate) dmg = dmg * 2;
                AtkTry(dmg, true);
            }
            else AtkTry(0, true);
        }
        else
        {
            typeValue = TypeCompare((int)EnemyStatus.ThisType, (int)PlayerStatus.ThisType);
            float DefOrMagicDef = EnemyStatus.ThisAtkForm == Status.AtkForm.물리 ? PlayerStatus.Def : PlayerStatus.MagicDef;
            float percent = 10000f*(1f-1f/((Mathf.Pow((float)EnemyStatus.Atk*typeValue/DefOrMagicDef, 2)+1f)));
            Debug.Log($"플레이어인가: {isPlayerTurn} | 공격 확률: {percent/100}%");
            if(ranNum < percent)
            {
                if(Random.Range(0,100) < EnemyStatus.CriRate) dmg = dmg * 2;
                AtkTry(dmg, false);
            }
            else AtkTry(0, false);
        }
    }
    void AtkTry(int dmg, bool isPlayerTurn)
    {
        if(isPlayerTurn)
        {
            if(RecentObj_P != null) RecentObj_P.GetComponent<Image>().color = new Color(PlayerColor.r, PlayerColor.g, PlayerColor.b , 0.7f);
            GameObject ActionUI = Instantiate(PlayerActionUI);
            RecentObj_P = ActionUI;
            ActionUI.transform.SetParent(Content.transform, false);

            if(dmg == 0) ActionUI.transform.GetChild(3).GetComponent<Text>().text = $"나의 공격! 공격이 빗나갔다.";
            else
            {
                HPUpdate(!isPlayerTurn, dmg);
                ActionUI.transform.GetChild(3).GetComponent<Text>().text = $"나의 공격! {dmg}의데미지를 입혔다. 적의 남은체력은 {EnemyHP}다.";
            }
        }
        else
        {
            if(RecentObj_E != null) RecentObj_E.GetComponent<Image>().color = new Color(EnemyColor.r, EnemyColor.g, EnemyColor.b , 0.7f);
            GameObject ActionUI = Instantiate(EnemyActionUI);
            RecentObj_E = ActionUI;
            ActionUI.transform.SetParent(Content.transform, false);
            if(dmg == 0) ActionUI.transform.GetChild(3).GetComponent<Text>().text = $"적의 공격! 공격이 빗나갔다.";
            else
            {
                HPUpdate(!isPlayerTurn, dmg);
                ActionUI.transform.GetChild(3).GetComponent<Text>().text = $"적의 공격! {dmg}의데미지를 입었다. 나의 남은체력은 {PlayerHP}다.";
            }
        }
        objCount++;
        int pos = 180*objCount-1250;
        if(pos>0) Content.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, pos+50);
    }
    float TypeCompare(int atkType, int defType)
    {
        // 0무 1물 2불 3풀 4빛 5어둠
        if(atkType == 1)
        {
            if(defType == 2) return 1.5f;
            if(defType == 3) return 0.75f;
        }
        if(atkType == 2)
        {
            if(defType == 3) return 1.5f;
            if(defType == 1) return 0.75f;
        }
        if(atkType == 3)
        {
            if(defType == 1) return 1.5f;
            if(defType == 2) return 0.75f;
        }
        if(atkType == 4 && defType == 5) return 1.5f;
        if(atkType == 5 && defType == 4) return 1.5f;
        return 1f;
    }
    public void CombatEndButtonClick()
    {
        switch(CombatResult)
        {
            case "승리":
                GameManager.GM.AllEventDic[VicEvent].EventInvoke();
                break;
            case "패배":
                GameManager.GM.AllEventDic[DefeatEvent].EventInvoke();
                break;
            case "무승부":
                GameManager.GM.AllEventDic[DrawEvent].EventInvoke();
                break;
            default:
                break;
        }
        gameObject.SetActive(false);
    }
    public void ForwardButtonClick()
    {
        passSec = passSec == 0.1f ? 0.05f : 0.1f;
    }
    public void PlayPauseButtonClick()
    {
        PlayPauseBttonImg.sprite = isPaused ? PauseImg : PlayImg;
        isPaused = !isPaused;

    }
    public void SkipButtonClick()
    {
        passSec = 0.001f;
    }
    // 전투 방식: 공격 주기마다 공격을 하며, 공격 시 100*(1+1/((공격력/방어력or항마력)^2+1))%의 확률로 공격에 성공하며,
    // 불리한 상성이면 계산 시 공격력이 0.75배가 되며, 유리한 상성이면 계산 시 공격력이 1.5배가 되며, 치명타 발생 시 피해량이 2배로 증가한다.
    // 빛과 어둠은 서로 유리한 속성 취급이므로 서로 공격력이 1.5배가 된다.
    // 그리고 피해량 만큼 체력이 줄어들고 먼저 체력이 0 이하가 된 사람이 패배한다.
}
