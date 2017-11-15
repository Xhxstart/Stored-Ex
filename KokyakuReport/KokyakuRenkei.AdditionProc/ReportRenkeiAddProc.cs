using KokyakuRenkei.Common;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokyakuRenkei.AdditionProc
{
    public class ReportRenkeiAddProc
    {
        #region 定数定義

        /// <summary>
        /// ロガー
        /// </summary>
        public ILog Logger;

        /// <summary>
        /// Utility
        /// </summary>
        public Utility Util;

        /// <summary>
        /// 帳票ID
        /// </summary>
        public string ReportId;

        /// <summary>
        /// 伝票No
        /// </summary>
        public string DempyoNo;

        /// <summary>
        /// イベント種類
        /// </summary>
        private string ActionType;

        /// <summary>
        /// レポートNo
        /// </summary>
        public string ReportNo;

        /// <summary>
        /// 実行日時
        /// </summary>
        public DateTime JikkonTm;

        #endregion 定数定義

        #region 処理内容

        /// <summary>
        /// 処理内容
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="util">Utility</param>
        /// <param name="reportId">レポートID</param>
        /// <param name="dempyoNo">伝票No</param>
        /// <param name="actionType">イベント種類</param>
        /// <param name="reportNo">報告書No</param>
        /// <param name="jikkonTm">実行日時</param>
        public void Run(ILog logger, Utility util, string reportId, string dempyoNo, string actionType, string reportNo, DateTime jikkonTm)
        {
            logger.Info("ReportRenkeiAddProc#Run() Start");
            Logger = logger;
            Util = util;
            ReportId = reportId;
            DempyoNo = dempyoNo;
            ActionType = actionType;
            ReportNo = reportNo;
            JikkonTm = jikkonTm;
            logger.Info("ReportRenkeiAddProc#Run() End");
        }

        #endregion 処理内容
    }
}