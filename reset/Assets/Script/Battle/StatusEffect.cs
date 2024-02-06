public class StatusEffect  //���� ������ ȿ���� ���� ������
{
    //���� ��ȹ - Entity�� �ݺ��۾����� ���� �з��� �������� �������� ���� �ڵ带 ������ ��� ��������
    //�� �������� Set�̸�()���� �������� �޼��� ���
    //SetEffect(Entity target, string name, int turn, int amount = -1)�� ���� ������ ����
    bool ispowerUp = false;
    bool ispowerDown = false;
    bool isshield = false;   //���� ���� ����
    bool isfaint = false;    //���� ���� ����
    bool issleep = false;    //���� ���� ����
    bool isdamageeffect = false;    //���ظ� �ִ��� ����
    bool isimmunesleep = false;
    bool canheal = true;
    int effectamount = 0;    //ȿ���� ��
    int effectturn = 0;    //���� �� ��
    string effectname;
    #region PowerUp
    public void SetPowerUp(int amount, int turn)
    {
        effectamount = amount;
        effectturn = turn;
        ispowerUp = true;
        effectname = "powerup";
    }

    public int GetAllAttackUp()
    {
        if (ispowerUp)
        {
            return effectamount;
        }
        return 0;
    }
    #endregion

    #region PowerDown
    public void SetPowerDown(int amount, int turn)
    {
        effectamount = amount;
        effectturn = turn;
        ispowerDown = true;
    }

    public int GetAllAttackDown()
    {
        if (ispowerDown)
        {
            return effectamount;
        }
        return 0;
    }
    #endregion

    #region Shield
    public void SetShield(int amount, int turn)
    {
        effectamount = amount;
        effectturn = turn;
        isshield = true;
    }
    #endregion

    #region Faint
    public void SetFaint(int turn)  //���� ����
    {
        effectturn = turn;
        isfaint = true;
    }

    public bool GetFaint()  //�ش� ��ġ���� ���� �鿪 üũ
    {
        if (isfaint)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region Sleep
    public void SetSleep(int turn)
    {
        effectturn = turn;
    }

    /*public bool GetSleep()  //����� �� canplay�� �ٷ� ������
    {
        int sleep = Random.Range(0, 10);    //0~9�� ����
        Debug.Log(sleep);
        if(sleep < 7)   //0,1,2,3,4,5,6 = 70% = ����
        {
            return false;
        }
        return true;
    }*/

    public void SetImmuneSleep(int turn)
    {
        effectturn = turn;
        isimmunesleep = true;
    }

    public bool GetImmuneSleep()
    {
        if (isimmunesleep)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region Poison
    public void SetPoison(int turn)
    {
        effectturn = turn;
        isdamageeffect = true;
    }
    #endregion

    #region Burn
    public void SetBurn(int damage, int turn)
    {
        effectturn = turn;
        effectamount = damage;
        canheal = false;    //�̰� �־�� ȸ�� �Ұ��� �߰� ������
        effectname = "burn";
    }
    #endregion

    #region HealBlock
    public void SetHealBlock(int turn)
    {
        effectturn = turn;
        canheal = false;
    }

    public bool GetHealBlock()
    {
        return canheal;
    }
    #endregion

    #region HealTurn
    public void SetHealTurn(int turn)
    {
        effectturn = turn;
        effectname = "healturn";
    }
    #endregion
    public void DecreaseEffectTurn()
    {
        effectturn--;
    }

    public int GetEffectTurn()
    {
        return effectturn;
    }

    public (bool, int) CheckDamageEffect()
    {
        switch (effectname)
        {
            case "poison":
                return (true, effectturn);
            case "burn":
                return (true, effectamount);
            case "healturn":
                if (!canheal)   //ȸ�� �Ұ���� 0 ȸ��
                    return (true, 0);
                return (true, -effectturn);   //���̶� ������� �ݴ� 
            default:
                return (false, 0);
        }
    }
}