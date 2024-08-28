using System.Collections.Generic;
using UnityEngine;

namespace RecentAssets
{
    public class RecentAssetsSettings : ScriptableObject
    {
        public int MaxRecentAssets;
        public List<string> BannedPatterns;
    }
}