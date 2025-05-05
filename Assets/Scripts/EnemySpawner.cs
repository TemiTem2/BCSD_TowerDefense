using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    //[SerializeField]
    //private GameObject enemyPrefab; //�� ������
    [SerializeField] 
    private GameObject enemyHPSliderPrefab; //�� ü���� ��Ÿ���� Slider UI ������
    [SerializeField]
    private Transform canvasTransform;//UI canvas ������Ʈ transform
    //[SerializeField]
    //private float spawnTime;  //�� ���� �ֱ�
    [SerializeField]
    private Transform[] wayPoints; //���� �������� �̵����
    [SerializeField]
    private PlayerHP playerHP; //�÷��̾� ü�� ������Ʈ
    [SerializeField]
    private PlayerGold playerGold; //��� ������Ʈ
    private Wave currentWave;     //���� ���̺� ����
    private int currentEnemyCount;  //���� ���̺꿡 �����ִ� �� ����(���̺� ���۽� max, �� ����� -1)
    private List<Enemy> enemyList;//��� ���� ����

    //���� ������ ������ EnemySpawner���� �ϱ� ������ Set�� �ʿ�X
    public List<Enemy> EnemyList => enemyList;
    //���� ���̺��� �����ִ� ��, �ִ� �� ����
    public int CurrentEnemyCount => currentEnemyCount;
    public int MaxEnemyCount => currentWave.maxEnemyCount;

    private void Awake()
    {
        //�� ����Ʈ �޸� �Ҵ�
        enemyList = new List<Enemy>();
        //�� ���� �ڷ�ƾ �Լ� ȣ��
        //StartCoroutine("spawnEnemy");
    }

    public void StartWave(Wave wave)
    {
        //�Ű������� �޾ƿ� ���̺� ���� ����
        currentWave = wave;
        //���� ���̺��� �ִ� �� ���ڸ� ����
        currentEnemyCount = currentWave.maxEnemyCount;
        //���� ���̺� ����
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        //���� ���̺꿡�� ������ �� ����
        int spawnEnemyCount = 0;

        //while (true)
        //���� ���̺꿡�� �����Ǿ�� �ϴ� ���� ���ڸ�ŭ ���� �����ϰ� �ڷ�ƾ ����
        while ( spawnEnemyCount < currentWave.maxEnemyCount )
        {
            //GameObject clone = Instantiate(enemyPrefab); //�� ������Ʈ ����
            //���̺꿡 �����ϴ� ���� ������ ���� ������ �� ������ ���� �����ϵ��� �����ϰ� �� ������Ʈ ����
            int enemyIndex = Random.Range(0,currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();          //��� ���������� ENMY������Ʈ

            //this�� �ڽ��� EnemySpawner ����
            enemy.Setup(this,wayPoints);//setupȣ��
            enemyList.Add(enemy);

            SpawnEnemyHPSlider(clone); //�� ü���� ��Ÿ���� Slider ����

            spawnEnemyCount++;

            //yield return new WaitForSeconds(spawnTime);
            yield return new WaitForSeconds(currentWave.spawnTime); //spawnTime��ŭ ���
        }
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy,int gold)
    {
        if (type == EnemyDestroyType.Arrive) //������
        {
            playerHP.TakeDamage(1);//-1 ü��
        }

        else if (type == EnemyDestroyType.Kill) 
        {
            playerGold.CurrentGold += gold;
        }
        enemyList.Remove(enemy);

        //�� ����� �� ���� ǥ�� ����
        currentEnemyCount--;
        //����Ʈ���� ����� �� ���� ����
        enemyList.Remove(enemy);
        //�� ������Ʈ ����
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        //�� ü���� ��Ÿ���� Slider UI����
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);
        //Slider UI ������Ʈ�� parent("Canvas"������Ʈ)�� �ڽ����� ����
        //tip.UI�� ĵ������ �ڽ����� �����Ǿ��־�� ���δ�
        sliderClone.transform.SetParent(canvasTransform);
        //������������ �ٲ� ũ�⸦ �ٽ� (1,1,1)�� ����
        sliderClone.transform.localScale = Vector3.one;

        //Slider UI�� �i�ƴٴ� ����� ������ �Ǽ���
        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        //Slider UI�� �ڽ��� ü�� ������ ǥ���ϵ��� ����
        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}
