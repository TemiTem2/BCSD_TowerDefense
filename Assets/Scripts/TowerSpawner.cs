using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    [SerializeField]
    private int towerBuildGold = 50; //타워 건설에 사용되는 골드
    [SerializeField]
    private EnemySpawner enemySpawner;
    [SerializeField]
    private PlayerGold playerGold;//타워건설시 골드 감소를 위해

    public void SpawnTower(Transform tileTransform)
    {
        //타워건설 여부 확인
        //1.돈이 있는가
        if (towerBuildGold > playerGold.CurrentGold)
        {
            return;
        }
        Tile tile = tileTransform.GetComponent<Tile>();

        //2.타워 건설이 되어있는가
        if (tile.IsBuildTower == true) 
        { 
            return;
        }
        tile.IsBuildTower = true;//건설 유무
        playerGold.CurrentGold -= towerBuildGold;//골드 감소
        Vector3 position = tileTransform.position + Vector3.back;                       //선택한 타일의 위치에 건설
        GameObject clone = Instantiate(towerPrefab, position, Quaternion.identity);     // ""
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);//enemy 정보전달
    }
}
