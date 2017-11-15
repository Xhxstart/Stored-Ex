using KokyakuRenkei.Common;
using KokyakuRenkei.Common.Api;
using KokyakuRenkei.Common.Const;
using log4net;
using System;
using System.Data;
using System.Linq;

namespace KokyakuRenkei.AdditionProc
{
    public class TorihikisakiAddProc
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
        /// イベント種類
        /// </summary>
        private string ActionType;

        /// <summary>
        /// 報告書フォーマットID
        /// </summary>
        public string FormatId;

        /// <summary>
        /// 報告書No
        /// </summary>
        public string ReportNo;

        /// <summary>
        /// 報告書情報
        /// </summary>
        public ResReport ResReport;

        /// <summary>
        /// 今回実行日時
        /// </summary>
        public DateTime KonkaiJikkonTm;

        /// <summary>
        /// 見込みランク
        /// </summary>
        private string mikomiRank;

        /// <summary>
        /// 取引先略称
        /// </summary>
        private string torihikisakiSnm = string.Empty;

        #endregion 定数定義

        #region 処理内容

        /// <summary>
        /// 処理内容
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="util">Utility</param>
        /// <param name="actionType">イベント種類</param>
        /// <param name="formatId">フォーマットID</param>
        /// <param name="reportNo">報告書No</param>
        /// <param name="resReport">報告書情報</param>
        /// <param name="konkaiJikkonTm">今回実行日時</param>
        public void Run(ILog logger, Utility util, string actionType, string formatId, string reportNo, ResReport resReport, DateTime konkaiJikkonTm)
        {
            logger.Debug("TorihikisakiAddProc#Run() Start");
            Logger = logger;
            Util = util;
            ActionType = actionType;
            FormatId = formatId;
            ReportNo = reportNo;
            ResReport = resReport;
            KonkaiJikkonTm = konkaiJikkonTm;
            mikomiRank = GetRank();
            // 個別処理
            IndivProc();
            logger.Debug("TorihikisakiAddProc#Run() End");
        }

        #endregion 処理内容

        #region 個別処理

        /// <summary>
        /// 個別処理
        /// </summary>
        private void IndivProc()
        {
            Logger.Debug("TorihikisakiAddProc#IndivProc() Start");
            // 取引先略称取得
            GetTorihikisakiSnm();
            // 顧客管理連携
            KokyakuKanriRenkei();
            // 基幹連携
            KikanRenkei();
            Logger.Debug("TorihikisakiAddProc#IndivProc() End");
        }

        #endregion 個別処理

        #region 取引先略称チェック

        private void CheckTorihikisakiSnm()
        {
            if (string.IsNullOrEmpty(torihikisakiSnm))
            {
                var msg = Utility.GetMsg(MsgConst.KK0038E);
                Logger.Error(msg);
                throw new Exception(msg);
            }
        }

        #endregion 取引先略称チェック

        #region 取引先略称取得

        private void GetTorihikisakiSnm()
        {
            Logger.Info("TorihikisakiAddProc#GetTorihikisakiSnm() Start");
            if (ResReport.report_data.ContainsKey(WebApiConst.TORIHIKISAKI_SNM)
                && ResReport.report_data[WebApiConst.TORIHIKISAKI_SNM] != null
                && !string.IsNullOrEmpty(ResReport.report_data[WebApiConst.TORIHIKISAKI_SNM].ToString()))
            {
                torihikisakiSnm = ResReport.report_data[WebApiConst.TORIHIKISAKI_SNM].ToString();
            }
            Logger.Info("TorihikisakiAddProc#GetTorihikisakiSnm() End");
        }

        #endregion 取引先略称取得

        #region 顧客管理連携

