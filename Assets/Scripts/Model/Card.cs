
public class Card 
{
    public CardValue CardValue { get; set; }  // ������� �����
    public CardSuit CardSuit { get; set; }    // ����� �����
    public Card Parent { get; set; }          // ������ �����
    public Card Child { get; set; }           // ������� �����
    public bool FacedUp { get; set; }         // ��������� ��



}


public enum CardSuit
{
    Spades,  // ����
    Hearts,  // �����
    Clubs,   // �����
    Diamonds // �����
}

public enum CardValue
{
    Two,
    Three,
    Four,
    Five,
    Six, 
    Seven, 
    Eight, 
    Nine, 
    Ten, 
    Jack, 
    Queen, 
    King, 
    Ace
}
