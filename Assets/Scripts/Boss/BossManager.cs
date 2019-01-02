using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    #endregion
    private UIController uiController;
    private Boss boss;

    private void Awake()
    {
        this.uiController = GameObject.Find("UI").GetComponent<UIController>();
    }

    //Leave boss module
    public void BossExit()
    {
        Debug.Log("Leave Boss module");
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
        this.boss = new Boss(BigFloat.BuildNumber(100000), 30);
        selectRoom.SetActive(false);
        bossRoom.SetActive(true);
    }

    //Deals damage to boss
    public void HitBoss(Vector2 touchPos)
    {
        Debug.Log("Boss hit");
        Tap tap = new Tap(BigFloat.BuildNumber(Random.Range(800, 1200)), false);
        this.boss.TakeDamage(tap.amount);
        HealthbarUpdate(this.boss.HealthPercentage());
        uiController.ShowTapValue(bossRoom, touchPos, tap);
    }

    //Updates boss health bar
    private void HealthbarUpdate(float value)
    {
        Slider slider = bossHealthBar.GetComponent<Slider>();
        slider.value = value;
    }
}

public class Boss
{
    public BigFloat MaxHealth;
    public BigFloat CurrentHealth;
    public float Time;

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