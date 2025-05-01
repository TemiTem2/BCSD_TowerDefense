using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;

    public void SpawnTower(Transform tileTransform)
    {
        Tile tile = tileTransform.GetComponent<Tile>();

        if (tile.IsBuildTower == true) 
        { 
            return;
        }
        tile.IsBuildTower = true;
        Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);
    }
}
