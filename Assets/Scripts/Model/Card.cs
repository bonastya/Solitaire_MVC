
using System;
using UnityEngine;

public class Card 
{
    public CardValue CardValue { get; set; }  // ������� �����
    public CardSuit CardSuit { get; set; }    // ����� �����

    #nullable enable
    public Card? Parent { get; set; }      // ������ �����
    public Card? Child { get; set; }        // ������� �����
    #nullable disable

    public bool FacedUp  { get; set; } = true;        // ��������� ��

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