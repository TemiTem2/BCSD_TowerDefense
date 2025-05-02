using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    [SerializeField]
    private int towerBuildGold = 50; //Ÿ�� �Ǽ��� ���Ǵ� ���
    [SerializeField]
    private EnemySpawner enemySpawner;
    [SerializeField]
    private PlayerGold playerGold;//Ÿ���Ǽ��� ��� ���Ҹ� ����

    public void SpawnTower(Transform tileTransform)
    {
        //Ÿ���Ǽ� ���� Ȯ��
        //1.���� �ִ°�
        if (towerBuildGold >playerGold.CurrentGold) return;
        Tile tile = tileTransform.GetComponent<Tile>();

        //2.Ÿ�� �Ǽ��� �Ǿ��ִ°�
        if (tile.IsBuildTower == true) 
        { 
            return;
        }
        tile.IsBuildTower = true;//�Ǽ� ����
        playerGold.CurrentGold -= towerBuildGold;//��� ����
        GameObject clone = Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);//������ Ÿ���� ��ġ�� �Ǽ�
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);//enemy ��������
    }
}
