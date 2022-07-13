using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using Yomikiru;

public class ResultSequence : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;

    private void Start()
    {
        playerAnimator.SetBool("isWin", true);
    }
}
