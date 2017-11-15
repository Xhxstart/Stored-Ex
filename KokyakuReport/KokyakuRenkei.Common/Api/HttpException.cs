using System;
using System.Net;

namespace KokyakuRenkei.Common.Api
{
    public class HttpException : Exception
    {
        /// <summary>
        /// Httpステータスコード
        /// </summary>
        /// <remarks>
        /// 『404 NotFound』の404がこれに当たる
        /// </remarks>
        public HttpStatusCode ErrCode { get; private set; }

        /// <summary>
        /// レスポンスで受け取ったエラーメッセージ
        /// </summary>
        /// <remarks>
        /// 報告書作成依頼ステータス取得のレスポンスで受け取る
        /// </remarks>
        public string ErrResMsg { get; private set; }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public HttpException() { }

        /// <summary>
        /// ログ出力後のメッセージ表示用コンストラクタ
        /// </summary>
        /// <remarks>
        /// ダイアログのメッセージで表示する値(HttpStatusCode)のみを渡す
        /// </remarks>
        /// <param name="code">エラー発生時のHttpStatusCode</param>
        public HttpException(int code)
        {
            this.ErrCode = (HttpStatusCode)code;
        }

        /// <summary>
        /// Httpエラー発生時の基本コンストラクタ
        /// </summary>
        /// <param name="code">エラー時のHttpStatusCode</param>
        /// <param name="proc">エラー発生処理</param>
        public HttpException(HttpStatusCode code)
        {
            this.ErrCode = code;
        }
    }
}