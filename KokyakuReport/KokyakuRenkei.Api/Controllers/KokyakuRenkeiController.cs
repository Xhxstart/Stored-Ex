using KokyakuRenkei.Api.Models;
using KokyakuRenkei.Common;
using KokyakuRenkei.Common.Api;
using KokyakuRenkei.Common.Const;
using log4net;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace KokyakuRenkei.Api.Controllers
{
    public class KokyakuRenkeiController : ApiController
    {
        #region 変数定義

        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion 変数定義

        #region 快作レポート＋外部連携IF「イベント通知」処理

        /// <summary>
        /// 快作レポート＋外部連携IF「イベント通知」処理
        /// </summary>
        /// <param name="action_type">イベント種類</param>
        /// <param name="format_id">報告書フォーマットID</param>
        /// <param name="report_no">報告書No</param>
        public void Get(string action_type, string format_id, string report_no)
        {
            logger.Info(Utility.GetMsg(MsgConst.KK0033I));
            logger.Info("KokyakuRenkeiController#Get() Start");
            try
            {
                var args = new string[] { CommConst.TRANSACTION, action_type, format_id, report_no };
                var renkei = new Renkei(args);
                renkei.util = new Utility(logger);
                renkei.logger = logger;
                renkei.Run();
            }
            catch (Exception ex)
            {
                logger.Error(Utility.ReleaseMsg(Utility.GetMsg(MsgConst.KK0001E), new string[] { action_type, format_id, report_no }));
                logger.Error(ex);
            }

            logger.Info("KokyakuRenkeiController#Get() End");
            logger.Info(Utility.GetMsg(MsgConst.KK0034I));
        }

        #endregion 快作レポート＋外部連携IF「イベント通知」処理

        #region 報告書登録

        /// <summary>
        /// 報告書登録・更新・削除
        /// </summary>
        /// <param name="request">リクエスト</param>
        public async Task<object> Post([FromBody]KokyakuRenkeiModels request)
        {
            logger.Info(Utility.GetMsg(MsgConst.KK0041I));
            logger.Info("KokyakuRenkeiController#Post() Start");
            var responseInfo = new ResponseInfo();
            responseInfo.ReportNo = request.report_no;
            try
            {
                var renkeiService = new RenkeiService(request.to_reportplus, request.action_type, request.report_id, request.dempyo_no, request.report_no, decimal.Zero);
                renkeiService.util = new Utility(logger);
                renkeiService.logger = logger;
                responseInfo = await renkeiService.Run();
            }
            catch (Exception ex)
            {
                responseInfo.Result = CommConst.RESULT_NG;
                responseInfo.Message = ex.Message;
            }
            logger.Info("KokyakuRenkeiController#Post() End");
            logger.Info(Utility.GetMsg(MsgConst.KK0042I));
            return CreateOutJsonData(responseInfo);
        }

        #endregion 報告書登録

        #region オプション

        /// <summary>
        /// オプション
        /// </summary>
        /// <returns></returns>
        public string Options()
        {
            return null;
        }

        #endregion オプション

        #region 返却Jsonデータ作成

        /// <summary>
        /// 返却Jsonデータ作成
        /// </summary>
        /// <param name="responseInfo"></param>
        /// <returns></returns>
        private object CreateOutJsonData(ResponseInfo responseInfo)
        {
            var jsonData = new
            {
                result = responseInfo.Result,
                report_no = responseInfo.ReportNo,
                upd_tm = responseInfo.UpdDateTime,
                message = responseInfo.Message
            };
            return jsonData;
        }

        #endregion 返却Jsonデータ作成
    }
}