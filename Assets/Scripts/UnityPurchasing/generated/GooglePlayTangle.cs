// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("mFSIOR5wryLewqzHvCQmM73eNikLAPN51FAWjm3ix5ksRNBeKpuZAu8bsvAuFbtv7wQV2spXlbbeoNzNZIDU7McBoZ/MF1DjlvDV+drRZ9biFHp73YiUyGXhvCYHkZ1POxw/eZSCImiFrmHknjQGi9Rjf8i6Pj2qhXfUtsv79xqqDuZdFd1ZtVW+A0ZmE1gEJWYj9u6eDNATnzpQr2dKxVna1NvrWdrR2Vna2tt1PlJlG42C1B/PG1Pnp2eSmnsewZQ66wErp3Br1gfiX6LAw2LXloX95fXLo/Bq+zhyYY2G8DDwosy8WqWI3q9WddYWoaVzLThi+8+GBcufWfeICH9AYNXrWdr569bd0vFdk10s1tra2t7b2FGmKzHlG63vrtnY2tva");
        private static int[] order = new int[] { 9,3,2,7,10,10,12,13,13,11,12,12,13,13,14 };
        private static int key = 219;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
