using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate towerTemplate;    //타워 정보
    [SerializeField]
    private EnemySpawner enemySpawner;      //적 리스트
    [SerializeField]
    private PlayerGold playerGold;  //타워건설시 골드 감소를 위해
    [SerializeField]
    private SystemTextViewer systemTextViewer; //메세지 출력

    public void SpawnTower(Transform tileTransform)
    {
        //타워건설 여부 확인
        //1.돈이 있는가
        if (towerTemplate.weapon[0].cost> playerGold.CurrentGold)
        {
            //돈 없음 출력
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }
        Tile tile = tileTransform.GetComponent<Tile>();

        //2.타워 건설이 되어있는가
        if (tile.IsBuildTower == true) 
        {
            //건설 불가 출력
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }
        tile.IsBuildTower = true;//건설 유무
        playerGold.CurrentGold -= towerTemplate.weapon[0].cost;//골드 감소
        Vector3 position = tileTransform.position + Vector3.back;                       //선택한 타일의 위치에 건설
        GameObject clone = Instantiate(towerTemplate.towerPrefab, position, Quaternion.identity);     // ""
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner,playerGold, tile);//enemy 정보전달
    }
}
