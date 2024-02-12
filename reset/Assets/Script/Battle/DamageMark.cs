using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageMark : MonoBehaviour
{
    [SerializeField] TMP_Text damageTMP;
    Transform transform;
    public float speed = 0.5f;
    public float time = 1.0f;
    int damage;
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        Destroy(gameObject, time);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, speed * Time.deltaTime, 0);
        Debug.Log(speed * Time.deltaTime);
    }

    public void SetDamage(int damage)   //����� ���� ���
    {
        this.damage = damage;
        Debug.Log(damage);
        damageTMP.text = damage.ToString();
    }
    //1. 2�ʰ� �ö󰡴� ���
    //2. 2�� �� �����ϴ� ���
    //3. ����� ���� �޾Ƽ� ���ڸ� ǥ���ϴ� ���
}