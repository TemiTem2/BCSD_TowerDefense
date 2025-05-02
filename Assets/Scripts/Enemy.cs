using UnityEngine;
using System.Collections;

public enum EnemyDestroyType { Kill = 0 , Arrive }
public class Enemy : MonoBehaviour
{
    private int wayPointCount; //�̵���� ����
    private Transform[] waypoints;//�̵���� ����
    private int currentIndex = 0;//���� ��ǥ���� �ε���
    private Movement2D movement2D;//������Ʈ �̵� ����
    private EnemySpawner enemySpawner;
    [SerializeField]
    private int gold = 10; //�� ����� ȹ���ϴ� ���
    public void Setup(EnemySpawner enemySpawner,Transform[] waypoints)
    {
        movement2D = GetComponent<Movement2D>();
        this.enemySpawner = enemySpawner;

        //�� �̵� ��� WayPoints ���� ����
        wayPointCount = waypoints.Length;
        this.waypoints = new Transform[wayPointCount];
        this.waypoints = waypoints;

        //���� ��ġ�� ù��° wayPoint ��ġ�� ����
        transform.position = waypoints[currentIndex].position;

        //���̵�/��ǥ���� ���� �ڷ�ƾ �Լ� ����

        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        //���� �̵� ���� ����
        NextMoveTo();

        while (true)
        {
            //ȸ��
            transform.Rotate(Vector3.forward * 10);

            //���� ������ġ�� ��ǥ��ġ�� �Ÿ��� 0.02 * movement2D.movespeed���� ������ if���ǹ� �Ӥ���
            if (Vector3.Distance(transform.position, waypoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
            {
                NextMoveTo();
            }

            yield return null;
        }
    }


    private void NextMoveTo()
    {
        //���� �̵��� way����Ʈ�� �����ִٸ�
        if (currentIndex < wayPointCount - 1)
        {
            transform.position = waypoints[currentIndex].position;

            currentIndex++;
            Vector3 direction = (waypoints[currentIndex].position-transform.position).normalized;
            movement2D.MoveTo(direction);
        }

        else
        {
            //��ǥ������ �����ϸ� ���� ���� �ʵ���
            gold = 0;
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        //EnemySpawner ���� ����Ʈ�� �� ������ �����ϱ� ������ ���� Destroy�� ���� �ʰ�
        //EnemySpawner���� �Լ� ȣ��
        
        enemySpawner.DestroyEnemy(type, this,gold);
    }
}
