using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSpriteManager : MonoBehaviour
{
    public Sprite[] SpadesSprites;
    public Sprite[] HeartsSprites;
    public Sprite[] ClubsSprites;
    public Sprite[] DiamondsSprites;

    public Sprite BackSideSprite;

    public void UpdateView(Card cardModel, CardView cardView)
    {
        Sprite sprite = BackSideSprite; 
        if (cardModel.FacedUp)
        {
            switch (cardModel.CardSuit)
            { 
                case CardSuit.Spades:
                    sprite = SpadesSprites[(int)cardModel.CardValue];
                    break;
                case CardSuit.Hearts:
                    sprite = HeartsSprites[(int)cardModel.CardValue];
                    break;
                case CardSuit.Clubs:
                    sprite = ClubsSprites[(int)cardModel.CardValue];
                    break;
                case CardSuit.Diamonds:
                    sprite = DiamondsSprites[(int)cardModel.CardValue];
                    break;

            }

            cardView.cardButton.enabled = true;

        }
        else
        {
            cardView.cardButton.enabled = false;
        }

        cardView.SetSprite(sprite);

    }
}

