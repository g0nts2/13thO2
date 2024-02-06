using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Random = UnityEngine.Random;  //���� ����� ����

public class Entity : MonoBehaviour //�ش� ������ ���� ���ڸ� ���� ��ȹ �׷��� �ٸ� monsterSO�� ����
{
    private Dictionary<string, Action> monsterPatterns = new Dictionary<string, Action>();  //���� ���� �� ���� ���� ������� ����Ǿ� ť�� ����
    [SerializeField] SpriteRenderer entity;
    [SerializeField] SpriteRenderer charater;
    [SerializeField] SpriteRenderer patternUI;
    [SerializeField] TMP_Text healthTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text shieldTMP;
    [SerializeField] GameObject hpline;
    [SerializeField] Sprite AttackUI;
    [SerializeField] Sprite ShieldUI;
    [SerializeField] Sprite EffectUI;
    [SerializeField] Sprite WhatUI;

    List<StatusEffect> myStatusEffect = new List<StatusEffect>();
    public Monster monster;
    public int attack;
    public int maxhealth = 40;
    public int health = 40;
    public int pastHealth;
    float hppercent;
    public int shield = 0;
    public string monsterfunctionname;
    public bool isMine;
    public bool myTurn;
    public bool isDie;
    public bool isDamaged;
    public bool isBossOrEmpty;
    public bool attackable;
    //�����̻� ����
    public bool debuffPoisonBool;
    public int debuffPosionInt = 0;
    public Vector3 originPos;
    public int liveCount = 0;
    public bool canplay = true;
    public bool issleep = false;
    public bool hasmask = false;

    private int pattern;
    private string patternname;
    private bool isfirst = true;   //ù�Ͽ� UI������ ���ؼ� ���� ��ġ
    private int addtionpattern = 0;

