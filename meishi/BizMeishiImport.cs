using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HitachiBusiness.K_Base.CommonLib;
using HitachiBusiness.K_Base.CommonLib.Base;
using HitachiBusiness.K_Base.CommonLib.ParamData;
using HitachiBusiness.K_Base.CommonLib.ParamData.Kk;
using HitachiBusiness.K_Base.ToolKitLib.Controller;
using HitachiBusiness.K_Base.ToolKitLib.Exceptions;
using HitachiBusiness.K_Base.ToolKitLib.Message;
using HitachiBusiness.K_Base.ToolKitLib.ParamData;
using HitachiBusiness.K_Base.ToolKitLib.Utility;
using HitachiBusiness.RWAT.Server.Common;
using HitachiBusiness.RWAT.Server.Dao;
using Microsoft.VisualBasic.FileIO;

namespace HitachiBusiness.K_Base.BizAPI_KokyakuKanri
{
    /// <summary>
    /// 名刺インポート
    /// </summary>
    public class BizMeishiImport : BaseBizAPI
    {
        #region 常量定義

        /// <summary>
        /// 英数字チェック用インスタンス
        /// </summary>
        public const string CHK_EISUJI = @"^[0-9A-Za-z]*$";

        /// <summary>
        /// 全角文字チェック用インスタンス
        /// </summary>
        public const string CHK_ZENKAKU = @"^[^ -~｡-ﾟ]*$";

        /// <summary>
        /// YYYY/MM/DD日付チェック用インスタンス
        /// </summary>
        public const string CHK_HIDUKE = @"^\d{4}(\-|\/|\.)\d{1,2}\1\d{1,2}$";

        /// <summary>
        /// TEL,FAXチェック用インスタンス
        /// </summary>
        private const string CHK_NUM = @"^[0-9-]*$";

        /// <summary>
        /// 数字チェック用インスタンス
        /// </summary>
        private const string CHK_SUJI = @"^[0-9]*$";

        /// <summary>
        /// エラーフラグ
        /// </summary>
        public const string ERROR_FLG = "ERROR_FLG";

        /// <summary>
        /// エラーフラグ
        /// </summary>
        public const string ERROR_FLG1 = "ERROR_FLG1";

        /// <summary>
        /// エラーフラグ
        /// </summary>
        public const string ERROR_FLG2 = "ERROR_FLG2";

        /// <summary>
        /// エラーフラグ
        /// </summary>
        public const string ERROR_FLG3 = "ERROR_FLG3";

        /// <summary>
        /// エラーフラグ
        /// </summary>
        public const string ERROR_FLG4 = "ERROR_FLG4";

        /// <summary>
        /// 親項目名
        /// </summary>
        public const string PD_SEARCH_CONDITION = "SearchCondition";

        /// <summary>
        /// 親項目名
        /// </summary>
        public const string RESULT_LIST = "ResultList";

        /// <summary>
        /// 頁総数
        /// </summary>
        public const decimal PAGE_CNT = 10;

        /// <summary>
        /// 現在画面RowNoFr
        /// </summary>
        public const string NOW_ROW_FR = "ROW_NO_NOW_FR";

        /// <summary>
        /// 現在画面RowNoTo
        /// </summary>
        public const string NOW_ROW_TO = "ROW_NO_NOW_TO";

        #endregion 常量定義

        #region クエリ名

        /// <summary>
        /// 名刺WKデータ登録処理クエリ名 BizMeishiImport
        /// </summary>
        private const string QUERY_NAME_BIZ_IMPORT_MEISHI_WK = "BizMeishiImport";

        /// <summary>
        /// 名刺データインサート処理クエリ名 BizMeishiInsert
        /// </summary>
        private const string QUERY_NAME_BIZ_INSERT_MEISHI = "BizMeishiInsert";

        /// <summary>
        /// 名刺データインサート処理クエリ名 BizMeishiCheck
        /// </summary>
        private const string QUERY_NAME_BIZ_CHECK_MEISHI = "BizMeishiCheck";

        /// <summary>
        /// 名刺データ登録処理クエリ名 BizMeishiGetUpd
        /// </summary>
        private const string QUERY_NAME_BIZ_GETUPD_MEISHI = "BizMeishiGetUpd";

