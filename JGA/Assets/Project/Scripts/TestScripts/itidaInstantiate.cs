using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itidaInstantiate : MonoBehaviour
{
    GameObject gameObject;

    // Start is called before the first frame update
    void Start()
    {
        gameObject = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Player.prefab");
        Instantiate(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
