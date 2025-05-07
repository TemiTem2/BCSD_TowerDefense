using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate towerTemplate;    //Ÿ�� ����
    [SerializeField]
    private EnemySpawner enemySpawner;      //�� ����Ʈ
    [SerializeField]
    private PlayerGold playerGold;  //Ÿ���Ǽ��� ��� ���Ҹ� ����
    [SerializeField]
    private SystemTextViewer systemTextViewer; //�޼��� ���

    public void SpawnTower(Transform tileTransform)
    {
        //Ÿ���Ǽ� ���� Ȯ��
        //1.���� �ִ°�
        if (towerTemplate.weapon[0].cost> playerGold.CurrentGold)
        {
            //�� ���� ���
            systemTextViewer.PrintText(SystemType.Money);
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
        tile.IsBuildTower = true;//�Ǽ� ����
        playerGold.CurrentGold -= towerTemplate.weapon[0].cost;//��� ����
        Vector3 position = tileTransform.position + Vector3.back;                       //������ Ÿ���� ��ġ�� �Ǽ�
        GameObject clone = Instantiate(towerTemplate.towerPrefab, position, Quaternion.identity);     // ""
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner,playerGold, tile);//enemy ��������
    }
}
