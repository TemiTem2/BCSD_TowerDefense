using UnityEngine;

public class SliderPositionAutoSetter : MonoBehaviour
{
    [SerializeField]
    private Vector3 distance = Vector3.down * 20.0f;
    private Transform targetTransform;
    private RectTransform rectTransform;

    public void Setup(Transform target)
    {
        //slider UI�� �i�ƴٴ� target ����
        targetTransform = target;
        //RectTransform ������Ʈ ���� ������
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        //���� �ı� �Ǹ� Slider UI�� ����
        if (targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        //������Ʈ�� ��ġ�� ���ŵ� ���Ŀ� Slider UI�� �Բ� ��ġ�� �����ϱ�����
        //LateUpdate

        //����,��Ʈ�� ���� ��ǥ�� �������� ȭ�鿡���� ��ǥ���� ����
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        //ȭ�鳻���� ��ǥ + distance��ŭ ������ ��ġ�� Slider UI�� ��ġ�� ����
        rectTransform.position = screenPosition + distance;
    }
}
