
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public List<Map> mapList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    public void VoteMap(int _mapId)
    {
        mapList[_mapId].vote++;
    }

    public int GetMostVotedMap()
    {
        int _mostVotedMap = 0;
        int _biggestVote = 0;
        for(int i = 0; i < mapList.Count; i++)
        {
            if(mapList[i].vote > _biggestVote)
            {
                _mostVotedMap = i;
                _biggestVote = mapList[i].vote;
            }
        }

        return _mostVotedMap;
    }
}