        /// <summary>
        /// 一括顧客設定処理クエリ名 BizSetMeishiKokyaku
        /// </summary>
        private const string QUERY_NAME_BIZ_SETKOKYAKU_MEISHI = "BizSetMeishiKokyaku";

        #endregion クエリ名

        #region ファイルパス名

        /// <summary>
        /// ダンーロドファイルパス
        /// </summary>
        public string outputFilePath = "";

        #endregion ファイルパス名

        #region エーラフラグ

        /// <summary>
        /// エーラフラグ
        /// </summary>
        private bool errflg = false;

        #endregion エーラフラグ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ctrlData">ctrlData</param>
        public BizMeishiImport(ControllData ctrlData) : base(ctrlData) { }

        /// <summary>
        ///コンタクトインポート処理
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="uploadPath">アップロードファイルパス(物理パス)</param>
        /// <param name="rowNoFr">LineFR</param>
        /// <param name="rowNoTo">LineTO</param>
        ///  <param name="Errorflg">Errorflg</param>
        /// <returns></returns>
        public PDMeishiImportSetCond ImportData(string tableName, string uploadPath, string rowNoFr, string rowNoTo, PDMeishiImportSearchCond pdSearchCond, out long allCount, out long Errorflg)
        {
            //インポート用ワークフォルダパス処理
            string tempPath = GetTempPath();
            //セッションIDフォルダを作成する
            Directory.CreateDirectory(Path.Combine(tempPath, base.ctrlData.UserInfo.SessionId));

            //(1-1)のパス文字列と(1-2)を結合し、文字列変数strTempPathに格納
            string strTempPath = Path.Combine(tempPath, base.ctrlData.UserInfo.SessionId);
            //登録用ファイルパス
            string strWritePath = Path.Combine(strTempPath, Path.GetFileName(uploadPath));
            //インポートファイル作成処理
            MakeImportFile(uploadPath, strWritePath);

            PDMeishiImportSetCond pdSetCond = SaveData(strWritePath, tableName, rowNoFr, rowNoTo, pdSearchCond, out allCount, out Errorflg);

            //で作成した【一時フォルダ】を削除する。
            //Directory.Delete(strTempPath, true);
            return pdSetCond;
        }

        /// <summary>
        /// インポート用ワークフォルダパス処理
        /// </summary>
        /// <returns>インポート用ワークフォルダのルートパス</returns>
        private string GetTempPath()
        {
            //要素名
            string dbServerSetting = "DBServerSetting";
            //取得する値のキー名
            string workPath = "WorkPath";
            //アプリケーション定義参照クラスのGetKeyValueメソッドを呼出し、DBサーバ共有フォルダパス設定値を取得する
            string dbServertPath = AppConfig.GetKeyValue(dbServerSetting, workPath);
            string path = Path.Combine(dbServertPath, "MeishiImport");
            //フォルダチェック
            if (!Directory.Exists(path))
            {
                //フォルダ作成
                Directory.CreateDirectory(path);
            }
            //パスをリターンする
            return path;
        }

