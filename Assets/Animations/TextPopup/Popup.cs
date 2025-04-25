using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] Text text;
    public string textValue;

    void Start()
    {
        text.text = textValue;
        Destroy(gameObject, 1.5f);
    }
}
