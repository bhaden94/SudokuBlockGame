using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public List<GameObject> bonusList;

    private void Start()
    {
        GameEvents.ShowBonusScreen += ShowBonusScreen;
    }

    private void OnDisable()
    {
        GameEvents.ShowBonusScreen -= ShowBonusScreen;
    }

    private void ShowBonusScreen(List<Config.SquareColor> bonusColorsToShow)
    {
        List<GameObject> validBonuses = new List<GameObject>();

        foreach (var bonus in bonusList)
        {
            var bonusComponent = bonus.GetComponent<Bonus>();
            if (bonusColorsToShow.Contains(bonusComponent.color))
            {
                validBonuses.Add(bonus);
            }
        }

        StartCoroutine(ActivateBonus(validBonuses));
    }

    private IEnumerator ActivateBonus(List<GameObject> objs)
    {
        foreach (var obj in objs)
        {
            obj.SetActive(true);
            yield return new WaitForSeconds(2);
            DeactivateBonus(obj);
        }
    }

    private void DeactivateBonus(GameObject obj)
    {
        obj.SetActive(false);
    }
}
