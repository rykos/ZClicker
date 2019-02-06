using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildRecrutation : MonoBehaviour
{
    private int heroesCount = 4;
    public GameObject heroPreviewPrefab;
    public GameObject HeroList;

    private void OnEnable()
    {
        GenerateHeroes(heroesCount);
    }

    private void OnDisable()
    {
        DestroyAllHeroes();
    }

    private void GenerateHeroes(int amount)
    {
        heroesCount -= amount;
        for (int i = 0; i < amount; i++)
        {
            var newHero = Instantiate(heroPreviewPrefab, HeroList.transform);
            newHero.GetComponent<GuildHero>().Hero = new Hero("Huber", new Level((uint)Random.Range(1, 100), 0));
        }
    }

    private void DestroyAllHeroes()
    {
        for (var i = 0; i < HeroList.transform.childCount; i++)
        {
            Destroy(HeroList.transform.GetChild(i).gameObject);
        }
    }
}
