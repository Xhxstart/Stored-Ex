using Codeplex.Data;
using KokyakuRenkei.Common.Api;
using KokyakuRenkei.Common.Const;
using KokyakuRenkei.Common.Mapping.Master;
using KokyakuRenkei.Common.Mapping.Report;
using KokyakuRenkei.Common.Mapping.Transaction;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KokyakuRenkei.Common
{
    /// <summary>
    /// 顧客連携処理
    /// </summary>
    public class Renkei
    {
        #region 変数定義

        /// <summary>
        /// バッチ引数
        /// </summary>
        private string[] Args;

        /// <summary>
        /// Utility
        /// </summary>
        public Utility util;

        /// <summary>
        /// バッチ実行種別
        /// </summary>
        private string Type;

        /// <summary>
        /// 処理ID
        /// </summary>
        private string ProcId;

        /// <summary>
        /// イベント種類
        /// </summary>
        private string ActionType;

        /// <summary>
        /// 報告書フォーマットID
        /// </summary>
        private string FormatId;

        /// <summary>
        /// 報告書No
        /// </summary>
        private string ReportNo;

        /// <summary>
        /// トランザクションマッピング
        /// </summary>
        private TranMapping transactionMapping;

        /// <summary>
        /// マスタマッピング
        /// </summary>
        private MstMapping masterMapping;

        /// <summary>
        /// 報告書情報
        /// </summary>
        private ResReport resReport;

        /// <summary>
        /// 追加処理クラス名
        /// </summary>
        private dynamic classNm;

        /// <summary>
        /// 履歴登録済フラグ
        /// </summary>
        private bool zenkaiRenkeiFlg = false;

        /// <summary>
        /// 前回実行日時
        /// </summary>
        private DateTime zenkaiJikkonTm;

        /// <summary>
        /// 今回実行日時
        /// </summary>
        private DateTime konkaiJikkonTm;

        /// <summary>
        /// ロガー
        /// </summary>
        public ILog logger;

        /// <summary>
        /// トランザクションマッピングデータ
        /// </summary>
        private static List<TranMapping> transactionMappingData;

        /// <summary>
        /// マスタマッピングデータ
        /// </summary>
        private static List<MstMapping> masterMappingData;

        /// <summary>
        /// 追加クラスのアセンブリ
        /// </summary>
        private static Assembly assembly;

        #endregion 変数定義

        #region コンストラクタ

        private Renkei()
        { }

        public Renkei(string[] args)
        {
            Args = args;
            // 今回実行日時
            konkaiJikkonTm = DateTime.Now;
        }

        #endregion コンストラクタ

        #region 顧客連携処理

        /// <summary>
        /// 快作レポート＋→顧客連携処理
        /// </summary>
        public void Run()
        {
            logger.Info("Renkei#Run() Start");
            try
            {
                // バッチ引数チェック
                CheckArgs(Args);
                // トランザクション連携の場合
                if (CommConst.TRANSACTION.Equals(Type))
                {
                    // 快作レポート＋→顧客管理→基幹
                    TransactionProc();
                }
                else
                {
                    // 基幹→顧客管理→快作レポート＋
                    MasterProc();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            logger.Info("Renkei#Run() End");
        }

        #endregion 顧客連携処理

        #region privateメソッド

        #region バッチ引数チェック

        /// <summary>
        /// バッチ引数チェック
        /// </summary>
        /// <param name="args"></param>
        private void CheckArgs(string[] args)
        {
            logger.Info("Renkei#CheckArgs() Start");
            if (args.Length == 0)
            {
                var msg = Utility.GetMsg(MsgConst.KK0002E);
                logger.Error(msg);
                throw new Exception(msg);
            }
            if (args.Length >= 1 && string.IsNullOrEmpty(args[0]))
            {
                var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0003E), new string[] { CommConst.MSG_ARGUS_TYPE });
                logger.Error(msg);
                throw new Exception(msg);
            }
            if (args.Length >= 1 && (!CommConst.MASTER.Equals(args[0]) && !CommConst.TRANSACTION.Equals(args[0])))
            {
                var msg = Utility.GetMsg(MsgConst.KK0024E);
                logger.Error(msg);
                throw new Exception(msg);
            }
            // トランザクション連携の場合
            if (CommConst.TRANSACTION.Equals(args[0]))
            {
                if (args.Length < 2 || (args.Length >= 2 && string.IsNullOrEmpty(args[1])))
                {
                    var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0003E), new string[] { CommConst.MSG_ARGUS_ACTION_TYPE });
                    logger.Error(msg);
                    throw new Exception(msg);
                }
                if (args.Length < 3 || (args.Length >= 3 && string.IsNullOrEmpty(args[2])))
                {
                    var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0003E), new string[] { CommConst.MSG_ARGUS_FORMAT_ID });
                    logger.Error(msg);
                    throw new Exception(msg);
                }
                if (args.Length < 4 || (args.Length >= 4 && string.IsNullOrEmpty(args[3])))
                {
                    var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0003E), new string[] { CommConst.MSG_ARGUS_REPORT_NO });
                    logger.Error(msg);
                    throw new Exception(msg);
                }
            }
            else
            {
                // マスタ連携の場合
                if (args.Length < 2 || (args.Length >= 2 && string.IsNullOrEmpty(args[1])))
                {
                    var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0003E), new string[] { CommConst.MSG_ARGUS_PROC_ID });
                    logger.Error(msg);
                    throw new Exception(msg);
                }
            }
            Type = args[0];
            // トランザクション連携の場合
            if (CommConst.TRANSACTION.Equals(Type))
            {
                ActionType = args[1];
                FormatId = args[2];
                ReportNo = args[3];
            }
            else
            {
                // マスタ連携の場合
                ProcId = args[1];
            }

            logger.Info("Renkei#CheckArgs() End");
        }

        #endregion バッチ引数チェック

        #region トランザクション処理

        /// <summary>
        /// トランザクション処理
        /// </summary>
        private void TransactionProc()
        {
            logger.Info(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0013I), new string[] { ActionType, FormatId, ReportNo }));
            logger.Info("Renkei#TransactionProc() Start");
            // イベント種類リスト
            var actionTypeAry = Utility.Config.AppSettings.Settings[CommConst.ACTION_TYPE_LIST].Value.Split(CommConst.CHAR_COMMA).ToArray();
            // 対象イベントあり
            if (Array.IndexOf(actionTypeAry, ActionType) != -1)
            {
                // トランザクションテーブルマッピングマスタデータ取得
                var dt = util.GetTranMappingMst(FormatId);
                // 対象フォーマットID定義あり
                if (dt.Rows.Count > 0)
                {
                    var seq = decimal.Zero;
                    // リカバリー場合
                    if (Args.Length >= 5 && !string.IsNullOrEmpty(Args[4]))
                    {
                        seq = Convert.ToDecimal(Args[4]);
                    }
                    else
                    {
                        // トランザクション連携テーブルにデータインサート
                        seq = util.InsTranRenkeiData(ActionType, FormatId, ReportNo, konkaiJikkonTm);
                    }
                    // トランザクションマッピング取得
                    GetTransactionMapping();
                    // 報告書情報取得API
                    ReqGetReportInfo();
                    // トランザクションテーブル更新
                    UpdTransactionTable();
                    // 追加クラス処理（イベント種類削除の場合呼ばない）
                    if (!CommConst.REPORT_DELETE.Equals(ActionType))
                    {
                        var clazzNm = dt.Rows[0][CommConst.ADDITION_PROC_CLASS_NM].ToString();
                        if (!string.IsNullOrEmpty(clazzNm))
                        {
                            // クラス名
                            classNm = dt.Rows[0][CommConst.ADDITION_PROC_CLASS_NM];
                            // 追加処理
                            AdditionClassProc();
                        }
                    }
                    // トランザクション連携テーブルにデータアップデート
                    util.UpdTranRenkeiZumiData(seq, konkaiJikkonTm);
                }
            }
            logger.Info("Renkei#TransactionProc() End");
            logger.Info(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0014I), new string[] { ActionType, FormatId, ReportNo }));
        }

        #endregion トランザクション処理

        #region マスタ処理

        /// <summary>
        /// マスタ処理
        /// </summary>
        private void MasterProc()
        {
            logger.Info("Renkei#MasterProc() Start");
            // マスタマッピング取得
            GetMasterMapping();
            // マスタマッピングある
            if (masterMapping != null)
            {
                // 基幹データ元に顧客連携
                KikanKokyakuKanriRenkei();
                // 顧客管理データ元に快作レポート＋連携
                KokyakuKanriKaisakuReportRenkei();
            }
            logger.Info("Renkei#MasterProc() End");
        }

        #endregion マスタ処理

        #region 基幹→顧客連携

        /// <summary>
        /// 基幹→顧客管理連携
        /// </summary>
        private void KikanKokyakuKanriRenkei()
        {
            logger.Info("Renkei#KikanKokyakuKanriRenkei() Start");
            // マスタ連携バッチ実行履歴テーブルデータ取得
            var dt = util.GetMstRenkeiBatchJikkouRireikiTbl(ProcId);
            // 前回連携あり
            if (dt.Rows.Count > 0)
            {
                zenkaiRenkeiFlg = true;
                if (dt.Rows[0][SqlConst.COL_KOKYAKU_KANRI_ZENKAI_JIKKON_TM] == null || string.IsNullOrEmpty(dt.Rows[0][SqlConst.COL_KOKYAKU_KANRI_ZENKAI_JIKKON_TM].ToString()))
                {
                    zenkaiRenkeiFlg = false;
                    zenkaiJikkonTm = konkaiJikkonTm;
                }
                else
                {
                    zenkaiJikkonTm = (DateTime)dt.Rows[0][SqlConst.COL_KOKYAKU_KANRI_ZENKAI_JIKKON_TM];
                }
            }
            else
            {
                zenkaiRenkeiFlg = false;
            }
            var dbAccess = new DbAccess(logger, Utility.GetConnection(CommConst.CUSTOMER_CONNECTION));
            try
            {
                dbAccess.BeginTransaction();
                // マスタテーブル更新
                UpdMasterTable(dbAccess, masterMapping);
                // マスタ連携バッチ実行履歴テーブル更新（前回実行日時指定）
                UpdateMstRenkeiBatchJikkouRireikiByZenkaiJikkonTm(dbAccess);
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
            logger.Info("Renkei#KikanKokyakuKanriRenkei() End");
        }

        #endregion 基幹→顧客連携

        #region 顧客管理→快作レポート＋連携

        /// <summary>
        /// 顧客管理→快作レポート＋連携
        /// </summary>
        private void KokyakuKanriKaisakuReportRenkei()
        {
            logger.Info("Renkei#KokyakuKanriKaisakuReportRenkei() Start");
            // マスタ連携バッチ実行履歴テーブルデータ取得
            var dt = util.GetMstRenkeiBatchJikkouRireikiTbl(ProcId);
            // webapi前回実行日時
            zenkaiJikkonTm = DateTime.Now;
            // 前回連携ありか
            if (dt.Rows.Count > 0)
            {
                zenkaiRenkeiFlg = true;
                if (dt.Rows[0][SqlConst.COL_WEBAPI_ZENKAI_JIKKON_TM] == null || string.IsNullOrEmpty(dt.Rows[0][SqlConst.COL_WEBAPI_ZENKAI_JIKKON_TM].ToString()))
                {
                    zenkaiRenkeiFlg = false;
                    zenkaiJikkonTm = konkaiJikkonTm;
                }
                else
                {
                    zenkaiJikkonTm = (DateTime)dt.Rows[0][SqlConst.COL_WEBAPI_ZENKAI_JIKKON_TM];
                }
            }
            else
            {
                zenkaiRenkeiFlg = false;
            }
            // 報告データマスタアップロードAPI
            UploadReportMaster();
            // マスタ連携バッチ実行履歴テーブル更新（webapi前回実行日時指定）
            UpdateMstRenkeiBatchJikkouRireikiByWebapiZenkaiJikkonTm();
            logger.Info("Renkei#KokyakuKanriKaisakuReportRenkei() End");
        }

        #endregion 顧客管理→快作レポート＋連携

        #region 報告データマスタアップロードAPI

        /// <summary>
        /// 報告データマスタアップロードAPI
        /// </summary>
        private void UploadReportMaster()
        {
            logger.Info("Renkei#UploadReportMaster() Start");
            // 連携元テーブル名、連携先マスタID、連携先マスタ名設定した場合
            masterMapping.TablesList.Where(x => !string.IsNullOrEmpty(x.MasterId) && !string.IsNullOrEmpty(x.MasterNm) && !string.IsNullOrEmpty(x.DestTableNm)).Select(x => x).ToList().ForEach(x =>
            {
                var csvData = string.Empty;
                // 顧客管理に未連携データあり
                if (GetKokyakuKanriData(x).Rows.Count > 0)
                {
                    zenkaiRenkeiFlg = false;
                    GetKokyakuKanriData(x).AsEnumerable().Select((row, idx) => new { Idx = idx, Row = row }).ToList().ForEach(t =>
                    {
                        var masterHeaderNmLst = x.ColumnList.Where(y => !string.IsNullOrEmpty(y.MasterHeaderNm)).Select(y => y).ToList();
                        if (masterHeaderNmLst.Count == 0)
                        {
                            var msg = Utility.GetMsg(MsgConst.KK0037E);
                            logger.Error(msg);
                            throw new Exception(msg);
                        }
                        if (t.Idx == 0 && masterHeaderNmLst.Count > 0)
                        {
                            csvData = csvData + CommConst.SINGLE_DOUBLE_QUOTATION + string.Join(CommConst.SINGLE_DOUBLE_QUOTATION + CommConst.COMMA + CommConst.SINGLE_DOUBLE_QUOTATION, masterHeaderNmLst.Select(z => z.MasterHeaderNm).ToList()) + CommConst.SINGLE_DOUBLE_QUOTATION + CommConst.NEW_LINE;
                        }

                        if (masterHeaderNmLst.Where(skipRow => skipRow.BlankColumnSkipRowFlag && t.Row[skipRow.DestColumnNm] != null && !string.IsNullOrEmpty(t.Row[skipRow.DestColumnNm].ToString())).Count() == masterHeaderNmLst.Where(skipRow => skipRow.BlankColumnSkipRowFlag).Count())
                        {
                            masterHeaderNmLst.ForEach(y =>
                            {
                                csvData = csvData + CommConst.SINGLE_DOUBLE_QUOTATION + util.EscapeDoubleQuotation(t.Row[y.DestColumnNm].ToString()) + CommConst.SINGLE_DOUBLE_QUOTATION + CommConst.COMMA;
                            });
                            csvData = csvData.Substring(0, csvData.Length - CommConst.COMMA.Length) + CommConst.NEW_LINE;
                        }
                        logger.Debug(WebApiConst.MASTER_RENKEI_CSV_DATA);
                        logger.Debug(csvData);
                    });
                    var csvDataLst = csvData.Split(new string[] { CommConst.NEW_LINE }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (csvDataLst.Count >= 2)
                    {
                        util.ReqUpdReportMaster(x.MasterId, CreateReportMasterJsonData(x.MasterNm, csvData));
                    }
                }
            });

            logger.Info("Renkei#UploadReportMaster() End");
        }

        #endregion 報告データマスタアップロードAPI

        #region 報告データマスタ更新Json作成

        /// <summary>
        /// 報告データマスタ更新Json作成
        /// </summary>
        /// <param name="masterName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string CreateReportMasterJsonData(string masterName, string data)
        {
            logger.Debug("Renkei#CreateReportMasterJsonData() Start");
            var obj = new
            {
                master_name = masterName,
                items = data
            };
            logger.Debug("Renkei#CreateReportMasterJsonData() End");
            return DynamicJson.Serialize(obj);
        }

        #endregion 報告データマスタ更新Json作成

        #region マスタ連携バッチ実行履歴テーブル更新（前回実行日時指定）

        /// <summary>
        /// マスタ連携バッチ実行履歴テーブル更新（前回実行日時指定）
        /// </summary>
        /// <param name="dbAccess"></param>
        private void UpdateMstRenkeiBatchJikkouRireikiByZenkaiJikkonTm(DbAccess dbAccess)
        {
            logger.Info("Renkei#UpdateMstRenkeiBatchJikkouRireikiByZenkaiJikkonTm() Start");
            var parameters = new object[] { SqlConst.P_KOKYAKU_KANRI_ZENKAI_JIKKON_TM + CommConst.COMMA + konkaiJikkonTm, SqlConst.P_PROC_ID + CommConst.COMMA + ProcId };
            var count = dbAccess.ExecuteNonQuery(SqlConst.QUERY_KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL_UPDATE_BY_PROC_ID_AND_ZENKAI_JIKKON_TM, parameters);
            if (count == 0)
            {
                parameters = new object[] { SqlConst.P_PROC_ID + CommConst.COMMA + ProcId, SqlConst.P_KOKYAKU_KANRI_ZENKAI_JIKKON_TM + CommConst.COMMA + konkaiJikkonTm };
                dbAccess.ExecuteNonQuery(SqlConst.QUERY_KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL_INSERT_BY_ZENKAI_JIKKON_TM, parameters);
            }
            logger.Info("Renkei#UpdateMstRenkeiBatchJikkouRireikiByZenkaiJikkonTm() End");
        }

        #endregion マスタ連携バッチ実行履歴テーブル更新（前回実行日時指定）

        #region マスタ連携バッチ実行履歴テーブル更新（webapi前回実行日時指定）

        /// <summary>
        /// マスタ連携バッチ実行履歴テーブル更新（webapi前回実行日時指定）
        /// </summary>
        private void UpdateMstRenkeiBatchJikkouRireikiByWebapiZenkaiJikkonTm()
        {
            logger.Info("Renkei#UpdateMstRenkeiBatchJikkouRireikiByWebapiZenkaiJikkonTm() Start");
            var dbAccess = new DbAccess(logger, Utility.GetConnection(CommConst.CUSTOMER_CONNECTION));
            try
            {
                dbAccess.BeginTransaction();
                var parameters = new object[] { SqlConst.P_WEBAPI_ZENKAI_JIKKON_TM + CommConst.COMMA + konkaiJikkonTm, SqlConst.P_PROC_ID + CommConst.COMMA + ProcId };
                var count = dbAccess.ExecuteNonQuery(SqlConst.QUERY_KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL_UPDATE_BY_PROC_ID_AND_WEBAPI_ZENKAI_JIKKON_TM, parameters);
                if (count == 0)
                {
                    parameters = new object[] { SqlConst.P_PROC_ID + CommConst.COMMA + ProcId, SqlConst.P_WEBAPI_ZENKAI_JIKKON_TM + CommConst.COMMA + konkaiJikkonTm };
                    dbAccess.ExecuteNonQuery(SqlConst.QUERY_KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL_INSERT_BY_WEBAPI_ZENKAI_JIKKON_TM, parameters);
                }
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
            logger.Info("Renkei#UpdateMstRenkeiBatchJikkouRireikiByWebapiZenkaiJikkonTm() End");
        }

        #endregion マスタ連携バッチ実行履歴テーブル更新（webapi前回実行日時指定）

        #region マスタテーブル更新

        /// <summary>
        /// マスタテーブル更新
        /// </summary>
        /// <param name="dbAccess"></param>
        /// <param name="masterMapping"></param>
        private void UpdMasterTable(DbAccess dbAccess, MstMapping masterMapping)
        {
            logger.Info("Renkei#UpdMasterTable() Start");
            // 連携元テーブル名設定した場合
            masterMapping.TablesList.Where(x => !string.IsNullOrEmpty(x.SrcTableNm) && !string.IsNullOrEmpty(x.DestTableNm)).Select(x => x).ToList().ForEach(x =>
            {
                // マスタ情報よりデータ取得（基幹データ）
                GetBasicData(x).AsEnumerable().ToList().ForEach(row =>
        {
            var query = SqlConst.UPDATE + CommConst.HALF_SPACE + x.DestTableNm + CommConst.HALF_SPACE + SqlConst.SET;
            var parametersLst = new List<object>();
            x.ColumnList.Where(y => !string.IsNullOrEmpty(y.SrcColumnNm) && !string.IsNullOrEmpty(x.DestTableNm) && !y.DestColumnNm.Equals(x.ExtractKey)).Select(y => y).ToList().ForEach(y =>
    {
        query = query + CommConst.HALF_SPACE + y.DestColumnNm + CommConst.HALF_SPACE + CommConst.EQUAL + CommConst.HALF_SPACE + CommConst.AT + y.DestColumnNm + CommConst.COMMA;
        parametersLst.Add(CommConst.AT + y.DestColumnNm + CommConst.COMMA + row[y.SrcColumnNm]);
    });
            // 更新日時
            query = query + CommConst.HALF_SPACE + x.UpdTmColumnNm + CommConst.HALF_SPACE + CommConst.EQUAL + CommConst.HALF_SPACE + CommConst.AT + x.UpdTmColumnNm + CommConst.HALF_SPACE + SqlConst.WHERE + CommConst.HALF_SPACE + x.ExtractKey + CommConst.HALF_SPACE + CommConst.EQUAL + CommConst.HALF_SPACE + CommConst.AT + x.ExtractKey;
            parametersLst.Add(CommConst.AT + x.UpdTmColumnNm + CommConst.COMMA + konkaiJikkonTm);
            parametersLst.Add(CommConst.AT + x.ExtractKey + CommConst.COMMA + row[x.ExtractKey]);

            // 連携先テーブルへアップデート（顧客管理）
            var count = dbAccess.ExecuteNonQuery(query, parametersLst.ToArray());
            if (count == 0)
            {
                var insSql = SqlConst.INSERT + CommConst.HALF_SPACE + SqlConst.INTO + CommConst.HALF_SPACE + x.DestTableNm + CommConst.HALF_SPACE;
                var colSql = CommConst.LEFT_BRACKET;
                var valSql = SqlConst.VALUES + CommConst.LEFT_BRACKET;
                var values = new List<object>();
                x.ColumnList.Where(y => !string.IsNullOrEmpty(y.SrcColumnNm) && !string.IsNullOrEmpty(x.DestTableNm)).Select(y => y).ToList().ForEach(y =>
        {
            colSql = colSql + y.DestColumnNm + CommConst.COMMA + CommConst.HALF_SPACE;
            valSql = valSql + CommConst.AT + y.DestColumnNm + CommConst.COMMA + CommConst.HALF_SPACE;
            values.Add(CommConst.AT + y.DestColumnNm + CommConst.COMMA + row[y.SrcColumnNm]);
        });
                // 登録日時
                colSql = colSql + SqlConst.COL_INS_TM + CommConst.COMMA + CommConst.HALF_SPACE;
                valSql = valSql + SqlConst.P_INS_TM + CommConst.COMMA + CommConst.HALF_SPACE;
                values.Add(SqlConst.P_INS_TM + CommConst.COMMA + konkaiJikkonTm);
                // 更新日時
                colSql = colSql + SqlConst.COL_UPD_TM + CommConst.COMMA + CommConst.HALF_SPACE;
                valSql = valSql + SqlConst.P_UPD_TM + CommConst.COMMA + CommConst.HALF_SPACE;
                values.Add(SqlConst.P_UPD_TM + CommConst.COMMA + konkaiJikkonTm);

                colSql = colSql.Substring(0, colSql.LastIndexOf(CommConst.COMMA)) + CommConst.RIGHT_BRACKET;
                valSql = valSql.Substring(0, valSql.LastIndexOf(CommConst.COMMA)) + CommConst.RIGHT_BRACKET;
                // 連携先テーブルへインサート（顧客管理）
                dbAccess.ExecuteNonQuery(insSql + colSql + valSql, values.ToArray());
            }
        });
            });
            logger.Info("Renkei#UpdMasterTable() End");
        }

        #endregion マスタテーブル更新

        #region マスタ情報よりデータ取得（基幹データ）

        /// <summary>
        /// マスタ情報よりデータ取得（基幹データ）
        /// </summary>
        /// <param name="masterInfo"></param>
        /// <returns></returns>
        private DataTable GetBasicData(MstInfo masterInfo)
        {
            logger.Debug("Renkei#GetBasicData() Start");
            var dt = new DataTable();
            // 連携先テーブル名設定した場合
            if (masterInfo.DestTableNm != null)
            {
                var dbAccess = new DbAccess(logger, Utility.GetConnection(CommConst.BASIC_CONNECTION));
                try
                {
                    var query = SqlConst.SELECT + CommConst.HALF_SPACE + string.Join(CommConst.COMMA + CommConst.HALF_SPACE, masterInfo.ColumnList.Where(x => !string.IsNullOrEmpty(x.SrcColumnNm) && !string.IsNullOrEmpty(x.DestColumnNm)).Select(x => x.SrcColumnNm).ToList()) + CommConst.HALF_SPACE + SqlConst.FROM + CommConst.HALF_SPACE + masterInfo.SrcTableNm;
                    var condition = SqlConst.DEFAULT_CONDITION;
                    if (!string.IsNullOrEmpty(masterInfo.Condition))
                    {
                        condition = masterInfo.Condition;
                    }
                    query = query + CommConst.HALF_SPACE + SqlConst.WHERE + CommConst.HALF_SPACE + condition + CommConst.HALF_SPACE;
                    var parameters = new object[] { };
                    if (zenkaiRenkeiFlg)
                    {
                        query = query + SqlConst.AND + CommConst.HALF_SPACE + masterInfo.UpdTmColumnNm + CommConst.HALF_SPACE + CommConst.GREATER_THAN + CommConst.HALF_SPACE + SqlConst.P_KOKYAKU_KANRI_ZENKAI_JIKKON_TM + CommConst.HALF_SPACE + SqlConst.AND + CommConst.HALF_SPACE + masterInfo.UpdTmColumnNm + CommConst.HALF_SPACE + CommConst.LESS_THAN_OR_EQUAL + CommConst.HALF_SPACE + SqlConst.P_KONKAI_JIKKON_TM;
                        parameters = new object[] { SqlConst.P_KOKYAKU_KANRI_ZENKAI_JIKKON_TM + CommConst.COMMA + zenkaiJikkonTm, SqlConst.P_KONKAI_JIKKON_TM + CommConst.COMMA + konkaiJikkonTm };
                    }
                    dt = dbAccess.Reader(query, parameters);
                }
                finally
                {
                    dbAccess.Close();
                }
            }
            logger.Debug("Renkei#GetBasicData() End");
            return dt;
        }

        #endregion マスタ情報よりデータ取得（基幹データ）

        #region マスタ情報よりデータ取得（顧客管理データ）

        /// <summary>
        /// マスタ情報よりデータ取得（顧客管理データ）
        /// </summary>
        /// <param name="masterInfo"></param>
        /// <returns></returns>
        private DataTable GetKokyakuKanriData(MstInfo masterInfo)
        {
            logger.Debug("Renkei#GetKokyakuKanriData() Start");
            var dt = new DataTable();
            // マスタID、マスタ名、連携先テーブル名設定した場合
            if (!string.IsNullOrEmpty(masterInfo.MasterId) && !string.IsNullOrEmpty(masterInfo.MasterNm) && !string.IsNullOrEmpty(masterInfo.DestTableNm))
            {
                var dbAccess = new DbAccess(logger, Utility.GetConnection(CommConst.CUSTOMER_CONNECTION));
                try
                {
                    var query = SqlConst.SELECT + CommConst.HALF_SPACE + string.Join(CommConst.COMMA + CommConst.HALF_SPACE, masterInfo.ColumnList.Where(x => !string.IsNullOrEmpty(x.DestColumnNm) && !string.IsNullOrEmpty(x.MasterHeaderNm)).Select(x => x.DestColumnNm).ToList()) + CommConst.HALF_SPACE + SqlConst.FROM + CommConst.HALF_SPACE + masterInfo.DestTableNm;
                    var condition = SqlConst.DEFAULT_CONDITION;
                    query = query + CommConst.HALF_SPACE + SqlConst.WHERE + CommConst.HALF_SPACE + condition + CommConst.HALF_SPACE;
                    var parameters = new object[] { };
                    if (zenkaiRenkeiFlg)
                    {
                        query = query + SqlConst.AND + CommConst.HALF_SPACE + masterInfo.UpdTmColumnNm + CommConst.HALF_SPACE + CommConst.GREATER_THAN + CommConst.HALF_SPACE + SqlConst.P_WEBAPI_ZENKAI_JIKKON_TM + CommConst.HALF_SPACE + SqlConst.AND + CommConst.HALF_SPACE + masterInfo.UpdTmColumnNm + CommConst.HALF_SPACE + CommConst.LESS_THAN_OR_EQUAL + CommConst.HALF_SPACE + SqlConst.P_KONKAI_JIKKON_TM;
                        parameters = new object[] { SqlConst.P_WEBAPI_ZENKAI_JIKKON_TM + CommConst.COMMA + zenkaiJikkonTm, SqlConst.P_KONKAI_JIKKON_TM + CommConst.COMMA + konkaiJikkonTm };
                    }
                    dt = dbAccess.Reader(query, parameters);
                }
                finally
                {
                    dbAccess.Close();
                }
            }
            logger.Debug("Renkei#GetKokyakuKanriData() End");
            return dt;
        }

        #endregion マスタ情報よりデータ取得（顧客管理データ）

        #region トランザクションマッピング取得

        /// <summary>
        /// トランザクションマッピング取得
        /// </summary>
        private void GetTransactionMapping()
        {
            logger.Debug("Renkei#GetTransactionMapping() Start");
            // マッピングJson解析
            if (transactionMappingData == null)
            {
                transactionMappingData = util.JsonParse(CommConst.TRANSACTION_MAPPING_JSON);
            }

            // レポート情報リスト取得
            var transactionInfoLst = transactionMappingData.Where(x => x.FormatId.Equals(FormatId)).Select(x => x).ToList();
            if (transactionInfoLst.Count == 0)
            {
                logger.Error(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0011E), new string[] { FormatId }));
            }
            if (transactionInfoLst.Count >= 2)
            {
                logger.Error(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0012E), new string[] { FormatId }));
            }
            if (transactionInfoLst.ToList().Count != 1)
            {
                throw new Exception();
            }

            transactionMapping = transactionInfoLst.FirstOrDefault();
            logger.Debug("Renkei#GetTransactionMapping() End");
        }

        #endregion トランザクションマッピング取得

        #region 報告書情報取得Api

        /// <summary>
        /// 報告書情報取得Api
        /// </summary>
        private void ReqGetReportInfo()
        {
            logger.Debug("Renkei#ReqGetReportInfo() Start");
            // 報告書情報取得
            if (CommConst.REPORT_DELETE.Equals(ActionType))
            {
                resReport = new ResReport();
                resReport.report_id = ReportNo;
                resReport.report_data = new Dictionary<string, object>();
            }
            else
            {
                resReport = (ResReport)util.ReqGetReportInfo(FormatId, ReportNo);
            }

            logger.Debug("Renkei#ReqGetReportInfo() End");
        }

        #endregion 報告書情報取得Api

        #region マスタマッピング情報取得

        /// <summary>
        /// マスタマッピング取得
        /// </summary>
        private void GetMasterMapping()
        {
            logger.Debug("Renkei#GetMasterMapping() Start");
            // マッピングJson解析
            if (masterMappingData == null)
            {
                masterMappingData = util.JsonParse(CommConst.MASTER_MAPPING_JSON);
            }

            // レポート情報リスト取得
            var masterInfoLst = masterMappingData.Where(x => x.ProcId.Equals(ProcId)).Select(x => x).ToList();
            if (masterInfoLst.Count >= 2)
            {
                logger.Error(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0025E), new string[] { ProcId }));
                throw new Exception();
            }

            masterMapping = masterInfoLst.FirstOrDefault();
            logger.Debug("Renkei#GetMasterMapping() End");
        }

        #endregion マスタマッピング情報取得

        #region トランザクションテーブルカラム値設定

        /// <summary>
        /// トランザクションテーブルカラム値設定
        /// </summary>
        private void UpdTransactionColumnValue()
        {
            logger.Debug("Renkei#UpdTransactionColumnValue() Start");
            transactionMapping.UpdTableList.Where(table => !string.IsNullOrEmpty(table.TableNm)).Select(table => table).ToList().ForEach(table =>
                {
                    table.ColumnList.Where(col => !string.IsNullOrEmpty(col.ColumnNm) && !string.IsNullOrEmpty(col.RefReportDataKey)).Select(col => col).ToList().ForEach(col =>
                        {
                            var reportDataKey = transactionMapping.ControlList.Where(x => !string.IsNullOrEmpty(x.ReportDataKey)).Select(x => x).ToList().Where(x => x.ReportDataKey.Equals(col.RefReportDataKey)).Select(x => x.ReportDataKey).FirstOrDefault();
                            if (string.IsNullOrEmpty(reportDataKey))
                            {
                                var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0026E), new string[] { col.RefReportDataKey });
                                logger.Error(msg);
                                throw new Exception(msg);
                            }
                            else
                            {
                                if (resReport.report_data.ContainsKey(reportDataKey))
                                {
                                    col.Value = resReport.report_data[reportDataKey];
                                }
                                else
                                {
                                    col.Value = resReport.ReflectPropertyValue(reportDataKey);
                                }
                                if (col.Value == null)
                                {
                                    col.Value = string.Empty;
                                }
                            }
                        });
                });
            logger.Debug("Renkei#UpdTransactionColumnValue() End");
        }

        /// <summary>
        /// トランザクションテーブル削除カラム値設定
        /// </summary>
        private void UpdTransactionDelColumnValue()
        {
            logger.Debug("Renkei#UpdTransactionDelColumnValue() Start");
            transactionMapping.DelTableList.Where(table => !string.IsNullOrEmpty(table.TableNm)).Select(table => table).ToList().ForEach(table =>
            {
                table.ColumnList.Where(col => !string.IsNullOrEmpty(col.ColumnNm) && !string.IsNullOrEmpty(col.RefReportDataKey)).Select(col => col).ToList().ForEach(col =>
                {
                    var reportDataKey = transactionMapping.ControlList.Where(x => !string.IsNullOrEmpty(x.ReportDataKey)).Select(x => x).ToList().Where(x => x.ReportDataKey.Equals(col.RefReportDataKey)).Select(x => x.ReportDataKey).FirstOrDefault();
                    if (string.IsNullOrEmpty(reportDataKey))
                    {
                        var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0026E), new string[] { col.RefReportDataKey });
                        logger.Error(msg);
                        throw new Exception(msg);
                    }
                    else
                    {
                        if (resReport.report_data.ContainsKey(reportDataKey))
                        {
                            col.Value = resReport.report_data[reportDataKey];
                        }
                        else
                        {
                            col.Value = resReport.ReflectPropertyValue(reportDataKey);
                        }
                        if (col.Value == null)
                        {
                            col.Value = string.Empty;
                        }
                    }
                });
            });
            logger.Debug("Renkei#UpdTransactionDelColumnValue() End");
        }

        #endregion トランザクションテーブルカラム値設定

        #region トランザクションテーブル更新

        /// <summary>
        /// トランザクションテーブル更新
        /// </summary>
        private void UpdTransactionTable()
        {
            logger.Info("Renkei#UpdTransactionTable() Start");

            var dbAccess = new DbAccess(logger, Utility.GetConnection(CommConst.CUSTOMER_CONNECTION));
            try
            {
                dbAccess.BeginTransaction();
                // イベント種類が削除の場合
                if (CommConst.REPORT_DELETE.Equals(ActionType))
                {
                    UpdTransactionDelColumnValue();
                    transactionMapping.DelTableList.Where(table => !string.IsNullOrEmpty(table.TableNm)).Select(table => table).ToList().ForEach(table =>
                    {
                        var query = string.Empty;
                        var parameters = new object[] { };
                        CreateTranDelQuery(table, out query, out parameters);
                        dbAccess.ExecuteNonQuery(query, parameters);
                    });
                }
                else
                {
                    UpdTransactionColumnValue();
                    transactionMapping.UpdTableList.Where(table => !string.IsNullOrEmpty(table.TableNm)).Select(table => table).ToList().ForEach(table =>
                    {
                        var query = string.Empty;
                        var parameters = new object[] { };
                        CreateTranUpdQuery(table, out query, out parameters);
                        // 更新
                        var updCnt = dbAccess.ExecuteNonQuery(query, parameters);
                        if (updCnt == 0)
                        {
                            CreateTranInsQuery(table, out query, out parameters);
                            // 登録
                            dbAccess.ExecuteNonQuery(query, parameters);
                        }
                    });
                }
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
            logger.Info("Renkei#UpdTransactionTable() End");
        }

        #endregion トランザクションテーブル更新

        #region トランザクション更新クエリー作成

        /// <summary>
        /// トランザクション更新クエリー作成
        /// </summary>
        /// <param name="table"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        private void CreateTranUpdQuery(TranTable table, out string query, out object[] parameters)
        {
            logger.Debug("Renkei#CreateTranUpdQuery() Start");
            query = string.Empty;
            var values = new List<object>();
            var updSql = SqlConst.UPDATE + CommConst.HALF_SPACE + table.TableNm + CommConst.HALF_SPACE;
            var setLst = table.ColumnList.Where(x => !string.IsNullOrEmpty(x.ColumnNm) && !x.Condition).Select(x => x).ToList();
            var setSql = SqlConst.SET;
            if (setLst.Count > 0)
            {
                setLst.ForEach(col =>
                {
                    object value = null;
                    if (col.DefaultValue != null)
                    {
                        value = col.DefaultValue;
                    }
                    else
                    {
                        value = col.Value;
                    }
                    setSql = setSql + CommConst.HALF_SPACE + col.ColumnNm + CommConst.HALF_SPACE + CommConst.EQUAL + CommConst.HALF_SPACE + CommConst.AT + col.ColumnNm + CommConst.COMMA;
                    values.Add(CommConst.AT + col.ColumnNm + CommConst.COMMA + value);
                });
                // 更新日時
                setSql = CommConst.HALF_SPACE + setSql + CommConst.HALF_SPACE + SqlConst.COL_UPD_TM + CommConst.HALF_SPACE + CommConst.EQUAL + CommConst.HALF_SPACE + SqlConst.P_UPD_TM + CommConst.COMMA;
                values.Add(SqlConst.P_UPD_TM + CommConst.COMMA + konkaiJikkonTm);

                setSql = setSql.Substring(0, setSql.Length - CommConst.COMMA.Length);
            }
            else
            {
                // エラー処理
                var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0028E), new string[] { table.TableNm });
                logger.Error(msg);
                throw new Exception(msg);
            }
            var whereLst = table.ColumnList.Where(x => !string.IsNullOrEmpty(x.ColumnNm) && x.Condition).Select(x => x).ToList();
            var whereSql = SqlConst.WHERE;

            if (whereLst.Count > 0)
            {
                whereLst.ForEach(col =>
                {
                    whereSql = CommConst.HALF_SPACE + whereSql + CommConst.HALF_SPACE;
                    object value = null;
                    if (col.DefaultValue != null)
                    {
                        value = col.DefaultValue;
                    }
                    else
                    {
                        value = col.Value;
                    }
                    whereSql = whereSql + col.ColumnNm + CommConst.HALF_SPACE + CommConst.EQUAL + CommConst.HALF_SPACE + CommConst.AT + col.ColumnNm + CommConst.HALF_SPACE + SqlConst.AND;
                    values.Add(CommConst.AT + col.ColumnNm + CommConst.COMMA + value);
                });
                whereSql = whereSql.Substring(0, whereSql.Length - SqlConst.AND.Length);
            }
            else
            {
                // エラー処理
                var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0029E), new string[] { table.TableNm });
                logger.Error(msg);
                throw new Exception(msg);
            }
            query = updSql + setSql + whereSql;
            parameters = values.ToArray();
            logger.Debug("Renkei#CreateTranUpdQuery() End");
        }

        #endregion トランザクション更新クエリー作成

        #region トランザクション登録クエリー作成

        /// <summary>
        /// トランザクション登録クエリー作成
        /// </summary>
        /// <param name="table"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        private void CreateTranInsQuery(TranTable table, out string query, out object[] parameters)
        {
            logger.Debug("Renkei#CreateTranInsQuery() Start");
            query = string.Empty;
            var values = new List<object>();
            var insSql = SqlConst.INSERT + CommConst.HALF_SPACE + SqlConst.INTO + CommConst.HALF_SPACE + table.TableNm;
            var colSql = CommConst.LEFT_BRACKET;
            var valSql = SqlConst.VALUES + CommConst.LEFT_BRACKET;
            table.ColumnList.Where(col => !string.IsNullOrEmpty(col.ColumnNm)).Select(col => col).ToList().ForEach(col =>
              {
                  object value = null;
                  if (col.DefaultValue != null)
                  {
                      value = col.DefaultValue;
                  }
                  else
                  {
                      value = col.Value;
                  }
                  colSql = colSql + col.ColumnNm + CommConst.COMMA + CommConst.HALF_SPACE;
                  valSql = valSql + CommConst.AT + col.ColumnNm + CommConst.COMMA + CommConst.HALF_SPACE;
                  values.Add(CommConst.AT + col.ColumnNm + CommConst.COMMA + value);
              });
            // 登録日時
            colSql = colSql + SqlConst.COL_INS_TM + CommConst.COMMA + CommConst.HALF_SPACE;
            valSql = valSql + SqlConst.P_INS_TM + CommConst.COMMA + CommConst.HALF_SPACE;
            values.Add(SqlConst.P_INS_TM + CommConst.COMMA + konkaiJikkonTm);
            // 更新日時
            colSql = colSql + SqlConst.COL_UPD_TM + CommConst.COMMA + CommConst.HALF_SPACE;
            valSql = valSql + SqlConst.P_UPD_TM + CommConst.COMMA + CommConst.HALF_SPACE;
            values.Add(SqlConst.P_UPD_TM + CommConst.COMMA + konkaiJikkonTm);

            colSql = colSql.Substring(0, colSql.LastIndexOf(CommConst.COMMA)) + CommConst.RIGHT_BRACKET;
            valSql = valSql.Substring(0, valSql.LastIndexOf(CommConst.COMMA)) + CommConst.RIGHT_BRACKET;

            query = insSql + colSql + valSql;
            parameters = values.ToArray();
            logger.Debug("Renkei#CreateTranInsQuery() End");
        }

        #endregion トランザクション登録クエリー作成

        #region トランザクション削除クエリー作成

        /// <summary>
        /// トランザクション削除クエリー作成
        /// </summary>
        /// <param name="table"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        private void CreateTranDelQuery(TranTable table, out string query, out object[] parameters)
        {
            logger.Debug("Renkei#CreateTranDelQuery() Start");
            query = string.Empty;
            var values = new List<object>();
            var updSql = SqlConst.DELETE + CommConst.HALF_SPACE + SqlConst.FROM + CommConst.HALF_SPACE + table.TableNm + CommConst.HALF_SPACE;
            var whereLst = table.ColumnList.Where(x => !string.IsNullOrEmpty(x.ColumnNm) && x.Condition).Select(x => x).ToList();
            var whereSql = SqlConst.WHERE;

            if (whereLst.Count > 0)
            {
                whereLst.ForEach(col =>
                {
                    object value = null;
                    if (col.DefaultValue != null)
                    {
                        value = col.DefaultValue;
                    }
                    else
                    {
                        value = col.Value;
                    }
                    whereSql = whereSql + CommConst.HALF_SPACE + col.ColumnNm + CommConst.HALF_SPACE + CommConst.EQUAL + CommConst.HALF_SPACE + CommConst.AT + col.ColumnNm + CommConst.HALF_SPACE + SqlConst.AND;
                    values.Add(CommConst.AT + col.ColumnNm + CommConst.COMMA + value);
                });
                whereSql = whereSql.Substring(0, whereSql.Length - SqlConst.AND.Length);
            }
            else
            {
                // エラー処理
                var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0029E), new string[] { table.TableNm });
                logger.Error(msg);
                throw new Exception(msg);
            }
            query = updSql + whereSql;
            parameters = values.ToArray();
            logger.Debug("Renkei#CreateTranDelQuery() End");
        }

        #endregion トランザクション削除クエリー作成

        #region 追加クラス処理

        /// <summary>
        /// 追加クラス処理
        /// </summary>
        private void AdditionClassProc()
        {
            logger.Info("Renkei#AdditionClassProc() Start");
            if (assembly == null)
            {
                assembly = Assembly.Load(AssemblyName.GetAssemblyName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace(CommConst.FILE_URI, "")) + CommConst.YEN_MARK + Utility.Config.AppSettings.Settings[CommConst.ADDITION_DLL_PATH].Value));
            }
            dynamic clazz = Activator.CreateInstance(assembly.GetType(classNm));
            var runMethodParams = new object[] { logger, util, ActionType, FormatId, ReportNo, resReport, konkaiJikkonTm };
            var runMethod = clazz.GetType().GetMethod(CommConst.RUN_METHOD);
            runMethod.Invoke(clazz, runMethodParams);
            logger.Info("Renkei#AdditionClassProc() End");
        }

        #endregion 追加クラス処理

        #endregion privateメソッド
    }
}