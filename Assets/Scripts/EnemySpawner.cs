using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab; //�� ������
    [SerializeField]
    private float spawnTime;  //�� ���� �ֱ�
    [SerializeField]
    private Transform[] wayPoints; //���� �������� �̵����

    private void Awake()
    {
        StartCoroutine("spawnEnemy");
    }

    private IEnumerator spawnEnemy()
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab); //�� ������Ʈ ����
            Enemy enemy = clone.GetComponent<Enemy>();//��� ���������� ENMY������Ʈ

            enemy.Setup(wayPoints);//setupȣ��

            yield return new WaitForSeconds(spawnTime);
        }
    }
}
