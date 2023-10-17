using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static CardManager Inst { get; private set;} //싱글톤


    private void Awake()
    {
        Inst = this; // 싱글톤 인스턴스 설정
        cardfuction = GetComponent<CardFunctionManager>(); // CardFunction 컴포넌트 가져오기
        //cardfuction.SetCardManager(this); // CardFunctionManager 클래스에 CardManager 인스턴스 주입
    }

    [SerializeField] ItemSO itemSO;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] List<Card> myCards;
    [SerializeField] Transform cardSpawnPoint;
    //[SerializeField] Transform otherCardSpqwnPoint;
    [SerializeField] Transform myCardLeft;
    [SerializeField] Transform myCardRight;
    [SerializeField] ECardState eCardState;
    //[SerializeField] Item item;   //왜 넣었던거지
    List<Item> itemBuffer;
    Card selectCard;    //선택된 카드 담음
    CardFunctionManager cardfuction;
    public CostManager costManager;
    bool isMyCardDrag;
    bool onMyCardArea;
    enum ECardState { Nothing, CanMouseOver, CanMouseDrag }
    int myPutCount; //엔티티 스폰
    bool intrusionencore = false;
    bool intrusioncounter = false;

    private List<string> intrusionList = new List<string>();

    public Item PopItem()   //맨앞의 카드 빼는 용도
    {
        if (itemBuffer.Count == 0)
            SetupItemBuffer();

        Item item = itemBuffer[0];
        itemBuffer.RemoveAt(0);
        return item;
    }
    void SetupItemBuffer()  //카드 순서 랜덤으로 바뀌게 하는 용도
    {
        itemBuffer = new List<Item>();
        for(int i = 0; i < itemSO.items.Length; i++)
        {
            Item item = itemSO.items[i];
            for (int j = 0; j < item.percent; j++)
                itemBuffer.Add(item);
        }    

        for (int i = 0; i < itemBuffer.Count; i++ )
        {
            int rand = Random.Range(i, itemBuffer.Count);
            Item temp = itemBuffer[i];
            itemBuffer[i] = itemBuffer[rand];
            itemBuffer[rand] = temp;
        }
    }


    void Start()
    {
        SetupItemBuffer();
        TurnManager.OnAddCard += AddCard;
    }

    void OnDestroy()
    {
        TurnManager.OnAddCard -= AddCard;    
    }

    void Update()
    {
        if (isMyCardDrag)
            CardDrag();

        DetectCardArea();
        SetECardState();
    }
    
    void AddCard(bool isMine)
    {
        var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Utils.QI);
        var card = cardObject.GetComponent<Card>();
        card.Setup(PopItem(), isMine);
        if(isMine)
            myCards.Add(card);
            
        SetOriginOrder(isMine);
        CardAlignment();
    }

    void SetOriginOrder(bool isMine)
    {
        int count = myCards.Count;
        for (int i = 0; i < count; i++)
        {
            var targetCard = myCards[i];
            targetCard.GetComponent<Order>().SetOriginOrder(i);
        }
    }

   void CardAlignment()  // 카드 정렬
    {
        List<PRS> originCardPRSs = new List<PRS>();
        originCardPRSs = RoundAlignment(myCardLeft, myCardRight, myCards.Count, 0.5f, Vector3.one * 0.05f);  //맨 끝 사이즈 수정 시 카드 사이즈 변경됨
        for (int i = 0; i < myCards.Count; i++)
        {
            var targetCard = myCards[i];
            targetCard.originPRS = originCardPRSs[i];
            targetCard.MoveTransform(targetCard.originPRS, true, 0.7f);

        }
    }

    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1: objLerps = new float[] { 0.5f }; break;
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        for (int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            var targetRot = Utils.QI;
            if(objCount >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                curve = height >= 0 ? curve : -curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }

        return results;
    }

    public bool TryPutCard(bool isMine)    //엔티티 스폰
    {
        Card card = isMine ? selectCard : selectCard;
        var targetCards = isMine ? myCards : myCards;

        targetCards.Remove(card);
        card.transform.DOKill();
        DestroyImmediate(card.gameObject);
        if(isMine)
        {
            selectCard = null;
            myPutCount++;
        }
        CardAlignment();
        return true;
    }

    public void UseCard()
    {
        if (cardfuction != null)
        {
            GetSelectCardType(selectCard.cardtype, selectCard.functionname);
            cardfuction.UseSelectCard(selectCard.functionname);
        }
    }

    #region MyCard
    public void CardMouseOver(Card card)    //카드 위에 마우스를 올려 놓을 때(사용 상태 X) 
    {
        if (eCardState == ECardState.Nothing)
            return;

        selectCard = card;
        EnlargeCard(true, card);
    }

    public void CardMouseExit(Card card)    //카드 위에 마우스를 뗄 때(사용 상태 X) 
    {
        EnlargeCard(false, card);
    }

    public void CardMouseDown() //카드 사용 중 마우스 누를 때
    {
        if (eCardState != ECardState.CanMouseDrag)
        {
            return;    
        }
        if(selectCard.cardtype =="Intrusion")   //클릭 시 현재 난입 개수, 중복 확인 용도
        {
            if (IsFullList())
            {
                GameManager.Inst.Notification("난입은 최대 5개다.");
                return;
            }
            if (IsIntrusionDuplication(selectCard.functionname))
            {
                GameManager.Inst.Notification("중복된 난입은 허용하지 않는다.");
                return;
            }
                
        }
        if (costManager.CompareCost(selectCard))    //코스트 비교
        {   
            isMyCardDrag = true;
            if(onMyCardArea)
            {
                return;
            }

        }
        GameManager.Inst.Notification("코스트가 부족하다");
    }

    public void CardMouseUp()   //마우스를 뗄 때 카드 사용
    {
        isMyCardDrag = false;

        if (eCardState != ECardState.CanMouseDrag)
            return;
        if (selectCard.cardtype == "Intrusion")
        {
            if (IsFullList())
            {
                return;
            }
            if(IsIntrusionDuplication(selectCard.functionname))
            {
                return;
            }

        }

        if (costManager.CompareCost(selectCard))
        {
            if (onMyCardArea)
            {
            }
            else
            {
                if (selectCard.selectable)
                {
                    GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
                    foreach(GameObject obj in monsters)
                    {
                        if (IsMouseCollidingWithObject(obj))
                        {
                            cardfuction.SetTarget(obj);
                        }
                    }
                }
                CostManager.Inst.SubtractCost(selectCard);
                CostManager.Inst.ShowCost();
                UseCard();
                IntrusionConditionCheck();
                EntityManager.Inst.FindDieEntity();
                TryPutCard(true);
                EntityManager.Inst.CheckBuffDebuff();
            }
        }
    }

    void CardDrag() //카드 드래그 중일 때
    {
        if (!onMyCardArea && !selectCard.selectable)
        {
            selectCard.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectCard.originPRS.scale), false);
        }
        else if(!onMyCardArea && selectCard.selectable)     //선택 해야하는 케이스에 사용
        {
            selectCard.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectCard.originPRS.scale), false);
            //Debug.Log("select");
        }
        
    }

    void DetectCardArea()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);
        int layer = LayerMask.NameToLayer("MyCardArea");
        onMyCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);
    }

    void EnlargeCard(bool isEnlarge, Card card) //카드 확대
    {
        if (isEnlarge)
        {
            Vector3 enlargePos = new Vector3(card.originPRS.pos.x, -3.0f, -0.1f);
            card.MoveTransform(new PRS(enlargePos, Utils.QI, Vector3.one * 0.1f), false);
        }
        else
            card.MoveTransform(card.originPRS, false);

        card.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
    }

    void SetECardState()
    {
        if (TurnManager.Inst.isLoading)
            eCardState = ECardState.Nothing;

        else if (!TurnManager.Inst.myTurn)
            eCardState = ECardState.CanMouseOver;

        else if (TurnManager.Inst.myTurn)
            eCardState = ECardState.CanMouseDrag;     
    }

    #endregion
    //난입
    #region Intrusion 
    //앙코르
    #region Encore
    public void SetIntrusionEncore()
    {
        intrusionencore = true;
    }
    public bool ConditionIntrusionEncore()  //앙코르 조건 확인
    {
        if (selectCard.cardtype == "Action" && EntityManager.Inst.IsDieEntity())
            return true;
        return false;
    }
    public void UseIntrusionEncore()    //앙코르 능력 발동
    {
        cardfuction.UseSelectCard(selectCard.functionname);
        intrusionList.Remove("Encore"); //리스트 지우기
    }
    #endregion
    //반격
    #region Counter
    public void SetIntrusionCounter()
    {
        intrusioncounter = true;
    }
    public bool ConditionIntrusionCounter()  //반격 조건 확인
    {
        if (selectCard.cardtype == "Action" && EntityManager.Inst.IsDieEntity())
            return true;
        return false;
    }

    public void UseIntrusionCounter()    //반격 능력 발동
    {
        cardfuction.UseSelectCard(selectCard.functionname);
        intrusionList.Remove("Counter"); //리스트 지우기
    }

    #endregion

    public void GetSelectCardType(string type, string functionName)
    {
        if (type == "Intrusion")
        {
            intrusionList.Add(functionName);
        }
    }

    public bool IsIntrusionDuplication(string function)
    {
        foreach(string key in intrusionList)
        {
            if(key == function)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsFullList()    //설치된 난입 카드의 개수 설정
    {
        if (intrusionList.Count >= 5)
        {
            return true;
        }
        return false;
    }

    public void IntrusionConditionCheck()   //조건 확인용
    {
        if(intrusionencore == true && ConditionIntrusionEncore() == true)   //앙코르 조건 확인
        {
            UseIntrusionEncore();
        }
    }
    #endregion

    bool IsMouseCollidingWithObject(GameObject obj)
    {
        // 원하는 레이어 마스크 설정 (예: "Monster" 레이어만 고려)
        int layerMask = LayerMask.GetMask("Entity");

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hitInfo = Physics2D.Raycast(mousePosition, Vector2.zero, float.MaxValue, layerMask);

        if (hitInfo.collider != null && hitInfo.collider.gameObject == obj)
        {
            // 마우스와 obj가 충돌한 경우
            return true;
        }

        return false; // 충돌하지 않은 경우
    }
}
