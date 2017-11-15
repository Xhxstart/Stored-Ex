using System.Collections.Generic;

namespace KokyakuRenkei.Common.Mapping.Transaction
{
    public class TranTable
    {
        public string TableNm { get; set; }
        public List<TranColumn> ColumnList { get; set; }
    }
}