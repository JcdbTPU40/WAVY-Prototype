using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    private float totalTime; //���v����
    [SerializeField] private int minutes; //�������Ԃ̕�
    [SerializeField] private float seconds; //�������Ԃ̕b
    private float oldSeconds; //�^�C�}�[�̈��萫�A���炩���̕\��
    private TextMeshProUGUI timerText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalTime = minutes * 60 + seconds; //��i3�� * 60 = 180 180 + 0�b = ���v180�b)
        oldSeconds = 0f;
        timerText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        totalTime = minutes * 60 + seconds; //���v���Ԃ̌v�Z
        totalTime -= Time.deltaTime;

        minutes = (int)totalTime / 60;�@  //�@�Đݒ�
        seconds = totalTime - minutes * 60;

        if ((int)seconds != (int)oldSeconds)
        {
            timerText.text = minutes.ToString("00") + ":" + ((int)seconds).ToString("00"); //UI�̃e�L�X�g�\���i���ԁj
        }
        oldSeconds = seconds;

        if (totalTime <= 0f)
        {
            return; //0�b�ȉ��ɂȂ����珈�����~�߂��I�@//�r�[���V�[���̐؂�ւ��ɂȂ�\��B
        }
    }
}
