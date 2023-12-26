using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    private Player _base;
    private int _targetValue = 0;
    
    public void Initialize()
    {
        _base = GetComponent<Player>();
    }
    
    public void MakeAMove()
    {
        StartCoroutine(Think());
    }
    
    private IEnumerator Think()
    {
        while (ShouldTakeCard())
        {
            CardsDeck.Instance.GetCardFromTheDeck(_base);
            yield return new WaitForSeconds(1f);
        }
    }
    
    private bool ShouldTakeCard()
    {
        float chance = CalculateProbability();

        if (chance >= 0.5f)
        {
            Debug.LogWarning($"{_base.name} Takes!");
        }
        else
        {
            Debug.LogWarning($"{_base.name} Passes!");
        }
        
        if (_targetValue > 0)
        {
            if (_base.GetOverallValue() < _targetValue)
            {
                return true;
            }
        }
        
        return chance >= 0.5f;
    }
    
    private float CalculateProbability()
    {
        int remaining = 21 - _base.GetOverallValue(); 
        if (remaining <= 0) return 0.0f; 

        int favorableOutcomes = 0;
        int totalOutcomes = 13; 
        
        for (int i = 2; i <= 10; i++)
        {
            if (i <= remaining)
            {
                favorableOutcomes++;
            }
        }
        
        favorableOutcomes++;
        
        if (10 <= remaining)
        {
            favorableOutcomes += 4;
        }
        
        return (float)favorableOutcomes / totalOutcomes;
    }

    public void SetTargetValue(int value)
    {
        _targetValue = value;
    }
}