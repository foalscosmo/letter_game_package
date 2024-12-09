using System.Threading.Tasks;
using com.appidea.MiniGamePlatform.CommunicationAPI;
using UnityEngine;

public class LetterGameEntryPoint : BaseMiniGameEntryPoint
{
    [SerializeField] private GameObject gamePrefab;
    
    protected override Task LoadInternal()
    {
        Instantiate(gamePrefab);
        return Task.CompletedTask;
    }

    protected override Task UnloadInternal()
    {
        return Task.CompletedTask;
    }
}
