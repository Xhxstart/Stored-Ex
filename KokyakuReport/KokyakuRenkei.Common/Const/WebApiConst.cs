namespace KokyakuRenkei.Common.Const
{
    public class WebApiConst
    {
        /// <summary>
        /// レスポンス(JSON)：error_type
        /// </summary>
        /// <remarks>
        /// マスタアップロードのエラーレスポンスで受け取る
        /// </remarks>
        public const string ITEM_NAME_JSON_ERROR_TYPE = "error_type";

        /// <summary>
        /// レスポンス(JSON)：error_id
        /// </summary>
        /// <remarks>
        /// 共通のエラーレスポンスで受け取る
        /// </remarks>
        public const string ITEM_NAME_JSON_ERROR_ID = "error_Id";

        /// <summary>
        /// レスポンス(JSON)：message
        /// </summary>
        /// <remarks>
        /// 報告書作成依頼ステータス取得のレスポンスで受け取る
        /// </remarks>
        public const string ITEM_NAME_JSON_MESSAGE = "message";

        /// <summary>
        /// WebAPIとの通信で使用するリソースパス：報告データマスタアップロード
        /// </summary>
        /// <remarks>
        /// WEBAPI_URLの{3}で使用
        /// </remarks>
        public const string PATH_UPLOAD_REPORT = "templates/{0}";

        /// <summary>
        /// 報告書情報取得
        /// </summary>
        public const string PATH_GET_REPORT_INFO = "formats/{0}/reports/{1}.json";

        /// <summary>
        /// WebAPIとの通信で使用するURL
        /// </summary>
        /// <remarks>
        /// {0}：{http} or {https}
        /// {1}：hostname
        /// {2}：WebAPIバージョン
        /// {3}：リソースパス
        /// </remarks>
        public const string URL = "{0}://{1}/kaisakuapi/{2}/{3}";

        /// <summary>
        /// WebAPIとの通信で使用するURL：メソッド(GET)
        /// </summary>
        /// <remarks>
        /// WEBAPI_URLの{1}で使用
        /// </remarks>
        public const string METHOD_GET = "GET";

        /// <summary>
        /// WebAPIとの通信で使用するURL：メソッド(PUT)
        /// </summary>
        /// <remarks>
        /// WEBAPI_URLの1}で使用
        /// </remarks>
        public const string METHOD_PUT = "PUT";

        /// <summary>
        /// WebAPIとの通信で使用するURL：メソッド(POST)
        /// </summary>
        /// <remarks>
        /// WEBAPI_URLの1}で使用
        /// </remarks>
        public const string METHOD_POST = "POST";

        /// <summary>
        /// WebAPIとの通信で使用するURL：メソッド(DELETE)
        /// </summary>
        /// <remarks>
        /// WEBAPI_URLの1}で使用
        /// </remarks>
        public const string METHOD_DELETE = "DELETE";

        /// <summary>
        /// WebAPIとの通信で使用するContent-Type：application/json;charset=UTF-8
        /// </summary>
        public const string CONTENT_TYPE_JSON = "application/json;charset=UTF-8";

        /// <summary>
        /// Protocol
        /// </summary>
        public const string PROTOCOL = "Protocol";

        /// <summary>
        /// HostName
        /// </summary>
        public const string HOST_NAME = "HostName";

        /// <summary>
        /// WebApiVer
        /// </summary>
        public const string VER = "WebApiVer";

        /// <summary>
        /// ProxyServer
        /// </summary>
        public const string PROXY_SERVER = "ProxyServer";

        /// <summary>
        /// ProxyServerAddress
        /// </summary>
        public const string PROXY_SERVER_ADDRESS = "ProxyServerAddress";

        /// <summary>
        /// ProxyServerPort
        /// </summary>
        public const string PROXY_SERVER_PORT = "ProxyServerPort";

        /// <summary>
        /// ProxyServerUserId
        /// </summary>
        public const string PROXY_SERVER_USERID = "ProxyServerUserId";

        /// <summary>
        /// ProxyServerPassword
        /// </summary>
        public const string PROXY_SERVER_PASSWORD = "ProxyServerPassword";

        /// <summary>
        /// Use
        /// </summary>
        public const string USE = "Use";

        /// <summary>
        /// WebAPIとの通信で使用するURL：http
        /// </summary>
        /// <remarks>
        /// WEBAPI_URLの{0}で使用
        /// </remarks>
        public const string URL_HTTP = "http";

        /// <summary>
        /// WebAPIとの通信で使用するURL：https
        /// </summary>
        /// <remarks>
        /// WEBAPI_URLの{0}で使用
        /// </remarks>
        public const string URL_HTTPS = "https";

        /// <summary>
        /// WebAPIとの通信時、ヘッダに追加する認証情報
        /// </summary>
        /// <remarks>
        /// APIキーを指定する
        /// </remarks>
        public const string HEADER_CERTIFICATE = "X-KAISAKU-Authorization: {0}";

        /// <summary>
        /// AuthorizationKey
        /// </summary>
        public const string AUTHORIZATION_KEY = "AuthorizationKey";

        /// <summary>
        /// RequestTimeout
        /// </summary>
        public const string STR_REQUEST_TIMEOUT = "RequestTimeout";

        /// <summary>
        /// HTTPエラー以外の場合のエラーコード：-1
        /// </summary>
        public const int NOT_HTTP_ERROR_CODE = -1;

        /// <summary>
        /// HTTPプロキシ未使用のエラーコード：-10
        /// </summary>
        public const int NOT_HTTP_PROXY_CODE = -10;

        /// <summary>
        /// リクエストタイムアウト時間（ミリ秒）
        /// </summary>
        public const int REQUEST_TIMEOUT = 600000;

        /// <summary>
        /// マスタアップロードのパラメータ：master_name
        /// </summary>
        public const string ITEM_NAME_MASTER_PARAM_NAME = "master_name";

        /// <summary>
        /// マスタアップロードのパラメータ：items
        /// </summary>
        public const string ITEM_NAME_MASTER_PARAM_ITEMS = "items";

        /// <summary>
        /// 取引先コード（WebApi）
        /// </summary>
        public const string TORIHIKISAKI_CD = "取引先コード";

        /// <summary>
        /// 取引先略称（WebApi）
        /// </summary>
        public const string TORIHIKISAKI_SNM = "取引先略称";

        /// <summary>
        /// 連携対象
        /// </summary>
        public const string RENKEI_TARGET = "連携対象";

        /// <summary>
        /// 得意先
        /// </summary>
        public const string RENKEI_TARGET_TOKUISAKI = "得意先";

        /// <summary>
        /// 納入先
        /// </summary>
        public const string RENKEI_TARGET_NONYUSAKI = "納入先";

        /// <summary>
        /// 仕入先
        /// </summary>
        public const string RENKEI_TARGET_SHIRESAKI = "仕入先";

        /// <summary>
        /// MIKOMI_RANK
        /// </summary>
        public const string MIKOMI_RANK = "見込ランク";

        /// <summary>
        /// 顧客マスタ連携
        /// </summary>
        public const string KOKYAKU_MST_RENKEI = "顧客マスタ連携";

        /// <summary>
        /// マスタ連携ＣＳＶデータ：
        /// </summary>
        public const string MASTER_RENKEI_CSV_DATA = "マスタ連携ＣＳＶデータ：";

        /// <summary>
        /// WebAPIとの通信で使用するリソースパス：報告書登録
        /// </summary>
        public const string PATH_INSERT_REPORT = "formats/{0}";

        /// <summary>
        /// WebAPIとの通信で使用するリソースパス：報告書更新
        /// </summary>
        public const string PATH_UPDATE_REPORT = "formats/{0}/reports/{1}";

        /// <summary>
        /// WebAPIとの通信で使用するリソースパス：報告書削除
        /// </summary>
        public const string PATH_DELETE_REPORT = "formats/{0}/reports/{1}";

        /// <summary>
        /// ユーザID
        /// </summary>
        public const string USER_ID_KEY = "user_id";

        /// <summary>
        /// 二重登録防止のためのキー
        /// </summary>
        public const string REGISTER_KEY = "register_key";

        /// <summary>
        /// 指示タイトル
        /// </summary>
        public const string INSTRUCT_TITLE_KEY = "instruct_title";

        /// <summary>
        /// 指示内容
        /// </summary>
        public const string INSTRUCT_CONTENTS_KEY = "instruct_contents";

        /// <summary>
        /// 指示回答
        /// </summary>
        public const string INSTRUCT_ANSWER_KEY = "instruct_answer";

        /// <summary>
        /// 報告書データキー
        /// </summary>
        public const string REPORT_DATA_KEY = "report_data";

        /// <summary>
        /// 報告書添付画像キー
        /// </summary>
        public const string IMAGES_KEY = "images";

        /// <summary>
        /// 報告書添付画像名キー
        /// </summary>
        public const string IMAGES_NAME_KEY = "name";

        /// <summary>
        /// 報告書添付画像データキー
        /// </summary>
        public const string IMAGES_DATA_KEY = "data";

        /// <summary>
        /// 画像ファイル名
        /// </summary>
        public const string PHOTO = "photo";

        /// <summary>
        /// 登録
        /// </summary>
        public const string INSERT = "登録";

        /// <summary>
        /// 更新
        /// </summary>
        public const string UPDATE = "更新";

        /// <summary>
        /// 削除
        /// </summary>
        public const string DELETE = "削除";
    }
}