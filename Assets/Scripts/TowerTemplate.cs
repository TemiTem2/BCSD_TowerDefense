using UnityEngine;

[CreateAssetMenu]
public class TowerTemplate : ScriptableObject
{
    public GameObject towerPrefab;           //Ÿ�� ������
    public GameObject followTowerPrefab;     //�ӽ� Ÿ�� ������
    public Weapon[] weapon;                  //������ Ÿ�� ����

    [System.Serializable]
    public struct Weapon
    {
        public Sprite sprite;   //Ÿ�� �̹���
        public float damage;    //���ݷ�
        public float rate;      //����
        public float range;     //���ݹ���
        public int cost;        //�ʿ� ���
        public int sell;        //�Ǹ� ���
    }
}
