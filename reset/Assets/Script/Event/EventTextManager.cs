using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EventTextManager : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text texts; //�̺�Ʈ ������ �� ��
    public TMP_Text selectionExplainText; //������ ���� �� ��
    public GameObject selections;
    string selectedButton;
    SaveData savedata;

    int playerHp;
    int playerMaxHp;

    int textFlow = 0; //�ؽ�Ʈ ���� ǥ��

    void Start()
    {
        savedata = GameObject.Find("SaveData").GetComponent<SaveData>();
        ChangeText();
        //����Ǿ��ִ� HP �� MaxHP ������
        playerHp = 0;
        playerMaxHp = 0;
        AudioManager.instance.PlayBGM(AudioManager.BGM.riddle);
    }

    void EventClose()
    { //�̺�Ʈ�� �����鼭 �ٲ� ������ ���Ӱ� ���
        savedata.SetPlayerHealth(savedata.GetPlayerHealth() + playerHp);
        savedata.SetPlayerMaxHealth(savedata.GetPlayerMaxHealth() + playerMaxHp);
        SceneManager.LoadScene("MapScene");
    }

    public void Select()
    { //��ư ����
        //���� ������ ��ư�� name�� ������
        string clickObjName = EventSystem.current.currentSelectedGameObject.gameObject.name;

        print("��ư Ŭ�� Ȯ�ο�: " + clickObjName);

        if (selectedButton == clickObjName)//Ȯ������ ���� �̹� �����ߴ� ��ư�� �� ���� ������ ��� ���� 
        {
            //���� ���� �ϸ� �ٷ� ����ǰ� Map���� �Ѿ�µ� ���߿� 
            //��� UI â�� ���� �Ѿ� �� �� �ֵ��� �����ؾ���
            switch (selectedButton)
            {
                case "Blue":
                    Blue();//�޼��� ����
                    break;
                case "Red":
                    Red();//�޼��� ����
                    break;
                case "None":
                    None();//�޼��� ����
                    break;
            }
            return;
        }
        switch (clickObjName) //������ �׸� ���� ���� �����ֱ�
        {
            case "Blue":
                selectionExplainText.text = selectionsExplains[0];
                AudioManager.instance.PlaySFX(AudioManager.SFX.openClick); // �ӽ�
                break;
            case "Red":
                selectionExplainText.text = selectionsExplains[1];
                AudioManager.instance.PlaySFX(AudioManager.SFX.openClick); // �ӽ�
                break;
            case "None":
                selectionExplainText.text = selectionsExplains[2];
                AudioManager.instance.PlaySFX(AudioManager.SFX.openClick); // �ӽ�
                break;
        }
        selectedButton = clickObjName; //������ ��ư ����

    }
    #region
    //�̺�Ʈ ��ư ���� string 
    string[] selectionsExplains =
    {
        "ü�� 8 ȸ��,\n �ִ� ü���� �����ʴ´�.",
        "50% Ȯ���� �ִ� ü�� 10����, \n�Ǵ� 50%Ȯ���� ���� ü�� 10����",
        "������ ������ �ֿ��Դ°� �ƴϷ���! ������ ���� ����.\n\n��ȭ�� ����."
    };
    void Blue()
    {
        //ü�� 8ȸ�� �ִ�ü�� �� �Ѱ�
        playerHp += 8;
        print("ü���� ȸ���ߴ�! ����ü��: " + playerHp.ToString());
        EventClose();

    }
    void Red()
    {
        //50/50���� �ִ�ü�� 10����or ����ü�� 10����, ���� ���� ����
        //���� ���� ��� �� �־ ���߿� �߰��ؾ��� 
        int rand = UnityEngine.Random.Range(0, 2);
        if (rand == 0)
        {
            playerMaxHp += 10;
            print("�ִ�ü�� ����! �ִ� ü��:" + playerMaxHp.ToString());
        }
        else
        {
            playerHp -= 10;
            print("ü�� ���� ����ü��:" + playerHp.ToString());
        }
        EventClose();

    }
    void None()
    {
        print("�ƹ��ϵ� �Ͼ�� �ʾҴ�.");
        EventClose();
        //�ƹ� �� ����
    }
    #endregion//�̺�Ʈ ������ �޼���
    public void ChangeText()
    {
        if (textFlow == event1.Length - 1)
        {
            texts.text = event1[textFlow];
            texts.gameObject.SetActive(false);
            selections.gameObject.SetActive(true);
            //�� ��ȯ
        }
        else
        {

            texts.text = event1[textFlow];
            textFlow++;
        }

    }

    //�̺�Ʈ ��ũ��Ʈ �ε� ���߿� scv ������ �е��� �ٲٴ����� ��������� ���� �� �� ���� ���� �ֽ��ϴ�.
    string[] event1 = {
        "-������ ����-",
        "���ڶ� ���׷κ��� �������� ���� �Ȱ� �ִ� ��\n" +
            "�׳� �տ��� ��ġ ������� ���긣���� ������ �� ������ ��Ÿ����.\n\n" +
            "�̿� �������� ǥ��. ���𰡸� ���Ϸ��� ����...\n\n �����ǰ� ���� �� ������" +
            "���� ���׷κ꿡 �־��� \n������ ��Ȥ���� �����ִ� ���ϴ�.",
        "�������� ���긣�� ���ڰ� ���ڶ󿡰� ���� �ǳٴ�.\n\n",
            "���װ� ���� ������ ����� ������ �� �� ���� ������� �ϳ׿�.\n" +
            "���������� ������ ���� �ڸ��� ��Ű�ٰ� �ᱹ��\n\n" +
            "����.. ���׷κ��� �Ϻΰ� �� �츮�� �ڶ����� �������̽ʴϴ١�",
        "�� ������ �ִ� ���Ŵ� ����� �ʴ°� ���� �ſ���.. �Դٰ� ��Ż���� ������ �� ģ���� �ְŵ��.��",
        "�� ���� ��ġ�� ���ڴ� �ڸ��� �����.",
        "�� �������� Ŀ�ٶ� ���Ű� �����ִ�.\n\n" +
            "�ϳ��� ���� Ǫ�� ���Ű� �� �ٸ� �ϳ��� ���� ���Ŵ�.\n" +
            "���ڶ�� ��ħ ���� ���� �� ���Ÿ� ������ ����ϴ� ���̴�.",
        "a"
    };


}
