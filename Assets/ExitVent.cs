using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ExitVent : Interactable
{
    public override void UseInteractable(BeeSwarm bees)

    {
        SceneManager.LoadScene("Main Menu");
    }
}
