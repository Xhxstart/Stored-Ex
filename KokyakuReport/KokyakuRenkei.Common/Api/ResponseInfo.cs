using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokyakuRenkei.Common.Api
{
    public class ResponseInfo
    {
        public string Result { get; set; }
        private string reportNo;

        public string ReportNo
        {
            get
            {
                if (this.reportNo == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.reportNo;
                }
            }
            set
            {
                this.reportNo = value;
            }
        }

        private string updDateTime;

        public string UpdDateTime
        {
            get
            {
                if (this.updDateTime == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.updDateTime;
                }
            }
            set
            {
                this.updDateTime = value;
            }
        }

        private string message;

        public string Message
        {
            get
            {
                if (this.message == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.message;
                }
            }
            set
            {
                this.message = value;
            }
        }
    }
}