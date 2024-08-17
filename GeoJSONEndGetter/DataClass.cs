using System.Text.Json.Serialization;

namespace GeoJSONEndGetter
{
    public class DataClass
    {
        /// <summary>
        /// 上端
        /// </summary>
        [JsonPropertyName("top")]
        public double Top { get; set; } = double.NaN;

        /// <summary>
        /// 右端
        /// </summary>
        [JsonPropertyName("right")]
        public double Right { get; set; } = double.NaN;

        /// <summary>
        /// 下端
        /// </summary>
        [JsonPropertyName("bottom")]
        public double Bottom { get; set; } = double.NaN;

        /// <summary>
        /// 左端
        /// </summary>
        [JsonPropertyName("left")]
        public double Left { get; set; } = double.NaN;

        /// <summary>
        /// 各情報
        /// </summary>
        [JsonPropertyName("properties")]
        public Properties_ Properties { get; set; } = new();

        /// <summary>
        /// 各情報
        /// </summary>
        public class Properties_
        {
            /// <summary>
            /// コード
            /// </summary>
            [JsonPropertyName("code")]
            public int? Code { get; set; } = null;

            /// <summary>
            /// 名称
            /// </summary>
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// 名称(ひらがな)
            /// </summary>
            [JsonPropertyName("namekana")]
            public string NameKana { get; set; } = string.Empty;
        }
    }
}
