using UnityEngine;

public class CardSpriteManager : MonoBehaviour
{
    [Tooltip("Sprites of the Spades siutes (must be arranged in ascending order)")]
    public Sprite[] SpadesSprites;
    [Tooltip("Sprites of the Hearts siutes (must be arranged in ascending order)")]
    public Sprite[] HeartsSprites;
    [Tooltip("Sprites of the Clubs siutes (must be arranged in ascending order)")]
    public Sprite[] ClubsSprites;
    [Tooltip("Sprites of the Diamonds siutes (must be arranged in ascending order)")]
    public Sprite[] DiamondsSprites;

    public Sprite BackSideSprite;

    /// <summary>
    /// Установка спрайта в зависимости от параметров модели карты
    /// </summary>
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

