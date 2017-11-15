namespace KokyakuRenkei.Common.Const
{
    public class SqlConst
    {
        /// <summary>
        /// カラム（ACTION_TYPE）
        /// </summary>
        public const string COL_ACTION_TYPE = "ACTION_TYPE";

        /// <summary>
        /// カラム（FORMAT_ID）
        /// </summary>
        public const string COL_FORMAT_ID = "FORMAT_ID";

        /// <summary>
        /// カラム（REPORT_NO）
        /// </summary>
        public const string COL_REPORT_NO = "REPORT_NO";

        /// <summary>
        /// カラム（RENKEI_SEQ）
        /// </summary>
        public const string COL_RENKEI_SEQ = "RENKEI_SEQ";

        /// <summary>
        /// カラム（更新日時）
        /// </summary>
        public const string COL_UPD_TM = "UPD_TM";

        /// <summary>
        /// カラム（登録日時）
        /// </summary>
        public const string COL_INS_TM = "INS_TM";

        /// <summary>
        /// 顧客管理前回実行日時
        /// </summary>
        public const string COL_KOKYAKU_KANRI_ZENKAI_JIKKON_TM = "KOKYAKU_KANRI_ZENKAI_JIKKON_TM";

        /// <summary>
        /// WebApi前回実行日時
        /// </summary>
        public const string COL_WEBAPI_ZENKAI_JIKKON_TM = "WEBAPI_ZENKAI_JIKKON_TM";

        /// <summary>
        /// 顧客管理取引先コード
        /// </summary>
        public const string COL_KOKYAKU_KANRI_TORIHIKISAKI_CD = "TORIHIKISAKI_CD";

        /// <summary>
        /// 基幹取引先コード
        /// </summary>
        public const string COL_BASIC_TORIHIKISAKI_CD = "TORIHIKISAKI_CD";

        /// <summary>
        /// 見込みランク
        /// </summary>
        public const string COL_MIKOMI_RANK = "MIKOMI_RANK";

        /// <summary>
        /// SQLパラメータ（@ActionType）
        /// </summary>
        public const string P_ACTION_TYPE = "@actionType";

        /// <summary>
        /// SQLパラメータ（@formatId）
        /// </summary>
        public const string P_FORMAT_ID = "@formatId";

        /// <summary>
        /// SQLパラメータ（@reportNo）
        /// </summary>
        public const string P_REPORT_NO = "@reportNo";

        /// <summary>
        /// SQLパラメータ（@renkeiSeq）
        /// </summary>
        public const string P_RENKEI_SEQ = "@renkeiSeq";

        /// <summary>
        /// SQLパラメータ（@renkeiFlg）
        /// </summary>
        public const string P_RENKEI_FLG = "@renkeiFlg";

        /// <summary>
        /// 処理ID
        /// </summary>
        public const string P_PROC_ID = "@procId";

        /// <summary>
        /// 顧客管理前回実行日時
        /// </summary>
        public const string P_KOKYAKU_KANRI_ZENKAI_JIKKON_TM = "@kokyakuKanriZenkaijikkontm";

        /// <summary>
        /// webapi前回実行日時
        /// </summary>
        public const string P_WEBAPI_ZENKAI_JIKKON_TM = "@webapiZenkaiJikkonTm";

        /// <summary>
        /// 今回実行日時
        /// </summary>
        public const string P_KONKAI_JIKKON_TM = "@konkaiJikkonTm";

        /// <summary>
        /// 取引先略称
        /// </summary>
        public const string P_TORIHIKISAKI_SNM = "@torihikisakiSnm";

        /// <summary>
        /// 顧客管理取引先コード
        /// </summary>
        public const string P_KOKYAKU_KANRI_TORIHIKISAKI_CD = "@torihikisakiCd";

        /// <summary>
        /// 見込みランク
        /// </summary>
        public const string P_MIKOMI_RANK = "@mikomiRank";

        /// <summary>
        /// 取引先名称
        /// </summary>
        public const string STORED_P_TORIHIKISAKI_NM = "@TORIHIKISAKI_NM";

        /// <summary>
        /// 取引先略称
        /// </summary>
        public const string STORED_P_TORIHIKISAKI_SNM = "@TORIHIKISAKI_SNM";

        /// <summary>
        /// 得意先登録フラグ
        /// </summary>
        public const string STORED_P_TOKUISAKI_FLG = "@TOKUISAKI_FLG";

        /// <summary>
        /// 納入先登録フラグ
        /// </summary>
        public const string STORED_P_SHUKKASAKI_FLG = "@SHUKKASAKI_FLG";

        /// <summary>
        /// 仕入先登録フラグ
        /// </summary>
        public const string STORED_P_SHIRESAKI_FLG = "@SHIRESAKI_FLG";

        /// <summary>
        /// 連携済：1
        /// </summary>
        public const int RENKEI_FLG_ZUMI = 1;

        /// <summary>
        /// 連携未済：0
        /// </summary>
        public const int RENKEI_FLG_MISAI = 0;

        /// <summary>
        /// SQLパラメータ（@insTm）
        /// </summary>
        public const string P_INS_TM = "@insTm";

        /// <summary>
        /// SQLパラメータ（@insTm）
        /// </summary>
        public const string P_UPD_TM = "@updTm";

        /// <summary>
        /// デフォルト条件
        /// </summary>
        public const string DEFAULT_CONDITION = "( 1 = 1 )";

        /// <summary>
        /// UPDATE
        /// </summary>
        public const string UPDATE = "UPDATE";

        /// <summary>
        /// INSERT
        /// </summary>
        public const string INSERT = "INSERT";

        /// <summary>
        /// DELETE
        /// </summary>
        public const string DELETE = "DELETE";

        /// <summary>
        /// INTO
        /// </summary>
        public const string INTO = "INTO";

        /// <summary>
        /// VALUES
        /// </summary>
        public const string VALUES = "VALUES";

        /// <summary>
        /// SET
        /// </summary>
        public const string SET = "SET";

        /// <summary>
        /// WHERE
        /// </summary>
        public const string WHERE = "WHERE";

        /// <summary>
        /// AND
        /// </summary>
        public const string AND = "AND";

        /// <summary>
        /// FROM
        /// </summary>
        public const string FROM = "FROM";

        /// <summary>
        /// SELECT
        /// </summary>
        public const string SELECT = "SELECT";

        // <summary>
        /// トランザクションテーブルマッピングマスタデータ取得
        /// </summary>
        public const string QUERY_KK_TRAN_MAPPING_MST_SELECT_BY_KEY = "SELECT * FROM KK_TRAN_MAPPING_MST WHERE FORMAT_ID = " + P_FORMAT_ID;

        /// <summary>
        /// トランザクション連携テーブルデータ取得（主キー指定）
        /// </summary>
        public const string QUERY_KK_TRAN_RENKEI_TBL_SELECT_BY_KEY = "SELECT * FROM KK_TRAN_RENKEI_TBL WHERE RENKEI_SEQ = " + P_RENKEI_SEQ;

        /// <summary>
        /// トランザクション連携テーブルデータ取得（連携フラグ指定）
        /// </summary>
        public const string QUERY_KK_TRAN_RENKEI_TBL_SELECT_BY_RENKEI_FLG = "SELECT * FROM KK_TRAN_RENKEI_TBL WHERE RENKEI_FLG = " + P_RENKEI_FLG;

        /// <summary>
        /// トランザクション連携テーブルデータ更新
        /// </summary>
        public const string QUERY_KK_TRAN_RENKEI_TBL_UPDATE_BY_KEY = "UPDATE KK_TRAN_RENKEI_TBL SET RENKEI_FLG = " + P_RENKEI_FLG + ", UPD_TM = " + P_UPD_TM + " WHERE RENKEI_SEQ = " + P_RENKEI_SEQ;

        /// <summary>
        /// トランザクション連携テーブルデータ登録
        /// </summary>
        public const string QUERY_KK_TRAN_RENKEI_TBL_INSERT = "INSERT INTO KK_TRAN_RENKEI_TBL(FORMAT_ID, REPORT_NO, ACTION_TYPE, RENKEI_FLG, INS_TM, UPD_TM)  VALUES (" + P_FORMAT_ID + ", " + P_REPORT_NO + ", " + P_ACTION_TYPE + ", " + P_RENKEI_FLG + ", " + P_INS_TM + ", " + P_UPD_TM + ")";

        /// <summary>
        /// SELECT CAST(SCOPE_IDENTITY() AS decimal)
        /// </summary>
        public const string SCOPE_IDENTITY = "SELECT CAST(SCOPE_IDENTITY() AS decimal)";

        /// <summary>
        /// マスタ連携バッチ実行履歴テーブル
        /// </summary>
        public const string QUERY_KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL_SELECT_BY_PROC_ID = "SELECT * FROM KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL WHERE PROC_ID = " + P_PROC_ID;

        /// <summary>
        /// マスタ連携バッチ実行履歴テーブル登録（顧客管理前回実行日時指定）
        /// </summary>
        public const string QUERY_KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL_INSERT_BY_ZENKAI_JIKKON_TM = "INSERT INTO KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL(PROC_ID, KOKYAKU_KANRI_ZENKAI_JIKKON_TM) VALUES(" + P_PROC_ID + CommConst.COMMA + CommConst.HALF_SPACE + P_KOKYAKU_KANRI_ZENKAI_JIKKON_TM + ")";

        /// <summary>
        /// マスタ連携バッチ実行履歴テーブル登録（WebApi前回実行日時指定）
        /// </summary>
        public const string QUERY_KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL_INSERT_BY_WEBAPI_ZENKAI_JIKKON_TM = "INSERT INTO KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL(PROC_ID, WEBAPI_ZENKAI_JIKKON_TM) VALUES(" + P_PROC_ID + CommConst.COMMA + CommConst.HALF_SPACE + P_WEBAPI_ZENKAI_JIKKON_TM + ")";

        /// <summary>
        /// マスタ連携バッチ実行履歴テーブル更新（顧客管理前回実行日時指定）
        /// </summary>
        public const string QUERY_KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL_UPDATE_BY_PROC_ID_AND_ZENKAI_JIKKON_TM = "UPDATE KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL SET KOKYAKU_KANRI_ZENKAI_JIKKON_TM = " + P_KOKYAKU_KANRI_ZENKAI_JIKKON_TM + " WHERE PROC_ID = " + P_PROC_ID;

        /// <summary>
        /// マスタ連携バッチ実行履歴テーブル更新（WebApi前回実行日時指定）
        /// </summary>
        public const string QUERY_KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL_UPDATE_BY_PROC_ID_AND_WEBAPI_ZENKAI_JIKKON_TM = "UPDATE KK_MST_RENKEI_BATCH_JIKKOU_RIREIKI_TBL SET WEBAPI_ZENKAI_JIKKON_TM = " + P_WEBAPI_ZENKAI_JIKKON_TM + " WHERE PROC_ID = " + P_PROC_ID;

        /// <summary>
        /// 顧客取引先マスタ取得（取引先略称指定）
        /// </summary>
        public const string QUERY_KK_MST_TORIHIKISAKI_SELECT_BY_TORIHIKISAKI_SNM = "SELECT * FROM KK_MST_TORIHIKISAKI WHERE TORIHIKISAKI_SNM = " + P_TORIHIKISAKI_SNM;

        /// <summary>
        /// 顧客管理取引先マスタデータ取得（取引先コード指定）
        /// </summary>
        public const string QUERY_KK_MST_TORIHIKISAKI_SELECT_BY_TORIHIKISAKI_CD = "SELECT * FROM KK_MST_TORIHIKISAKI WHERE TORIHIKISAKI_CD = " + P_KOKYAKU_KANRI_TORIHIKISAKI_CD;

        /// <summary>
        /// 顧客取引先マスタ更新（取引先略称指定）
        /// </summary>
        public const string QUERY_KK_MST_TORIHIKISAKI_UPDATE_BY_TORIHIKISAKI_SNM = "UPDATE KK_MST_TORIHIKISAKI SET TORIHIKISAKI_CD = " + P_KOKYAKU_KANRI_TORIHIKISAKI_CD + ", MIKOMI_RANK = " + P_MIKOMI_RANK + ", UPD_TM = " + P_UPD_TM + " WHERE TORIHIKISAKI_SNM = " + P_TORIHIKISAKI_SNM;

        /// <summary>
        /// 顧客取引先マスタ更新（取引先コード指定）
        /// </summary>
        public const string QUERY_KK_MST_TORIHIKISAKI_UPDATE_BY_TORIHIKISAKI_CD = "UPDATE KK_MST_TORIHIKISAKI SET TORIHIKISAKI_SNM = " + P_TORIHIKISAKI_SNM + ", MIKOMI_RANK = " + P_MIKOMI_RANK + ", UPD_TM = " + P_UPD_TM + " WHERE TORIHIKISAKI_CD = " + P_KOKYAKU_KANRI_TORIHIKISAKI_CD;

        /// <summary>
        /// 顧客取引先マスタ登録
        /// </summary>
        public const string QUERY_KK_MST_TORIHIKISAKI_INSERT = "INSERT INTO KK_MST_TORIHIKISAKI(TORIHIKISAKI_CD, TORIHIKISAKI_SNM, MIKOMI_RANK, INS_TM, UPD_TM) VALUES(" + P_KOKYAKU_KANRI_TORIHIKISAKI_CD + ", " + P_TORIHIKISAKI_SNM + ", " + P_MIKOMI_RANK + ", " + P_INS_TM + ", " + P_UPD_TM + ")";

        /// <summary>
        /// 得意先作成ストアド
        /// </summary>
        public const string STORED_PROCEDURE_GENERATE_CODE_FOR_TORIHIKISAKI_OTHERS = "GenerateCodeForTorihikisakiOthers";

        /// <summary>
        /// CD_SECTION
        /// </summary>
        public const string P_CD_SECTION = "@cd_section";

        /// <summary>
        /// コードマスタ会社別
        /// </summary>
        public const string QUERY_CODE_KAISHA_BETU_SELECT = "SELECT CD_SECTION, CD_KEY, CD_NM FROM BC_MST_CODE_KAISHA_BETU ";

        /// <summary>
        /// SQLパラメータ（@reportId）
        /// </summary>
        public const string P_REPORT_ID = "@reportId";

        /// <summary>
        /// SQLパラメータ（@dempyoNo）
        /// </summary>
        public const string P_DEMPYO_NO = "@dempyoNo";

        /// <summary>
        /// レポート連携テーブルデータ登録
        /// </summary>
        public const string QUERY_KK_REPORT_RENKEI_TBL_INSERT = "INSERT INTO KK_REPORT_RENKEI_TBL(REPORT_ID, DEMPYO_NO, ACTION_TYPE, REPORT_NO, RENKEI_FLG, INS_TM, UPD_TM)  VALUES (" + P_REPORT_ID + ", " + P_DEMPYO_NO + ", " + P_ACTION_TYPE + ", " + P_REPORT_NO + ", " + P_RENKEI_FLG + ", " + P_INS_TM + ", " + P_UPD_TM + ")";

        /// <summary>
        /// レポート連携テーブルデータ登録
        /// </summary>
        public const string QUERY_KK_REPORT_RENKEI_TBL_INSERT_NOT_REPORT_NO = "INSERT INTO KK_REPORT_RENKEI_TBL(REPORT_ID, DEMPYO_NO, ACTION_TYPE, RENKEI_FLG, INS_TM, UPD_TM)  VALUES (" + P_REPORT_ID + ", " + P_DEMPYO_NO + ", " + P_ACTION_TYPE + ", " + P_RENKEI_FLG + ", " + P_INS_TM + ", " + P_UPD_TM + ")";

        /// <summary>
        /// レポート連携テーブルデータ取得（連携フラグ指定）
        /// </summary>
        public const string QUERY_KK_REPORT_RENKEI_TBL_SELECT_BY_RENKEI_FLG = "SELECT * FROM KK_REPORT_RENKEI_TBL WHERE RENKEI_FLG = " + P_RENKEI_FLG;

        /// <summary>
        /// レポート連携テーブルデータ更新
        /// </summary>
        public const string QUERY_KK_REPORT_RENKEI_TBL_UPDATE_BY_KEY = "UPDATE KK_REPORT_RENKEI_TBL SET REPORT_NO = " + P_REPORT_NO + ", RENKEI_FLG = " + P_RENKEI_FLG + ", UPD_TM = " + P_UPD_TM + " WHERE RENKEI_SEQ = " + P_RENKEI_SEQ;

        /// <summary>
        /// カラム（REPORT_ID）
        /// </summary>
        public const string COL_REPORT_ID = "REPORT_ID";

        /// <summary>
        /// カラム（DEMPYO_NO）
        /// </summary>
        public const string COL_DEMPYO_NO = "DEMPYO_NO";
    }
}