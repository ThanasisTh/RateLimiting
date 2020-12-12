using System;

namespace RateLimiting.Models
{
    public class Item
    {
        private readonly Random r = new Random();
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