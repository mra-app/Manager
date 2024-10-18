using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
    public List<Button> buttons = new List<Button>();
    public List<GameObject> tabs = new List<GameObject>();
    public List<GameObject> backs = new List<GameObject>();
    // Start is called before the first frame update
    void Awake()
    {
       for( int i = 0;i< tabs.Count;i++)
        {
            GameObject tab = tabs[i];//Funny! it must be
            GameObject back = backs[i];//Funny! it must be
            buttons[i].onClick.AddListener(()=>{
                  HideAll();
                  tab.SetActive(true);
                  back.SetActive(true);
            });
        }       
            
    }
    void HideAll()
    {
        foreach (var tab in tabs)
        {
                tab.SetActive(false);
        }
        foreach (var back in backs)
        {
                back.SetActive(false);
        }
    }

}
