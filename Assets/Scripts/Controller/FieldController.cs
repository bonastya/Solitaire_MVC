using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class FieldController : MonoBehaviour
{

    public List<List<RectTransform>> cardsGroups;          // Список групп карт
    public RectTransform[] cards;                          // Список карт на поле

    public List<List<Card>> cardsGroupsModel;              // Список групп карт (Model)


    void Start()
    {
        cardsGroups = new List<List<RectTransform>>();
        GroupCards();
        SetGroupsHierarchy();

        foreach (List<Card> cardsGroupModel in cardsGroupsModel)
        {
            print("Num: " + cardsGroupModel.Count);
            foreach (Card card in cardsGroupModel)
            {                  
                print("Card " + card.CardView.gameObject.name );
                print("Parent " + (card.Parent != null ? card.Parent.CardView.gameObject.name : "null"));
                print("Child " + (card.Child != null ? card.Child.CardView.gameObject.name : "null"));
                print("-----");
            }
            print("---------------------------");
        }


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

    private bool IfCardsIntersect(int i, int j)
    {
        Rect rectI = new Rect(cards[i].anchoredPosition, cards[i].sizeDelta);
        Rect rectJ = new Rect(cards[j].anchoredPosition, cards[j].sizeDelta);

        return rectI.Overlaps(rectJ);
    }



    private void SetGroupsHierarchy()
    {
        cardsGroupsModel = new List<List<Card>>();

        foreach (List<RectTransform> cardsGroup in cardsGroups)
        {
            cardsGroup.OrderBy(p => p.transform.GetSiblingIndex());
        }

        // С 0 до конца по кучке карт, 0 нижняя (предок) -> и вверх (потомки)
        foreach (List<RectTransform> cardsGroup in cardsGroups)
        {
            List<Card> groupModel = new List<Card>();

            // Добавляем первую карту
            Card cardModel0 = new Card();
            groupModel.Add(cardModel0);
            cardModel0.CardView = cardsGroup[0].GetComponent<CardView>();

            for (int i = 1; i < cardsGroup.Count; i++)
            {
                // Создать карточку
                // Присвоить ей предка 
                // Присвоить её как потомка предку

                Card cardModel = new Card();
                cardModel.Parent = groupModel[i - 1];
                groupModel[i - 1].Child = cardModel;

                // Оставить ссылку на View
                cardModel.CardView = cardsGroup[i].GetComponent<CardView>();

                groupModel.Add(cardModel);
            }

            cardsGroupsModel.Add(groupModel);

            groupModel[0].Parent = null;
            groupModel[groupModel.Count-1].Child = null;

        }

    }



}
