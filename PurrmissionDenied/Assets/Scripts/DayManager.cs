using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : Subscriber
{
    public List<DayDefinition> Days;

    [Header("Events")]
    public DayDefinitionGameEvent DayInit;

    [Header("Event Listeners")]
    public UnitGameEvent IncrementAndMove;

    [Header("Testing")]
    public bool EnableOverride;
    public int IndexOverride;

    private int index = 0;
    private bool setup = false;

    public override void Subscribe()
    {
        int persisterCount = FindObjectsOfType<DayManager>().Count();
        if (!setup)
        {
            if (persisterCount > 1)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                setup = true;
            }
        }
        IncrementAndMove?.Subscribe(OnIncrementAndMove);
    }

    public override void AfterSubscribe()
    {
        if (setup)
        {
            if (index < Days.Count())
            {
                if (EnableOverride)
                {
                    DayInit?.Emit(Days[IndexOverride]);
                    return;
                }
                DayInit?.Emit(Days[index]);
            }
        }
    }

    private void OnIncrementAndMove()
    {
        index++;
        if (index < Days.Count())
        {
            SceneManager.LoadScene("Day", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("EndScreen", LoadSceneMode.Single);
        }
    }
}
