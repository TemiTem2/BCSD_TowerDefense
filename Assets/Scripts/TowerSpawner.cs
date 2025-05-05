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
        if (towerBuildGold > playerGold.CurrentGold)
        {
            return;
        }
        Tile tile = tileTransform.GetComponent<Tile>();

        //2.Ÿ�� �Ǽ��� �Ǿ��ִ°�
        if (tile.IsBuildTower == true) 
        { 
            return;
        }
        tile.IsBuildTower = true;//�Ǽ� ����
        playerGold.CurrentGold -= towerBuildGold;//��� ����
        Vector3 position = tileTransform.position + Vector3.back;                       //������ Ÿ���� ��ġ�� �Ǽ�
        GameObject clone = Instantiate(towerPrefab, position, Quaternion.identity);     // ""
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);//enemy ��������
    }
}
