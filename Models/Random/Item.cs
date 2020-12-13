using System;

namespace RateLimiting.Models.Random
{
    /// <summary>
    /// Toy class for generating the randomness.
    /// </summary>
    public class Item
    {
        private readonly System.Random r = new System.Random();
        private byte[] randBytes;
        public Item(long len = 32) {
            randBytes = new byte[len];
        }
        public string random {
            get {
                r.NextBytes(randBytes);
                return Convert.ToBase64String(randBytes); 
            }
            set {}
        }
    }
}