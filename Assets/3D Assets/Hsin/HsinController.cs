using System.Collections;
using UnityEngine;


public class HsinController : MonoBehaviour
{
    public Animator animator;
    public int spermBar = 0;
    public GameObject HsinSexCam;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        FirstPersonController Player = other.GetComponent<FirstPersonController>();

        StartCoroutine(Sex(Player));
    }

    IEnumerator Sex(FirstPersonController Player)
    {
        HsinSexCam.SetActive(true);
        while (true)
        {
            

            int index;
            /* if (spermBar < 80)
            {   
                animator.Play("Sex", 0, 0f);
                yield return new WaitForSeconds(0.146f);
                index = Random.Range(1, 8);
                spermBar += index;

                yield return new WaitForSeconds(0.4f);
            } */
            
            if (spermBar < 100)
            {
                animator.Play("Close Sex", 0, 0f);
                spermBar += 1;
                yield return new WaitForSeconds(0.33f);
            }

            if (spermBar >= 100)
            {
                animator.Play("Climax", 0, 0f);
                yield return new WaitForSeconds(0.486f);
                index = Random.Range(1, 25);
                Player.HP -= index;
                yield return new WaitForSeconds(0.267f);
                yield return new WaitForSeconds(1f);
                spermBar = 0;
            }

            if (Player.HP <= 0)
            {
                break;
            }
        }
    }
}
