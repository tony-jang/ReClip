using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReClip.Clips
{
    public static class KeyGenerator
    {
        static int ctr = 0;
        public static long GenerateKey()
        {
            DateTime time = DateTime.Now;

            if (ctr == 9999)
            {
                ctr = 0;
            }

            var sb = new StringBuilder();

            sb.Append(time.Year.ToString().Substring(2));
            sb.Append(time.DayOfYear);
            sb.Append(time.Hour);
            sb.Append(time.Minute);
            sb.Append(time.Second);

            sb.Append(++ctr);

            long l = long.Parse(sb.ToString());

            return l;
        }
    }
}
