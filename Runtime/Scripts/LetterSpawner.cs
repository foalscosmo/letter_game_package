using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelConfig
{
    public string LevelName;
    public List<GameObject> Letters;
    public List<Transform> LettersSpawnPositions;
    public List<GameObject> SlotPrefabs;
    public List<Transform> SlotSpawnPositions;
    public GameObject AnimalPrefab;
    public Transform AnimalSpawnPosition;
}

public class LetterSpawner : MonoBehaviour
{
    public List<LevelConfig> Levels;
    public GameObject FinishPanel;

    private int currentLevelIndex = 0;
    private int placedObjectCount = 0;
    private List<GameObject> spawnedSlots = new List<GameObject>();


   private LetterDrag LetterDrag;

    public ParticleSystem FinishParticle;

    private LevelConfig CurrentLevel => Levels[currentLevelIndex];
    private string HappyAnimation = "Happy-Talking";
    private string IdleAnimation = "Idle";

    void Start()
    {
        LoadLevel(currentLevelIndex);
        LetterDrag = FindObjectOfType<LetterDrag>();
    }

    void LoadLevel(int levelIndex)
    {
        if (levelIndex >= Levels.Count) return;

        ClearPreviousLevel();

        LevelConfig level = Levels[levelIndex];

        if (level.AnimalPrefab != null)
            Instantiate(level.AnimalPrefab, level.AnimalSpawnPosition.position, Quaternion.identity);

        for (int i = 0; i < level.Letters.Count; i++)
        {
            if (i < level.LettersSpawnPositions.Count)
                Instantiate(level.Letters[i], level.LettersSpawnPositions[i].position, Quaternion.identity);
        }

        for (int i = 0; i < level.SlotPrefabs.Count; i++)
        {
            if (i < level.SlotSpawnPositions.Count)
            {
                GameObject slot = Instantiate(level.SlotPrefabs[i], level.SlotSpawnPositions[i].position, Quaternion.identity);
                spawnedSlots.Add(slot);
            }
        }


        placedObjectCount = 0;
    }

    void ClearPreviousLevel()
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Animal"))
            Destroy(obj);

        foreach (var obj in GameObject.FindGameObjectsWithTag("Letter"))
            Destroy(obj);

        foreach (var slot in spawnedSlots)
            Destroy(slot);
        spawnedSlots.Clear();


    }

    public void ObjectPlaced(Slot slot)
    {
        placedObjectCount++;

        slot.IsOccupied = true;
        if (placedObjectCount >= CurrentLevel.Letters.Count)
        {
            StartCoroutine(FinishLevel());
        }
    }

    IEnumerator FinishLevel()
    {
        placedObjectCount = 0;
        PlayAnimation();
        yield return new WaitForSeconds(2f);
        currentLevelIndex++;
        if (currentLevelIndex < 3)
        {
            LoadLevel(currentLevelIndex);
        }
        else
        {
            FinishPanel.SetActive(true);
            FinishParticle.Play();

        }
    }
    void PlayAnimation()
    {
        GameObject[] animals = GameObject.FindGameObjectsWithTag("Animal");

        foreach (GameObject animal in animals)
        {
            var skeletonAnimation = animal.GetComponent<SkeletonAnimation>();
            if (skeletonAnimation != null)
                StartCoroutine(AnimationDelay(animal));
        }
    }

    IEnumerator AnimationDelay(GameObject animal)
    {
        var skeletonAnimation = animal.GetComponent<SkeletonAnimation>();
        if (skeletonAnimation != null)
        {
            var trackEntry = skeletonAnimation.state.SetAnimation(0, HappyAnimation, false);
            yield return new WaitForSeconds(trackEntry.Animation.Duration);
            skeletonAnimation.state.SetAnimation(0, IdleAnimation, true);
        }
    }
}
