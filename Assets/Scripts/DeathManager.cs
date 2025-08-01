using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public static DeathManager instance;

    private void Awake(){
        if(DeathManager.instance==null)
            instance=this;
            else 
                Destroy(gameObject);
    }
    public void GameOver(){
        UIManager _ui= GetComponent<UIManager>();
        if(_ui!=null)
            _ui.ToggleDeathPanel();
    }
}
