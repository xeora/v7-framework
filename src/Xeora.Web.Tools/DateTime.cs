namespace Xeora.Web.Tools
{
    public static class DateTime
    {
        public static long Format(bool formatJustDate = false) =>
            DateTime.Format(System.DateTime.Now, formatJustDate);

        public static long Format(System.DateTime vDateTime, bool formatJustDate = false)
        {
            string tString = $"{vDateTime.Year:0000}{vDateTime.Month:00}{vDateTime.Day:00}";

            if (!formatJustDate)
                tString = $"{tString}{vDateTime.Hour:00}{vDateTime.Minute:00}{vDateTime.Second:00}";

            return long.Parse(tString);
        }

        public static System.DateTime Format(long vDateTime)
        {
            string dtString = vDateTime.ToString();

            if (dtString.Length >= 5 && dtString.Length <= 8 || 
                dtString.Length >= 11 && dtString.Length <= 14)
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

            if (vDateTime <= 0) return rDate;
            
            if (dtString.Length == 14)
            {
                return new System.DateTime(
                    int.Parse(dtString.Substring(0, 4)), 
                    int.Parse(dtString.Substring(4, 2)), 
                    int.Parse(dtString.Substring(6, 2)), 
                    int.Parse(dtString.Substring(8, 2)), 
                    int.Parse(dtString.Substring(10, 2)), 
                    int.Parse(dtString.Substring(12, 2))
                );
            }
            
            return new System.DateTime(
                int.Parse(dtString.Substring(0, 4)), 
                int.Parse(dtString.Substring(4, 2)), 
                int.Parse(dtString.Substring(6, 2))
            );
        }
    }
}