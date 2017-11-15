using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokyakuRenkei.Common.Mapping.Report
{
    public class ReportMapping
    {
        public string ReportId { get; set; }
        public string FormatId { get; set; }
        public string KAISHA_CD { get; set; }
        public string TableNm { get; set; }
        public string Condition { get; set; }
        public string RegisterKey { get; set; }
        public ReportInfo ReportInfo { get; set; }
    }
}