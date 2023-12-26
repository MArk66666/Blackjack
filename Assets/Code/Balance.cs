using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance
{
    private int _chips = 1000;
    private int _bet = 0;

    public Action OnBalanceChanged;
    
    public void ModifyBalance(int amount)
    {
        _chips += amount;
        OnBalanceChanged?.Invoke();
    }

    public void SetBetAmount(int amount)
    {
        if (amount > _chips) return;
        
        _bet = amount;
        ModifyBalance(-amount);
    }
    
    public int GetChipsAmount()
    {
        return _chips;
    }

    public int GetBetAmount()
    {
        return _bet;
    }
}