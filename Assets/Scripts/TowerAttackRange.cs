using UnityEngine;

public class TowerAttackRange : MonoBehaviour
{
    public void OnAttackRange(Vector3 position, float range)
    {
        gameObject.SetActive(true);

        //���ݹ��� ũ��
        float diameter = range * 2.0f;
        transform.localScale = Vector3.one * diameter;
        //���ݹ��� ��ġ
        transform.position = position;
    }

    public void OffAttackRange()
    {
        gameObject.SetActive(false);
    }
}
