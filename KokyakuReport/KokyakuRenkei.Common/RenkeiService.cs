using Codeplex.Data;
using KokyakuRenkei.Common.Api;
using KokyakuRenkei.Common.Const;
using KokyakuRenkei.Common.Mapping.Master;
using KokyakuRenkei.Common.Mapping.Report;
using KokyakuRenkei.Common.Mapping.Transaction;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace KokyakuRenkei.Common
{
    /// <summary>
    /// 快作ビジネスプラス快作レポートプラス連携サービス
    /// </summary>
    public class RenkeiService
    {
        #region 変数定義

        /// <summary>
        /// Utility
        /// </summary>
        public Utility util;

        /// <summary>
        /// 報告書フォーマットID
        /// </summary>
        private string FormatId;

        /// <summary>
        /// SQLServerとレポート＋トランザクション連携
        /// 1：SQLServer→レポート＋トランザクション連携
        /// 1以外：レポート＋→SQLServerトランザクション連携
        /// </summary>
        private string ToReportplus;

        /// <summary>
        /// アクションタイプ
        /// </summary>
        private string ActionType;

        /// <summary>
        /// 帳票ID
        /// </summary>
        private string ReportId;

        /// <summary>
        /// 伝票番号
        /// </summary>
        private string DempyoNo;

        /// <summary>
        /// 報告書No
        /// </summary>
        private string ReportNo;

        /// <summary>
        /// シーケンスNo
        /// </summary>
        private decimal Seq;

        /// <summary>
        /// ロガー
        /// </summary>
        public ILog logger;

        /// <summary>
        /// レポートマッピングデータ
        /// </summary>
        private static List<ReportMapping> reportMappingData;

        /// <summary>
        /// レポートマッピング
        /// </summary>
        private ReportMapping reportMapping;

        /// <summary>
        /// ベースデータ
        /// </summary>
        private List<ItemInfo> baseDataItemLst = new List<ItemInfo>();

        /// <summary>
        /// レポートデータ
        /// </summary>
        private List<ItemInfo> reportDataItemLst = new List<ItemInfo>();

        /// <summary>
        /// 画像データ
        /// </summary>
        private List<ItemInfo> imageDataItemLst = new List<ItemInfo>();

        /// <summary>
        /// コードマスタ会社別
        /// </summary>
        private static Dictionary<string, List<CodeKaishaBetu>> codeKaishaBetuDic = new Dictionary<string, List<CodeKaishaBetu>>();

        /// <summary>
        /// リクエストJson
        /// </summary>
        private List<JsonObject> requestJsonDataLst = new List<JsonObject>();

        /// <summary>
        /// 追加処理クラス名
        /// </summary>
        private dynamic classNm;

        /// <summary>
        /// 追加クラスのアセンブリ
        /// </summary>
        private static Assembly assembly;

        /// <summary>
        /// 実行日時
        /// </summary>
        private DateTime jikkonTm;

        private static SemaphoreSlim semaphore = new SemaphoreSlim(1);

        #endregion 変数定義

        #region コンストラクタ

        private RenkeiService()
        { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="toReportplus"></param>
        /// <param name="actionType"></param>
        /// <param name="reportId"></param>
        /// <param name="dempyoNo"></param>
        /// <param name="reportNo"></param>
        /// <param name="seq"></param>
        public RenkeiService(string toReportplus, string actionType, string reportId, string dempyoNo, string reportNo, decimal seq)
        {
            ToReportplus = toReportplus;
            ActionType = actionType;
            ReportId = reportId;
            DempyoNo = dempyoNo;
            ReportNo = reportNo;
            Seq = seq;
            jikkonTm = DateTime.Now;
        }

        #endregion コンストラクタ

        #region 快作ビジネス＋快作レポート＋間連携処理

        /// <summary>
        /// 快作ビジネス＋快作レポート＋連携処理
        /// </summary>
        public async Task<ResponseInfo> Run()

        {
            return await Task<ResponseInfo>.Run(() =>
             {
                 logger.Info("RenkeiService#Run() Start");
                 var responseInfo = new ResponseInfo();
                 if (CommConst.TO_REPORTPLUS_1.Equals(ToReportplus))
                 {
                     // ビジネス＋→レポート＋トランザクション連携
                     BizPlusToReportPlus(ref responseInfo);
                 }
                 logger.Info("RenkeiService#Run() End");
                 return responseInfo;
             });
        }

        /// <summary>
        /// 快作ビジネス＋快作レポート＋連携処理
        /// </summary>
        public ResponseInfo SyncRun()
        {
            logger.Info("RenkeiService#SyncRun() Start");
            var responseInfo = new ResponseInfo();
            if (CommConst.TO_REPORTPLUS_1.Equals(ToReportplus))
            {
                // ビジネス＋→レポート＋トランザクション連携
                BizPlusToReportPlus(ref responseInfo);
            }
            logger.Info("RenkeiService#SyncRun() End");
            return responseInfo;
        }

        #endregion 快作ビジネス＋快作レポート＋間連携処理

        #region ビジネス＋→レポート＋トランザクション連携

        /// <summary>
        /// ビジネス＋→レポート＋トランザクション連携
        /// </summary>
        /// <param name="responseInfo"></param>
        private void BizPlusToReportPlus(ref ResponseInfo responseInfo)
        {
            logger.Info(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0043I), new string[] { ActionType, ReportId, DempyoNo, ReportNo }));
            logger.Info("RenkeiService#BizPlusToReportPlus() Start");
            try
            {
                // チェック処理
                CheckProc();
                // レポートマッピング取得
                GetReportMapping();
                // トランザクションテーブルマッピングマスタデータ取得
                var mappingMstDt = util.GetTranMappingMst(FormatId);
                // 対象フォーマットID定義あり
                if (mappingMstDt.Rows.Count > 0)
                {
                    ReportRenkei(ref responseInfo);
                    var clazzNm = mappingMstDt.Rows[0][CommConst.ADDITION_PROC_CLASS_NM].ToString();
                    if (!string.IsNullOrEmpty(clazzNm))
                    {
                        // クラス名
                        classNm = mappingMstDt.Rows[0][CommConst.ADDITION_PROC_CLASS_NM];
                        // 追加処理
                        AdditionClassProc();
                    }

                    // レポート連携テーブルにデータアップデート
                    util.UpdReportRenkeiZumiData(Seq, responseInfo.ReportNo, jikkonTm);
                }
                else
                {
                    var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0059E), new string[] { FormatId });
                    logger.Error(msg);
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.StackTrace);
                logger.Error(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0045E), new string[] { ActionType, ReportId, DempyoNo, ReportNo }));
                throw;
            }
            logger.Info(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0044I), new string[] { ActionType, ReportId, DempyoNo, ReportNo }));
            logger.Info("RenkeiService#BizPlusToReportPlus() End");
        }

        #endregion ビジネス＋→レポート＋トランザクション連携

        #region チェック処理

        /// <summary>
        /// チェック処理
        /// </summary>
        private void CheckProc()
        {
            logger.Info("RenkeiService#CheckProc() Start");
            // 必須チェック（イベント種別、帳票ID、伝票番号）
            if (string.IsNullOrEmpty(ActionType) || string.IsNullOrEmpty(ReportId) || string.IsNullOrEmpty(DempyoNo))
            {
                var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0046E), new string[] { ActionType, ReportId, DempyoNo });
                logger.Error(msg);
                throw new Exception(msg);
            }
            // イベント種別チェック（C：登録、E：更新、D：削除）
            if (!CommConst.ACTION_TYPE_C.Equals(ActionType) && !CommConst.ACTION_TYPE_E.Equals(ActionType) && !CommConst.ACTION_TYPE_D.Equals(ActionType))
            {
                var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0047E), new string[] { ActionType });
                logger.Error(msg);
                throw new Exception(msg);
            }

            // 条件付き必須チェック
            if ((CommConst.ACTION_TYPE_E.Equals(ActionType) || CommConst.ACTION_TYPE_D.Equals(ActionType)) && string.IsNullOrEmpty(ReportNo))
            {
                var msg = Utility.GetMsg(MsgConst.KK0048E);
                logger.Error(msg);
                throw new Exception(msg);
            }
            logger.Info("RenkeiService#CheckProc() End");
        }

        #endregion チェック処理

        #region レポートマッピング取得

        /// <summary>
        /// レポートマッピング取得
        /// </summary>
        private void GetReportMapping()
        {
            logger.Info("RenkeiService#GetReportMapping() Start");
            // マッピングJson解析
            if (reportMappingData == null)
            {
                reportMappingData = util.JsonParse(CommConst.REPORT_MAPPING_JSON);
            }
            // レポート情報リスト取得
            var reportInfoLst = reportMappingData.Where(x => x.ReportId.Equals(ReportId)).Select(x => x).ToList();
            var msg = string.Empty;
            if (reportInfoLst.Count == 0)
            {
                msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0049E), new string[] { ReportId });
            }
            if (reportInfoLst.Count >= 2)
            {
                msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0050E), new string[] { ReportId });
            }
            if (reportInfoLst.ToList().Count != 1)
            {
                logger.Error(msg);
                throw new Exception(msg);
            }
            reportMapping = reportInfoLst.FirstOrDefault();
            FormatId = reportMapping.FormatId;
            if (reportMapping.ReportInfo.BaseData == null || (reportMapping.ReportInfo.BaseData != null && reportMapping.ReportInfo.BaseData.Where(x => WebApiConst.USER_ID_KEY.Equals(x.ReportDataKey)).Count() == 0))
            {
                msg = Utility.GetMsg(MsgConst.KK0058E);
                logger.Error(msg);
                throw new Exception(msg);
            }
            logger.Info("RenkeiService#GetReportMapping() End");
        }

        #endregion レポートマッピング取得

        #region レポート用データ取得

        /// <summary>
        /// レポート用データ取得
        /// </summary>
        /// <returns></returns>
        private DataTable GetReportData()
        {
            logger.Info("RenkeiService#GetReportData() Start");

            if (reportMapping.ReportInfo.BaseData != null)
            {
                baseDataItemLst = reportMapping.ReportInfo.BaseData.ToList();
            }
            if (reportMapping.ReportInfo.ReportData != null)
            {
                reportDataItemLst = reportMapping.ReportInfo.ReportData.ToList();
            }
            if (reportMapping.ReportInfo.ImageData != null && reportMapping.ReportInfo.ImageData.Where(x => x.Count == 2).Count() > 0)
            {
                imageDataItemLst = reportMapping.ReportInfo.ImageData.Where(x => x.Count == 2).Select(x => x.Select(y => y)).SelectMany(z => z).ToList();
            }

            var itemLst = baseDataItemLst.Union(reportDataItemLst).Union(imageDataItemLst).ToList();

            var dt = new DataTable();
            if (itemLst.Count() > 0)
            {
                var dbAccess = new DbAccess(logger, Utility.GetConnection(CommConst.BASIC_CONNECTION));
                try
                {
                    var query = SqlConst.SELECT + CommConst.HALF_SPACE + string.Join(CommConst.COMMA + CommConst.HALF_SPACE, itemLst.ToList().Where(x => !string.IsNullOrEmpty(x.ColumnNm)).Select(x => x.ColumnNm).ToList()) + CommConst.HALF_SPACE + SqlConst.FROM + CommConst.HALF_SPACE + reportMapping.TableNm;
                    var condition = SqlConst.DEFAULT_CONDITION;
                    if (!string.IsNullOrEmpty(reportMapping.Condition))
                    {
                        condition = reportMapping.Condition;
                    }
                    query = query + CommConst.HALF_SPACE + SqlConst.WHERE + CommConst.HALF_SPACE + condition + CommConst.HALF_SPACE;
                    var parameters = new List<object>();
                    if (itemLst.Where(x => !string.IsNullOrEmpty(x.ColumnNm) && x.Condition).Count() > 0)
                    {
                        itemLst.Where(x => x.Condition && !string.IsNullOrEmpty(x.ColumnNm) && !string.IsNullOrEmpty(x.DefaultValue)).ToList().ForEach(y =>
                        {
                            query = query + SqlConst.AND + CommConst.HALF_SPACE + y.ColumnNm + CommConst.HALF_SPACE + CommConst.EQUAL + CommConst.HALF_SPACE + CommConst.AT + y.ColumnNm + CommConst.HALF_SPACE;
                            parameters.Add(CommConst.AT + y.ColumnNm + CommConst.COMMA + y.DefaultValue);
                        });
                    }
                    if (!string.IsNullOrEmpty(reportMapping.KAISHA_CD))
                    {
                        query = query + SqlConst.AND + CommConst.HALF_SPACE + nameof(reportMapping.KAISHA_CD) + CommConst.HALF_SPACE + CommConst.EQUAL + CommConst.HALF_SPACE + CommConst.AT + nameof(reportMapping.KAISHA_CD) + CommConst.HALF_SPACE;
                        parameters.Add(CommConst.AT + nameof(reportMapping.KAISHA_CD) + CommConst.COMMA + reportMapping.KAISHA_CD);
                    }
                    if (itemLst.Where(x => x.DempyoHdrFlag).Count() > 0)
                    {
                        var reportIdColNm = itemLst.Where(y => y.DempyoHdrFlag).Select(y => y.ColumnNm).FirstOrDefault();
                        query = query + SqlConst.AND + CommConst.HALF_SPACE + reportIdColNm + CommConst.HALF_SPACE + CommConst.EQUAL + CommConst.HALF_SPACE + CommConst.AT + reportIdColNm + CommConst.HALF_SPACE;
                        parameters.Add(CommConst.AT + reportIdColNm + CommConst.COMMA + DempyoNo);
                    }

                    dt = dbAccess.Reader(query, parameters.ToArray());

                    if (dt.Rows.Count > 0)
                    {
                        GetMstCodeKaishaBetsu(itemLst.Where(item => !string.IsNullOrEmpty(item.CD_SECTION)).ToDictionary(dic => nameof(dic.CD_SECTION), dic => (object)dic.CD_SECTION));
                    }
                }
                finally
                {
                    dbAccess.Close();
                }
                if (dt.Rows.Count == 0)
                {
                    var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0051E), new string[] { DempyoNo });
                    logger.Error(msg);
                    throw new Exception(msg);
                }
            }

            logger.Info("RenkeiService#GetReportData() End");
            return dt;
        }

        #endregion レポート用データ取得

        #region コードマスタ会社別データ取得

        /// <summary>
        /// コードマスタ会社別データ取得
        /// </summary>
        /// <param name="cdSectionLst"></param>
        private void GetMstCodeKaishaBetsu(Dictionary<string, object> cdSectionDic)
        {
            if (!codeKaishaBetuDic.ContainsKey(FormatId))
            {
                if (cdSectionDic.Count > 0)
                {
                    var dbAccess = new DbAccess(logger, Utility.GetConnection(CommConst.BASIC_CONNECTION));
                    try
                    {
                        var codeKaishaBetuLst = new List<CodeKaishaBetu>();
                        var whereDic = new Dictionary<string, object>();
                        if (!string.IsNullOrEmpty(reportMapping.KAISHA_CD))
                        {
                            whereDic.Add(nameof(reportMapping.KAISHA_CD), reportMapping.KAISHA_CD);
                        }
                        cdSectionDic.ToList().ForEach(x =>
                        {
                            whereDic.Add(x.Key, x.Value);
                            var whereLst = whereDic.ToList();
                            var query = SqlConst.QUERY_CODE_KAISHA_BETU_SELECT + CommConst.HALF_SPACE + SqlConst.WHERE + CommConst.HALF_SPACE + String.Join(SqlConst.AND, whereLst.Select(dic => String.Format(" {0} = @{1} ", dic.Key, dic.Key)));
                            var parameters = new List<object>();
                            whereLst.ForEach(where =>
                            {
                                parameters.Add(CommConst.AT + where.Key + CommConst.COMMA + where.Value);
                            });
                            var dt = dbAccess.Reader(query, parameters.ToArray());
                            codeKaishaBetuLst.AddRange(util.ConvertDataTable<CodeKaishaBetu>(dt));
                            whereDic.Remove(x.Key);
                        });
                        codeKaishaBetuDic.Add(FormatId, codeKaishaBetuLst);
                    }
                    finally
                    {
                        dbAccess.Close();
                    }
                }
            }
        }

        #endregion コードマスタ会社別データ取得

        #region コードマスタのコード名称取得

        /// <summary>
        /// コードマスタのコード名称取得
        /// </summary>
        /// <param name="cdSection"></param>
        /// <param name="cdKey"></param>
        /// <returns></returns>
        private string GetCdNm(string cdSection, string cdKey)
        {
            return codeKaishaBetuDic[FormatId].Where(x => x.CD_SECTION.Equals(cdSection) && x.CD_KEY.Equals(cdKey)).Select(x => x.CD_NM).FirstOrDefault();
        }

        #endregion コードマスタのコード名称取得

        #region 報告書登録リクエスト作成

        /// <summary>
        /// 報告書登録リクエスト作成
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string CreateReportInsRequest(DataTable dt)
        {
            logger.Info("RenkeiService#CreateReportInsRequest() Start");
            var rtn = string.Empty;
            requestJsonDataLst.Clear();
            if (dt.Rows.Count > 0)
            {
                var colNmLst = dt.Columns.Cast<DataColumn>().Select(row => row.ColumnName).ToList();
                // 二重登録防止のためのキー作成
                CreateRegisterKey();
                // ベースデータ作成
                baseDataItemLst.RemoveAll(x => WebApiConst.INSTRUCT_ANSWER_KEY.Equals(x.ReportDataKey));
                CreateBaseData(dt, colNmLst);
                // 登録する報告書データの連想配列作成
                CreateReportData(dt, colNmLst);
                // 報告書の添付画像配列作成
                CreateImagesData(dt, colNmLst);
                // リクエストデータ作成
                if (requestJsonDataLst.Count > 0)
                {
                    rtn = JsonConvert.SerializeObject(requestJsonDataLst.ToDictionary(x => x.Key, x => x.Value), Formatting.Indented);
                    rtn = rtn.Replace(CommConst.DOUBLE_YEN_MARK, CommConst.YEN_MARK);
                }
            }
            logger.Info("RenkeiService#CreateReportInsRequest() End");
            return rtn;
        }

        #endregion 報告書登録リクエスト作成

        #region 報告書更新リクエスト作成

        /// <summary>
        /// 報告書更新リクエスト作成
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string CreateReportUpdRequest(DataTable dt)
        {
            logger.Info("RenkeiService#CreateReportUpdRequest() Start");
            var rtn = string.Empty;
            requestJsonDataLst.Clear();
            if (dt.Rows.Count > 0)
            {
                var colNmLst = dt.Columns.Cast<DataColumn>().Select(row => row.ColumnName).ToList();
                // ベースデータ作成
                baseDataItemLst.RemoveAll(x => WebApiConst.INSTRUCT_TITLE_KEY.Equals(x.ReportDataKey));
                baseDataItemLst.RemoveAll(x => WebApiConst.INSTRUCT_CONTENTS_KEY.Equals(x.ReportDataKey));
                CreateBaseData(dt, colNmLst);
                // 登録する報告書データの連想配列作成
                CreateReportData(dt, colNmLst);
                // 報告書の添付画像配列作成
                CreateImagesData(dt, colNmLst);
                // リクエストデータ作成
                if (requestJsonDataLst.Count > 0)
                {
                    rtn = JsonConvert.SerializeObject(requestJsonDataLst.ToDictionary(x => x.Key, x => x.Value), Formatting.Indented);
                    rtn = rtn.Replace(CommConst.DOUBLE_YEN_MARK, CommConst.YEN_MARK);
                }
            }
            logger.Info("RenkeiService#CreateReportUpdRequest() End");
            return rtn;
        }

        #endregion 報告書更新リクエスト作成

        #region 報告書連携

        private void ReportRenkei(ref ResponseInfo responseInfo)
        {
            // 登録
            if (CommConst.ACTION_TYPE_C.Equals(ActionType))
            {
                var dt = GetReportData();
                if (decimal.Zero.Equals(Seq))
                {
                    // レポート連携テーブルにデータインサート
                    Seq = util.InsReportRenkeiData(ReportId, DempyoNo, ActionType, ReportNo, jikkonTm);
                }
                semaphore.Wait();
                responseInfo = util.ReqReportApi(FormatId, ReportNo, CreateReportInsRequest(dt), CommConst.MODE_INS);
                semaphore.Release();
            }
            // 更新
            else if (CommConst.ACTION_TYPE_E.Equals(ActionType))
            {
                var dt = GetReportData();
                if (decimal.Zero.Equals(Seq))
                {
                    // レポート連携テーブルにデータインサート
                    Seq = util.InsReportRenkeiData(ReportId, DempyoNo, ActionType, ReportNo, jikkonTm);
                }
                semaphore.Wait();
                responseInfo = util.ReqReportApi(FormatId, ReportNo, CreateReportUpdRequest(dt), CommConst.MODE_UPD);
                semaphore.Release();
            }
            // 削除
            else if (CommConst.ACTION_TYPE_D.Equals(ActionType))
            {
                if (decimal.Zero.Equals(Seq))
                {
                    // レポート連携テーブルにデータインサート
                    Seq = util.InsReportRenkeiData(ReportId, DempyoNo, ActionType, ReportNo, jikkonTm);
                }
                semaphore.Wait();
                responseInfo = util.ReqReportApi(FormatId, ReportNo, string.Empty, CommConst.MODE_DEL);
                semaphore.Release();
            }
        }

        #endregion 報告書連携

        #region 二重登録防止のためのキー作成

        /// <summary>
        /// 二重登録防止のためのキー作成
        /// </summary>
        private void CreateRegisterKey()
        {
            logger.Info("RenkeiService#CreateRegisterKey() Start");
            requestJsonDataLst.Add(new JsonObject { Key = WebApiConst.REGISTER_KEY, Value = DateTime.Now.ToString(CommConst.YYYY_MM_DD_HH_MM_SS_FFF_FORMAT) + CommConst.UNDER_SCORE + (new Random()).Next(1, int.MaxValue).ToString() });
            logger.Info("RenkeiService#CreateRegisterKey() End");
        }

        #endregion 二重登録防止のためのキー作成

        #region ベースデータ作成

        /// <summary>
        /// ベースデータ作成
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colNmLst"></param>
        private void CreateBaseData(DataTable dt, List<string> colNmLst)
        {
            logger.Info("RenkeiService#CreateBaseData() Start");
            baseDataItemLst.Where(y => !string.IsNullOrEmpty(y.ReportDataKey)).ToList().ForEach(z =>
            {
                if (colNmLst.Contains(z.ColumnNm))
                {
                    z.Value = dt.Rows[0][z.ColumnNm];
                    if (!string.IsNullOrEmpty(z.CD_SECTION) && z.Value != null)
                    {
                        z.Value = GetCdNm(z.CD_SECTION, z.Value.ToString());
                    }
                }
                else
                {
                    z.Value = z.DefaultValue;
                }

                requestJsonDataLst.Add(new JsonObject { Key = z.ReportDataKey, Value = z.Value });
            });
            logger.Info("RenkeiService#CreateBaseData() End");
        }

        #endregion ベースデータ作成

        #region 登録する報告書データの連想配列作成

        /// <summary>
        /// 登録する報告書データの連想配列作成
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colNmLst"></param>
        private void CreateReportData(DataTable dt, List<string> colNmLst)
        {
            logger.Info("RenkeiService#CreateReportData() Start");
            var jsonReportDataLst = new List<JsonObject>();
            reportDataItemLst.Where(y => !string.IsNullOrEmpty(y.ReportDataKey)).ToList().ForEach(z =>
            {
                if (CommConst.HDR_DTL_TYPE_HDR.Equals(z.HdrDtlType))
                {
                    CreateReportDataLst(dt.AsEnumerable().ToList().FirstOrDefault(), colNmLst, z, -1, ref jsonReportDataLst);
                }
                else if (CommConst.HDR_DTL_TYPE_DTL.Equals(z.HdrDtlType))
                {
                    dt.AsEnumerable().ToList().Select((row, idx) => new { Row = row, Idx = idx }).ToList().ForEach(item =>
                    {
                        CreateReportDataLst(item.Row, colNmLst, z, item.Idx + 1, ref jsonReportDataLst);
                    });
                }
            });

            if (jsonReportDataLst.Count > 0)
            {
                requestJsonDataLst.Add(new JsonObject { Key = WebApiConst.REPORT_DATA_KEY, Value = jsonReportDataLst.ToDictionary(dic => dic.Key, dic => dic.Value) });
            }
            logger.Info("RenkeiService#CreateReportData() End");
        }

        /// <summary>
        /// レポートデータリスト作成
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colNmLst"></param>
        /// <param name="itemInfo"></param>
        /// <param name="increment"></param>
        /// <param name="jsonReportDataLst"></param>
        private void CreateReportDataLst(DataRow row, List<string> colNmLst, ItemInfo itemInfo, int increment, ref List<JsonObject> jsonReportDataLst)
        {
            if (colNmLst.Contains(itemInfo.ColumnNm))
            {
                itemInfo.Value = row[itemInfo.ColumnNm];
                if (!string.IsNullOrEmpty(itemInfo.CD_SECTION) && itemInfo.Value != null)
                {
                    itemInfo.Value = GetCdNm(itemInfo.CD_SECTION, itemInfo.Value.ToString());
                }
            }
            else
            {
                itemInfo.Value = itemInfo.DefaultValue;
            }
            if (increment == -1)
            {
                jsonReportDataLst.Add(new JsonObject { Key = itemInfo.ReportDataKey, Value = itemInfo.Value });
            }
            else
            {
                jsonReportDataLst.Add(new JsonObject { Key = itemInfo.ReportDataKey + CommConst.UNDER_SCORE + increment.ToString(), Value = itemInfo.Value });
            }
        }

        #endregion 登録する報告書データの連想配列作成

        #region 報告書の添付画像配列作成

        /// <summary>
        /// 報告書の添付画像配列作成
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colNmLst"></param>
        private void CreateImagesData(DataTable dt, List<string> colNmLst)
        {
            logger.Info("RenkeiService#CreateImagesData() Start");
            var jsonImageDataLst = new List<JsonObject>();
            var imageDtlCnt = imageDataItemLst.Where(x => CommConst.HDR_DTL_TYPE_DTL.Equals(x.HdrDtlType)).Count() / 2;
            imageDataItemLst.ForEach(z =>
            {
                if (CommConst.HDR_DTL_TYPE_HDR.Equals(z.HdrDtlType))
                {
                    CreateImageDataLst(dt.AsEnumerable().ToList().FirstOrDefault(), colNmLst, z, -1, ref jsonImageDataLst);
                }
                else if (CommConst.HDR_DTL_TYPE_DTL.Equals(z.HdrDtlType))
                {
                    dt.AsEnumerable().ToList().Select((row, idx) => new { Row = row, Idx = idx }).ToList().ForEach(item =>
                    {
                        CreateImageDataLst(item.Row, colNmLst, z, item.Idx * imageDtlCnt, ref jsonImageDataLst);
                    });
                }
            });
            if (jsonImageDataLst.Count > 0)
            {
                var nameLst = jsonImageDataLst.Where(x => WebApiConst.IMAGES_NAME_KEY.Equals(x.Key)).ToList();
                var dataLst = jsonImageDataLst.Where(x => WebApiConst.IMAGES_DATA_KEY.Equals(x.Key)).ToList();
                jsonImageDataLst.Clear();
                nameLst.Select((obj, idx) => new { Obj = obj, Idx = idx }).ToList().ForEach(item =>
                {
                    jsonImageDataLst.Add(item.Obj);
                    jsonImageDataLst.Add(dataLst[item.Idx]);
                });

                var imageDataLst = new List<Dictionary<string, object>>();
                for (var idx = 0; idx < jsonImageDataLst.Count; idx++)
                {
                    var imageData = jsonImageDataLst.Skip(idx * 2).Take(2).ToList();
                    if (imageData[0].Value != null && !string.IsNullOrEmpty(imageData[0].Value.ToString())
                        && imageData[1].Value != null && !string.IsNullOrEmpty(imageData[1].Value.ToString()))
                    {
                        imageDataLst.Add(imageData.ToDictionary(dic => dic.Key, dic => dic.Value));
                    }
                    if ((idx + 1) * 2 >= jsonImageDataLst.Count)
                    {
                        break;
                    }
                }

                if (imageDataLst.Count > 0)
                {
                    requestJsonDataLst.Add(new JsonObject { Key = WebApiConst.IMAGES_KEY, Value = imageDataLst });
                }
            }
            logger.Info("RenkeiService#CreateImagesData() End");
        }

        /// <summary>
        /// イメージデータリスト作成
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colNmLst"></param>
        /// <param name="itemInfo"></param>
        /// <param name="increment"></param>
        /// <param name="jsonImageDataLst"></param>
        private void CreateImageDataLst(DataRow row, List<string> colNmLst, ItemInfo itemInfo, int increment, ref List<JsonObject> jsonImageDataLst)
        {
            if (itemInfo.ReportDataKey.Equals(WebApiConst.IMAGES_DATA_KEY))
            {
                var imageRootPath = string.Empty;

                if (!string.IsNullOrEmpty(itemInfo.ImageRootPath))
                {
                    imageRootPath = itemInfo.ImageRootPath;
                }

                if (colNmLst.Contains(itemInfo.ColumnNm))
                {
                    var imageFileName = string.Empty;

                    itemInfo.Value = row[itemInfo.ColumnNm];
                    if (!string.IsNullOrEmpty(itemInfo.CD_SECTION) && itemInfo.Value != null)
                    {
                        itemInfo.Value = GetCdNm(itemInfo.CD_SECTION, itemInfo.Value.ToString());
                    }
                    if (itemInfo.Value != null)
                    {
                        imageFileName = itemInfo.Value.ToString();
                    }
                    var path = string.Empty;
                    if (string.IsNullOrEmpty(imageRootPath))
                    {
                        path = imageFileName;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(imageFileName))
                        {
                            path = imageRootPath + CommConst.YEN_MARK + imageFileName;
                        }
                    }
                    if (File.Exists(path))
                    {
                        itemInfo.Value = util.ImageFromFileToBase64(path);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(imageFileName))
                        {
                            var msg = Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0057E), new string[] { path });
                            logger.Error(msg);
                            throw new Exception(msg);
                        }
                    }
                }
            }
            else
            {
                itemInfo.Value = itemInfo.DefaultValue;
            }
            if (increment == -1 || itemInfo.ReportDataKey.Equals(WebApiConst.IMAGES_DATA_KEY))
            {
                jsonImageDataLst.Add(new JsonObject { Key = itemInfo.ReportDataKey, Value = itemInfo.Value });
            }
            else
            {
                jsonImageDataLst.Add(new JsonObject { Key = itemInfo.ReportDataKey, Value = WebApiConst.PHOTO + (int.Parse(itemInfo.Value.ToString().Replace(WebApiConst.PHOTO, "")) + increment).ToString() });
            }
        }

        #endregion 報告書の添付画像配列作成

        #region 追加クラス処理

        /// <summary>
        /// 追加クラス処理
        /// </summary>
        private void AdditionClassProc()
        {
            logger.Info("RenkeiService#AdditionClassProc() Start");
            if (assembly == null)
            {
                assembly = Assembly.Load(AssemblyName.GetAssemblyName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace(CommConst.FILE_URI, "")) + CommConst.YEN_MARK + Utility.Config.AppSettings.Settings[CommConst.ADDITION_DLL_PATH].Value));
            }
            dynamic clazz = Activator.CreateInstance(assembly.GetType(classNm));
            var runMethodParams = new object[] { logger, util, ReportId, DempyoNo, ActionType, ReportNo, jikkonTm };
            var runMethod = clazz.GetType().GetMethod(CommConst.RUN_METHOD);
            runMethod.Invoke(clazz, runMethodParams);
            logger.Info("RenkeiService#AdditionClassProc() End");
        }

        #endregion 追加クラス処理
    }
}