        /// <summary>
        /// インポートファイル作成処理
        /// </summary>
        /// <param name="readPath">入力ファイルパス（アップロードファイル）</param>
        /// <param name="writePath">出力ファイルパス（作成ファイルパス）</param>
        private void MakeImportFile(string readPath, string writePath)
        {
            //拡張子がCSV以外の場合
            if (Path.GetExtension(readPath).ToString().ToLower() != ".csv")
            {
                //KK2003W：ファイル形式が不正です。
                throw new SMBBTApplicationException(
                                       SMBBTMessageManager.createApplicationMessage("KK2003W"));
            }
            //ファイルパス
            string tempPath = Path.ChangeExtension(readPath, "tmp");
            //インスタンスswを生成
            using (StreamWriter sw = new StreamWriter(tempPath, false, Encoding.GetEncoding("shift_jis")))
            {
                //インスタンスparserを生成
                using (TextFieldParser parser = new TextFieldParser(readPath,
                                                                   Encoding.GetEncoding("shift_jis")))
                {
                    //TextFieldParserインスタンスparserのプロパティを設定する
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.HasFieldsEnclosedInQuotes = true;
                    //行番号(int)を定義
                    int lineNum = -1;
                    while (!parser.EndOfData)
                    {
                        //行番号をインクリメント
                        lineNum++;
                        //１行読み出し
                        string[] RdFields = parser.ReadFields();
                        //エラー内容変数を定義
                        string errorMsg = "";
                        if (RdFields.Length > 0 && lineNum > 0)
                        {
                            //列数チェックを行う
                            Boolean blnColumnCount = true;
                            if (RdFields.Length != 14 && RdFields.Length != 11)
                            {
                                blnColumnCount = false;
                            }
                            if (!blnColumnCount)
                            {
                                //KK2004W：項目数が不正です。
                                throw new SMBBTApplicationException(
                                            SMBBTMessageManager.createApplicationMessage("KK2004W"));
                            }
                            if (RdFields.Length == 14)
                            {
                                CheckMeishiData(RdFields, out errorMsg);
                            }
                            //データを作成する
                            StringBuilder stringBuilder = new StringBuilder();
                            //『会社コード(文字列化)』
                            stringBuilder.Append(base.ctrlData.UserInfo.KaisyaCode);
                            // 『カンマ』
                            stringBuilder.Append(",");
                            //『セッションID(文字列化)』
                            stringBuilder.Append(base.ctrlData.UserInfo.SessionId);
                            // 『カンマ』
                            stringBuilder.Append(",");
                            //『行番号(文字列化)』
                            stringBuilder.Append(lineNum.ToString());
                            // 『カンマ』
                            stringBuilder.Append(",");
                            // 『エラー内容』
                            stringBuilder.Append(errorMsg);
                            // 『カンマ』
                            stringBuilder.Append(",");
                            // 『コンタクトシーケンス(空文字)』
                            stringBuilder.Append("");
                            for (int i = 0; i < RdFields.Length; i++)
                            {
                                RdFields[i].Replace(Environment.NewLine, "");
                                // 『カンマ』
                                stringBuilder.Append(",");
                                stringBuilder.Append(RdFields[i]);
                            }
                            if (RdFields.Length == 11)
                            {
                                // 『カンマ』
                                stringBuilder.Append(",");
                                //『顧客管理NO 』
                                stringBuilder.Append("");
                                // 『カンマ』
                                stringBuilder.Append(",");
                                //『相手先担当者コード』
                                stringBuilder.Append("");
                                // 『カンマ』
                                stringBuilder.Append(",");
                                //『担当者コード 』
                                stringBuilder.Append("");
                            }
                            // 『カンマ』
                            stringBuilder.Append(",");
                            //『履歴ＮＯ』
                            stringBuilder.Append("0");
                            // 『カンマ』
                            stringBuilder.Append(",");
                            //『担当部署コード 』
                            stringBuilder.Append("");
                            // 『カンマ』
                            stringBuilder.Append(",");
                            //『削除フラグ』
                            stringBuilder.Append("0");
                            // 『カンマ』
                            stringBuilder.Append(",");
                            //『登録日時』
                            stringBuilder.Append(DateTime.Now.ToString());
                            // 『カンマ』
                            stringBuilder.Append(",");
                            //登録者コード
                            stringBuilder.Append(base.ctrlData.UserInfo.UserId);
                            // 『カンマ』
                            stringBuilder.Append(",");
                            //登録プログラムコード
                            stringBuilder.Append(base.ctrlData.ProgramCode);
                            // 『カンマ』
                            stringBuilder.Append(",");
                            //『更新日時』
                            stringBuilder.Append(DateTime.Now.ToString());
                            // 『カンマ』
                            stringBuilder.Append(",");
                            //『更新者コード』
                            stringBuilder.Append(base.ctrlData.UserInfo.UserId);
                            // 『カンマ』
                            stringBuilder.Append(",");
                            //『更新プログラムコード』
                            stringBuilder.Append(base.ctrlData.ProgramCode);

                            stringBuilder.ToString();
                            //データを書き込む
                            sw.WriteLine(stringBuilder);
                        }
                    }
                    parser.Close();
                    if (lineNum == -1 || lineNum == 0)
                    {
                        //KK1001I：CSVに入力されているデータが0件
                        throw new SMBBTApplicationException(
                                    SMBBTMessageManager.createApplicationMessage("KK1001I"));
                    }
                }
                sw.Close();
            }
            //tempPathのファイルを出力ファイルパス（フルパス）にコピーする
            File.Copy(tempPath, writePath, true);
        }

