using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokyakuRenkei.Common.Mapping.Report
{
    public class ItemInfo
    {
        public string ReportDataKey { get; set; }
        public string ColumnNm { get; set; }
        public string DefaultValue { get; set; }
        public string CD_SECTION { get; set; }
        public string ImageRootPath { get; set; }
        public bool DempyoHdrFlag { get; set; }
        public string HdrDtlType { get; set; }
        public bool Condition { get; set; }
        public object Value { get; set; }
    }
}