using System;
using UnityEngine;
[Serializable]
public class Person
{
    public string shortName;
    public Animator animator;

    public Person(string shortName, Animator animator)
    {
        this.shortName = shortName;
        this.animator = animator;
    }
}
