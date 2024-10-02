using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class FieldController : MonoBehaviour
{

    public List<List<RectTransform>> cardsGroups;          // Список групп карт
    public RectTransform[] cards;                          // Список карт на поле


    void Start()
    {
        cardsGroups = new List<List<RectTransform>>();
        GroupCards();
/*        foreach (List<RectTransform> cardList in cardsGroups)
        {
            print("Num: " + cardList.Count);
            foreach (RectTransform card in cardList)
            {
                print(card.gameObject.name);
            }
            print("-----");
        }*/


    }

    private void GroupCards()
    {
        bool[] cardIsChecked = new bool[cards.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            if (!cardIsChecked[i])
            {
                List<RectTransform> group = new List<RectTransform>();
                group.Add(cards[i]);
                SearchIntersections(i, cardIsChecked, group);
                cardsGroups.Add(group);
            }
        }
    }

    private void SearchIntersections(int i, bool[] cardIsChecked, List<RectTransform> group)
    {
        cardIsChecked[i] = true;

        for (int j = 0; j < cards.Length; j++)
        {
            if (!cardIsChecked[j] && IfCardsIntersect(i, j))
            {
                group.Add(cards[j]);
                SearchIntersections(j, cardIsChecked, group);
            }
        }

    }

    /// <summary>
    /// If two cards of the field intersect
    /// </summary>
    /// <param name="i"> Number of the first card</param>
    /// <param name="j"> Number of the second card</param>
    /// <returns></returns>
    private bool IfCardsIntersect(int i, int j)
    {
        Rect rectI = new Rect(cards[i].anchoredPosition, cards[i].sizeDelta);
        Rect rectJ = new Rect(cards[j].anchoredPosition, cards[j].sizeDelta);

        return rectI.Overlaps(rectJ);
    }
}
