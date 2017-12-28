namespace Xeora.Web.Helper
{
    public class DateTime
    {
        public static long Format(bool formatJustDate = false) =>
            DateTime.Format(System.DateTime.Now, formatJustDate);

        public static long Format(System.DateTime vDateTime, bool formatJustDate = false)
        {
            string tString = string.Format("{0:0000}{1:00}{2:00}", vDateTime.Year, vDateTime.Month, vDateTime.Day);

            if (!formatJustDate)
                tString = string.Format("{0}{1:00}{2:00}{3:00}", tString, vDateTime.Hour, vDateTime.Minute, vDateTime.Second);

            return long.Parse(tString);
        }

        public static System.DateTime Format(long vDateTime)
        {
            string dtString = vDateTime.ToString();

            if ((dtString.Length >= 5 && dtString.Length <= 8) || 
                (dtString.Length >= 11 && dtString.Length <= 14))
            {
                if (dtString.Length >= 5 && dtString.Length <= 8)
                    dtString = dtString.PadLeft(8, '0');
                else if (dtString.Length >= 11 && dtString.Length <= 14)
                    dtString = dtString.PadLeft(14, '0');
            }
            else
            {
                if (vDateTime > 0)
                    throw new System.Exception("Long value must have 8 or between 14 steps according to its type!");
            }

            System.DateTime rDate = System.DateTime.MinValue;

            if (vDateTime > 0)
            {
                if (dtString.Length == 14)
                {
                    rDate = new System.DateTime(
                        int.Parse(dtString.Substring(0, 4)), 
                        int.Parse(dtString.Substring(4, 2)), 
                        int.Parse(dtString.Substring(6, 2)), 
                        int.Parse(dtString.Substring(8, 2)), 
                        int.Parse(dtString.Substring(10, 2)), 
                        int.Parse(dtString.Substring(12, 2))
                    );
                }
                else
                {
                    rDate = new System.DateTime(
                        int.Parse(dtString.Substring(0, 4)), 
                        int.Parse(dtString.Substring(4, 2)), 
                        int.Parse(dtString.Substring(6, 2))
                    );
                }
            }

            return rDate;
        }
    }
}