using System;

namespace RecentAssets
{
    [Serializable]
    public class RecentFile : IEquatable<RecentFile>
    {
        public string Guid;
        public bool IsPinned;

        public bool Equals(RecentFile other)
        {
            return other != null && Guid == other.Guid;
        }

        public override bool Equals(object obj)
        {
            return obj is RecentFile other && Equals(other);
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}