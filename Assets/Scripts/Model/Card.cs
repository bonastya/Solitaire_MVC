
using System;
using UnityEngine;
[Serializable]
public class Card 
{
    public CardValue CardValue { get; set; }  // Номинал карты
    public CardSuit CardSuit { get; set; }    // Масть карты

    #nullable enable
    public Card? Parent { get; set; }      // Предок карты
    public Card? Child { get; set; }        // Потомок карты
    #nullable disable

    public bool FacedUp  { get; set; } = false;        // Повернута ли

    public CardView CardView { get; set; }         // View


    public Card()
    {
        
    }
    public Card(CardValue cardValue, CardSuit cardSuit, Card? parent)
    {
        Parent = parent;
        FacedUp = false;
        CardValue = cardValue;
        CardSuit = cardSuit;
    }



}