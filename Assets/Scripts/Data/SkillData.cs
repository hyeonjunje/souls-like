using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Object/Skill Data")]
public class SkillData : ScriptableObject
{
    [SerializeField] private string _skillName;
    public string skillName { get { return _skillName; } }

    [SerializeField] private float _rate;
    public float rate { get { return _rate; } }

    [SerializeField] private float _maxRange;
    public float maxRange { get { return _maxRange; } }

    [SerializeField] private float _minRange;
    public float minRange { get { return _minRange; } }

    [SerializeField] private float _coolTime;
    public float coolTime { get { return _coolTime; } }

}