    void Start()
    {
        monsterPatterns["Snail"] = () => SnailPattern();
        monsterPatterns["Hcoronatus"] = () => HcoronatusPattern();
        pattern = Random.Range(0,10);
        ExecutePattern(monsterfunctionname);    //isfirst�� �̿��ؼ� ó���� ����� ������ ���ϰ� �� �� 
    }

    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;   
    }
    void OnTurnStarted(bool myTurn)
    {
        if (isBossOrEmpty)
            return;

        if (isMine == myTurn)
        {
            liveCount++;
        }
            
    }
    public void Setup(Monster monster)
    {
        this.monster = monster;
        maxhealth = monster.maxhealth;
        health = monster.health;
        attack = int.Parse(attackTMP.text);
        shield = monster.shield;
        monsterfunctionname = this.monster.monsterfunctionname;

        this.monster = monster;
        charater.sprite = this.monster.sprite;
        healthTMP.text = this.monster.health.ToString();
        shieldTMP.text = this.monster.shield.ToString();
        attackTMP.text = this.monster.attack.ToString();
    }

    private void OnMouseDown()
    {
        if (isMine)
            EntityManager.Inst.EntityMouseDown(this);
    }

    private void OnMouseUp()
    {
        if (isMine)
            EntityManager.Inst.EntityMouseUp();
    }

    private void OnMouseDrag()
    {
        if (isMine)
            EntityManager.Inst.EntityMouseDrag();
    }

    public bool Damaged(int damage)
    {
        healthTMP.text = health.ToString();

        if (health <= 0)
        {
            isDie = true;
            return true;
        }
        return false;
    }

    public int GetHealthTMP()      //SerializeField�� ���� ��ȣ�������� ���� ���� ������ ���
    {
        return int.Parse(healthTMP.text);
    }
    public int GetAttackTMP()      //SerializeField�� ���� ��ȣ�������� ���� ���� ������ ���
    {
        return int.Parse(attackTMP.text);
    }
    public int GetShieldTMP()      //SerializeField�� ���� ��ȣ�������� ���� ���� ������ ���
    {
        return int.Parse(shieldTMP.text);
    }

    public void SetHealthTMP()  //ü���� health�� ����
    {
        if (health >= maxhealth)
            health = maxhealth;
        healthTMP.text = health.ToString();
        hpline.transform.localScale = new Vector3(1 - (float)health/maxhealth, 0.65f, 1f);
    }

    public void SetShieldTMP()
    {
        shieldTMP.text = shield.ToString();
    }
    public int GetLiveCount()
    {
        return liveCount;
    }

    public void MoveTransform(Vector3 pos, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
            transform.DOMove(pos, dotweenTime);
        else
            transform.position = pos;
    }

    public void SetPastHealth()
    {
        pastHealth = health;
    }
    #region MonsterPattern
    #region Snail
    private void SnailPattern()
    {
        if (isfirst)
        {
            isfirst = false;
        }
        else
        {
            int damage = 8;
            damage += GetAllAttackUpEffect();
            damage -= GetAllAttackDownEffect();
            switch (patternname)
            {
                case "attack":
                    CardFunctionManager.Inst.Attack("player", damage, "normal", "monster");
                    break;
                case "effect":
                    CardFunctionManager.Inst.Poison("player", 4);
                    break;
                case "shield":
                    MakeShield(4, 1);
                    break;
                default:
                    break;
            }
        }

        pattern = Random.Range(0, 10);   //������ ���� ����
        switch (pattern)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
                patternname = "attack"; //���� �̹��� ����
                patternUI.sprite = AttackUI;
                break;
            case 5:
            case 6:
                patternname = "effect";
                patternUI.sprite = EffectUI;
                break;
            case 7:
            case 8:
            case 9:
                patternname = "shield";
                patternUI.sprite = ShieldUI;
                break;
        }
    }
    #endregion  Hcoronatus  ����Ͱ� ���� ���� ���ʻ縶�ͷ� �ӽô�ü

    private void HcoronatusPattern()
    {
        if (isfirst)
        {
            isfirst = false;
        }
        else
        {
            int damage = 5;
            damage += GetAllAttackUpEffect();
            damage -= GetAllAttackDownEffect();
            switch (patternname)
            {
                case "attack":
                    CardFunctionManager.Inst.Attack("player", damage, "normal", "monster");
                    CardFunctionManager.Inst.Attack("player", damage, "normal", "monster");
                    break;
                case "effect":
                    MakeAttackUp(2, 2);
                    break;
                case "shield":
                    MakeShield(10, 1);
                    Debug.Log("This monster make Shield");
                    break;
                default:
                    break;
            }
        }

        pattern = Random.Range(0, 10);   //������ ���� ����
        switch (pattern)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
                patternname = "attack"; //���� �̹��� ����
                patternUI.sprite = AttackUI;
                break;
            case 5:
            case 6:
                patternname = "effect";
                patternUI.sprite = EffectUI;
                break;
            case 7:
            case 8:
            case 9:
                patternname = "shield";
                patternUI.sprite = ShieldUI;
                break;
        }
        Debug.Log(patternname);
    }
    #endregion
    #region Utils

    // ���� ���� ���� �޼���
    public void ExecutePattern(string patternName)
    {
        if (monsterPatterns.TryGetValue(patternName, out Action monsterPattern))
        {
            // �ش� ������ ��� ����
            monsterPattern();
        }
        else
        {
            Debug.Log("MonsterPattern not found.");
        }
    }

    #endregion

    #region MakeEffect  //�۵���� ���� ������
    public void MakeAttackUp(int damage, int count)
    {
        Debug.Log("Effect - Attack Up");
        StatusEffect newEffect = new StatusEffect();
        newEffect.SetPowerUp(damage, count);
        myStatusEffect.Add(newEffect);
    }

    public void MakeAttackDown(int damage, int count)
    {
        Debug.Log("Effect - Attack Down");
        StatusEffect newEffect = new StatusEffect();
        newEffect.SetPowerDown(damage, count);
        myStatusEffect.Add(newEffect);
    }

    public void MakeShield(int amount, int turn)
    {
        Debug.Log("Effect - Shield");
        StatusEffect newEffect = new StatusEffect();
        newEffect.SetShield(amount, turn);
        myStatusEffect.Add(newEffect);
        shield += amount;
        SetShieldTMP();
    }

    public void MakeFaint(int turn) //���� ����
    {
        Debug.Log("Effect - Faint");
        StatusEffect newEffect = new StatusEffect();
        newEffect.SetFaint(turn);
        myStatusEffect.Add(newEffect);
    }

    public void MakeSleep(int turn) //���� ����
    {
        Debug.Log("Effect - Sleep");
        StatusEffect newEffect = new StatusEffect();
        newEffect.SetSleep(turn);
        myStatusEffect.Add(newEffect);
        issleep = true;
    }

    public void MakeImmuneSleep(int turn)   //���� �鿪 ����
    {
        Debug.Log("Effect - Immnue Sleep");
        StatusEffect newEffect = new StatusEffect();
        newEffect.SetImmuneSleep(turn);
        myStatusEffect.Add(newEffect);
    }

    public void MakePoison(int turn)
    {
        Debug.Log("Effect - Poison");
        StatusEffect neweffect = new StatusEffect();
        neweffect.SetPoison(turn);
        myStatusEffect.Add(neweffect);
    }

    public void MakeBurn(int damage, int turn)
    {
        Debug.Log("Effect - Burn");
        StatusEffect neweffect = new StatusEffect();
        neweffect.SetBurn(damage, turn);
        myStatusEffect.Add(neweffect);
    }

    public void MakeHealTurn(int turn)
    {
        Debug.Log("Effect - HealTurn");
        StatusEffect neweffect = new StatusEffect();
        neweffect.SetHealTurn(turn);
        myStatusEffect.Add(neweffect);
    }
    #endregion
    public int GetAllAttackUpEffect()   //���ݷ� ���� ȿ�� ��������
    {
        int result = 0;
        foreach(StatusEffect obj in myStatusEffect)
        {
            result += obj.GetAllAttackUp();
        }
        return result;
    }

    public int GetAllAttackDownEffect()   //���ݷ� ���� ȿ�� ��������
    {
        int result = 0;
        foreach (StatusEffect obj in myStatusEffect)
        {
            result += obj.GetAllAttackDown();
        }
        return result;
    }
    public bool GetSleep()  //�ӽÿ�
        {
            int sleep = Random.Range(0, 10);    //0~9�� ����
            foreach (StatusEffect obj in myStatusEffect)
            {
                if (obj.GetImmuneSleep())
                {
                    Debug.Log("I can't sleep");
                    sleep = 100;
                    break;
                }
            }
            Debug.Log(sleep);
            if (sleep < 7)   //0,1,2,3,4,5,6 = 70% = ����
            {
                Debug.Log("I sleep");
                return true;
            }
            Debug.Log("I don't sleep");
            return false;
        }
    public void GetAllCC()
    {
        foreach (StatusEffect obj in myStatusEffect)
        {
            if(obj.GetFaint())
            {
                Debug.Log("You are faint");
                canplay = false;
                break;
            }
        }
    }

    public void CheckEffect()
    {
        for(int i = myStatusEffect.Count - 1; i >= 0; i--)  //�ݵ�� �������� ���� �� 
        {
            Debug.Log(myStatusEffect[i].CheckDamageEffect());
            if (myStatusEffect[i].CheckDamageEffect().Item1)  //�������� ȿ�� ����
            {
                health -= myStatusEffect[i].CheckDamageEffect().Item2;
                SetHealthTMP();
            }
            myStatusEffect[i].DecreaseEffectTurn();
            if(myStatusEffect[i].GetEffectTurn() <= 0)
            {
                myStatusEffect.RemoveAt(i);
            }
        }
    }

    public bool CheckBlockHeal()
    {
        foreach(StatusEffect A in myStatusEffect)
        {
            if (A.GetHealBlock())
                return true;
        }
        return false;
    }
}