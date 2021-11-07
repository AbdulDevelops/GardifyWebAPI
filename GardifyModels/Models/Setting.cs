using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class Setting : _BaseEntity
    {
        public string Key { get; set; }


        public string Value { get; set; }

        public string Comment { get; set; }

        public SettingType SettingType { get; set; }
        public Type ActualType
        {
            get
            {
                switch (SettingType)
                {
                    case SettingType.String: return typeof(string);
                    case SettingType.Integer: return typeof(int);
                    case SettingType.Decimal: return typeof(decimal);
                    case SettingType.DateTime: return typeof(DateTime);
                    case SettingType.Other: return typeof(object);
                    default: return typeof(string);
                }
            }
        }

        public T ActualValue<T>()
        {
            T result = (T)Convert.ChangeType(Value, typeof(T));
            return result;
        }
    }

    public class SettingCreateModel
    {

        public string Key { get; set; }

        public string Value { get; set; }

        public string Comment { get; set; }

        public SettingType SettingType { get; set; }
    }

    public enum SettingType
    {
        String = 0, Integer = 1, Decimal = 2, DateTime = 3, Other = 4, Boolean = 5, Character = 6
    }
}