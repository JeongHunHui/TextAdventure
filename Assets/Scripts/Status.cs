using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public string StatusName; // 누구의 능력치인가?
    // --------공격 관련 스탯(무기류에 의해서 상승)--------
    public enum AtkForm{물리, 마법};
    public AtkForm ThisAtkForm;
    public enum Type{무, 냉기, 불꽃, 대지, 빛, 어둠};
    public Type ThisType;
    // 냉기 > 불꽃 > 대지 > 냉기 | 빛 >< 어둠 (유리한 상성 > 불리한 상성)
    // 유리한 상성으로 공격하면 가하는 피해가 1 증가한다. 불리한 상성으로 공격하면 공격력이 0.5배가 된다.
    // 빛과 어둠은 서로 가하는 피해가 1 증가한다. 대신에 서로 공격할 때 공격력이 0.5배가 된다.

    public int Atk;         // 공격력:      공격을 가할 때 입히는 데미지
    public int AtkDelay;    // 공격주기:    공격을 하는 주기, 11 이면 1.1초 마다 공격
    public int CriRate;     // 치명확률:    공격시 치명타가 발생할 확률, 10 이면 10% 확률로 치명타를 가함. 치명타는 데미지가 2배로 증가함. 100을 넘을 수 없음.

    // --------방어 관련 스탯(방어구에 의해서 상승)--------
    public int HP;          // 체력:        데미지를 받으면 체력이 닳고, 0이되면 전투에서 패배함
    public int Def;         // 방어력:      물리공격을 받으면 상대의 공격력과 비교해서 방어할 확률이 정해짐
    public int MagicDef;    // 항마력:      마법공격을 받으면 상대의 공격력과 비교해서 방어할 확률이 정해짐

    // --------% 스탯 및 능력(장신구에 의해서 상승)--------
    public int AtkPer;      // 공격력%:     x% 만큼 공격력이 상승
    public int DefPer;      // 방어력%:     x% 만큼 방어력이 상승
    public int MagicDefPer; // 항마력%:     x% 만큼 항마력이 상승
    public Dictionary<string, int> AbilityDic; // 능력 들을 저장하는 딕셔너리

    // --------기타 스탯들(버프, 기타 사유에 의해서 상승)--------
    public int AtkDelayPer; // 공격주기%:   x% 만큼 공격 주기 감소
    public int CriRatePer;  // 치명확률%:   x% 만큼 치명확률이 상승

    // 물리 | 화염속성 | 공격력:19348 | 공격주기:1.23초 | 치명확률:100% | 단단한 나무로 만든 활이다.
    
    // 전투 방식: 공격 주기마다 공격을 하며, 공격 시 100*(1+1/((공격력/방어력or항마력)^2+1))%의 확률로 공격에 성공하며,
    // 불리한 상성이면 계산 시 공격력이 0.5배가 되며, 유리한 상성이면 피해가 1증가, 치명타 발생 시 피해량이 2배로 증가한다.
    // 그리고 피해량 만큼 체력이 줄어들고 먼저 체력이 0 이하가 된 사람이 패배한다.
    public Status()
    {
        StatusName = "나";
        ThisAtkForm = AtkForm.물리;
        ThisType = Type.무;
        Atk = 10;
        AtkDelay = 20;
        CriRate = 10;
        HP = 3;
        Def = 10;
        MagicDef = 5;
        AtkPer = 0;
        DefPer = 0;
        MagicDefPer = 0;
        AbilityDic = new Dictionary<string, int>();
        AtkDelayPer = 0;
        CriRatePer = 0;
    }

    public Status(string statusName, string atkForm, string type, string atk, string atkDelay, string criRate, 
        string hp, string def, string magicDef, string abilityData)
    {
        StatusName = statusName;
        ThisAtkForm = (AtkForm)System.Enum.Parse(typeof(AtkForm), atkForm);
        ThisType = (Type)System.Enum.Parse(typeof(Type), type);
        Atk = int.Parse(atk);
        AtkDelay = int.Parse(atkDelay);
        CriRate = int.Parse(criRate);
        HP = int.Parse(hp);
        Def = int.Parse(def);
        MagicDef = int.Parse(magicDef);
        AtkPer = 0;
        DefPer = 0;
        MagicDefPer = 0;
        AbilityDic = new Dictionary<string, int>();
        AtkDelayPer = 0;
        CriRatePer = 0;
    }

    public void StatusUpdate(int minusPlus, int itemFormat, string statusData)
    {
        string [] values = statusData.Split(',');
        switch (itemFormat)
        {
            case 2: // 공격형태, 속성, 공격력, 공격주기, 치명확률
                if(values[0] == "물리" || minusPlus == -1) ThisAtkForm = AtkForm.물리;
                else ThisAtkForm = AtkForm.마법;
                if(minusPlus == -1) ThisType = Type.무;
                else switch(values[1])
                {
                    case "무":
                        ThisType = Type.무;
                        break;
                    case "냉기":
                        ThisType = Type.냉기;
                        break;
                    case "불꽃":
                        ThisType = Type.불꽃;
                        break;
                    case "대지":
                        ThisType = Type.대지;
                        break;
                    case "빛":
                        ThisType = Type.빛;
                        break;
                    case "어둠":
                        ThisType = Type.어둠;
                        break;
                    default:
                        break;
                }
                Atk += int.Parse(values[2]) * minusPlus;
                if(minusPlus == 1) AtkDelay = int.Parse(values[3]);
                else AtkDelay = 20;
                CriRate += int.Parse(values[4]) * minusPlus;
                break;
            case 3:
            case 4:
            case 5: // HP, 방어력, 항마력
                HP += int.Parse(values[0]) * minusPlus;
                Def += int.Parse(values[1]) * minusPlus;
                MagicDef += int.Parse(values[2]) * minusPlus;
                break;
            case 6: // 공격력%, 방어력%, 항마력%, 능력
                if(values[0] != ""){ AtkPer += int.Parse(values[0]) * minusPlus; }
                if(values[1] != ""){ DefPer += int.Parse(values[1]) * minusPlus; }
                if(values[2] != ""){ MagicDefPer += int.Parse(values[2]) * minusPlus; }
                if(values[3] != "")
                {
                    /*string [] abilitys = values[3].Split(',');
                    foreach(string ability in abilitys)
                    {
                        string [] datas = ability.Split('_');
                        //능력 딕셔너리에 능력 더하기 코드로 구현하기
                    }*/
                }
                break;
            case 7: // 가방 칸수
                GameManager.GM.MaxWeight += int.Parse(values[0]) * minusPlus;
                break;
            default:
                break;
        }    
    }

    public string StatusText()
    {
        return $"전투력:{GetCombetStat()} HP:{HP} Def:{Def} MagicDef:{MagicDef} Atk:{Atk} AtkDelay:{AtkDelay} CriRate:{CriRate} Form:{ThisAtkForm} Type:{ThisType}";
    }

    public int GetCombetStat()
    {
        return Mathf.CeilToInt((float)Atk * (15f/(float)AtkDelay) * (1f+(float)CriRate/100f) + ((float)Def+(float)MagicDef)/2f + (float)HP*2f);
    }
}
