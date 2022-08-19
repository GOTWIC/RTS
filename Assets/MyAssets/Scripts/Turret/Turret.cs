using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Turret : NetworkBehaviour
{
    [SerializeField] Targeter targeter = null;
}
