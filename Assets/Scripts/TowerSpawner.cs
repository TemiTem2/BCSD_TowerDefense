using UnityEngine;
using System.Collections;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate towerTemplate;    //Ÿ�� ����
    [SerializeField]
    private EnemySpawner enemySpawner;      //�� ����Ʈ
    [SerializeField]
    private PlayerGold playerGold;  //Ÿ���Ǽ��� ��� ���Ҹ� ����
    [SerializeField]
    private SystemTextViewer systemTextViewer; //�ý��� �޼��� ���
    private bool isOnTowerButton = false;   //Ÿ�� �Ǽ� ��ư ��������
    private GameObject followTowerClone = null; //�ӽ� Ÿ�� ���Ÿ� ���� ����

    public void ReadyToSpawnTower()
    {
        //��ư �ߺ� �Է� ����
        if ( isOnTowerButton == true )
        {
            return;
        }

        //Ÿ�� �Ǽ� ���� ���� Ȯ��
        if (towerTemplate.weapon[0].cost > playerGold.CurrentGold )
        {
            //�Ǽ� �Ұ��� �޼���
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        //Ÿ�� �Ǽ� ����
        isOnTowerButton = true;
        //�ӽ�Ÿ�� ����
        followTowerClone = Instantiate(towerTemplate.followTowerPrefab);
        //Ÿ���Ǽ� ��� �ڷ�ƾ�Լ�
        StartCoroutine("OnTowerCancelSystem");
    }    
    public void SpawnTower(Transform tileTransform)
    {
        //Ÿ�� �Ǽ� ��ư ���������� ȣ��
        if ( isOnTowerButton == false )
        {
            return;
        }    
        
        Tile tile = tileTransform.GetComponent<Tile>();

        //2.Ÿ�� �Ǽ��� �Ǿ��ִ°�
        if (tile.IsBuildTower == true) 
        {
            //�Ǽ� �Ұ� ���
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }

        isOnTowerButton = false; //��ư �ʱ�ȭ
        tile.IsBuildTower = true;//Ÿ�Ͽ� �Ǽ��� ǥ��
        playerGold.CurrentGold -= towerTemplate.weapon[0].cost;//��� ����
        Vector3 position = tileTransform.position + Vector3.back;                       //������ Ÿ���� ��ġ�� �Ǽ�
        GameObject clone = Instantiate(towerTemplate.towerPrefab, position, Quaternion.identity);     // ""
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner,playerGold, tile);//enemy ��������

        Destroy(followTowerClone);  //�ӽ� Ÿ�� ����
        StopCoroutine("OnTowerCancelSystem");   //�Ǽ� ��� �ڷ�ƾ �Լ� ����
    }

    private IEnumerator OnTowerCancelSystem()
    {
        while( true )
        {
            //esc, ��Ŭ���� �Ǽ� ���
            if ( Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1) )
            {
                isOnTowerButton = false;
                Destroy(followTowerClone); //�ӽ�Ÿ�� ����
                break;
            }

            yield return null;
        }
    }
}
