using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokyakuRenkei.Common.Mapping.Report
{
    public class JsonObject
    {
        public string Key { get; set; }
        private object _value;

        public object Value
        {
            get
            {
                if (this._value == null || string.IsNullOrEmpty(this._value.ToString()))
                {
                    return "";
                }
                else
                {
                    return this._value;
                }
            }
            set
            {
                this._value = value;
            }
        }
    }
}