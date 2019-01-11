using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossManager : MonoBehaviour
{
    #region EditorPredefined
    [SerializeField]
    private GameObject selectRoom;
    [SerializeField]
    private GameObject bossPreview;
    [SerializeField]
    private GameObject bossRoom;
    [SerializeField]
    private GameObject bossHealthBar;
    [SerializeField]
    private TextMeshProUGUI TimeTMP;
    #endregion
    private UIController uiController;
    private Boss boss;

    private void Awake()
    {
        this.uiController = GameObject.Find("UI").GetComponent<UIController>();
    }
    private void Update()
    {
        if (boss != null)
        {
            if (boss.timerActive)
            {
                TimeTMP.text = string.Format("{0:0.0}s", boss.Time);
                boss.Time -= Time.deltaTime;
            }
            if (boss.Time < 0)
            {
                boss.Time = 0;
                boss.timerActive = false;
                boss.Vulnerable = false;

            }
        }
    }

    //Leave boss module
    public void BossExit()
    {
        Debug.Log("Leave Boss module");
        uiController.SwitchBossUI();
    }

    //Enable character interface
    public void CharacterInterface()
    {
        Debug.Log("Open character interface");
    }

    //Fight selected boss
    public void Fight()
    {
        Debug.Log("Fight selected boss");
        CreateBoss();
    }

    //Deals damage to boss
    public void HitBoss(Vector2 touchPos)
    {
        if (boss.Vulnerable)
        {
            Debug.Log("Boss hit");
            Tap tap = new Tap(BigFloat.BuildNumber(Random.Range(800, 1200)), false);
            this.boss.TakeDamage(tap.amount);
            HealthbarUpdate(this.boss.HealthPercentage());
            uiController.ShowTapValue(bossRoom, touchPos, tap);
            if (this.boss.HealthPercentage() <= 0)
            {
                BossDied();
            }
        }
        else
        {
            uiController.ShowTapString(bossRoom, touchPos, "Blocked");
        }
    }

    //Boss was killed by the player
    private void BossDied()
    {
        Debug.Log("Boss died");
        boss.Vulnerable = false;
        boss.timerActive = false;
        GameObject.Find("BossModel").transform.Rotate(new Vector3(0,0,-90));
    }

    //Updates boss health bar
    private void HealthbarUpdate(float value)
    {
        Slider slider = bossHealthBar.GetComponent<Slider>();
        slider.value = value;
    }

    private void CreateBoss()
    {
        this.boss = new Boss(BigFloat.BuildNumber(100000), 30);
        this.boss.timerActive = true;
        selectRoom.SetActive(false);
        bossRoom.SetActive(true);
    }
}

public class Boss
{
    public BigFloat MaxHealth;
    public BigFloat CurrentHealth;
    public float Time;
    public bool Vulnerable = true;
    public bool timerActive = false;

    public Boss(BigFloat health, float time)
    {
        this.MaxHealth = health;
        this.CurrentHealth = MaxHealth;
        this.Time = time;
    }

    public void TakeDamage(BigFloat damage)
    {
        this.CurrentHealth -= damage;
    }
    public float HealthPercentage()
    {
        float percentage = (float)(CurrentHealth / MaxHealth);
        return percentage*100;
    }
}