        /// <summary>
        /// 5.5 名刺データチェック処理
        /// </summary>
        /// <param name="row">行のフィールドデータ</param>
        /// <param name="error_msg">エラー内容</param>
        private void CheckMeishiData(string[] row, out string error_msg)
        {
            Regex chk_suji = new Regex(CHK_SUJI);
            error_msg = "";
            //顧客管理NO（row[11]）チェック
            if (!string.IsNullOrEmpty(row[11]) && row[11].Length > 10)
            {
                row[11] = "";
                error_msg = "顧客管理NOが10文字を超えています。CSVファイルを修正してください。";
            }
            else if (chk_suji.IsMatch(row[11]) != true)
            {
                row[11] = "";
                error_msg = "顧客管理NOが数字ではありません。 CSVファイルを修正してください。";
            }
        }

        /// <summary>
        /// WKテプールに登録処理
        /// </summary>
        /// <param name="filePath">登録ファイルパス（作成ファイルパス）</param>
        /// <param name="tableName">テーブル名</param>
        /// <returns>エラー一覧テープル</returns>
        private PDMeishiImportSetCond SaveData(string filePath, string tableName, string rowNoFr, string rowNoTo, PDMeishiImportSearchCond pdSearchCond, out long allCount, out long Errorflg)
        {
            Dictionary<string, string> condParam = new Dictionary<string, string>();
            DateTime dt;
            if (!string.IsNullOrEmpty(pdSearchCond.SHUTOKU_DATE))
            {
                DateTime.TryParse(pdSearchCond.SHUTOKU_DATE, out dt);
                pdSearchCond.SHUTOKU_DATE = dt.ToString("d");
            }
            condParam = pdSearchCond.toSrchCondition();
            //以下のパラメータをcondParamにセット
            condParam.Add(ConstParamDataKeys.KAISHA_CD, base.ctrlData.UserInfo.KaisyaCode);
            condParam.Add(ConstParamDataKeys.USER_ID, base.ctrlData.UserInfo.UserId);
            condParam.Add(ConstParamDataKeys.PGM_CD, base.ctrlData.ProgramCode);
            condParam.Add(ConstParamDataKeys2.FILE_PATH, filePath);
            condParam.Add(ConstParamDataKeys.SESSION_ID, base.ctrlData.UserInfo.SessionId);
            condParam.Add(ConstParamDataKeys.GET_ROW_FR, rowNoFr);
            condParam.Add(ConstParamDataKeys.GET_ROW_TO, rowNoTo);

            string queryname = QUERY_NAME_BIZ_IMPORT_MEISHI_WK;
            long count;

            DataSet dsResult = DBAccessManager.Query<string>(base.ctrlData, Const_KokyakuKanri.SYSTEM_CD,
                                                             queryname, condParam, out count, true);
            //(4)の戻り値dsResultの１つ目のテーブルの先頭行先頭列が０以外の場合はエラー
            if (Convert.ToInt32(dsResult.Tables[0].Rows[0][0]) != 0)
            {
                //KK2003W：KK2003W。
                throw new SMBBTApplicationException(SMBBTMessageManager.createApplicationMessage("KK2003W"));
            }
            //(4)の戻り値dsResultの２つ目のテーブルを変数dtListに格納
            PDMeishiImportSetCond pdSetCond = new PDMeishiImportSetCond(ConvertData(dsResult.Tables[1]));
            allCount = long.Parse(dsResult.Tables[2].Rows[0][0].ToString());

            Errorflg = long.Parse(dsResult.Tables[3].Rows[0][0].ToString());
            //dtListをリターンする
            return pdSetCond;
        }

