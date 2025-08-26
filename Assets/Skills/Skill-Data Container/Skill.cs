using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Base Skill")]
public class Skill : ScriptableObject
{
    [Header("Skill Info")]
    public string skillName;
    public Sprite skillIcon;
    public float cooldown = 1f;

    [Header("Effects & Animation")]
    public GameObject vfxPrefab;
    public string animationTrigger;

    [Header("UI & Description")]
    [TextArea(3, 5)] 
    public string description;
    public Sprite statIcon;

    public virtual string GetStatDetails()
    {
        return ""; 
    }
    public virtual void Activate(GameObject user)
    {
        Debug.Log(skillName + " was activated by " + user.name);
    }
}