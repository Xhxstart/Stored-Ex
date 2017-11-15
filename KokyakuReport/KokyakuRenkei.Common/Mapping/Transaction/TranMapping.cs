using System.Collections.Generic;

namespace KokyakuRenkei.Common.Mapping.Transaction
{
    public class TranMapping
    {
        public string FormatId { get; set; }
        public List<TranControl> ControlList { get; set; }
        public List<TranTable> UpdTableList { get; set; }
        public List<TranTable> DelTableList { get; set; }
    }
}