        /// <summary>
        /// 名刺データチェック処理
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="rowNoFr">LINE_NO_FR</param>
        /// <param name="rowNoTo">LINE_NO_TO</param>
        ///  <param name="pdSetCond">Condition</param> pdSetCond
        /// <returns>エラー一覧テープル</returns>
        public PDMeishiImportSetCond CheckData(string tableName, string rowNoFr, string rowNoTo, PDMeishiImportSetCond pdSetCond, out long allCount, out long ErrorLine1, out long ErrorLine2, out long ErrorLine3, out long ErrorLine4)
        {
            BizMeishiImport bizMeishiImport = new BizMeishiImport(ctrlData);

            Dictionary<string, string> condParam = new Dictionary<string, string>();

            //DataRow dr = new DataRow();

            //以下のパラメータをcondParamにセット
            condParam.Add(ConstParamDataKeys.KAISHA_CD, base.ctrlData.UserInfo.KaisyaCode);
            condParam.Add(ConstParamDataKeys.USER_ID, base.ctrlData.UserInfo.UserId);
            condParam.Add(ConstParamDataKeys.PGM_CD, base.ctrlData.ProgramCode);
            condParam.Add(ConstParamDataKeys.SESSION_ID, base.ctrlData.UserInfo.SessionId);
            condParam.Add(ConstParamDataKeys.GET_ROW_FR, rowNoFr);
            condParam.Add(ConstParamDataKeys.GET_ROW_TO, rowNoTo);
            string queryname = QUERY_NAME_BIZ_CHECK_MEISHI;
            long count = 0;
            allCount = 0;
            ErrorLine1 = 0;
            ErrorLine2 = 0;
            ErrorLine3 = 0;
            ErrorLine4 = 0;
            DataSet dsResult = DBAccessManager.Query<string>(base.ctrlData, Const_KokyakuKanri.SYSTEM_CD,
                                                             queryname, condParam, out count, true);
            //0つ目のテーブルの先頭行先頭列が行番号
            //CSVファイルを修正する必要があるエラーがある場合
            if (dsResult.Tables[0].Rows.Count != 0)
            {
                ErrorLine1 = long.Parse(dsResult.Tables[0].Rows[0][0].ToString());
                //KK2006W：{999})。CSVファイルを修正して再度インポートしてください。
            }
            //１つ目のテーブルの先頭行先頭列が行番号
            //複数の相手先担当者と一致したデータに対して相手先担当者
            else if (dsResult.Tables[1].Rows.Count != 0)
            {
                ErrorLine4 = long.Parse(dsResult.Tables[1].Rows[0][0].ToString());
                //KK2009W：{999})。複数の相手先担当者と一致したデータに対して相手先担当者
            }
            //二つ目のテーブルの先頭行先頭列が行番号
            //「登録顧客」が未指定のデータがある場合
            else if (dsResult.Tables[2].Rows.Count != 0)
            {
                //KK2007W：顧客が指定されていないデータがあります(行番号:{999})。
                ErrorLine2 = long.Parse(dsResult.Tables[2].Rows[0][0].ToString());
            }

            //三つ目のテーブルの先頭行先頭列が行番号
            //複数の相手先担当者と一致したデータに、「登録相手先担当者」未指定がある。
            else if (dsResult.Tables[3].Rows.Count != 0)
            {
                ErrorLine3 = long.Parse(dsResult.Tables[3].Rows[0][0].ToString());
            }
            //(4)の戻り値dsResultのテーブルを変数dtListに格納
            pdSetCond = new PDMeishiImportSetCond(ConvertData(dsResult.Tables[4]));
            allCount = long.Parse(dsResult.Tables[5].Rows[0][0].ToString());
            //dtListをリターンする
            return pdSetCond;
        }

        /// <summary>
        /// 名刺テプールに登録処理
        /// </summary>
        /// <param name="filePath">登録ファイルパス（作成ファイルパス）</param>
        /// <param name="tableName">テーブル名</param>
        /// <returns>エラー一覧テープル</returns>
        public PDMeishiImportSetCond InsertData(string tableName, string rowNoFr, string rowNoTo, PDMeishiImportSetCond pdSetCond, out long allCount, out long Errorflg)
        {
            BizMeishiImport bizMeishiImport = new BizMeishiImport(ctrlData);

            Dictionary<string, string> condParam = new Dictionary<string, string>();

            //以下のパラメータをcondParamにセット
            condParam.Add(ConstParamDataKeys.KAISHA_CD, base.ctrlData.UserInfo.KaisyaCode);
            condParam.Add(ConstParamDataKeys.USER_ID, base.ctrlData.UserInfo.UserId);
            condParam.Add(ConstParamDataKeys.PGM_CD, base.ctrlData.ProgramCode);
            condParam.Add(ConstParamDataKeys.SESSION_ID, base.ctrlData.UserInfo.SessionId);
            condParam.Add(ConstParamDataKeys.GET_ROW_FR, rowNoFr);
            condParam.Add(ConstParamDataKeys.GET_ROW_TO, rowNoTo);
            string queryname = QUERY_NAME_BIZ_INSERT_MEISHI;
            long count;
            Errorflg = 0;
            DataSet dsResult = DBAccessManager.Query<string>(base.ctrlData, Const_KokyakuKanri.SYSTEM_CD,
                                                             queryname, condParam, out count, true);
            dsResult.Tables[1].Clear();
            //(4)の戻り値dsResultのテーブルを変数dtListに格納
            pdSetCond = new PDMeishiImportSetCond(ConvertData(dsResult.Tables[1]));
            allCount = 0;
            //dtListをリターンする
            return pdSetCond;
        }

