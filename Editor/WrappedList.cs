using System;
using System.Collections.Generic;

namespace RecentAssets
{
    [Serializable]
    public class WrappedList<T>
    {
        public List<T> List;
    }
}