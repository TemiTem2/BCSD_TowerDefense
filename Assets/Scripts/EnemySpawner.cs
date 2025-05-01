using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab; //�� ������
    [SerializeField]
    private float spawnTime;  //�� ���� �ֱ�
    [SerializeField]
    private Transform[] wayPoints; //���� �������� �̵����
    private List<Enemy> enemyList;


    public List<Enemy> EnemyList => enemyList;

    private void Awake()
    {
        enemyList = new List<Enemy>();
        StartCoroutine("spawnEnemy");
    }

    private IEnumerator spawnEnemy()
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab); //�� ������Ʈ ����
            Enemy enemy = clone.GetComponent<Enemy>();//��� ���������� ENMY������Ʈ

            enemy.Setup(this,wayPoints);//setupȣ��
            enemyList.Add(enemy);

            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void DestroyEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}
