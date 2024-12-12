using System;
using System.Threading.Tasks;
using com.appidea.MiniGamePlatform.CommunicationAPI;
using UnityEngine;

public class LetterGameEntryPoint : BaseMiniGameEntryPoint
{
    [SerializeField] private GameObject gamePrefab;
    
    protected override Task LoadInternal()
    {
        var gameManager = Instantiate(gamePrefab);
        gameManager.GetComponent<LetterSpawner>().SetEntryPoint(this);
        return Task.CompletedTask;
    }

    protected override Task UnloadInternal()
    {
        return Task.CompletedTask;
    }
}



