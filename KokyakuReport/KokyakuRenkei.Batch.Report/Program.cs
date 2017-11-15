using KokyakuRenkei.Common;
using KokyakuRenkei.Common.Const;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokyakuRenkei.Batch.Report
{
    internal class Program
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static int Main(string[] args)
        {
            var rtnCd = CommConst.BATCH_NORMAL;
            logger.Info(Utility.GetMsg(MsgConst.KK0054I));
            try
            {
                var util = new Utility(logger);
                var dt = util.GetReportRenkeiMisaiData(SqlConst.RENKEI_FLG_MISAI);
                if (dt.Rows.Count > 0)
                {
                    dt.AsEnumerable().GroupBy(grpKey => new { ReportId = grpKey[SqlConst.COL_REPORT_ID], DempyoId = grpKey[SqlConst.COL_DEMPYO_NO] }).ToList().ForEach(grp =>
                    {
                        var updTm = DateTime.Now;
                        var dbAccess = new DbAccess(logger, Utility.GetConnection(CommConst.BASIC_CONNECTION));
                        try
                        {
                            dbAccess.BeginTransaction();
                            grp.OrderByDescending(x => x[SqlConst.COL_RENKEI_SEQ]).ToList().Select((x, idx) => new { Idx = idx, Row = x }).ToList().ForEach(t =>
                            {
                                if (t.Idx == 0)
                                {
                                    var renkeiService = new RenkeiService(CommConst.TO_REPORTPLUS_1, t.Row[SqlConst.COL_ACTION_TYPE].ToString(), t.Row[SqlConst.COL_REPORT_ID].ToString(), t.Row[SqlConst.COL_DEMPYO_NO].ToString(), t.Row[SqlConst.COL_REPORT_NO].ToString(), decimal.Parse(t.Row[SqlConst.COL_RENKEI_SEQ].ToString()));
                                    renkeiService.util = util;
                                    renkeiService.logger = logger;
                                    renkeiService.SyncRun();
                                }
                                else
                                {
                                    var parameters = new object[] { SqlConst.P_REPORT_NO + CommConst.COMMA + t.Row[SqlConst.COL_REPORT_NO].ToString(), SqlConst.P_RENKEI_FLG + CommConst.COMMA + SqlConst.RENKEI_FLG_ZUMI, SqlConst.P_UPD_TM + CommConst.COMMA + updTm, SqlConst.P_RENKEI_SEQ + CommConst.COMMA + (decimal)t.Row[SqlConst.COL_RENKEI_SEQ] };
                                    dbAccess.ExecuteNonQuery(SqlConst.QUERY_KK_REPORT_RENKEI_TBL_UPDATE_BY_KEY, parameters);
                                }
                            });
                            dbAccess.Commit();
                        }
                        catch (Exception ex)
                        {
                            dbAccess.RollBack();
                            throw;
                        }
                        finally
                        {
                            dbAccess.Close();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(Utility.GetMsg(MsgConst.KK0056E));
                rtnCd = CommConst.BATCH_ABNORMAL;
            }
            logger.Info(Utility.GetMsg(MsgConst.KK0055I));
            return rtnCd;
        }
    }
}