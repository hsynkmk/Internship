using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MineSpawnera : MonoBehaviour
{
    [SerializeField] private GameObject[] minePrefabs; // Array of mine prefabs
    [SerializeField] private TextMeshProUGUI infoText;
    private int cooperCount;
    private int ironCount;
    private int siliconCount;

    private GameObject selectedMinePrefab; // The currently selected mine prefab

    private void Update()
    {
        TryPlaceMine();
    }

    // Attempt to place the selected mine prefab
    private void TryPlaceMine()
    {
        if (selectedMinePrefab != null)
        {
            PlaceMinePrefabAt();
            ClearSelectedMine();
        }
    }

    // Place the selected mine prefab at the given position
    private void PlaceMinePrefabAt()
    {
        ResourceManager.availableResources.Enqueue(selectedMinePrefab.transform);
    }

    // Select a mine from the build menu
    public void SelectMine(int index)
    {
        if (IsValidMineIndex(index))
        {
            selectedMinePrefab = minePrefabs[index];
        }
    }

    // Check if the mine index is valid
    private bool IsValidMineIndex(int index)
    {
        return index >= 0 && index < minePrefabs.Length;
    }

    // Clear the selected mine prefab and reset placement
    private void ClearSelectedMine()
    {
        selectedMinePrefab = null;
    }

    public void UpdateInfoText()
    {
        if (selectedMinePrefab.CompareTag("Cooper Mine"))
            cooperCount++;
        else if (selectedMinePrefab.CompareTag("Iron Mine"))
            ironCount++;
        else if(selectedMinePrefab.CompareTag("Silicon"))
            siliconCount++;

        infoText.text = $"Spent Objects\nCooper: {cooperCount}\nIron: {ironCount}\nSilicon count: {siliconCount}";
    }
}