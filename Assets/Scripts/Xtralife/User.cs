using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "User", menuName = "User")]
public class User : ScriptableObject
{
    public string GamerId;
    public string GamerSecretID;

    public string DisplayName;
}
