
using System;
using UnityEngine;
[Serializable]
public class Card 
{
    public CardValue CardValue { get; set; }  // ������� �����
    public CardSuit CardSuit { get; set; }    // ����� �����

    #nullable enable
    public Card? Parent { get; set; }      // ������ �����
    public Card? Child { get; set; }        // ������� �����
    #nullable disable

    public bool FacedUp  { get; set; } = false;        // ��������� ��

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