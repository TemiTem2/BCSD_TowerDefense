using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab; //�� ������
    [SerializeField] 
    private GameObject enemyHPSliderPrefab; //�� ü���� ��Ÿ���� Slider UI ������
    [SerializeField]
    private Transform canvasTransform;//UI canvas ������Ʈ transform
    [SerializeField]
    private float spawnTime;  //�� ���� �ֱ�
    [SerializeField]
    private Transform[] wayPoints; //���� �������� �̵����
    private List<Enemy> enemyList;//��� ���� ���


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

            SpawnEnemyHPSlider(clone); //�� ü���� ��Ÿ���� Slider ����

            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void DestroyEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
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
