using UnityEngine;

public class PopupManager : MonoBehaviour
{

    [SerializeField] Camera cam;
    [SerializeField] GameObject popupPrefab;


    void Start()
    {
        
    }

    void Update()
    {
        // make when a user presses on a button to come out a popup animation
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -cam.transform.position.z;

            Vector3 screenPos = cam.ScreenToWorldPoint(mousePos);

            GameObject popupObj = Instantiate(popupPrefab, screenPos, new Quaternion());
            popupObj.GetComponent<Popup>().textValue = "No save found.";
        }
    }

}
