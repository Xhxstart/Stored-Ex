using System.Collections.Generic;

namespace KokyakuRenkei.Common.Mapping.Master
{
    public class MstInfo
    {
        public string MasterId { get; set; }
        public string MasterNm { get; set; }
        public string DestTableNm { get; set; }
        public string SrcTableNm { get; set; }
        public string ExtractKey { get; set; }
        public string Condition { get; set; }
        public string UpdTmColumnNm { get; set; }
        public List<MstColumn> ColumnList { get; set; }
    }
}