using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab; //적 프리팹
    [SerializeField]
    private float spawnTime;  //적 생성 주기
    [SerializeField]
    private Transform[] wayPoints; //현재 스테이지 이동경로

    private void Awake()
    {
        StartCoroutine("spawnEnemy");
    }

    private IEnumerator spawnEnemy()
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab); //적 오브젝트 생성
            Enemy enemy = clone.GetComponent<Enemy>();//방금 생성된적의 ENMY컴포넌트

            enemy.Setup(wayPoints);//setup호출

            yield return new WaitForSeconds(spawnTime);
        }
    }
}
