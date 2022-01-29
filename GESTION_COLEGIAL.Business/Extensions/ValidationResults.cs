using System;

namespace GESTION_COLEGIAL.Business.Extensions
{
    internal class ValidationResults
    {
        public static Boolean BooleanListIsTrue(Boolean[] list)
        {
            Boolean value = true;
            for (int i = 0; i < list.Length; i++)
            {
                if (!list[i])
                {
                    //Se encontro un valor falso.
                    value = false;
                    return value;
                }
            }
            return value;
        }
    }
}
