using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{

    public List<GameObject> maps;
    public GameObject activeMap;
    public MapMarkersList mapMarkers;
    public bool mapActive = false;
    public AudioClip mapSound;
    Image playerCurrentPosition;
    float cooldown = 1f;
    public void LoadMap(string mapName)
    {
        if (mapName != "")
        {
            bool canLoadMap = false;
            if (!activeMap)
            {
                canLoadMap = true;
            }
            else if (activeMap && activeMap.name != mapName)
            {
                canLoadMap = true;
            }
            if (canLoadMap)
            {
                foreach (GameObject m in maps)
                {
                    if (m.name == mapName)
                    {
                        activeMap = Instantiate(m, Vector2.zero, Quaternion.identity) as GameObject;
                        activeMap.name = mapName;
                        activeMap.transform.SetParent(transform);
                        activeMap.transform.localScale = Vector3.one;
                        activeMap.transform.localPosition = Vector3.zero;
                        activeMap.transform.localEulerAngles = Vector3.zero;
                        mapMarkers = activeMap.GetComponent<MapMarkersList>();
                        mapActive = false;
                        break;
                    }
                }
            }
        }
    }
    void Update()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.unscaledDeltaTime;
        }
    }
    public void SetPlayerPosition(string scenename)
    {
        foreach (Image img in mapMarkers.playerMarkers)
        {
            if (scenename == img.name)
            {
                img.enabled = true;
                playerCurrentPosition = img;
            }
            else
                img.enabled = false;
        }
        ShowMap(false);
    }
    public void ToggleMap()
    {
        if (cooldown <= 0)
        {
            cooldown = 1f;
            StartCoroutine("ActivateMap");
        }
    }
    IEnumerator ActivateMap()
    {
        Time.timeScale = 0f;
        GameManager.instance.gui.Fade("Black");

        yield return new WaitForSecondsRealtime(1f);
        GameManager.instance.gmAu.au.PlayOneShot(mapSound);
        GameManager.instance.gui.Fade("Game");
        ShowMap(!mapActive);
        mapActive = !mapActive;
        if (!mapActive)
            Time.timeScale = 1f;
    }
    void ShowMap(bool active)
    {
        mapMarkers.mapBack.enabled = active;
        mapMarkers.mapBlack.enabled = active;
        foreach (Stateful st in mapMarkers.markers)
        {
            st.mapMarkerImg.enabled = active;
        }
        playerCurrentPosition.enabled = active;
    }
    public void SetMarkerActive(string markerName)
    {
        foreach (Stateful st in mapMarkers.markers)
        {
            if (st.name == markerName)
            {
                st.gameObject.SetActive(true);
                st.ObjectActive(true);
                break;
            }
        }
    }
}