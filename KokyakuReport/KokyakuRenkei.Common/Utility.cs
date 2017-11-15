using Codeplex.Data;
using KokyakuRenkei.Common.Api;
using KokyakuRenkei.Common.Const;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace KokyakuRenkei.Common
{
    public class Utility
    {
        #region 変数定義

        public static Configuration Config;

        private static XDocument Message;

        /// <summary>
        /// ロガー
        /// </summary>
        public ILog logger;

        #endregion 変数定義

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Utility(ILog log)
        {
            if (Config == null)
            {
                Config = GetAppConfig(CommConst.APP_COMMON_CONFIG_FILE);
            }
            logger = log;
        }

        #endregion コンストラクタ

        #region App.config取得

        /// <summary>
        /// App.config取得
        /// </summary>
        /// <param name="appConfig"></param>
        /// <returns></returns>
        private Configuration GetAppConfig(string appConfig)
        {
            var configFile = new ExeConfigurationFileMap();
            configFile.ExeConfigFilename = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace(CommConst.FILE_URI, "")) + CommConst.YEN_MARK + CommConst.APP_COMMON_CONFIG_FILE;
            return ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
        }

        #endregion App.config取得

        #region コネクション取得

        public static string GetConnection(string connectName)
        {
            return Utility.Config.ConnectionStrings.ConnectionStrings[connectName].ToString();
        }

        #endregion コネクション取得

        #region Jsonファイル解析

        /// <summary>
        /// Jsonファイル解析
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public dynamic JsonParse(string filePath)
        {
            dynamic rtn;
            using (var reader = new StreamReader(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace(CommConst.FILE_URI, "")) + CommConst.YEN_MARK + CommConst.JSON_FOLDER + CommConst.YEN_MARK + filePath))
            {
                rtn = DynamicJson.Parse(reader.ReadToEnd(), Encoding.UTF8);
            }
            return rtn;
        }

        #endregion Jsonファイル解析

        #region メッセージ埋込文字変換

        /// <summary>
        /// メッセージ埋込文字変換
        /// </summary>
        /// <param name="msg">メッセージ</param>
        /// <param name="values">変換内容</param>
        /// <returns></returns>
        public static string ReleaseMsg(string msg, params string[] values)
        {
            if (values != null)
            {
                values.ToList().Select((value, idx) => new { idx, value }).ToList().ForEach(x =>
                {
                    msg = msg.Replace(CommConst.LEFT_BRACES + x.idx + CommConst.RIGHT_BRACES, x.value);
                });
            }

            return msg;
        }

        #endregion メッセージ埋込文字変換

        #region ダブルコーテーションエスケップ

        public string EscapeDoubleQuotation(string src)
        {
            if (string.IsNullOrEmpty(src))
            {
                return string.Empty;
            }
            else
            {
                return src.Replace(CommConst.SINGLE_DOUBLE_QUOTATION, CommConst.DOUBLE_DOUBLE_QUOTATION);
            }
        }

        #endregion ダブルコーテーションエスケップ

        #region メッセージ取得

        /// <summary>
        /// メッセージ取得
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public static string GetMsg(string msgId)
        {
            var message = string.Empty;
            if (Message == null)
            {
                Message = XDocument.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace(CommConst.FILE_URI, "")) + CommConst.YEN_MARK + CommConst.MESSAGE + CommConst.YEN_MARK + CommConst.MESSAGES);
            }
            var msg = Message.Descendants(CommConst.MESSAGE).Where(x => msgId.Equals(x.Attribute(CommConst.ID).Value)).Select(x => x.Value).FirstOrDefault();
            if (msg != null)
            {
                message = msgId + CommConst.SEMI_COLON + msg;
            }
            return message;
        }

        #endregion メッセージ取得

        #region DBアクセス

        #region トランザクションマッピングマスタデータ取得

        /// <summary>
        /// トランザクションマッピングマスタデータ取得
        /// </summary>
        /// <param name="formatId"></param>
        /// <returns></returns>
        public DataTable GetTranMappingMst(string formatId)
        {
            logger.Debug("Utility#GetTranMappingMst() Start");
            var dt = new DataTable();
            var dbAccess = new DbAccess(logger, GetConnection(CommConst.CUSTOMER_CONNECTION));
            try
            {
                var parameters = new object[] { SqlConst.P_FORMAT_ID + CommConst.COMMA + formatId };
                dt = dbAccess.Reader(SqlConst.QUERY_KK_TRAN_MAPPING_MST_SELECT_BY_KEY, parameters);
            }
            finally
            {
                dbAccess.Close();
            }
            logger.Debug("Utility#GetTranMappingMst() End");
            return dt;
        }

        #endregion トランザクションマッピングマスタデータ取得

        #region トランザクション連携テーブルデータ取得（連携未済分）

        /// <summary>
        /// トランザクション連携テーブルデータ取得（連携未済分）
        /// </summary>
        /// <param name="renkeiFlag"></param>
        /// <returns></returns>
        public DataTable GetTranRenkeiMisaiData(int renkeiFlag)
        {
            logger.Debug("Utility#GetTranRenkeiMisaiData() Start");
            var dt = new DataTable();
            var dbAccess = new DbAccess(logger, GetConnection(CommConst.CUSTOMER_CONNECTION));
            try
            {
                var parameters = new object[] { SqlConst.P_RENKEI_FLG + CommConst.COMMA + renkeiFlag };
                dt = dbAccess.Reader(SqlConst.QUERY_KK_TRAN_RENKEI_TBL_SELECT_BY_RENKEI_FLG, parameters);
            }
            finally
            {
                dbAccess.Close();
            }
            logger.Debug("Utility#GetTranRenkeiMisaiData() End");
            return dt;
        }

        #endregion トランザクション連携テーブルデータ取得（連携未済分）

        #region トランザクション連携テーブルデータ取得（キー指定）

        /// <summary>
        /// トランザクション連携テーブルデータ取得（キー指定）
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        public DataTable GetTranRenkeiDataByKey(decimal seq)
        {
            logger.Debug("Utility#GetTranRenkeiDataByKey() Start");
            var dt = new DataTable();
            var dbAccess = new DbAccess(logger, GetConnection(CommConst.CUSTOMER_CONNECTION));
            try
            {
                var parameters = new object[] { SqlConst.P_RENKEI_SEQ + CommConst.COMMA + seq };
                dt = dbAccess.Reader(SqlConst.QUERY_KK_TRAN_RENKEI_TBL_SELECT_BY_KEY, parameters);
            }
            finally
            {
                dbAccess.Close();
            }
            logger.Debug("Utility#GetTranRenkeiDataByKey() End");
            return dt;
        }

        #endregion トランザクション連携テーブルデータ取得（キー指定）

        #region トランザクション連携テーブルにデータインサート

        /// <summary>
        /// トランザクション連携テーブルにデータインサート
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="formatId"></param>
        /// <param name="reportNo"></param>
        /// <param name="dateTime"></param>
        public decimal InsTranRenkeiData(string actionType, string formatId, string reportNo, DateTime dateTime)
        {
            logger.Debug("Utility#InsTranRenkeiData() Start");
            var seq = decimal.Zero;
            var dbAccess = new DbAccess(logger, GetConnection(CommConst.CUSTOMER_CONNECTION));
            try
            {
                dbAccess.BeginTransaction();
                var parameters = new object[] { SqlConst.P_FORMAT_ID + CommConst.COMMA + formatId, SqlConst.P_REPORT_NO + CommConst.COMMA + reportNo, SqlConst.P_ACTION_TYPE + CommConst.COMMA + actionType, SqlConst.P_RENKEI_FLG + CommConst.COMMA + SqlConst.RENKEI_FLG_MISAI, SqlConst.P_INS_TM + CommConst.COMMA + dateTime, SqlConst.P_UPD_TM + CommConst.COMMA + dateTime };
                seq = dbAccess.ExecuteScalar(SqlConst.QUERY_KK_TRAN_RENKEI_TBL_INSERT, parameters);
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
            logger.Debug("Utility#InsTranRenkeiData() End");
            return seq;
        }

        #endregion トランザクション連携テーブルにデータインサート

        #region トランザクション連携テーブルにデータアップデート

        /// <summary>
        /// トランザクション連携テーブルにデータアップデート
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="dateTime"></param>
        public void UpdTranRenkeiZumiData(decimal seq, DateTime dateTime)
        {
            logger.Debug("Utility#UpdTranRenkeiZumiData() Start");
            var dbAccess = new DbAccess(logger, GetConnection(CommConst.CUSTOMER_CONNECTION));
            try
            {
                dbAccess.BeginTransaction();
                var parameters = new object[] { SqlConst.P_RENKEI_FLG + CommConst.COMMA + SqlConst.RENKEI_FLG_ZUMI, SqlConst.P_UPD_TM + CommConst.COMMA + dateTime, SqlConst.P_RENKEI_SEQ + CommConst.COMMA + seq };
                dbAccess.ExecuteNonQuery(SqlConst.QUERY_KK_TRAN_RENKEI_TBL_UPDATE_BY_KEY, parameters);
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

            logger.Debug("Utility#UpdTranRenkeiZumiData() End");
        }

        #endregion トランザクション連携テーブルにデータアップデート

        #region マスタ連携バッチ実行履歴テーブルデータ取得

        /// <summary>
        /// マスタ連携バッチ実行履歴テーブルデータ取得
        /// </summary>
        /// <param name="procId">処理ID</param>
        /// <returns></returns>
        public DataTable GetMstRenkeiBatchJikkouRireikiTbl(string procId)
        {
            logger.Debug("Utility#GetMstRenkeiBatchJikkouRireikiTbl() Start");
            var dt = new DataTable();
            var dbAccess = new DbAccess(logger, GetConnection(CommConst.CUSTOMER_CONNECTION));
            try
            {
                var parameters = new object[] { SqlConst.P_PROC_ID + CommConst.COMMA + procId };
                dt = dbAccess.Reader(SqlConst.QUERY_KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL_SELECT_BY_PROC_ID, parameters);
            }
            finally
            {
                dbAccess.Close();
            }
            logger.Debug("Utility#GetMstRenkeiBatchJikkouRireikiTbl() End");
            return dt;
        }

        #endregion マスタ連携バッチ実行履歴テーブルデータ取得

        #region 取引先マスタデータ取得（取引先略称指定）

        /// <summary>
        /// 取引先マスタデータ取得（取引先略称指定）
        /// </summary>
        /// <param name="torihikisakiSnm">取引先略称</param>
        /// <returns></returns>
        public DataTable GetKokyakuKanriTorihikisakiSelectByTorihikisakiSnm(string torihikisakiSnm)
        {
            logger.Debug("Utility#GetKokyakuKanriTorihikisakiSelectByTorihikisakiSnm() Start");
            var dt = new DataTable();
            var dbAccess = new DbAccess(logger, GetConnection(CommConst.CUSTOMER_CONNECTION));
            try
            {
                var parameters = new object[] { SqlConst.P_TORIHIKISAKI_SNM + CommConst.COMMA + torihikisakiSnm };
                dt = dbAccess.Reader(SqlConst.QUERY_KK_MST_TORIHIKISAKI_SELECT_BY_TORIHIKISAKI_SNM, parameters);
            }
            finally
            {
                dbAccess.Close();
            }
            logger.Debug("Utility#GetKokyakuKanriTorihikisakiSelectByTorihikisakiSnm() End");
            return dt;
        }

        #endregion 取引先マスタデータ取得（取引先略称指定）

        #endregion DBアクセス

        #region WebApi関連

        #region 報告書情報取得API

        /// <summary>
        /// 報告書情報取得API
        /// </summary>
        /// <param name="formatId">報告書フォーマットID</param>
        /// <param name="reportId">報告書No</param>
        /// <returns>レスポンス</returns>
        public dynamic ReqGetReportInfo(string formatId, string reportId)
        {
            logger.Info("Utility#ReqGetReportInfo() Start");
            var resrcPath = string.Format(WebApiConst.PATH_GET_REPORT_INFO, formatId, reportId);
            var url = CreateRequestUrl(resrcPath);
            HttpWebResponse res = null;
            try
            {
                // リクエスト作成
                var req = (HttpWebRequest)WebRequest.Create(url);
                CreateRequestConfig(req, WebApiConst.METHOD_GET, string.Empty);
                // リクエスト情報(Method:URI)をログ出力
                logger.Info(Utility.GetMsg(MsgConst.KK0021I) + req.RequestUri.ToString());
                logger.Info(string.Format(Utility.GetMsg(MsgConst.KK0020I), req.Method, req.RequestUri.ToString()));
                // レスポンス作成
                res = (HttpWebResponse)req.GetResponse();
                if (HttpStatusCode.OK.Equals(res.StatusCode))
                {
                    // レスポンス情報取得
                    using (var st = res.GetResponseStream())
                    {
                        var response = new StreamReader(st).ReadToEnd();
                        // レスポンス情報出力
                        logger.Info(Utility.GetMsg(MsgConst.KK0022I) + response);
                        logger.Info("Utility#ReqGetReportInfo() End");
                        return DynamicJson.Parse(response);
                    }
                }
                else
                {
                    // OK以外は処理失敗としてエラーとする
                    throw new HttpException(res.StatusCode);
                }
            }
            catch (WebException webEx)
            {
                Dictionary<string, object> errRes;
                // HTTPプロトコルエラーかどうか調べる
                var statusCd = ConvWebStatusToHttpStatus(webEx, out errRes);
                if (WebApiConst.NOT_HTTP_ERROR_CODE.Equals(statusCd))
                {
                    // 例外ログを出力
                    logger.Error(webEx.Message);
                    logger.Error(webEx.StackTrace);
                    throw;
                }
                else if (WebApiConst.NOT_HTTP_PROXY_CODE.Equals(statusCd))
                {
                    // ログ出力
                    logger.Error(Utility.GetMsg(MsgConst.KK0015E));
                    logger.Error(webEx.Message);
                    logger.Error(webEx.StackTrace);
                    throw;
                }
                else
                {
                    // プロトコルエラー用のログ出力
                    if (errRes.ContainsKey(WebApiConst.ITEM_NAME_JSON_ERROR_TYPE))
                    {
                        // 登録/更新処理エラーログ
                        OutputMessageErrorHttpUpload(webEx, errRes, statusCd);
                    }
                    else
                    {
                        // その他エラーログ
                        OutputMessageErrorHttp(webEx, errRes, statusCd);
                    }
                    throw new HttpException(statusCd);
                }
            }
            catch (HttpException apiEx)
            {
                OutputMessageErrorHttp(apiEx);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                throw;
            }
        }

        #endregion 報告書情報取得API

        #region 報告データマスタ更新API

        /// <summary>
        /// 報告データマスタ更新API
        /// </summary>
        /// <param name="masterId">マスタID</param>
        /// <param name="request"></param>
        public void ReqUpdReportMaster(string masterId, string request)
        {
            logger.Info("Utility#ReqUpdReportMaster() Start");
            logger.Info(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0039I), new string[] { masterId }));
            // リクエストURL作成
            var resrcPath = string.Format(WebApiConst.PATH_UPLOAD_REPORT, masterId);
            var url = CreateRequestUrl(resrcPath);
            try
            {
                // リクエスト作成
                var req = (HttpWebRequest)WebRequest.Create(url);
                CreateRequestConfig(req, WebApiConst.METHOD_PUT, request);
                // リクエスト情報(Method:URI)をログ出力
                logger.Info(Utility.GetMsg(MsgConst.KK0021I) + req.RequestUri.ToString());
                logger.Debug(string.Format(Utility.GetMsg(MsgConst.KK0020I), req.Method, req.RequestUri.ToString()));
                // レスポンス取得
                var res = (HttpWebResponse)req.GetResponse();
                if (HttpStatusCode.OK.Equals(res.StatusCode))
                {
                    // レスポンス情報取得
                    using (var st = res.GetResponseStream())
                    {
                        // レスポンス情報出力
                        logger.Info(Utility.GetMsg(MsgConst.KK0022I) + new StreamReader(st).ReadToEnd());
                    }
                }
                else
                {
                    // OK以外は処理失敗としてエラーとする
                    throw new HttpException(res.StatusCode);
                }
            }
            catch (WebException webEx)
            {
                Dictionary<string, object> errRes;
                // HTTPプロトコルエラーかどうか調べる
                var statusCd = ConvWebStatusToHttpStatus(webEx, out errRes);
                if (WebApiConst.NOT_HTTP_ERROR_CODE.Equals(statusCd))
                {
                    // 例外ログを出力
                    logger.Error(webEx.Message);
                    logger.Error(webEx.StackTrace);
                    throw;
                }
                else if (WebApiConst.NOT_HTTP_PROXY_CODE.Equals(statusCd))
                {
                    // ログ出力
                    logger.Error(Utility.GetMsg(MsgConst.KK0015E));
                    logger.Error(webEx.Message);
                    logger.Error(webEx.StackTrace);
                    throw;
                }
                else
                {
                    // プロトコルエラー用のログ出力
                    if (errRes.ContainsKey(WebApiConst.ITEM_NAME_JSON_ERROR_TYPE))
                    {
                        // 登録/更新処理エラーログ
                        OutputMessageErrorHttpUpload(webEx, errRes, statusCd);
                    }
                    else
                    {
                        // その他エラーログ
                        OutputMessageErrorHttp(webEx, errRes, statusCd);
                    }
                    throw new HttpException(statusCd);
                }
            }
            catch (HttpException apiEx)
            {
                OutputMessageErrorHttp(apiEx);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                throw;
            }
            logger.Info(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0040I), new string[] { masterId }));
            logger.Info("Utility#ReqUpdReportMaster() End");
        }

        #endregion 報告データマスタ更新API

        /// <summary>
        /// リクエストURLの作成
        /// </summary>
        /// <param name="resrcPath">処理毎のリソースパス</param>
        /// <returns>リクエストURL</returns>
        public string CreateRequestUrl(string resrcPath)
        {
            return string.Format(WebApiConst.URL, GetProtocol(), GetHostName(), GetWebApiVer(), resrcPath);
        }

        /// <summary>
        /// ステータスコード変換メソッド
        /// </summary>
        /// <remarks>
        /// WebExceptionのステータスコードをHttpWebResponseのステータスコードに変換
        /// </remarks>
        /// <param name="logger">ロガー</param>
        /// <param name="webEx">WebException</param>
        /// <param name="statusCd">ステータスコード</param>
        /// <returns>エラーコード</returns>
        private int ConvWebStatusToHttpStatus(WebException webEx, out Dictionary<string, object> errRes)
        {
            var ret = WebApiConst.NOT_HTTP_ERROR_CODE;
            errRes = new Dictionary<string, object>();
            // プロトコルエラーか確認
            if (WebExceptionStatus.ProtocolError.Equals(webEx.Status))
            {
                var resEx = (HttpWebResponse)webEx.Response;
                if (WebApiConst.CONTENT_TYPE_JSON.Equals(resEx.ContentType))
                {
                    using (var st = resEx.GetResponseStream())
                    {
                        errRes = (Dictionary<string, object>)DynamicJson.Parse(new StreamReader(st).ReadToEnd());
                    }
                }
                // 数値変換
                ret = (int)resEx.StatusCode;
            }
            else
            {
                // エラーメッセージが『リモート名を解決できませんでした。』の場合は、
                // 専用エラーコードを設定する
                if (webEx.Message.Contains(Utility.GetMsg(MsgConst.KK0016E)))
                {
                    ret = WebApiConst.NOT_HTTP_PROXY_CODE;
                }
            }
            return ret;
        }

        /// <summary>
        /// エラー状況によるメッセージ作成
        /// </summary>
        /// <param name="code">HTTPエラーコード</param>
        /// <returns>出力するメッセージ</returns>
        private static string CreateErrorMessage(int code)
        {
            var statusCode = (HttpStatusCode)code;
            var msg = string.Empty;
            switch (statusCode)
            {
                case HttpStatusCode.BadRequest: // 400
                    msg = Utility.GetMsg(MsgConst.KK0017E);
                    break;

                case HttpStatusCode.NotFound:   // 404
                    msg = Utility.GetMsg(MsgConst.KK0018E);
                    break;
                // 上記以外
                default:
                    msg = ReleaseMsg(Utility.GetMsg(MsgConst.KK0019E), new string[] { code.ToString() });
                    break;
            }
            return msg;
        }

        /// <summary>
        /// HTTP処理中の例外発生時のログ出力
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="webEx">発生エラーオブジェクト</param>
        /// <param name="errRes">エラーレスポンス</param>
        /// <param name="errCode">エラー時のHttpStatusCode</param>
        private void OutputMessageErrorHttp(WebException webEx, Dictionary<string, object> errRes, int errCode)
        {
            logger.Error(CreateErrorMessage(errCode));
            // エラーレスポンスに規定のキーがあれば出力
            if (errRes.ContainsKey(WebApiConst.ITEM_NAME_JSON_ERROR_ID))
            {
                logger.Error(string.Format(Utility.GetMsg(MsgConst.KK0020I), errRes[WebApiConst.ITEM_NAME_JSON_ERROR_ID], errRes[WebApiConst.ITEM_NAME_JSON_MESSAGE]));
            }
            // 例外を出力
            logger.Error(webEx.Message);    // メッセージ
            logger.Error(webEx.StackTrace); // スタックトレース
            logger.Error(webEx.Response.Headers);
            OutputErrorResponse(errRes);
        }

        /// <summary>
        /// HTTP処理中の例外発生時のログ出力
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="webEx">発生エラーオブジェクト</param>
        /// <param name="errRes">エラーレスポンス</param>
        /// <param name="errCode">エラー時のHttpStatusCode</param>
        private void OutputMessageErrorHttpUpload(WebException webEx, Dictionary<string, object> errRes, int errCode)
        {
            // 例外を出力
            logger.Error(CreateErrorMessage(errCode));
            logger.Error(webEx.Message);    // メッセージ
            logger.Error(webEx.StackTrace); // スタックトレース
            OutputErrorResponse(errRes);   // エラーレスポンスの情報を出力
        }

        /// <summary>
        /// HTTPエラー発生時のログ出力
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="apiEx">発生エラーオブジェクト</param>
        public void OutputMessageErrorHttp(HttpException apiEx)
        {
            logger.Error(CreateErrorMessage((int)apiEx.ErrCode));
        }

        /// <summary>
        /// エラーレスポンス情報出力
        /// </summary>
        /// <param name="res">取得したレスポンス</param>
        public void OutputErrorResponse(Dictionary<string, object> res)
        {
            foreach (KeyValuePair<string, object> dict in res)
            {
                // 取得したレスポンスの項目を全てログに出力する
                if (dict.Value is string)
                {
                    logger.Error(string.Format(Utility.GetMsg(MsgConst.KK0020I), dict.Key, dict.Value));
                }
                else if (dict.Value is Dictionary<string, string>)
                {
                    var work = (Dictionary<string, string>)dict.Value;
                    foreach (KeyValuePair<string, string> dict2 in work)
                    {
                        logger.Error(string.Format(Utility.GetMsg(MsgConst.KK0020I) + "：{2}", dict.Key, dict2.Key, dict2.Value));
                    }
                }
                else if (dict.Value is Dictionary<string, object>)
                {
                    var work = (Dictionary<string, object>)dict.Value;
                    OutputErrorResponse(work);
                }
            }
        }

        #region WebApi関連定義取得

        private static string GetProtocol()
        {
            var protocol = WebApiConst.URL_HTTP;
            if (WebApiConst.URL_HTTPS.ToLower().Equals(Config.AppSettings.Settings[WebApiConst.PROTOCOL].Value))
            {
                protocol = WebApiConst.URL_HTTPS;
            }
            return protocol;
        }

        private static string GetHostName()
        {
            return Config.AppSettings.Settings[WebApiConst.HOST_NAME].Value;
        }

        private static string GetWebApiVer()
        {
            return Config.AppSettings.Settings[WebApiConst.VER].Value;
        }

        private static bool IsProxyUser()
        {
            return WebApiConst.USE.Equals(Config.AppSettings.Settings[WebApiConst.PROXY_SERVER].Value);
        }

        private static string GetProxyAddress()
        {
            return Config.AppSettings.Settings[WebApiConst.PROXY_SERVER_ADDRESS].Value; ;
        }

        private static string GetProxyPort()
        {
            return Config.AppSettings.Settings[WebApiConst.PROXY_SERVER_PORT].Value; ;
        }

        private static string GetProxyUserId()
        {
            return Config.AppSettings.Settings[WebApiConst.PROXY_SERVER_USERID].Value; ;
        }

        private static string GetProxyPassword()
        {
            return Config.AppSettings.Settings[WebApiConst.PROXY_SERVER_PASSWORD].Value; ;
        }

        private static string GetAuthorizationKey()
        {
            return Config.AppSettings.Settings[WebApiConst.AUTHORIZATION_KEY].Value; ;
        }

        private static int GetRequestTimeout()
        {
            var requestTimeout = WebApiConst.REQUEST_TIMEOUT;
            var reqTimeout = Config.AppSettings.Settings[WebApiConst.STR_REQUEST_TIMEOUT].Value;
            if (!string.IsNullOrEmpty(reqTimeout))
            {
                requestTimeout = int.Parse(reqTimeout);
            }
            return requestTimeout;
        }

        #endregion WebApi関連定義取得

        /// <summary>
        /// リクエスト設定の作成
        /// </summary>
        /// <param name="req">リクエスト</param>
        /// <param name="method">メソッド</param>
        /// <param name="request">ボディ部</param>
        private void CreateRequestConfig(HttpWebRequest req, string method, string request)
        {
            req.Timeout = GetRequestTimeout();
            req.Method = method;
            // 認証キー取得
            var apiKey = GetAuthorizationKey();
            if (!string.IsNullOrEmpty(apiKey))
            {
                // Base64エンコード
                var cnvKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(apiKey));
                // ヘッダに認証追加
                req.Headers.Add(string.Format(WebApiConst.HEADER_CERTIFICATE, cnvKey));
            }
            if (IsProxyUser())
            {
                // 認証情報設定
                var proxy = new WebProxy(GetProxyAddress(), int.Parse(GetProxyPort()));
                proxy.Credentials = new NetworkCredential(GetProxyUserId(), GetProxyPassword());

                req.Proxy = proxy;
            }
            else
            {
                // プロキシを使用しない
                req.Proxy = null;
            }
            // GET以外の場合はヘッダ/パラメータを設定する
            if (WebApiConst.METHOD_POST.Equals(method) || WebApiConst.METHOD_PUT.Equals(method) || WebApiConst.METHOD_DELETE.Equals(method))
            {
                req.ContentType = WebApiConst.CONTENT_TYPE_JSON;
                if (!WebApiConst.METHOD_DELETE.Equals(method))
                {
                    var data = Encoding.UTF8.GetBytes(request);
                    req.ContentLength = data.Length;
                    using (var stream = req.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
        }

        #endregion WebApi関連

        #region 報告書API

        /// <summary>
        /// 報告書API
        /// </summary>
        /// <param name="formatId">フォーマットID</param>
        /// <param name="reportId">レポートID</param>
        /// <param name="request">リクエスト</param>
        /// <param name="mode">リクエスト</param>
        public ResponseInfo ReqReportApi(string formatId, string reportId, string request, string mode)
        {
            logger.Info("Utility#ReqReportApi() Start");
            CreateLog(MsgConst.KK0052I, mode);
            var responseInfo = new ResponseInfo();
            // リクエストURL作成
            var resrcPath = CreateResrcPath(formatId, reportId, mode);
            var url = CreateRequestUrl(resrcPath);
            try
            {
                // リクエスト作成
                var req = (HttpWebRequest)WebRequest.Create(url);
                CreateRequest(req, request, mode);
                // リクエスト情報(Method:URI)をログ出力
                logger.Info(Utility.GetMsg(MsgConst.KK0021I) + req.RequestUri.ToString());
                logger.Debug(string.Format(Utility.GetMsg(MsgConst.KK0020I), req.Method, request));
                // レスポンス取得
                var res = (HttpWebResponse)req.GetResponse();
                CreateResponseInfo(ref responseInfo, res, mode);
            }
            catch (WebException webEx)
            {
                Dictionary<string, object> errRes;
                // HTTPプロトコルエラーかどうか調べる
                var statusCd = ConvWebStatusToHttpStatus(webEx, out errRes);
                if (WebApiConst.NOT_HTTP_ERROR_CODE.Equals(statusCd))
                {
                    // 例外ログを出力
                    logger.Error(webEx.Message);
                    logger.Error(webEx.StackTrace);
                    throw;
                }
                else if (WebApiConst.NOT_HTTP_PROXY_CODE.Equals(statusCd))
                {
                    // ログ出力
                    logger.Error(Utility.GetMsg(MsgConst.KK0015E));
                    logger.Error(webEx.Message);
                    logger.Error(webEx.StackTrace);
                    throw;
                }
                else
                {
                    // プロトコルエラー用のログ出力
                    if (errRes.ContainsKey(WebApiConst.ITEM_NAME_JSON_ERROR_TYPE))
                    {
                        // 登録/更新処理エラーログ
                        OutputMessageErrorHttpUpload(webEx, errRes, statusCd);
                    }
                    else
                    {
                        // その他エラーログ
                        OutputMessageErrorHttp(webEx, errRes, statusCd);
                    }
                    throw new Exception(webEx.Message);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                throw;
            }
            CreateLog(MsgConst.KK0053I, mode);
            logger.Info("Utility#ReqReportApi() End");
            return responseInfo;
        }

        #endregion 報告書API

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="mode"></param>
        private void CreateLog(string messageId, string mode)
        {
            if (CommConst.MODE_INS.Equals(mode))
            {
                logger.Info(Utility.ReleaseMsg(Utility.GetMsg(messageId), new string[] { WebApiConst.INSERT }));
            }
            else if (CommConst.MODE_UPD.Equals(mode))
            {
                logger.Info(Utility.ReleaseMsg(Utility.GetMsg(messageId), new string[] { WebApiConst.UPDATE }));
            }
            else if (CommConst.MODE_DEL.Equals(mode))
            {
                logger.Info(Utility.ReleaseMsg(Utility.GetMsg(messageId), new string[] { WebApiConst.DELETE }));
            }
        }

        #region 画像からBase64に変換

        /// <summary>
        /// 画像からBase64に変換
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string ImageFromFileToBase64(string path)
        {
            using (var image = Image.FromFile(path))
            {
                using (var m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    return Convert.ToBase64String(m.ToArray());
                }
            }
        }

        #endregion 画像からBase64に変換

        #region データテーブルからオブジェクト変換

        /// <summary>
        /// データテーブルからオブジェクト変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<T> ConvertDataTable<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType));
                    }
                }
                return objT;
            }).ToList();
        }

        #endregion データテーブルからオブジェクト変換

        #region リクエストパス作成

        /// <summary>
        /// リクエストパス作成
        /// </summary>
        /// <param name="formatId"></param>
        /// <param name="reportId"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private string CreateResrcPath(string formatId, string reportId, string mode)
        {
            var resrcPath = string.Empty;
            if (CommConst.MODE_INS.Equals(mode))
            {
                resrcPath = string.Format(WebApiConst.PATH_INSERT_REPORT, formatId);
            }
            else if (CommConst.MODE_UPD.Equals(mode))
            {
                resrcPath = string.Format(WebApiConst.PATH_UPDATE_REPORT, formatId, reportId);
            }
            else if (CommConst.MODE_DEL.Equals(mode))
            {
                resrcPath = string.Format(WebApiConst.PATH_DELETE_REPORT, formatId, reportId);
            }
            return resrcPath;
        }

        #endregion リクエストパス作成

        #region リクエスト作成

        /// <summary>
        /// リクエスト作成
        /// </summary>
        /// <param name="req"></param>
        /// <param name="request"></param>
        /// <param name="mode"></param>
        private void CreateRequest(HttpWebRequest req, string request, string mode)
        {
            if (CommConst.MODE_INS.Equals(mode))
            {
                CreateRequestConfig(req, WebApiConst.METHOD_POST, request);
            }
            else if (CommConst.MODE_UPD.Equals(mode))
            {
                CreateRequestConfig(req, WebApiConst.METHOD_PUT, request);
            }
            else if (CommConst.MODE_DEL.Equals(mode))
            {
                CreateRequestConfig(req, WebApiConst.METHOD_DELETE, string.Empty);
            }
        }

        #endregion リクエスト作成

        #region レスポンス情報作成

        /// <summary>
        /// レスポンス情報作成
        /// </summary>
        /// <param name="responseInfo"></param>
        /// <param name="res"></param>
        /// <param name="mode"></param>
        private void CreateResponseInfo(ref ResponseInfo responseInfo, HttpWebResponse res, string mode)
        {
            if (CommConst.MODE_INS.Equals(mode))
            {
                if (HttpStatusCode.Created.Equals(res.StatusCode))
                {
                    // レスポンス情報取得
                    using (var st = res.GetResponseStream())
                    {
                        var response = new StreamReader(st).ReadToEnd();
                        // レスポンス情報出力
                        logger.Info(Utility.GetMsg(MsgConst.KK0022I) + response);
                        var responseBody = (ResponseBody)DynamicJson.Parse(response);
                        responseInfo.Result = CommConst.RESULT_OK;
                        responseInfo.ReportNo = responseBody.report_id;
                        responseInfo.UpdDateTime = (DateTime.Parse(responseBody.last_update_date, null, DateTimeStyles.RoundtripKind)).ToString(CommConst.YYYY_MM_DD_HH_MM_SS_FFF_FORMAT);
                    }
                }
                else
                {
                    // OK以外は処理失敗としてエラーとする
                    throw new HttpException(res.StatusCode);
                }
            }
            else if (CommConst.MODE_UPD.Equals(mode))
            {
                if (HttpStatusCode.OK.Equals(res.StatusCode))
                {
                    // レスポンス情報取得
                    using (var st = res.GetResponseStream())
                    {
                        var response = new StreamReader(st).ReadToEnd();
                        // レスポンス情報出力
                        logger.Info(Utility.GetMsg(MsgConst.KK0022I) + response);
                        var responseBody = (ResponseBody)DynamicJson.Parse(response);
                        responseInfo.Result = CommConst.RESULT_OK;
                        responseInfo.ReportNo = responseBody.report_id;
                        responseInfo.UpdDateTime = (DateTime.Parse(responseBody.last_update_date, null, DateTimeStyles.RoundtripKind)).ToString(CommConst.YYYY_MM_DD_HH_MM_SS_FFF_FORMAT);
                    }
                }
                else
                {
                    // OK以外は処理失敗としてエラーとする
                    throw new HttpException(res.StatusCode);
                }
            }
            else if (CommConst.MODE_DEL.Equals(mode))
            {
                if (HttpStatusCode.OK.Equals(res.StatusCode))
                {
                    responseInfo.Result = CommConst.RESULT_OK;
                }
                else
                {
                    // OK以外は処理失敗としてエラーとする
                    throw new HttpException(res.StatusCode);
                }
            }
        }

        #endregion レスポンス情報作成

        #region レポート連携テーブルにデータインサート

        /// <summary>
        /// レポート連携テーブルにデータインサート
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="dempyoNo"></param>
        /// <param name="actionType"></param>
        /// <param name="reportNo"></param>
        /// <param name="dateTime"></param>
        public decimal InsReportRenkeiData(string reportId, string dempyoNo, string actionType, string reportNo, DateTime dateTime)
        {
            logger.Info("Utility#InsReportRenkeiData() Start");
            var seq = decimal.Zero;
            var dbAccess = new DbAccess(logger, GetConnection(CommConst.BASIC_CONNECTION));
            try
            {
                dbAccess.BeginTransaction();
                if (string.IsNullOrEmpty(reportNo))
                {
                    var parameters = new object[] { SqlConst.P_REPORT_ID + CommConst.COMMA + reportId, SqlConst.P_DEMPYO_NO + CommConst.COMMA + dempyoNo, SqlConst.P_ACTION_TYPE + CommConst.COMMA + actionType, SqlConst.P_RENKEI_FLG + CommConst.COMMA + SqlConst.RENKEI_FLG_MISAI, SqlConst.P_INS_TM + CommConst.COMMA + dateTime, SqlConst.P_UPD_TM + CommConst.COMMA + dateTime };
                    seq = dbAccess.ExecuteScalar(SqlConst.QUERY_KK_REPORT_RENKEI_TBL_INSERT_NOT_REPORT_NO, parameters);
                }
                else
                {
                    var parameters = new object[] { SqlConst.P_REPORT_ID + CommConst.COMMA + reportId, SqlConst.P_DEMPYO_NO + CommConst.COMMA + dempyoNo, SqlConst.P_ACTION_TYPE + CommConst.COMMA + actionType, SqlConst.P_REPORT_NO + CommConst.COMMA + reportNo, SqlConst.P_RENKEI_FLG + CommConst.COMMA + SqlConst.RENKEI_FLG_MISAI, SqlConst.P_INS_TM + CommConst.COMMA + dateTime, SqlConst.P_UPD_TM + CommConst.COMMA + dateTime };
                    seq = dbAccess.ExecuteScalar(SqlConst.QUERY_KK_REPORT_RENKEI_TBL_INSERT, parameters);
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
            logger.Info("Utility#InsReportRenkeiData() End");
            return seq;
        }

        #endregion レポート連携テーブルにデータインサート

        #region レポート連携テーブルにデータアップデート

        /// <summary>
        /// レポート連携テーブルにデータアップデート
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="reportNo"></param>
        /// <param name="dateTime"></param>
        public void UpdReportRenkeiZumiData(decimal seq, string reportNo, DateTime dateTime)
        {
            logger.Info("Utility#UpdReportRenkeiZumiData() Start");
            var dbAccess = new DbAccess(logger, GetConnection(CommConst.BASIC_CONNECTION));
            try
            {
                dbAccess.BeginTransaction();
                var parameters = new object[] { SqlConst.P_REPORT_NO + CommConst.COMMA + reportNo, SqlConst.P_RENKEI_FLG + CommConst.COMMA + SqlConst.RENKEI_FLG_ZUMI, SqlConst.P_UPD_TM + CommConst.COMMA + dateTime, SqlConst.P_RENKEI_SEQ + CommConst.COMMA + seq };
                dbAccess.ExecuteNonQuery(SqlConst.QUERY_KK_REPORT_RENKEI_TBL_UPDATE_BY_KEY, parameters);
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

            logger.Info("Utility#UpdReportRenkeiZumiData() End");
        }

        #endregion レポート連携テーブルにデータアップデート

        #region レポート連携テーブルデータ取得（連携未済分）

        /// <summary>
        /// レポート連携テーブルデータ取得（連携未済分）
        /// </summary>
        /// <param name="renkeiFlag"></param>
        /// <returns></returns>
        public DataTable GetReportRenkeiMisaiData(int renkeiFlag)
        {
            logger.Info("Utility#GetReportRenkeiMisaiData() Start");
            var dt = new DataTable();
            var dbAccess = new DbAccess(logger, GetConnection(CommConst.BASIC_CONNECTION));
            try
            {
                var parameters = new object[] { SqlConst.P_RENKEI_FLG + CommConst.COMMA + renkeiFlag };
                dt = dbAccess.Reader(SqlConst.QUERY_KK_REPORT_RENKEI_TBL_SELECT_BY_RENKEI_FLG, parameters);
            }
            finally
            {
                dbAccess.Close();
            }
            logger.Info("Utility#GetReportRenkeiMisaiData() End");
            return dt;
        }

        #endregion レポート連携テーブルデータ取得（連携未済分）
    }
}