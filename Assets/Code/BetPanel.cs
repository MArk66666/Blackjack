using System;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.UI;

public class BetPanel : MonoBehaviour
{
    [SerializeField] private RadialSlider betSlider;
    [SerializeField] private Button submitButton;
    [SerializeField] private List<GameObject> panelComponents;
    
    private Balance _playerBalance;

    public static BetPanel Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _playerBalance = Table.Instance.GetMainPlayer().GetPlayerBalance();

        if (panelComponents.Contains(betSlider.gameObject) == false)
        {
            panelComponents.Add(betSlider.gameObject);
        }
        
        if (panelComponents.Contains(submitButton.gameObject) == false)
        {
            panelComponents.Add(submitButton.gameObject);
        }
        
        SetupSubmitButton();
        TogglePanel(true);
    }

    private void SetupSubmitButton()
    {
        submitButton.onClick.AddListener(Bet);    
    }
    
    private void UpdateValues()
    {
        betSlider.maxValue = _playerBalance.GetChipsAmount();
        betSlider.currentValue = 1;
        betSlider.UpdateUI();
    }

    private void Bet()
    {
        float betAmount = betSlider.currentValue;

        if (betAmount > _playerBalance.GetChipsAmount() && betAmount == 0) return;
        
        _playerBalance.SetBetAmount(Mathf.RoundToInt(betAmount));
        CardsDeck.Instance.StartGame();
        TogglePanel(false);
    }

    public void TogglePanel(bool value)
    {
        for (int i = 0; i < panelComponents.Count; i++)
        {
            panelComponents[i].SetActive(value);
        }
        UpdateValues();
    }
}