        /// <summary>
        /// 名刺WKテプールに顧客一括設定
        /// </summary>
        /// <param name="filePath">登録ファイルパス（作成ファイルパス）</param>
        /// <param name="tableName">テーブル名</param>
        /// <returns>エラー一覧テープル</returns>
        public PDMeishiImportSetCond SetMeishiKokyaku(string tableName, string rowNoFr, string rowNoTo, PDMeishiImportSearchCond pdSearchCond, out long allCount)
        {
            Dictionary<string, string> condParam = new Dictionary<string, string>();
            //DataRow dr = new DataRow();
            condParam = pdSearchCond.toSrchCondition();
            //以下のパラメータをcondParamにセット
            condParam.Add(ConstParamDataKeys.KAISHA_CD, base.ctrlData.UserInfo.KaisyaCode);
            condParam.Add(ConstParamDataKeys.USER_ID, base.ctrlData.UserInfo.UserId);
            condParam.Add(ConstParamDataKeys.PGM_CD, base.ctrlData.ProgramCode);
            condParam.Add(ConstParamDataKeys.SESSION_ID, base.ctrlData.UserInfo.SessionId);
            condParam.Add(ConstParamDataKeys.GET_ROW_FR, rowNoFr);
            condParam.Add(ConstParamDataKeys.GET_ROW_TO, rowNoTo);
            string queryname = QUERY_NAME_BIZ_SETKOKYAKU_MEISHI;
            long count;

            DataSet dsResult = DBAccessManager.Query<string>(base.ctrlData, Const_KokyakuKanri.SYSTEM_CD,
                                                             queryname, condParam, out count, true);

            //(4)の戻り値dsResultのテーブルを変数dtListに格納
            PDMeishiImportSetCond pdSetCond = new PDMeishiImportSetCond(ConvertData(dsResult.Tables[1]));

            allCount = long.Parse(dsResult.Tables[2].Rows[0][0].ToString());
            //dtListをリターンする
            return pdSetCond;
        }

        /// <summary>
        /// 名刺WKテプールに更新処理（登録用）
        /// </summary>
        /// <param name="rowNoFr">LINE_NO_FR</param>
        /// <param name="rowNoTo">LINE_NO_TO</param>
        /// <param name="pdSetCond">pdSetCond</param>
        /// <returns>エラー一覧テープル</returns>
        public void UpdateMeishiData(string rowNoFr, string rowNoTo, PDMeishiImportSetCond pdSetCond)
        {
            Dictionary<string, string> condParam = new Dictionary<string, string>();
            StringBuilder l_Filter = new StringBuilder();     //抽出条件
            condParam = pdSetCond.toSrchCondition();
            string rowNoNowFr = pdSetCond.LINE_NO1;
            string rowNoNowTo = pdSetCond.LINE_NO10 ?? pdSetCond.LINE_NO9 ?? pdSetCond.LINE_NO8 ?? pdSetCond.LINE_NO7 ?? pdSetCond.LINE_NO6 ?? pdSetCond.LINE_NO5 ?? pdSetCond.LINE_NO4 ?? pdSetCond.LINE_NO3 ?? pdSetCond.LINE_NO2 ?? pdSetCond.LINE_NO1;
            //以下のパラメータをcondParamにセット
            condParam.Add(ConstParamDataKeys.KAISHA_CD, base.ctrlData.UserInfo.KaisyaCode);
            condParam.Add(ConstParamDataKeys.USER_ID, base.ctrlData.UserInfo.UserId);
            condParam.Add(ConstParamDataKeys.PGM_CD, base.ctrlData.ProgramCode);
            condParam.Add(ConstParamDataKeys.SESSION_ID, base.ctrlData.UserInfo.SessionId);
            condParam.Add(ConstParamDataKeys.GET_ROW_FR, rowNoFr);
            condParam.Add(ConstParamDataKeys.GET_ROW_TO, rowNoTo);
            condParam.Add(NOW_ROW_FR, rowNoNowFr ?? "");
            condParam.Add(NOW_ROW_TO, rowNoNowTo ?? "");
            condParam.Add("AYITETANTOSHA_CDA", pdSetCond.AYITETANTOSHA_CD10 ?? "");
            condParam.Add("KOKYAKUKANRI_NOA", pdSetCond.KOKYAKUKANRI_NO10.ToString() ?? "");
            string queryname = QUERY_NAME_BIZ_GETUPD_MEISHI;
            long count;

            DataSet dsResult = DBAccessManager.Query<string>(base.ctrlData, Const_KokyakuKanri.SYSTEM_CD,
                                                             queryname, condParam, out count, true);
        }

