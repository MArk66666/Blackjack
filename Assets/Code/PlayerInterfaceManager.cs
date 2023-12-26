using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class PlayerInterfaceManager : MonoBehaviour
{
    [SerializeField] private TMP_Text balanceField;
    
    public delegate void DecisionAction();
    public event DecisionAction OnDecisionMade;
    
    public static PlayerInterfaceManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Table.Instance.GetMainPlayer().GetPlayerBalance().OnBalanceChanged += UpdateBalanceUI;
        UpdateBalanceUI();
    }
    
    public void UpdateBalanceUI()
    {
        Balance playerBalance = Table.Instance.GetMainPlayer().GetPlayerBalance();
        int chips = playerBalance.GetChipsAmount();
        
        StringBuilder balanceText = new StringBuilder();
        balanceText.Append(chips);
        balanceField.text = balanceText.ToString();
    }
    
    public void Take()
    {
        CardsDeck.Instance.GetCardFromTheDeck(Table.Instance.GetMainPlayer());
    }

    public void Pass()
    {
        OnDecisionMade?.Invoke();
    }
}