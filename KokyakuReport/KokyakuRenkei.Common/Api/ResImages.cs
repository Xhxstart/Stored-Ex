namespace KokyakuRenkei.Common.Api
{
    public class ResImages
    {
        /// <summary>
        /// 画像表示領域名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 画像ファイルのバイナリデータをBase64エンコーディングした文字列
        /// </summary>
        public string data { get; set; }
    }
}