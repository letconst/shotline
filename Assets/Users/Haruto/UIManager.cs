using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BattleStart()
    {
        SceneManager.sceneLoaded += MenuToBattleExchange;

        SceneManager.LoadScene("ForestScene"); // �^�X�N�P
    }

    private void MenuToBattleExchange(Scene a_scene, LoadSceneMode a_mode)
    {
        var l_object = GameObject.Find("Player"); // �^�X�N�Q
        var l_script = l_object.GetComponent<PlayerController>(); // �^�X�N�Q
        l_script.PlayerSpeedSet(0.1f); // �^�X�N�Q

        SceneManager.sceneLoaded -= MenuToBattleExchange;
    }
}
