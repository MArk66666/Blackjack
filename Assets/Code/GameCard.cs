using UnityEngine;

public class GameCard
{
    private Sprite _icon;
    private CardSuit _suit;
    private int _value;
    private bool _hidden;

    public void SetCardImage(Sprite newIcon)
    {
        _icon = newIcon;
    }

    public void SetCardSuit(CardSuit newSuit)
    {
        _suit = newSuit;
    }

    public void SetCardValue(int newValue)
    {
        _value = newValue;
    }

    public void Hide(bool value)
    {
        _hidden = value;
    }
    
    public Sprite GetCardImage()
    {
        return _icon;
    }

    public CardSuit GetCardSuit()
    {
        return _suit;
    }

    public int GetCardValue()
    {
        return _value;
    }

    public bool IsVisible()
    {
        return !_hidden;
    }
    
    public bool IsAce()
    {
        return _value == 1;
    }
    
    public string GetCardName()
    {
        string valueName = string.Empty;

        if (_value <= 10)
        {
            if (_value == 1)
            {
                valueName = "ace";
            }
            else
            {
                valueName = _value.ToString();
            }
        }
        else
        {
            switch (_value)
            {
                case 11:
                    valueName = "jack";
                    break;
                case 12:
                    valueName = "queen";
                    break;
                case 13:
                    valueName = "king";
                    break;
            }
        }
        return $"{valueName}_of_{_suit.ToString().ToLower()}";
    }
}