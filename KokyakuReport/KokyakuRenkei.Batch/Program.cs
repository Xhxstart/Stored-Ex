using KokyakuRenkei.Common;
using KokyakuRenkei.Common.Const;
using log4net;
using System;
using System.Data;
using System.Linq;

namespace KokyakuRenkei.Batch
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

            var type = string.Empty;
            if (args.Length >= 1)
            {
                type = args[0];
            }
            // トランザクション連携
            if (CommConst.TRANSACTION.Equals(type))
            {
                logger.Info(Utility.GetMsg(MsgConst.KK0031I));
                try
                {
                    var util = new Utility(logger);
                    var dt = util.GetTranRenkeiMisaiData(SqlConst.RENKEI_FLG_MISAI);
                    if (dt.Rows.Count > 0)
                    {
                        dt.AsEnumerable().GroupBy(grpKey => new { FormatId = grpKey[SqlConst.COL_FORMAT_ID], ReportNo = grpKey[SqlConst.COL_REPORT_NO] }).ToList().ForEach(grp =>
                        {
                            var updTm = DateTime.Now;
                            var dbAccess = new DbAccess(logger, Utility.GetConnection(CommConst.CUSTOMER_CONNECTION));
                            try
                            {
                                dbAccess.BeginTransaction();
                                grp.OrderByDescending(x => x[SqlConst.COL_RENKEI_SEQ]).ToList().Select((x, idx) => new { Idx = idx, Row = x }).ToList().ForEach(t =>
                                {
                                    if (t.Idx == 0)
                                    {
                                        var argsAry = new string[] { type, t.Row[SqlConst.COL_ACTION_TYPE].ToString(), t.Row[SqlConst.COL_FORMAT_ID].ToString(), t.Row[SqlConst.COL_REPORT_NO].ToString(), t.Row[SqlConst.COL_RENKEI_SEQ].ToString() };
                                        var renkei = new Renkei(argsAry);
                                        renkei.util = util;
                                        renkei.logger = logger;
                                        renkei.Run();
                                        updTm = (DateTime)util.GetTranRenkeiDataByKey((decimal)t.Row[SqlConst.COL_RENKEI_SEQ]).Rows[0][SqlConst.COL_UPD_TM];
                                    }
                                    else
                                    {
                                        var parameters = new object[] { SqlConst.P_RENKEI_FLG + CommConst.COMMA + SqlConst.RENKEI_FLG_ZUMI, SqlConst.P_UPD_TM + CommConst.COMMA + updTm, SqlConst.P_RENKEI_SEQ + CommConst.COMMA + (decimal)t.Row[SqlConst.COL_RENKEI_SEQ] };
                                        dbAccess.ExecuteNonQuery(SqlConst.QUERY_KK_TRAN_RENKEI_TBL_UPDATE_BY_KEY, parameters);
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
                    logger.Error(Utility.GetMsg(MsgConst.KK0023E));
                    logger.Error(ex);
                    rtnCd = CommConst.BATCH_ABNORMAL;
                }
                logger.Info(Utility.GetMsg(MsgConst.KK0032I));
            }
            else
            {
                // マスタ連携
                var procId = string.Empty;
                if (args.Length >= 2)
                {
                    procId = args[1];
                }
                logger.Info(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0035I), new string[] { procId }));
                try
                {
                    var renkei = new Renkei(args);
                    renkei.util = new Utility(logger);
                    renkei.logger = logger;
                    renkei.Run();
                }
                catch (Exception ex)
                {
                    logger.Error(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0030E), new string[] { procId }));
                    logger.Error(ex);
                    rtnCd = CommConst.BATCH_ABNORMAL;
                }
                logger.Info(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0036I), new string[] { procId }));
            }
            return rtnCd;
        }
    }
}