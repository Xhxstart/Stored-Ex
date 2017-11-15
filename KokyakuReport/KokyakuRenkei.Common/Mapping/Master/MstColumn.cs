namespace KokyakuRenkei.Common.Mapping.Master
{
    public class MstColumn
    {
        public string DestColumnNm { get; set; }
        public string SrcColumnNm { get; set; }
        public string MasterHeaderNm { get; set; }
        public bool BlankColumnSkipRowFlag { get; set; }
    }
}