        /// <summary>
        /// 顧客管理連携
        /// </summary>
        private void KokyakuKanriRenkei()
        {
            Logger.Info("TorihikisakiAddProc#KokyakuKanriKikanRenkei() Start");
            var rank = GetRank();
            // 取引先コード未設定
            if (ResReport.report_data.ContainsKey(WebApiConst.TORIHIKISAKI_CD)
                && (ResReport.report_data[WebApiConst.TORIHIKISAKI_CD] == null || string.IsNullOrEmpty(ResReport.report_data[WebApiConst.TORIHIKISAKI_CD].ToString())))
            {
                // 顧客マスタへ登録する
                if (ResReport.report_data.ContainsKey(WebApiConst.KOKYAKU_MST_RENKEI)
                && ResReport.report_data[WebApiConst.KOKYAKU_MST_RENKEI] != null
                && CommConst.KOKYAKU_MST_RENKEI_FLAG.Equals(ResReport.report_data[WebApiConst.KOKYAKU_MST_RENKEI].ToString()))
                {
                    // 取引先略称チェック
                    CheckTorihikisakiSnm();
                    // 顧客管理取引先マスタ更新
                    UpdKokyakuKanriTorihikisakiByTorihikisakiSnm(string.Empty, torihikisakiSnm, false);
                }
            }
            else
            {
                // 取引先コードあり
                UpdKokyakuKanriTorihikisakiByTorihikisakiCd(ResReport.report_data[WebApiConst.TORIHIKISAKI_CD].ToString());
            }
            Logger.Info("TorihikisakiAddProc#KokyakuKanriKikanRenkei() End");
        }

        #endregion 顧客管理連携

        #region 基幹連携