        /// <summary>
        /// 名刺WKテプールに更新取得処理（ページング用）
        /// </summary>
        /// <param name="filePath">登録ファイルパス（作成ファイルパス）</param>
        /// <param name="tableName">テーブル名</param>
        /// <returns>エラー一覧テープル</returns>
        public PDMeishiImportSetCond UpdGetMeishiData(string tableName, string rowNoFr, string rowNoTo, PDMeishiImportSetCond pdSetCond, out long allCount)
        {
            Dictionary<string, string> condParam = new Dictionary<string, string>();
            StringBuilder l_Filter = new StringBuilder();     //抽出条件
            condParam = pdSetCond.toSrchCondition();
            string rowNoNowFr = pdSetCond.LINE_NO1;
            string rowNoNowTo = pdSetCond.LINE_NO10 ?? pdSetCond.LINE_NO9 ?? pdSetCond.LINE_NO8 ?? pdSetCond.LINE_NO7 ?? pdSetCond.LINE_NO6 ?? pdSetCond.LINE_NO5 ?? pdSetCond.LINE_NO4 ?? pdSetCond.LINE_NO3 ?? pdSetCond.LINE_NO2 ?? pdSetCond.LINE_NO1;
            //以下のパラメータをcondParamにセット
            condParam.Add(ConstParamDataKeys.KAISHA_CD, base.ctrlData.UserInfo.KaisyaCode);
            condParam.Add(ConstParamDataKeys.USER_ID, base.ctrlData.UserInfo.UserId);
            condParam.Add(ConstParamDataKeys.PGM_CD, base.ctrlData.ProgramCode);
            condParam.Add(ConstParamDataKeys.SESSION_ID, base.ctrlData.UserInfo.SessionId);
            condParam.Add(ConstParamDataKeys.GET_ROW_FR, rowNoFr);
            condParam.Add(ConstParamDataKeys.GET_ROW_TO, rowNoTo);
            condParam.Add(NOW_ROW_FR, rowNoNowFr ?? "");
            condParam.Add(NOW_ROW_TO, rowNoNowTo ?? "");
            condParam.Add("AYITETANTOSHA_CDA", pdSetCond.AYITETANTOSHA_CD10 ?? "");
            condParam.Add("KOKYAKUKANRI_NOA", pdSetCond.KOKYAKUKANRI_NO10.ToString() ?? "");
            string queryname = QUERY_NAME_BIZ_GETUPD_MEISHI;
            long count;

            DataSet dsResult = DBAccessManager.Query<string>(base.ctrlData, Const_KokyakuKanri.SYSTEM_CD,
                                                             queryname, condParam, out count, true);

            //(4)の戻り値dsResultのテーブルを変数dtListに格納
            pdSetCond = new PDMeishiImportSetCond(ConvertData(dsResult.Tables[0]));

            allCount = long.Parse(dsResult.Tables[1].Rows[0][0].ToString());
            //dtListをリターンする
            return pdSetCond;
        }

