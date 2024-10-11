using System.Collections.Generic;

/// <summary>
/// �������� �������� ��������� ����
/// </summary>
public class FieldState
{
    /// <summary>
    /// ������ ����, ��������� ���������
    /// </summary>
    public List<List<Card>> CardGroups { get; private set; }   

    /// <summary>
    /// ������� ����� ������ ������
    /// </summary>
    public List<Card?> TopCardsInGroups { get; private set; }   

    /// <summary>
    /// ���������� �������� ���������������� ������
    /// </summary>
    public List<List<(CardValue, CardSuit)>> CardCombinations { get; private set; }   

    /// <summary>
    /// ����������� ����� �� ���� � ������ (��� ����� ���� �����)
    /// </summary>
    public int CardsNumber { get;  set; }

    /// <summary>
    /// ������� ����������� ���� �� ���� (��� ����� ���� �����)
    /// </summary>
    public int CurrentFieldCardsNumber { get;  set; }

    /// <summary>
    /// ����� ����� �������� ������ (���)
    /// </summary>
    public List<Card> Bank { get; private set; }

    /// <summary>
    /// ������� ����������� ���� � �����
    /// </summary>
    public int CurrentBankCardsNumber { get;  set; }

    /// <summary>
    /// ���� ����������� ������������ ���� � �������� ��� � ������ �� ������ 1 ������� � 1 ������
    /// </summary>
    public bool IFcardsPositionedCorrectly { get;  set; }

    /// <summary>
    /// ������� ����� ������������������
    /// </summary>
    public Card currentCard;

    public FieldState() {
        CardGroups = new List<List<Card>>();
        Bank = new List<Card>();
        TopCardsInGroups = new List<Card>();
        CardCombinations = new List<List<(CardValue, CardSuit)>>();
    }


}
