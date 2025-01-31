using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageMark : MonoBehaviour
{
    [SerializeField] TMP_Text damageTMP;
    [SerializeField] SpriteRenderer spriteRenderer;
    Transform transform;
    public float speed = 0.5f;
    public float time = 1.0f;
    int damage;
    Order order;
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        Destroy(gameObject, time);
        order = GetComponent<Order>();
        order.SetOrder(spriteRenderer.sortingOrder);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, speed * Time.deltaTime, 0);
    }

    public void SetDamage(int damage)   //대미지 설정 기능
    {
        this.damage = damage;
        damageTMP.text = damage.ToString();
    }
    //1. 2초간 올라가는 기능
    //2. 2초 뒤 삭제하는 기능
    //3. 대미지 값을 받아서 숫자를 표시하는 기능
}