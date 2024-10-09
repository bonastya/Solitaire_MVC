
using System;
using UnityEngine;

public class Card 
{
    public CardValue CardValue { get; set; }  // Номинал карты
    public CardSuit CardSuit { get; set; }    // Масть карты

    #nullable enable
    public Card? Parent { get; set; }      // Предок карты
    public Card? Child { get; set; }        // Потомок карты
    #nullable disable

    public bool FacedUp  { get; set; } = true;        // Повернута ли

    public CardView CardView { get; set; }         // View


    public Card()
    {
        
    }
    public Card(/*Card? parent, Card? child,*/ CardValue cardValue, CardSuit cardSuit)
    {
        /*Parent = parent;
        Child = child;*/
        FacedUp = true;
        CardValue = cardValue;
        CardSuit = cardSuit;
    }



}