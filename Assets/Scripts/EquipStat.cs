using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipStat
{
    // 무기
    enum AtkForm{Physical, Magic};
    enum Type{None, Cold, Flame, Ground, Light, Dark};
    int Atk;
    int AtkPer; // 공격력 증가%
    int AtkDelay;   // 100이면 1초에 1번공격, 1이면 0.01초에 1번 공격
    int AtkDelayPer; // 공격주기 감소% -> AtkDelay / ((100 + AtkDelayPer)/100) -> 100이면 공격주기 절반
    int CriRate; // 0~100%, 소숫점 생략
    int CriRatePer; // 치명확률 증가%

    // 방어구
    int HP;
    int Def;
    int DefPer;  // 방어력 증가%
    int MagicDef;
    int MagicDefPer;  // 항마력 증가%
}