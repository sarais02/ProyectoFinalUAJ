//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Rendering;

//public class AchievableGrid
//{
//   float minX, minZ, maxX, maxZ;
//    bool achieve = false;
//    public bool Achieve { get { return achieve; } }

//    public bool CheckBots(List<Transform> bots)
//    {

//        foreach (var bot in bots)
//        {
//            if (bot.position.x >= minX && bot.position.x <= maxX && bot.position.z >= minZ && bot.position.z <= maxZ)
//            {
//                achieve = true;
//                return true;
//            }
//        }

//        return false;
//    }

//    public void SetParams(float minX, float maxX, float minZ, float maxZ)
//    {
//        this.minX = minX;
//        this.minZ = minZ;
//        this.maxX = maxX;
//        this.maxZ = maxZ;
//    }

//}
