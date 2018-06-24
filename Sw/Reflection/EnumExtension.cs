using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class EnumExtension
    {
        public static TAtt GetAttribute<TAtt>(this Enum enumer)
            where TAtt : Attribute
        {
            //if (!typeof(TEnum).IsEnum)
            //{
            //    throw new ArgumentException($"{typeof(TEnum)} must be an enumerated type");
            //}

            var enumType = enumer.GetType();
            var enumField = enumType.GetMember(enumer.ToString()).FirstOrDefault();
            var atts = enumField.GetCustomAttributes(typeof(TAtt), false);

            if (atts != null && atts.Any())
            {
                return atts.First() as TAtt;
            }
            else
            {
                throw new NullReferenceException($"Attribute of type {typeof(TAtt)} is not fond on {enumer}");
            }
        }
    }
}
