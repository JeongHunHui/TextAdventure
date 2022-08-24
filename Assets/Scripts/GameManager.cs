using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    [SerializeField] // 게임 초기 설정들
    private int startHP, startMental, startGold, startStamina, startMaxWeight, startFame;
    //-------------------------------------------------------------------------------------
    [SerializeField]
    private TextAsset EventTextFile; // 이벤트 정보가 담긴 텍스트 파일
    [SerializeField]
    private TextAsset SelectionTextFile; // 선택지 정보가 담긴 텍스트 파일
    [SerializeField]
    private TextAsset ItemDataTextFile; // 아이템들의 정보가 담긴 텍스트 파일
    [SerializeField]
    private TextAsset TradeItemListTextFile; // 거래 품목 정보가 담긴 텍스트 파일
    [SerializeField]
    private TextAsset TradeDataTextFile; // Trade 정보가 담긴 텍스트 파일
    [SerializeField]
    private TextAsset EnemyDataTextFile; // 적의 능력치 정보가 담긴 텍스트 파일
    //-------------------------------------------------------------------------------------
    private static GameManager gm = null; // 싱글턴 오브젝트
    public static GameManager GM{ get{return gm;} } // gm의 접근자
    //-------------------------------------------------------------------------------------
    public Status PlayerStatus; // 플레이어 능력치
    //-------------------------------------------------------------------------------------
    public delegate void ResultAction(string actionData); // 선택지의 결과함수 델리게이트
    public delegate bool Question(string conditionData); // 실행 조건(질문) 델리게이트
    //-------------------------------------------------------------------------------------
    public Dictionary<string, ResultAction> AllResultActionDic; // 모든 결과함수 딕셔너리
    public Dictionary<string, Question> AllQuestionDic; // 모든 질문 딕셔너리
    public Dictionary<string, List<Selection>> AllSelectionListDic; // 모든 선택지리스트 딕셔너리
    public Dictionary<string, Event> AllEventDic; // 모든 이벤트 딕셔너리
    public Dictionary<string, List<Event>> AllEventListDic; // 모든 이벤트 리스트 딕셔너리
    public Dictionary<string, List<TradeItem>> TradeItemListDic; // 모든 이벤트 리스트 딕셔너리
    public Dictionary<string, List<Item>> EquipListDic; // 모든 아이템 리스트 딕셔너리
    public Dictionary<string, Item> ItemDic; // 모든 아이템 딕셔너리
    public Dictionary<string, int> HoldAbilityDic; // 보유중인 능력 딕셔너리
    public Dictionary<string, string> TradeDataDic; // Trade데이타를 모은 딕셔너리
    public Dictionary<string, Status> EnemyDataDic; // 적 능력치 데이타를 모은 딕셔너리
    //-------------------------------------------------------------------------------------
    [SerializeField] // 인스펙터에서 할당하는 변수들
    private GameObject TradePannel, SmallAlertUI, SmallAlertUIInTrade, SmallPannelInInventory, 
        HPObj, MentalObj, TimeBG, StaminaBG, InventoryPannel, AlertPannel, CombatPannel;
    [SerializeField]
    private Sprite FillHPSpr, FillMentalSpr, EmptyHeartSpr;
    //-------------------------------------------------------------------------------------
    public List<Item> HoldItemList; // 보유 중인 아이템들의 리스트
    //-------------------------------------------------------------------------------------
    public Text GoldText, DayText, TimeText, LogText;
    public GameObject EventTextUI, SelectionPannel, ScrollView, LogPannel;
    //-------------------------------------------------------------------------------------
    [SerializeField]
    private Color MorningColor, NoonColor, SunsetColor, NightColor, DawnColor;
    private Color [] TimeColorArray;
    public enum Time{아침, 점심, 저녁, 밤, 새벽};
    private Time _currentTime;
    public Time CurrentTime
    {
        get{ return _currentTime; }
        set
        {
            int timeIndex = (int)_currentTime + 1;
            if(timeIndex > 4)
            {
                _currentTime = (Time)0;
                Day += 1;
                timeIndex = 0;
            }
            else _currentTime = (Time)timeIndex;
            TimeText.text = _currentTime.ToString();
            TimeBG.GetComponent<Image>().color = TimeColorArray[timeIndex];
        }
    }
    //-------------------------------------------------------------------------------------
    private int _gold = 0; // 현재 보유중인 골드
    public int Gold
    {
        get{ return _gold; }
        set
        {
            int gold = _gold;

            if(value <= 0) _gold = 0;
            else _gold = value;
            if(_gold - gold != 0) StateChange("골드", _gold - gold);
            GoldText.text = _gold.ToString();
        }
    }
    //-------------------------------------------------------------------------------------
    private int MaxHP = 5;
    private int _hp = 0; // 현재 보유중인 체력
    public int HP
    {
        get{ return _hp; }
        set
        {
            int hp = _hp;
            if(value > MaxHP) _hp = MaxHP;
            else if(value <= 0) _hp = 0;
            else _hp = value;

            if(_hp - hp != 0){ StateChange("체력", _hp - hp); }
            int i;
            for(i = 0; i < _hp; i++){ HPObj.transform.GetChild(i).GetComponent<Image>().sprite = FillHPSpr; }
            for(int i2 = i; i2 < MaxHP; i2++){ HPObj.transform.GetChild(i).GetComponent<Image>().sprite = EmptyHeartSpr; }
        }
    }
    //-------------------------------------------------------------------------------------
    private int MaxMental = 5;
    private int _mental = 0; // 현재 보유중인 정신력
    public int Mental
    {
        get{ return _mental; }
        set
        {
            int mental = _mental;
            if(value > MaxMental) _mental = MaxMental;
            else if(value <= 0) _mental = 0;
            else _mental = value;

            if(_mental - mental != 0){ StateChange("정신력", _mental - mental); }
            int i;
            for(i = 0; i < _mental; i++){ MentalObj.transform.GetChild(i).GetComponent<Image>().sprite = FillMentalSpr; }
            for(int i2 = i; i2 < MaxMental; i2++){ MentalObj.transform.GetChild(i).GetComponent<Image>().sprite = EmptyHeartSpr; }
        }
    }
    //-------------------------------------------------------------------------------------
    private int _day; // 현재 날짜
    public int Day
    {
        get{ return _day; }
        set
        {
            _day = value;
            DayText.text = _day.ToString() + "일";
        }
    }
    //-------------------------------------------------------------------------------------
    private int MaxStamina = 100;
    private int _stamina = 0; // 현재 보유중인 스테미나
    private float staminaXSize;
    public int Stamina
    {
        get{ return _stamina; }
        set
        {
            int stamina = _stamina;
            if(value > MaxStamina) _stamina = MaxStamina;
            else if(value <= 0) _stamina = 0;
            else _stamina = value;
            
            if(_stamina - stamina != 0){ StateChange("스테미나", _stamina - stamina); }
            RectTransform StaminaTransform = StaminaBG.transform.GetChild(0).GetComponent<RectTransform>();
            float size = (float)_stamina/(float)MaxStamina;
            StaminaTransform.sizeDelta = new Vector2(staminaXSize*size, StaminaTransform.sizeDelta.y);
            string str;
            if(_stamina > 50) str = "보통";
            else if(_stamina > 20) str = "피곤";
            else if(_stamina > 0) str = "과로";
            else str = "탈진";
            StaminaBG.transform.GetChild(1).GetComponent<Text>().text = str;
        }
    }
    //-------------------------------------------------------------------------------------
    private int _fame; // 현재 명성
    public int Fame
    {
        get{ return _fame; }
        set
        {
            int fame = _fame;
            _fame = value;
            StateChange("명성", _fame - fame);
        }
    }
    //-------------------------------------------------------------------------------------
    private int _weight; // 현재 무게
    public int MaxWeight; // 인벤토리 최대치
    public int Weight
    {
        get
        {
            int weight = 0;
            foreach(Item it in HoldItemList){ if(!it.IsWearing) weight += it.ItemWeight * it.ItemCount; }
            return weight;
        }
        set{ _weight = value; }
    }
    //-------------------------------------------------------------------------------------
    private bool isGameOver; // 게임오버가 되면 true, 아니면 false
    public string Log; // 게임의 진행상태를 기록해놓는 텍스트
    public string StateChangeData; // 체력, 골드, 물건 등 상태의 변화를 기록하는 변수
    private int index;
    //-------------------------------------------------------------------------------------
    void Awake() // 싱글턴 오브젝트
    {
        if(gm){ DestroyImmediate(gameObject); return; }
        gm = this; DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        AllResultActionDic = new Dictionary<string, ResultAction>(); // 결과 함수 딕셔너리 생성
        AllQuestionDic = new Dictionary<string, Question>(); // 선택지 해금조건 딕셔너리 생성
        AllSelectionListDic = new Dictionary<string, List<Selection>>(); // 선택지 딕셔너리 생성
        AllEventDic = new Dictionary<string, Event>(); // 이벤트 딕셔너리 생성
        AllEventListDic = new Dictionary<string, List<Event>>();
        ItemDic = new Dictionary<string, Item>(); // 아이템 리스트 딕셔너리 생성
        TradeItemListDic = new Dictionary<string, List<TradeItem>>(); // 상인 아이템 리스트 딕셔너리 생성
        TradeDataDic = new Dictionary<string, string>(); // TradeData를 모은 딕셔너리 생성
        HoldAbilityDic = new Dictionary<string, int>(); // 보유중인 능력을 담은 딕셔너리. 문자열이 능력이름, 정수가 능력 레벨
        EnemyDataDic = new Dictionary<string, Status>(); // 적의 능력치 데이터를 담은 딕셔너리

        HoldItemList = new List<Item>(); // 보유 중인 아이템들을 담은 리스트
        TimeColorArray = new Color[]{MorningColor, NoonColor, SunsetColor, NightColor, DawnColor};
        staminaXSize = StaminaBG.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;

        AddAction(); // 만들어둔 결과 함수와 선택지 해금조건들을 딕셔너리에 등록
        ItemDataTextRead(); TradeItemListTextRead(); TradeDataTextRead(); SelectionTextRead(); EnemyDataTextRead();
        NewGame(""); // 게임 시작!
    }
    void AddAction() // 만들어둔 결과 함수와 선택지 해금조건들을 딕셔너리에 등록하는 함수
    {
        AllResultActionDic.Add("NewGame", NewGame);
        AllResultActionDic.Add("SetGold", SetGold);
        AllResultActionDic.Add("SetHP", SetHP);
        AllResultActionDic.Add("SetMental", SetMental);
        AllResultActionDic.Add("SetDay", SetDay);
        AllResultActionDic.Add("AddItem", AddItem);
        AllResultActionDic.Add("RemoveItem", RemoveItem);
        AllResultActionDic.Add("AddAbility", AddAbility);
        AllResultActionDic.Add("RemoveAbility", RemoveAbility);
        AllResultActionDic.Add("SetFame", SetFame);

        AllResultActionDic.Add("EventInvokeByName", EventInvokeByName);
        AllResultActionDic.Add("NewEvent", NewEvent);
        AllResultActionDic.Add("RandomEvent", RandomEvent);
        AllResultActionDic.Add("SmallPannelActive", SmallAlertUI.GetComponent<SmallAlertPannel>().GetSmallPannelActive());
        AllResultActionDic.Add("SmallPannelInTradeActive", SmallAlertUIInTrade.GetComponent<SmallAlertPannel>().GetSmallPannelActive());
        AllResultActionDic.Add("SmallPannelInInventoryActive", SmallPannelInInventory.GetComponent<SmallAlertPannel>().GetSmallPannelActive());
        AllResultActionDic.Add("ActiveTradePannel", TradePannel.GetComponent<Trade>().GetActiveTradePannel());
        AllResultActionDic.Add("ActiveInventoryPannel", InventoryPannel.GetComponent<Inventory>().GetActiveInventoryPannel());
        AllResultActionDic.Add("ActiveCombatPannel", CombatPannel.GetComponent<Combat>().GetActiveCombatPannel());
        //------------
        AllQuestionDic.Add("GoldO", GoldO);
        AllQuestionDic.Add("ItemO", ItemO);
        AllQuestionDic.Add("RandomTF", RandomTF);
        AllQuestionDic.Add("AbilityO", AbilityO);
        AllQuestionDic.Add("FameO", FameO);
    }
    void SelectionTextRead() // 선택지들의 정보를 담은 텍스트 파일을 읽고 게임에 반영하는 함수
    {
        StringReader sr = new StringReader(SelectionTextFile.text);
        string source = sr.ReadLine();
        string [] values;
        while (source != null)
        {
            values = source.Split('|');
            // 선택지 리스트 이름, 선택지 이름, 선택지 텍스트, 선택지 해금 조건 텍스트, 선택지 함수 텍스트
            Selection sel = gameObject.AddComponent<Selection>();
            // string SelName, string SelectionText, string IsSelActiveText, string SelActionText
            sel.SelName = values[1];
            sel.SelectionText = values[2];
            sel.IsSelActiveText = values[3];
            sel.SelActionText = values[4];

            List<Selection> sl = new List<Selection>();
            if(AllSelectionListDic.ContainsKey(values[0]) == false)
            {
                sl.Add(sel);
                AllSelectionListDic.Add(values[0], sl);
            }
            else
            {
                sl = AllSelectionListDic[values[0]];
                sl.Add(sel);
                AllSelectionListDic[values[0]] = sl;
            }
            Destroy(sel);
            source = sr.ReadLine();
        }
    }
    void EventTextRead() // 이벤트들의 정보를 담은 텍스트 파일을 읽고 게임에 반영하는 함수
    {
        StringReader sr = new StringReader(EventTextFile.text);
        string source = sr.ReadLine();
        string [] values;
        while (source != null)
        {
            values = source.Split('|');
            //이벤트 리스트, 이벤트 이름, 이벤트발생수치, 선택지리스트, 이벤트텍스트
            Event ev = gameObject.AddComponent<Event>();
            ev.EventFormat = values[0];
            ev.EventName = values[1];
            ev.InvokeRate = int.Parse(values[2]);
            ev.SelectionList = AllSelectionListDic[values[3]];
            ev.EventText = values[4].Replace("^",System.Environment.NewLine);
            
            if(!AllEventDic.ContainsKey(ev.EventName)) AllEventDic.Add(ev.EventName, ev);
            if(!AllEventListDic.ContainsKey(values[0]))
            {
                List<Event> el = new List<Event>();
                el.Add(ev);
                AllEventListDic.Add(values[0], el);
            }
            else
            {
                List<Event> el = new List<Event>();
                el = AllEventListDic[values[0]];
                el.Add(ev);
                AllEventListDic[values[0]] = el;
            }
            //-------------------------------------
            Destroy(ev);
            source = sr.ReadLine();
        }
    }
    void ItemDataTextRead()
    {
        StringReader sr = new StringReader(ItemDataTextFile.text);
        string source = sr.ReadLine();
        string [] values;
        while (source != null)
        {
            values = source.Split('|');
            //아이템 형식(숫자)|아이템 이름|아이템가격|아이템설명|아이템스탯(장비템만)|아이템 보유가능 수|아이템정보
            //0|커피콩|100|좋은 향이 나는 커피콩이다. 끓이면 좋은 향이 나는 차를 만들 수 있다.|Info
            Item item = gameObject.AddComponent<Item>();
            item.ItemUpdate(int.Parse(values[0]), values[1], int.Parse(values[2]), values[3], values[4], int.Parse(values[5]), values[6]);
            ItemDic.Add(values[1], item);
            Destroy(item);
            source = sr.ReadLine();
        }
    }
    void TradeItemListTextRead()
    {
        StringReader sr = new StringReader(TradeItemListTextFile.text);
        string source = sr.ReadLine();
        string [] values;
        while (source != null)
        {
            values = source.Split('|');
            //아이템 리스트 이름|아이템이름|거래횟수
            List<TradeItem> tradeItemList = new List<TradeItem>();
            TradeItem tradeItem = gameObject.AddComponent<TradeItem>();
            tradeItem.TradeItemUpdate(ItemDic[values[1]], int.Parse(values[2]));
            if(TradeItemListDic.ContainsKey(values[0]))
            {
                tradeItemList = TradeItemListDic[values[0]];
                tradeItemList.Add(tradeItem);
                TradeItemListDic[values[0]] = tradeItemList;
            }
            else
            {
                tradeItemList.Add(tradeItem);
                TradeItemListDic.Add(values[0], tradeItemList);
            }
            Destroy(tradeItem);
            source = sr.ReadLine();
        }
    }
    void TradeDataTextRead()
    {
        StringReader sr = new StringReader(TradeDataTextFile.text);
        string source = sr.ReadLine();
        string [] values;
        while (source != null)
        {
            values = source.Split('|');
            TradeDataDic.Add(values[0], source);
            source = sr.ReadLine();
        }
    }
    void EnemyDataTextRead()
    {
        StringReader sr = new StringReader(EnemyDataTextFile.text);
        string source = sr.ReadLine();
        string [] values;
        while (source != null)
        {
            values = source.Split('|');
            Status stat = new Status(values[0], values[1], values[2], values[3], values[4], 
                values[5], values[6], values[7], values[8], values[9]);
            EnemyDataDic.Add(values[0], stat);
            source = sr.ReadLine();
        }
    }
    public int GetItemCount(string itemName)
    {
        int count = 0;
        foreach(Item it in HoldItemList){ if(it.ItemName == itemName) count += it.ItemCount; }
        return count;
    }
    public void ActionAnalyze(string actData)
    {
        string Act = actData;
        if(Act.StartsWith("?"))
        {
            string question = "";
            string act = "";
            bool isTrue = false;
            string ActClone = Act;
            foreach(char ch in Act)
            {
                if(ch == '>')
                {
                    isTrue = QuestionAnalyze(question);
                    ActClone = ActClone.Remove(0,1);
                    break;
                }
                else question += ch;
                ActClone = ActClone.Remove(0,1);
            }
            Act = ActClone;
            if(isTrue) // 질문이 true면 ' 앞문장을 act로 등록
            {
                foreach(char ch in Act)
                {
                    if(ch == '`') break;
                    else act += ch;
                }
            }
            else // 질문이 false면 ' 뒷문장을 act로 등록
            {
                bool isElse = false;
                foreach(char ch in Act)
                {
                    if(ch == '`' && !isElse) isElse = true;
                    else if(isElse) act += ch;
                }
            }
            ActionAnalyze(act);
        }
        else
        {
            string[] ActArray = Act.Split('+'); // +를 기준으로 문자열을 나눔
            foreach (string ActKey in ActArray) // 결과 함수 텍스트를 읽어서 Selection클래스에 넣음
            {
                string ActClone = ActKey;
                if(ActClone.StartsWith("&"))
                {
                    ActClone = ActClone.Remove(0,1);
                    AllResultActionDic["EventInvokeByName"](ActClone);
                }
                else if(ActClone.StartsWith("{"))
                {
                    ActClone = ActClone.Remove(0,1);
                    string [] ActArray2 = ActClone.Split('}');
                    AllResultActionDic[ActArray2[1]](ActArray2[0]);
                }
                else
                {
                    AllResultActionDic[ActKey]("");
                }
            }
        }
    }
    public bool QuestionAnalyze(string questionText) // ?로 시작하는 문장 분석
    {
        if(questionText == "True") return true;
        string question = questionText;
        char logicalOperater = '/';
        char newLogicalOperater = ' ';
        bool isEnd = false;
        bool TF = false; // 이 함수가 반환할 변수
        question = question.Remove(0,1); // ? 제거
        while(!isEnd)
        {
            string qData = "";
            string q = "";
            bool isData = true;
            bool isReverse = false;
            question = question.Remove(0,1); // { 제거
            string questionClone = question; // question 변수의 변형을 막기위해 선언
            foreach(char ch in question) // ?부터 ~ {질문데이타}질문 ~ >까지 읽는 반복문
            {
                if(!isData)
                {
                    if(ch == '!') isReverse = true;
                    else if(ch == '/' || ch == '*') // 논리연산자를 읽었으면 변수 업데이트후 반복문 종료
                    {
                        newLogicalOperater = ch;
                        questionClone = questionClone.Remove(0,1);
                        break;
                    }
                    else q += ch; // 데이터를 읽는중이 아니면 q에 데이터 넣기
                }
                else if(ch =='}' && isData) isData = false;
                // } 를 읽었다는건 데이터 부분이 끝난것이므로 isData를 false
                else qData += ch; // 데이터를 읽는중이면 qData에 데이터 넣기
                questionClone = questionClone.Remove(0,1); // 이미 읽은 문자 제거
            }
            bool refinedQuestion = AllQuestionDic[q](qData);
            if(isReverse) refinedQuestion = !refinedQuestion; // 질문이름 앞에 !이 있었으면 질문값 반전

            if(logicalOperater == '/') TF = TF | refinedQuestion;
            else TF = TF & refinedQuestion; // 논리연산자값에 따라 TF와 연산후 할당
        
            logicalOperater = newLogicalOperater; // 논리연산자 갱신

            if(questionClone == "") isEnd = true; // 다 읽었으면 리턴
            else question = questionClone; // 아직 남았으면 다시 반복
        }
        return TF;
    }
    public void StateChange(string changeData)
    {
        if(StateChangeData == "") StateChangeData += changeData;
        else StateChangeData += ", " + changeData;
    }
    public void StateChange(string changeWhat, int changeData) // 체력, 골드, 정신력 등
    {
        string str = StateChangeData == "" ? changeWhat : ", " + changeWhat;
        str += changeData >= 0 ? " +" : " ";
        StateChangeData +=  str + changeData.ToString();
    }
    public bool CheckInvenMax(){ return Weight >= MaxWeight ? true : false; }
    /*public void RemoveEquip(int equipIndex) // 인벤토리에서만 버릴 수 있음
    {
        StateChange("-" + HoldEquipList[int.Parse(equipIndex)].ItemName);
        HoldEquipList.RemoveAt(int.Parse(equipIndex));
    }*/

    //-----------선택지 함수들-----------
    void NewGame(string data)
    {
        _gold = startGold;          Gold = startGold;
        _hp = startHP;              HP = startHP;
        _mental = startMental;      Mental = startMental;
        _currentTime = Time.밤;     CurrentTime = Time.새벽;
        _stamina = startStamina;    Stamina = startStamina;
        _fame = startFame;          MaxWeight = startMaxWeight;
        PlayerStatus = new Status();
        index = 1;
        Weight = 0;
        Day = 0;
        Log = ""; StateChangeData = "";
        isGameOver = false;
        HoldItemList.Clear(); HoldAbilityDic.Clear(); AllEventDic.Clear(); AllEventListDic.Clear();
        AddItem("검"); AddItem("가죽 투구"); AddItem("가죽 갑옷"); AddItem("가죽 장화"); AddItem("부적"); AddItem("낡은 가죽 가방");
        HoldItemList[0].WearingEquip(); HoldItemList[1].WearingEquip();
        HoldItemList[2].WearingEquip(); HoldItemList[3].WearingEquip();
        HoldItemList[4].WearingEquip(); HoldItemList[5].WearingEquip();
        EventTextRead();
        InventoryPannel.SetActive(false); TradePannel.SetActive(false);
        AllEventDic["Start"].EventInvoke(); // 프롤로그 이벤트 진행
    }
    void SetGold(string goldS){ Gold += int.Parse(goldS); }
    void SetHP(string hpS){ HP += int.Parse(hpS); }
    void SetMental(string mentalS){ Mental += int.Parse(mentalS); }
    void SetDay(string dayS){ Day += int.Parse(dayS); }
    void SetStamina(string staminaS){ Stamina += int.Parse(staminaS); }
    void SetFame(string fameS){ Fame += int.Parse(fameS); }
    public void AddItem(string itemName)
    // 잡화템인지 장비템인지 확인, 장비템이면 인벤토리 확인후 템 추가, 잡화템은 보유중인지 확인후 보유중이면 남았는지 확인
    {
        if(!CheckInvenMax()) // 인벤토리에 남는공간이 있으면
        {
            if((int)ItemDic[itemName].ThisItemFormat >= 2)
            {
                Item it1 = gameObject.AddComponent<Item>();
                it1.ItemUpdate(ItemDic[itemName]);
                it1.ItemIndex = index;
                index++;
                HoldItemList.Add(it1);
                //Weight = GetWeight();
                StateChange("+" + itemName);
                Destroy(it1);
                return;
            }
            int count = 0;
            foreach(Item item in HoldItemList)
            {
                if(item.ItemName == itemName)
                {
                    HoldItemList[count].ItemCount++;
                    //Weight = GetWeight();
                    StateChange("+" + itemName);
                    return;
                } count++;
            }
            Item it = gameObject.AddComponent<Item>();
            it.ItemUpdate(ItemDic[itemName]);
            HoldItemList.Add(it);
            //Weight = GetWeight();
            StateChange("+" + itemName);
            Destroy(it);
        }
        else{ AllResultActionDic["SmallPannelActive"]("인벤토리가 부족합니다."); }
    }
    public void RemoveItem(string itemName)
    {
        for(int i = 0; i < HoldItemList.Count; i++)
        {
            if(HoldItemList[i].ItemName == itemName)
            {
                HoldItemList[i].ItemCount--;
                StateChange("-" + itemName);
                if(HoldItemList[i].ItemCount <= 0) HoldItemList.RemoveAt(i);
                Weight = Weight;
                return;
            }
        }
    }
    void AddAbility(string abilityData)
    {
        string [] values = abilityData.Split(','); // 능력이름, 증가할레벨
        string abilityName = values[0]; int abilityLevel = int.Parse(values[1]);
        StateChange($"{abilityName} Lv +{abilityLevel}");
        if(HoldAbilityDic.ContainsKey(abilityName)){ HoldAbilityDic[abilityName] += abilityLevel; }
        else HoldAbilityDic.Add(abilityName, abilityLevel);
    }
    void RemoveAbility(string abilityData)
    {
        string [] values = abilityData.Split(','); // 능력이름, 증가할레벨
        string abilityName = values[0]; int abilityLevel = int.Parse(values[1]);
        if(HoldAbilityDic.ContainsKey(abilityName))
        {
            StateChange($"{abilityName} Lv -{abilityLevel}");
            HoldAbilityDic[abilityName] -= abilityLevel;
            if(HoldAbilityDic[abilityName] <= 0){ HoldAbilityDic.Remove(abilityName); }
        }
    }
    public void EventInvokeByName(string actionData){ AllEventDic[actionData].EventInvoke(); }
    void NewEvent(string actionData) // 랜덤으로 활성화된 이벤트들 중 하나를 발동, 게임 오버시 재시작
    {
        if(isGameOver)
        {
            /*if(ItemO("행운의 부적"))
            {
                RemoveItem("행운의 부적");
                if(HP<1) HP = 1;
                if(Mental<1) Mental = 1;
                isGameOver = false;
                AllEventDic["Lucky"].EventInvoke();
            }
            else */AllEventDic["GameOver"].EventInvoke();
        }
        else
        {
            CurrentTime = Time.아침;
            Stamina -= 10;
            int rateSum = 0;
            for(int i = 0; i < AllEventListDic["Active"].Count; i++)
            {
                rateSum += AllEventListDic["Active"][i].InvokeRate;
            }
            int ranNum = Random.Range(0,rateSum);
            int totalRate = 0;
            for(int i = 0; i < AllEventListDic["Active"].Count; i++)
            {
                totalRate += AllEventListDic["Active"][i].InvokeRate;
                if(totalRate > ranNum)
                {
                    AllEventListDic["Active"][i].EventInvoke();
                    break;
                }
            }
        }
    }
    void RandomEvent(string eventListName)
    {
        List<Event> evList = AllEventListDic[eventListName];
        int rateSum = 0;
        foreach(Event ev in evList)
        {
            rateSum += ev.InvokeRate;
        }
        int ranNum = Random.Range(0,rateSum);
        int totalRate = 0;
        foreach(Event ev in evList)
        {
            totalRate += ev.InvokeRate;
            if(totalRate > ranNum)
            {
                ev.EventInvoke();
                break;
            }
        }
    }
    //-----------선택지 함수들끝-----------
    //-----------선택지 조건 함수들-----------
    public bool GoldO(string tfData){ return int.Parse(tfData) <= Gold ? true : false; }
    public bool ItemO(string itemName)
    {
        for(int i = 0; i<HoldItemList.Count; i++)
        { 
            if(HoldItemList[i].ItemName == itemName) return true;
        }
        return false;
    }
    bool RandomTF(string rate)
    {
        string [] values = rate.Split('/');
        int a = int.Parse(values[0]); int b = int.Parse(values[1]);
        return Random.Range(0, b) < a ? true : false;
    }
    bool FameO(string fameData){ return int.Parse(fameData)>=Fame ? true : false; }
    bool AbilityO(string abilityData)
    {
        string [] values = abilityData.Split(',');
        string abilityName = values[0]; int level = int.Parse(values[1]);
        if(HoldAbilityDic.ContainsKey(abilityName))
        {
            if(HoldAbilityDic[abilityName] >= level) return true;
        }
        return false;
    }
    //-----------선택지 조건 함수들끝-----------
}