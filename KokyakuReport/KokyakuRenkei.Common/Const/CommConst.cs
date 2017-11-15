namespace KokyakuRenkei.Common.Const
{
    public class CommConst
    {
        /// <summary>
        /// マスタ連携
        /// </summary>
        public const string MASTER = "M";

        /// <summary>
        /// トランザクション連携
        /// </summary>
        public const string TRANSACTION = "T";

        /// <summary>
        /// 報告書削除
        /// </summary>
        public const string REPORT_DELETE = "D";

        /// <summary>
        /// AppCommon.config
        /// </summary>
        public const string APP_COMMON_CONFIG_FILE = "AppCommon.config";

        /// <summary>
        /// messages.xml
        /// </summary>
        public const string MESSAGES = "messages.xml";

        /// <summary>
        /// Message
        /// </summary>
        public const string MESSAGE = "Message";

        /// <summary>
        /// id
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// action_type
        /// </summary>
        public const string ACTION_TYPE = "action_type";

        /// <summary>
        /// format_id
        /// </summary>
        public const string FORMAT_ID = "format_id";

        /// <summary>
        /// report_no
        /// </summary>
        public const string REPORT_NO = "report_no";

        /// <summary>
        /// イベント種類
        /// </summary>
        public const string MSG_ARGUS_ACTION_TYPE = "イベント種類";

        /// <summary>
        /// 報告書フォーマットID
        /// </summary>
        public const string MSG_ARGUS_FORMAT_ID = "報告書フォーマットID";

        /// <summary>
        /// 報告書No
        /// </summary>
        public const string MSG_ARGUS_REPORT_NO = "報告書No";

        /// <summary>
        /// 処理ID
        /// </summary>
        public const string MSG_ARGUS_PROC_ID = "処理ID";

        // <summary>
        /// イベント種類リスト
        /// </summary>
        public const string ACTION_TYPE_LIST = "ActionTypeList";

        // <summary>
        /// ランクリスト
        /// </summary>
        public const string RANK_LIST = "RankList";

        /// <summary>
        /// 顧客DB定義
        /// </summary>
        public const string CUSTOMER_CONNECTION = "CustomerConnection";

        /// <summary>
        /// 基幹DB定義
        /// </summary>
        public const string BASIC_CONNECTION = "BasicConnection";

        /// <summary>
        /// bin
        /// </summary>
        public const string BIN = "bin";

        /// <summary>
        /// 種類
        /// </summary>
        public const string MSG_ARGUS_TYPE = "種別（マスタ（M）かトランザクション(T)を設定してください。）";

        /// <summary>
        /// ','
        /// </summary>
        public const char CHAR_COMMA = ',';

        /// <summary>
        /// スペース
        /// </summary>
        public const string HALF_SPACE = " ";

        /// <summary>
        /// "{"
        /// </summary>
        public const string LEFT_BRACES = "{";

        /// <summary>
        /// "}"
        /// </summary>
        public const string RIGHT_BRACES = "}";

        /// <summary>
        /// "("
        /// </summary>
        public const string LEFT_BRACKET = "(";

        /// <summary>
        /// ")"
        /// </summary>
        public const string RIGHT_BRACKET = ")";

        /// <summary>
        /// "
        /// </summary>
        public const string SINGLE_DOUBLE_QUOTATION = "\"";

        /// <summary>
        /// ""
        /// </summary>
        public const string DOUBLE_DOUBLE_QUOTATION = "\"\"";

        /// <summary>
        /// ","
        /// </summary>

        public const string COMMA = ",";

        /// <summary>
        /// ":"
        /// </summary>
        public const string SEMI_COLON = ":";

        /// <summary>
        /// ";"
        /// </summary>
        public const string COLON = ";";

        /// <summary>
        /// =
        /// </summary>
        public const string EQUAL = "=";

        /// <summary>
        /// \
        /// </summary>
        public const string YEN_MARK = @"\";

        /// <summary>
        /// file:///
        /// </summary>
        public const string FILE_URI = @"file:///";

        /// <summary>
        /// *
        /// </summary>
        public const string ASTERISK = "*";

        /// <summary>
        /// <=
        /// </summary>
        public const string LESS_THAN_OR_EQUAL = "<=";

        /// <summary>
        /// >
        /// </summary>
        public const string GREATER_THAN = ">";

        /// <summary>
        /// 改行コード（\r\n）
        /// </summary>
        public const string NEW_LINE = "\r\n";

        /// <summary>
        /// AdditionDllPath
        /// </summary>
        public const string ADDITION_DLL_PATH = "AdditionDllPath";

        /// <summary>
        /// Run
        /// </summary>
        public const string RUN_METHOD = "Run";

        /// <summary>
        /// Json
        /// </summary>
        public const string JSON_FOLDER = "Json";

        /// <summary>
        /// .json
        /// </summary>
        public const string FILE_EXT_JSON = ".json";

        /// <summary>
        /// MasterMapping.json
        /// </summary>
        public const string MASTER_MAPPING_JSON = "MasterMapping.json";

        /// <summary>
        /// TransactionMapping.json
        /// </summary>
        public const string TRANSACTION_MAPPING_JSON = "TransactionMapping.json";

        /// <summary>
        /// @
        /// </summary>
        public const string AT = "@";

        /// <summary>
        /// ADDITION_PROC_CLASS_NM
        /// </summary>
        public const string ADDITION_PROC_CLASS_NM = "ADDITION_PROC_CLASS_NM";

        /// <summary>
        /// 顧客マスタ連携要
        /// </summary>
        public const string KOKYAKU_MST_RENKEI_FLAG = "1";

        /// <summary>
        /// バッチ正常終了
        /// </summary>
        public const int BATCH_NORMAL = 0;

        /// <summary>
        /// バッチ異常終了
        /// </summary>
        public const int BATCH_ABNORMAL = -1;

        /// <summary>
        /// 連携種別（1：SQLServer→レポート＋トランザクション連携）
        /// </summary>
        public const string TO_REPORTPLUS_1 = "1";

        /// <summary>
        /// 連携結果OK
        /// </summary>
        public const string RESULT_OK = "OK";

        /// <summary>
        /// 連携結果NG
        /// </summary>
        public const string RESULT_NG = "NG";

        /// <summary>
        /// yyyy/MM/dd HH:mm:ss.fff
        /// </summary>
        public const string YYYY_MM_DD_HH_MM_SS_FFF_FORMAT = "yyyy/MM/dd HH:mm:ss.fff";

        /// <summary>
        /// ReportMapping.json
        /// </summary>
        public const string REPORT_MAPPING_JSON = "ReportMapping.json";

        /// <summary>
        /// 報告書登録
        /// </summary>
        public const string MODE_INS = "INS";

        /// <summary>
        /// 報告書更新
        /// </summary>
        public const string MODE_UPD = "UPD";

        /// <summary>
        /// 報告書削除
        /// </summary>
        public const string MODE_DEL = "DEL";

        /// <summary>
        /// アクションタイプ（C：登録）
        /// </summary>
        public const string ACTION_TYPE_C = "C";

        /// <summary>
        /// アクションタイプ（E：更新）
        /// </summary>
        public const string ACTION_TYPE_E = "E";

        /// <summary>
        /// アクションタイプ（D：削除）
        /// </summary>
        public const string ACTION_TYPE_D = "D";

        /// <summary>
        /// ヘッダ・明細区分（ヘッダ）
        /// </summary>
        public const string HDR_DTL_TYPE_HDR = "Hdr";

        /// <summary>
        /// ヘッダ・明細区分（明細）
        /// </summary>
        public const string HDR_DTL_TYPE_DTL = "Dtl";

        /// <summary>
        /// "_"
        /// </summary>
        public const string UNDER_SCORE = "_";

        /// <summary>
        /// \\
        /// </summary>
        public const string DOUBLE_YEN_MARK = @"\\";
    }
}