        /// <summary>
        /// 一テプールに転換
        /// </summary>
        /// <param name="filePath">登録ファイルパス（作成ファイルパス）</param>
        /// <param name="tableName">テーブル名</param>
        /// <returns>エラー一覧テープル</returns>
        private DataTable ConvertData(DataTable desctd)
        {
            var dt = new DataTable();
            var columns = new DataColumn[] {
                new DataColumn("LINE_NO1", Type.GetType("System.String")),
                new DataColumn("KOKYAKUKANRI_NO1", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_CD1", Type.GetType("System.String")),
                new DataColumn("KOKYAKU_NM1", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_NM1", Type.GetType("System.String")),
                new DataColumn("ERROR_MSG1", Type.GetType("System.String")),
                new DataColumn("LINE_NO2", Type.GetType("System.String")),
                new DataColumn("KOKYAKUKANRI_NO2", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_CD2", Type.GetType("System.String")),
                new DataColumn("KOKYAKU_NM2", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_NM2", Type.GetType("System.String")),
                new DataColumn("ERROR_MSG2", Type.GetType("System.String")),
                new DataColumn("LINE_NO3", Type.GetType("System.String")),
                new DataColumn("KOKYAKUKANRI_NO3", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_CD3", Type.GetType("System.String")),
                new DataColumn("KOKYAKU_NM3", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_NM3", Type.GetType("System.String")),
                new DataColumn("ERROR_MSG3", Type.GetType("System.String")),
                new DataColumn("LINE_NO4", Type.GetType("System.String")),
                new DataColumn("KOKYAKUKANRI_NO4", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_CD4", Type.GetType("System.String")),
                new DataColumn("KOKYAKU_NM4", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_NM4", Type.GetType("System.String")),
                new DataColumn("ERROR_MSG4", Type.GetType("System.String")),
                new DataColumn("LINE_NO5", Type.GetType("System.String")),
                new DataColumn("KOKYAKUKANRI_NO5", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_CD5", Type.GetType("System.String")),
                new DataColumn("KOKYAKU_NM5", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_NM5", Type.GetType("System.String")),
                new DataColumn("ERROR_MSG5", Type.GetType("System.String")),
                new DataColumn("LINE_NO6", Type.GetType("System.String")),
                new DataColumn("KOKYAKUKANRI_NO6", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_CD6", Type.GetType("System.String")),
                new DataColumn("KOKYAKU_NM6", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_NM6", Type.GetType("System.String")),
                new DataColumn("ERROR_MSG6", Type.GetType("System.String")),
                new DataColumn("LINE_NO7", Type.GetType("System.String")),
                new DataColumn("KOKYAKUKANRI_NO7", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_CD7", Type.GetType("System.String")),
                new DataColumn("KOKYAKU_NM7", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_NM7", Type.GetType("System.String")),
                new DataColumn("ERROR_MSG7", Type.GetType("System.String")),
                new DataColumn("LINE_NO8", Type.GetType("System.String")),
                new DataColumn("KOKYAKUKANRI_NO8", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_CD8", Type.GetType("System.String")),
                new DataColumn("KOKYAKU_NM8", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_NM8", Type.GetType("System.String")),
                new DataColumn("ERROR_MSG8", Type.GetType("System.String")),
                new DataColumn("LINE_NO9", Type.GetType("System.String")),
                new DataColumn("KOKYAKUKANRI_NO9", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_CD9", Type.GetType("System.String")),
                new DataColumn("KOKYAKU_NM9", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_NM9", Type.GetType("System.String")),
                new DataColumn("ERROR_MSG9", Type.GetType("System.String")),
                new DataColumn("LINE_NO10", Type.GetType("System.String")),
                new DataColumn("KOKYAKUKANRI_NO10", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_CD10", Type.GetType("System.String")),
                new DataColumn("KOKYAKU_NM10", Type.GetType("System.String")),
                new DataColumn("AYITETANTOSHA_NM10", Type.GetType("System.String")),
                new DataColumn("ERROR_MSG10", Type.GetType("System.String"))
            };
            dt.Columns.AddRange(columns);
            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);
            int i = 0;
            for (int j = 0; j < desctd.Rows.Count; j++)
            {
                for (int k = 0; k < desctd.Columns.Count; k++)
                {
                    if (desctd.Rows[j][k].ToString().Length > 0)
                    {
                        dt.Rows[0][i] = desctd.Rows[j][k].ToString();
                    }
                    else
                        dt.Rows[0][i] = "";
                    i++;
                }
            }
            return dt;
        }
    }
}