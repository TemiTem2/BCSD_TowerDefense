using UnityEngine;
using System.Collections;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate[] towerTemplate;    //타워 정보
    [SerializeField]
    private EnemySpawner enemySpawner;      //적 리스트
    [SerializeField]
    private PlayerGold playerGold;  //타워건설시 골드 감소를 위해
    [SerializeField]
    private SystemTextViewer systemTextViewer; //시스템 메세지 출력
    private bool isOnTowerButton = false;   //타워 건설 버튼 눌렀는지
    private GameObject followTowerClone = null; //임시 타워 제거를 위한 변수
    private int towerType;              //타워 속성

    public void ReadyToSpawnTower(int type)
    {
        towerType = type;

        //버튼 중복 입력 방지
        if ( isOnTowerButton == true )
        {
            return;
        }

        //타워 건설 가능 여부 확인
        if (towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold )
        {
            //건설 불가능 메세지
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        //타워 건설 누름
        isOnTowerButton = true;
        //임시타워 생성
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);
        //타워건설 취소 코루틴함수
        StartCoroutine("OnTowerCancelSystem");
    }    
    public void SpawnTower(Transform tileTransform)
    {
        //타워 건설 버튼 눌렀을때만 호출
        if ( isOnTowerButton == false )
        {
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

        isOnTowerButton = false; //버튼 초기화
        tile.IsBuildTower = true;//타일에 건설됨 표시
        playerGold.CurrentGold -= towerTemplate[towerType].weapon[0].cost;//골드 감소
        Vector3 position = tileTransform.position + Vector3.back;                       //선택한 타일의 위치에 건설
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefab, position, Quaternion.identity);     // ""
        clone.GetComponent<TowerWeapon>().Setup(this, enemySpawner,playerGold, tile);//enemy 정보전달

        //버프 효과 갱신
        OnBuffAllBuffTowers();

        Destroy(followTowerClone);  //임시 타워 삭제
        StopCoroutine("OnTowerCancelSystem");   //건설 취소 코루틴 함수 중지
    }

    private IEnumerator OnTowerCancelSystem()
    {
        while( true )
        {
            //esc, 우클릭시 건설 취소
            if ( Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1) )
            {
                isOnTowerButton = false;
                Destroy(followTowerClone); //임시타워 삭제
                break;
            }

            yield return null;
        }
    }

    public void OnBuffAllBuffTowers()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for ( int i = 0; i < towers.Length; i++ )
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if ( weapon.WeaponType == WeaponType.Buff )
            {
                weapon.OnBuffAroundTower();
            }
        }
    }
}
