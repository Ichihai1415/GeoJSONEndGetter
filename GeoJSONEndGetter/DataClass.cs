using Ichihai1415.GeoJSON;
using System.Text.Json.Serialization;

namespace GeoJSONEndGetter
{
    /// <summary>
    /// データクラス
    /// </summary>
    public class DataClass
    {
        /// <summary>
        /// 端
        /// </summary>
        [JsonPropertyName("ends")]
        public C_Ends? Ends { get; set; } = null;

        /// <summary>
        /// 端
        /// </summary>
        public class C_Ends
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
        }

        /// <summary>
        /// 各情報
        /// </summary>
        [JsonPropertyName("properties")]
        public GeoJSONScheme.GeoJSON_JMA_Map.C_Properties_JMA_Map Properties { get; set; } = new();
    }

    /// <summary>
    /// GeoJSONの端のデータ(配列にして保存すること)
    /// </summary>
    public class GeoJsonEnds
    {
        /// <summary>
        /// 名称
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// 更新日(マップデータファイルに記載)
        /// </summary>
        [JsonPropertyName("updateDate")]
        public required string UpdateDate { get; set; }

        /// <summary>
        /// 各データ
        /// </summary>
        [JsonPropertyName("data")]
        public required DataClass[] Data { get; set; }
    }
}
