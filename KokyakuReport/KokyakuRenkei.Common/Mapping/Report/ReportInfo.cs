using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokyakuRenkei.Common.Mapping.Report
{
    public class ReportInfo
    {
        public List<ItemInfo> BaseData { get; set; }
        public List<ItemInfo> ReportData { get; set; }
        public List<List<ItemInfo>> ImageData { get; set; }
    }
}