        /// <summary>
        /// 基幹連携
        /// </summary>
        private void KikanRenkei()
        {
            Logger.Info("TorihikisakiAddProc#KikanRenkei() Start");
            // ランクリスト
            if (ResReport.report_data.ContainsKey(WebApiConst.TORIHIKISAKI_CD)
            && (ResReport.report_data[WebApiConst.TORIHIKISAKI_CD] == null || string.IsNullOrEmpty(ResReport.report_data[WebApiConst.TORIHIKISAKI_CD].ToString())))
            {
                var rankAry = Utility.Config.AppSettings.Settings[CommConst.RANK_LIST].Value.Split(CommConst.CHAR_COMMA).ToArray();
                // 対象ランクあり
                if (Array.IndexOf(rankAry, mikomiRank) != -1)
                {
                    var dbAccess = new DbAccess(Logger, Utility.GetConnection(CommConst.CUSTOMER_CONNECTION));
                    var parameters = new object[] { SqlConst.P_TORIHIKISAKI_SNM + CommConst.COMMA + torihikisakiSnm };
                    var dt = dbAccess.Reader(SqlConst.QUERY_KK_MST_TORIHIKISAKI_SELECT_BY_TORIHIKISAKI_SNM, parameters);
                    if (dt.Rows.Count == 1)
                    {
                        // 顧客管理の取引先マスタの取引先コード未設定
                        if (string.IsNullOrEmpty(dt.Rows[0][SqlConst.COL_KOKYAKU_KANRI_TORIHIKISAKI_CD].ToString()))
                        {
                            // 取引先略称チェック
                            CheckTorihikisakiSnm();
                            UpdKokyakuKanriTorihikisakiByTorihikisakiSnm(GenerateCodeForTorihikisakiOthers(torihikisakiSnm), torihikisakiSnm, true);
                        }
                    }
                    if (dt.Rows.Count > 1)
                    {
                        var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0027E), new string[] { torihikisakiSnm });
                        Logger.Error(msg);
                        throw new Exception(msg);
                    }
                }
            }
            Logger.Info("TorihikisakiAddProc#KikanRenkei() End");
        }

        #endregion 基幹連携

        #region 得意先作成ストアド呼び出す

        private string GenerateCodeForTorihikisakiOthers(string torihikisakiSnm)
        {
            Logger.Debug("TorihikisakiAddProc#GenerateCodeForTorihikisakiOthers() Start");
            var ds = new DataSet();
            var dbAccess = new DbAccess(Logger, Utility.GetConnection(CommConst.BASIC_CONNECTION));
            try
            {
                var tokuisakiFlg = 0;
                var shukkasakiFlg = 0;
                var shiresakiFlg = 0;
                if (ResReport.report_data.ContainsKey(WebApiConst.RENKEI_TARGET)
                    && ResReport.report_data[WebApiConst.RENKEI_TARGET] != null
                    && !string.IsNullOrEmpty(ResReport.report_data[WebApiConst.RENKEI_TARGET].ToString()))
                {
                    var renkeiMstLst = ResReport.report_data[WebApiConst.RENKEI_TARGET].ToString().Split(new string[] { CommConst.NEW_LINE }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (renkeiMstLst.Contains(WebApiConst.RENKEI_TARGET_TOKUISAKI))
                    {
                        tokuisakiFlg = 1;
                    }
                    if (renkeiMstLst.Contains(WebApiConst.RENKEI_TARGET_NONYUSAKI))
                    {
                        shukkasakiFlg = 1;
                    }
                    if (renkeiMstLst.Contains(WebApiConst.RENKEI_TARGET_SHIRESAKI))
                    {
                        shiresakiFlg = 1;
                    }
                }
                var parameters = new object[] { SqlConst.STORED_P_TORIHIKISAKI_NM+ CommConst.COMMA + torihikisakiSnm,
                     SqlConst.STORED_P_TORIHIKISAKI_SNM + CommConst.COMMA + torihikisakiSnm,
                     SqlConst.STORED_P_TOKUISAKI_FLG + CommConst.COMMA + tokuisakiFlg,
                     SqlConst.STORED_P_SHUKKASAKI_FLG + CommConst.COMMA + shukkasakiFlg,
                     SqlConst.STORED_P_SHIRESAKI_FLG + CommConst.COMMA + shiresakiFlg
                };
                ds = dbAccess.ExecuteStoredProcedure(SqlConst.STORED_PROCEDURE_GENERATE_CODE_FOR_TORIHIKISAKI_OTHERS, parameters);
            }
            finally
            {
                dbAccess.Close();
            }
            Logger.Debug("TorihikisakiAddProc#GenerateCodeForTorihikisakiOthers() End");
            return ds.Tables[0].Rows[0][0].ToString();
        }

        #endregion 得意先作成ストアド呼び出す

        #region 報告書のランク取得

        /// <summary>
        /// 報告書のランク取得
        /// </summary>
        /// <returns></returns>
        private string GetRank()
        {
            var rank = string.Empty;
            if (ResReport.report_data.ContainsKey(WebApiConst.MIKOMI_RANK) && ResReport.report_data[WebApiConst.MIKOMI_RANK] != null)
            {
                rank = ResReport.report_data[WebApiConst.MIKOMI_RANK].ToString();
            }

            return rank;
        }

        #endregion 報告書のランク取得

        #region 顧客管理取引先マスタ更新（取引先略称指定）

        /// <summary>
        /// 顧客管理取引先マスタ更新（取引先略称指定）
        /// </summary>
        /// <param name="torihikisakiCd"></param>
        /// <param name="torihikisakiSnm"></param>
        /// <param name="updFlag"></param>
        private void UpdKokyakuKanriTorihikisakiByTorihikisakiSnm(string torihikisakiCd, string torihikisakiSnm, bool updFlag)
        {
            Logger.Debug("TorihikisakiAddProc#UpdKokyakuKanriTorihikisakiByTorihikisakiSnm() Start");
            var dbAccess = new DbAccess(Logger, Utility.GetConnection(CommConst.CUSTOMER_CONNECTION));
            try
            {
                var parameters = new object[] { SqlConst.P_TORIHIKISAKI_SNM + CommConst.COMMA + torihikisakiSnm };
                var dt = dbAccess.Reader(SqlConst.QUERY_KK_MST_TORIHIKISAKI_SELECT_BY_TORIHIKISAKI_SNM, parameters);
                // 見込みランク変更あり
                if (dt.Rows.Count == 1 && !mikomiRank.Equals(dt.Rows[0][SqlConst.COL_MIKOMI_RANK].ToString()) || updFlag)
                {
                    dbAccess.BeginTransaction();
                    parameters = new object[] { SqlConst.P_KOKYAKU_KANRI_TORIHIKISAKI_CD + CommConst.COMMA + torihikisakiCd, SqlConst.P_MIKOMI_RANK + CommConst.COMMA + mikomiRank, SqlConst.P_UPD_TM + CommConst.COMMA + KonkaiJikkonTm, SqlConst.P_TORIHIKISAKI_SNM + CommConst.COMMA + torihikisakiSnm };
                    dbAccess.ExecuteNonQuery(SqlConst.QUERY_KK_MST_TORIHIKISAKI_UPDATE_BY_TORIHIKISAKI_SNM, parameters);
                    dbAccess.Commit();
                }
                if (dt.Rows.Count == 0)
                {
                    dbAccess.BeginTransaction();
                    parameters = new object[] { SqlConst.P_KOKYAKU_KANRI_TORIHIKISAKI_CD + CommConst.COMMA + torihikisakiCd, SqlConst.P_TORIHIKISAKI_SNM + CommConst.COMMA + torihikisakiSnm, SqlConst.P_MIKOMI_RANK + CommConst.COMMA + mikomiRank, SqlConst.P_INS_TM + CommConst.COMMA + KonkaiJikkonTm, SqlConst.P_UPD_TM + CommConst.COMMA + KonkaiJikkonTm };
                    dbAccess.ExecuteNonQuery(SqlConst.QUERY_KK_MST_TORIHIKISAKI_INSERT, parameters);
                    dbAccess.Commit();
                }
                if (dt.Rows.Count > 1)
                {
                    var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0027E), new string[] { torihikisakiSnm });
                    Logger.Error(msg);
                    throw new Exception(msg);
                }
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
            Logger.Debug("TorihikisakiAddProc#UpdKokyakuKanriTorihikisakiByTorihikisakiSnm() End");
        }

        #endregion 顧客管理取引先マスタ更新（取引先略称指定）

        #region 顧客管理取引先マスタ更新（取引先コード指定）

        /// <summary>
        /// 顧客管理取引先マスタ更新（取引先コード指定）
        /// </summary>
        /// <param name="torihikisakiCd"></param>
        private void UpdKokyakuKanriTorihikisakiByTorihikisakiCd(string torihikisakiCd)
        {
            Logger.Debug("TorihikisakiAddProc#UpdKokyakuKanriTorihikisakiByTorihikisakiCd() Start");
            var dbAccess = new DbAccess(Logger, Utility.GetConnection(CommConst.CUSTOMER_CONNECTION));
            try
            {
                var parameters = new object[] { SqlConst.P_KOKYAKU_KANRI_TORIHIKISAKI_CD + CommConst.COMMA + torihikisakiCd };
                var dt = dbAccess.Reader(SqlConst.QUERY_KK_MST_TORIHIKISAKI_SELECT_BY_TORIHIKISAKI_CD, parameters);
                // 見込みランク変更あり
                if (dt.Rows.Count >= 1 && !mikomiRank.Equals(dt.Rows[0][SqlConst.COL_MIKOMI_RANK].ToString()))
                {
                    dbAccess.BeginTransaction();
                    parameters = new object[] { SqlConst.P_TORIHIKISAKI_SNM + CommConst.COMMA + torihikisakiSnm, SqlConst.P_MIKOMI_RANK + CommConst.COMMA + mikomiRank, SqlConst.P_UPD_TM + CommConst.COMMA + KonkaiJikkonTm, SqlConst.P_KOKYAKU_KANRI_TORIHIKISAKI_CD + CommConst.COMMA + torihikisakiCd };
                    dbAccess.ExecuteNonQuery(SqlConst.QUERY_KK_MST_TORIHIKISAKI_UPDATE_BY_TORIHIKISAKI_CD, parameters);
                    dbAccess.Commit();
                }
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
            Logger.Debug("TorihikisakiAddProc#UpdKokyakuKanriTorihikisakiByTorihikisakiCd() End");
        }

        #endregion 顧客管理取引先マスタ更新（取引先コード指定）
    }
}