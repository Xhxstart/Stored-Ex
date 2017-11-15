using System.Collections.Generic;

namespace KokyakuRenkei.Common.Api
{
    public class ResReport
    {
        private string formatId;

        /// <summary>
        /// </summary>
        /// 報告書フォーマットID
        public string format_id
        {
            get
            {
                if (this.formatId == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.formatId;
                }
            }
            set
            {
                this.formatId = value;
            }
        }

        private string formatName;

        /// <summary>
        /// 報告書名
        /// </summary>
        public string format_name
        {
            get
            {
                if (this.formatName == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.formatName;
                }
            }
            set
            {
                this.formatName = value;
            }
        }

        private string reportId;

        /// <summary>
        /// 報告書の報告書No
        /// </summary>
        public string report_id
        {
            get
            {
                if (this.reportId == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.reportId;
                }
            }
            set
            {
                this.reportId = value;
            }
        }

        private string groupId;

        /// <summary>
        /// 報告書作成者の所属グループID
        /// </summary>
        public string group_id
        {
            get
            {
                if (this.groupId == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.groupId;
                }
            }
            set
            {
                this.groupId = value;
            }
        }

        private string groupName;

        /// <summary>
        /// 報告書作成者の所属グループ名
        /// </summary>
        public string group_name
        {
            get
            {
                if (this.groupName == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.groupName;
                }
            }
            set
            {
                this.groupName = value;
            }
        }

        private string instructionAnswer;

        /// <summary>
        /// 指示回答
        /// </summary>
        public string instruction_answer
        {
            get
            {
                if (this.instructionAnswer == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.instructionAnswer;
                }
            }
            set
            {
                this.instructionAnswer = value;
            }
        }

        private string approveStatus;

        /// <summary>
        /// 報告書の承認状態
        /// </summary>
        public string approve_status
        {
            get
            {
                if (this.approveStatus == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.approveStatus;
                }
            }
            set
            {
                this.approveStatus = value;
            }
        }

        private string userId;

        /// <summary>
        /// 報告書作成者のユーザID
        /// </summary>
        public string user_id
        {
            get
            {
                if (this.userId == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.userId;
                }
            }
            set
            {
                this.userId = value;
            }
        }

        private string userName;

        /// <summary>
        /// 報告書作成者のユーザ名
        /// </summary>
        public string user_name
        {
            get
            {
                if (this.userName == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.userName;
                }
            }
            set
            {
                this.userName = value;
            }
        }

        private string createDt;

        /// <summary>
        /// 報告書の作成日時
        /// </summary>
        public string create_dt
        {
            get
            {
                if (this.createDt == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.createDt;
                }
            }
            set
            {
                this.createDt = value;
            }
        }

        private string lastUpdateDt;

        /// <summary>
        /// 報告書の最終更新日時
        /// </summary>
        public string last_update_dt
        {
            get
            {
                if (this.lastUpdateDt == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.lastUpdateDt;
                }
            }
            set
            {
                this.lastUpdateDt = value;
            }
        }

        /// <summary>
        /// 報告書データの連想配列
        /// </summary>
        public Dictionary<string, object> report_data { get; set; }

        ///// <summary>
        ///// 報告書データの値取得
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public object GetReportDataByKey(string key)
        //{
        //    object value;
        //    if (this.report_data.ContainsKey(key) && this.report_data[key] != null)
        //    {
        //        value = this.report_data[key];
        //    }
        //    else
        //    {
        //        value = string.Empty;
        //    }
        //    return value;
        //}

        /////// <summary>
        /////// 報告書の添付画像配列
        /////// </summary>
        //public List<ResImages> images { get; set; }

        /// <summary>
        /// プロパティの値取得
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public object ReflectPropertyValue(string property)
        {
            return this.GetType().GetProperty(property).GetValue(this, null);
        }
    }
}