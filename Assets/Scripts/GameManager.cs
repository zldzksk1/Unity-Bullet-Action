using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss = null;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCountA;
    public int enemyCountB;
    public int enemyCountC;
    public int enemyCountD;
    public int totalEnemies = 0;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public TMP_Text currScoreText;
    public TMP_Text bestScoreNotiText;
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

    public TMP_Text notification;

    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject stageZone;

    //
    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    void Awake()
    {
        enemyList = new List<int>();
        bestScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("BestScore"));

        if (PlayerPrefs.HasKey("BestScore"))
            PlayerPrefs.SetInt("BestScore", 0);
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
        EnemyTxtA.text = "X  " + enemyCountA.ToString();
        EnemyTxtB.text = "X  " + enemyCountB.ToString();
        EnemyTxtC.text = "X  " + enemyCountC.ToString();

        //boss healthbar indicator
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.currHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 300;
        }

    }

    public void updateNotification(bool isTimer, int number)
    {
        if (isTimer && number >= 0)
        {
            notification.text = "STAGE START IN " + number;

            if (number < 1)
            {
                notification.text = "START!";
            }
        }
        else if (!isTimer && number >= 0)
        {
            notification.text = "PRESS E";
        }

        else if (!isTimer)
        {
            notification.text = "";
        }

    }

    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        stageZone.SetActive(false);

        foreach (Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(true);
        }

        isBattle = true;
        StartCoroutine(InBattle());

    }

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f;
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        stageZone.SetActive(true);

        foreach (Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(false);
        }

        isBattle = false;
        stage++;
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        currScoreText.text = scoreTxt.text;

        int bestScore = PlayerPrefs.GetInt("BestScore");
        if (player.score > bestScore)
        {
            bestScoreNotiText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("BestScore", bestScore);
        }

    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator InBattle()
    {
        //enemy create
//        yield return new WaitForSeconds(1f);

        updateNotification(false, -1);

        if (stage % 5 == 0)
        {
            enemyCountD++;
            totalEnemies++;

            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemies enemy = instantEnemy.GetComponent<Enemies>();
            enemy.manager = this;

            enemy.target = player.transform;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCountA++;
                        totalEnemies++;
                        break;
                    case 1:
                        enemyCountB++;
                        totalEnemies++;
                        break;
                    case 2:
                        enemyCountC++;
                        totalEnemies++;
                        break;
                }
            }

            //instatiate on map
            while (enemyList.Count > 0)
            {
                int ranZon = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZon].position, enemyZones[ranZon].rotation);

                Enemies enemy = instantEnemy.GetComponent<Enemies>();
                enemy.target = player.transform;
                enemy.manager = this;
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(4f);
            }

        }

        while (totalEnemies > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);
        boss = null;
        StageEnd();
    }

    public void countKill(Enemies.Type type)
    {
        switch (type)
        {
            case Enemies.Type.A:
                enemyCountA--;
                break;
            case Enemies.Type.B:
                enemyCountB--;
                break;
            case Enemies.Type.C:
                enemyCountC--;
                break;
            case Enemies.Type.D:
                enemyCountD--;
                break;
        }

        totalEnemies--;
    }
}
