using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    Player enterPlayer;

    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    public TMP_Text conversation;
    public string[] sentences; 


    // Function to get into a shop
    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    // Function to get out from a shop
    public void Exit()
    {
        anim.SetTrigger("doHello");
        //Debug.Log(enterPlayer.isShop);
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int idx)
    {
        if (enterPlayer == null || enterPlayer.isShop == false)
            return;

        int price = itemPrice[idx];

        if (price > enterPlayer.coin )
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.coin -= price;

        //item position adjustment
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3,3);
        Instantiate(itemObj[idx], itemPos[idx].position + ranVec, itemPos[idx].rotation);
    }

    IEnumerator Talk()
    {
        //talk back to player
        conversation.text = sentences[1];
        yield return new WaitForSeconds(2f);

        //back to original sentence
        conversation.text = sentences[0];

    }
}
