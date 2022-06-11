using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResponse 
{

    public string YesResponse(string playerResponse)
    {
       return playerResponse = "yes";
    }

    public string NoResponse(string playerResponse)
    {
       return playerResponse = "no";
    }

    public string IDKResponse(string playerResponse)
    {
        return playerResponse = "idk";
    }

    public string ResetResponse(string playerResponse)
    {
        return playerResponse = null;
    }
}
