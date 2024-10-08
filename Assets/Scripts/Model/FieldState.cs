using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.UIElements;

public class FieldState
{
    public List<List<Card>> CardGroups { get; private set; }   // ������ ����
    public List<List<(CardValue, CardSuit)>> CardCombinations { get; private set; }   // ����������

    public int CardNumber { get;  set; }

    public List<Card> Bank { get; private set; }               // ���� ����

    public bool IFcardsPositionedCorrectly;

    public FieldState() {
        CardGroups = new List<List<Card>>();
        Bank = new List<Card>();
        CardCombinations = new List<List<(CardValue, CardSuit)>>();
    }


}
