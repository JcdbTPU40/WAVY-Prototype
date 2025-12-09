using UnityEngine;
using UnityEngine.UI;

public class PopUpController : MonoBehaviour
{
    // ... (既存の public フィールドはそのまま) ...
    public GameObject popUpPanel; 
    public Sprite defaultSprite;
    public Sprite pressedSprite;

    private Image buttonImage;
    private Button button;

    void Start()
    {
        // 必要なコンポーネントを取得 (既存のまま)
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();

        // ... (既存のクリックイベント登録、初期画像セットはそのまま) ...
    }

    // ⭐ 新しく追加するメソッド: ポーズ状態に応じて画像を更新する
    public void UpdateButtonSprite(bool isPaused)
    {
        if (buttonImage == null || defaultSprite == null || pressedSprite == null) 
        {
            // エラーが発生しないようにチェック
            return;
        }

        // isPaused が true なら pressedSprite、false なら defaultSprite を設定
        buttonImage.sprite = isPaused ? pressedSprite : defaultSprite;
    }

    // OnButtonClicked メソッドの修正（画像変更ロジックを削除）
    public void OnButtonClicked()
    {
        Debug.Log("オプションボタンがクリックされました！", this);
        
        if (GameManager.Instance != null)
        {
            // クリック時にGameManagerのポーズ切り替えを呼び出す
            GameManager.Instance.TogglePause();
        }

        // 元々あったポップアップUIの表示とボタンデザイン変更のコードは、
        // GameManagerに移管されるため、ここでは削除します。
    }
}