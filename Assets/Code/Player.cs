using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

[RequireComponent(typeof(Image))]
public class Player : MonoBehaviour
{
    [SerializeField] private Image playerIcon;    
    [SerializeField] private GameObject localDeckGrid;
    [SerializeField] private TMP_Text cardsValueText;
    [SerializeField] private TMP_Text betAmountText;
    
    private List<GameCard> _localDeck = new List<GameCard>();
    private int _cardsValue;

    private Image _background;
    private Color _backgroundColor = new Color(0, 0, 0, 0.4f);

    private Balance _balance = new Balance();
    
    private void Awake()
    {
        _background = GetComponent<Image>();
    }

    private void Start()
    {
        _balance.OnBalanceChanged += UpdateUI;
    }

    private void UpdateValue()
    {
        int aceCount = 0;
        _cardsValue = 0;

        foreach (GameCard card in _localDeck)
        {
            if (card.IsVisible() == false)
            {
                continue;
            }
            
            _cardsValue += card.GetCardValue();
            if (card.IsAce())
            {
                _cardsValue += 10;
                aceCount++;
            }
        }

        while (_cardsValue > 21 && aceCount > 0)
        {
            _cardsValue -= 10;
            aceCount--;
        }
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        cardsValueText.text = _cardsValue.ToString();
        betAmountText.text = _balance.GetBetAmount().ToString();
    }
    
    private void RemoveCard(GameCard card)
    {
        if (_localDeck.Contains(card) == false)
        {
            Debug.LogError($"{card.GetCardName()} is not available in the {gameObject.name}'s local deck!");
            return;
        }

        _localDeck.Remove(card);
        UpdateValue();
    }
    
    public void AddCard(GameCard card)
    {
        _localDeck.Add(card);
        UpdateValue();
    }

    public void SetupIcon(Sprite newIcon, Color color)
    {
        playerIcon.sprite = newIcon;
        playerIcon.color = color;
    }
    
    public void RefreshCards()
    {
        for (int i = 0; i < _localDeck.Count; i++)
        {
            _localDeck[i].Hide(false);
        }
        UpdateValue();
    }

    public void ClearDeck()
    {
        List<GameCard> temporaryDeck = GetLocalDeck();
        
        foreach (GameCard card in temporaryDeck)
        {   
            RemoveCard(card);
        }
        
        foreach(Transform child in localDeckGrid.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Select()
    {
        _background.color = _backgroundColor;
    }

    public void Deselect()
    {
        _background.color = new Color(0f, 0f, 0f, 0f);
    }
    
    public Transform GetPlayerLocalDeckGrid()
    {
        return localDeckGrid.transform;
    }

    public int GetOverallValue()
    {
        return _cardsValue;
    }

    public List<GameCard> GetLocalDeck()
    {
        return new List<GameCard>(_localDeck);
    }

    public Balance GetPlayerBalance()
    {
        return _balance;
    }
}