using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    public Item item;
    public List<Item> items;
    [SerializeField] Image coloruiimage;
    [SerializeField] Image costuiimage;
    [SerializeField] SpriteRenderer colorimg;
    [SerializeField] SpriteRenderer costcolor;
    [SerializeField] TMP_Text nameTMP;
    public string functionname;
    public string cardtype;
    public bool selectable;
    private void Start()
    {

    }

    public void ResetImage()
    {
        print("ī�� �̹��� ����");
        this.GetComponent<Image>().sprite = null;
    }
    public void Setup(Item item)    //Card.cs�� �����ϰ� ������ �ڵ� -���: ī�� ����
    {
        this.item = item;
        colorimg.sprite = this.item.colorimg;
        costcolor.sprite = this.item.costcolor;
        //character.sprite = this.item.sprite;
        coloruiimage.sprite = this.item.colorimg;       //UI���� ���Ƿ��� Image������Ʈ�� �����ؾ���
        costuiimage.sprite = this.item.costcolor;       //SO������ SpriteRender�� �̹����� 2�� �ִ½����� ��
        //characterui.sprite = this.item.sprite;          //�ʹ� ��Ŵٺ��� ���� ��Ȳ�� �׽�Ʈ ���� ���߿� ü�� ���ƿ��� �׽�Ʈ �غ���
        nameTMP.text = this.item.name;
        //costTMP.text = this.item.cost.ToString();
        //acitveTMP.text = this.item.active;
        functionname = this.item.functionname;
        cardtype = this.item.cardtype;
        selectable = this.item.selectable;

        if (this.item.color == 'R')  //���� ���� �����ϸ� �۾� �� �ٲ�
        {
            nameTMP.color = new Color32(255, 88, 88, 255);
            //costTMP.color = new Color32(255, 88, 88, 255);
        }
        else if (this.item.color == 'G')
        {
            nameTMP.color = new Color32(88, 255, 88, 255);
            //costTMP.color = new Color32(88, 255, 88, 255);
        }
        if (this.item.color == 'B')
        {
            nameTMP.color = new Color32(88, 88, 255, 255);
            //costTMP.color = new Color32(88, 88, 255, 255);
        }
    }
}
