using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseCollider : MonoBehaviour
{
    GameObject deckui;
    DeckUIManager deckuimanager;
    bool isclick = false;
    private void Start()
    {
    }
    void Update()
    {
        if (!isclick && Input.GetMouseButtonDown(0))
        {
            isclick = true;
        }
        else if(isclick && Input.GetMouseButtonUp(0))
        {
            isclick = false;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("ButtonNotInUI"));

            if (hit.collider != null && hit.collider.CompareTag("Button1"))
            {
                // ButtonNotInUI ���̾ ����� Collider�� �浹�ϰ�, Door �±׸� ���� ���� ������Ʈ�� Ŭ���Ǿ��� �� ������ �ڵ�
                PresstoStart("MapScene");
                AudioManager.instance.PlaySFX(AudioManager.SFX.openClick); // �ӽ� ȿ����
            }
            if (hit.collider != null && hit.collider.CompareTag("Button2"))
            {
                // ButtonNotInUI ���̾ ����� Collider�� �浹�ϰ�, Door �±׸� ���� ���� ������Ʈ�� Ŭ���Ǿ��� �� ������ �ڵ�
                PresstoStart("DeckBuildScene");
                AudioManager.instance.PlaySFX(AudioManager.SFX.openClick); // �ӽ� ȿ����
            }
        }
    }

    public void PresstoStart(string name)
    {
        SceneManager.LoadScene(name);
    }
}
