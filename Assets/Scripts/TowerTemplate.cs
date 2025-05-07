using UnityEngine;

[CreateAssetMenu]
public class TowerTemplate : ScriptableObject
{
    public GameObject towerPrefab;           //타워 프리팹
    public GameObject followTowerPrefab;     //임시 타워 프리팹
    public Weapon[] weapon;                  //레벨별 타워 정보

    [System.Serializable]
    public struct Weapon
    {
        public Sprite sprite;   //타워 이미지
        public float damage;    //공격력
        public float rate;      //공속
        public float range;     //공격범위
        public int cost;        //필요 골드
        public int sell;        //판매 골드
    }
}
