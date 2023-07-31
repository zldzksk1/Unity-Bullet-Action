using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCountA;
    public int enemyCountB;
    public int enemyCountC;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public TMP_Text bestScoreTxt;
    public TMP_Text scoreTxt;
    public TMP_Text stageTxt;
    public TMP_Text playTimeTxt;
    public TMP_Text healthTxt;
    public TMP_Text ammoTxt;
    public TMP_Text coinTxt;
    public Image hammerImg;
    public Image handGunImg;
    public Image machineGunImg;
    public Image grenadeImg;
    public TMP_Text EnemyTxtA;
    public TMP_Text EnemyTxtB;
    public TMP_Text EnemyTxtC;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    void Awake()
    {
        bestScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("BestScore"));    
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    void Update()
    {
        if (isBattle)
        {
            playTime += Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        stageTxt.text = "STAGE " + stage;

        //time 
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);

        //player status indicator
        scoreTxt.text = string.Format("{0:n0}", player.score);
        healthTxt.text = player.health + " / " + player.maxHealth;
        coinTxt.text = string.Format("{0:n0}", player.coin);

        //weapon ammo indicator
        if (player.equipWeapon == null)
        {
            ammoTxt.text = " - / " + player.ammo;
        }
        else if (player.equipWeapon.type == Weapons.Type.Melee)
        {
            ammoTxt.text = " ¡Ä / " + player.ammo;

        }
        else if (player.equipWeapon.type == Weapons.Type.Range)
        {
            ammoTxt.text = player.equipWeapon.currAmmo + " / " + player.ammo;
        }

        //weapon indicator
        hammerImg.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        handGunImg.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        machineGunImg.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        grenadeImg.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        //nums of monster you have killed
        EnemyTxtA.text = enemyCountA.ToString();
        EnemyTxtB.text = enemyCountB.ToString();
        EnemyTxtC.text = enemyCountC.ToString();

        //boss healthbar indicator
        bossHealthBar.localScale = new Vector3((float)boss.currHealth / boss.maxHealth, 1, 1);
    }


}
