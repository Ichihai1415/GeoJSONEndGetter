using GeoJSONEndGetter;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;

Console.WriteLine("GeoJSONのパスを入力してください。");
var path = Console.ReadLine() ?? throw new Exception("パスが入力されていません。");
path = path.Replace("\"", "");
if (!File.Exists(path)) throw new FileNotFoundException("ファイルが見つかりません。");
var rawJson = File.ReadAllText(path) ?? throw new Exception("ファイルの読み込みに失敗しました。");
var mapJson = JsonNode.Parse(rawJson) ?? throw new Exception("JSONの変換に失敗しました。");
var json_type = mapJson["type"] ?? throw new Exception("JSONの認識に失敗しました。");
if (json_type.ToString() != "FeatureCollection") throw new Exception("typeがFeatureCollectionではありません。");
var json_features = mapJson["features"] ?? throw new Exception("JSONの処理に失敗しました。", new Exception("featuresが見つかりません。"));
var json_features_valid = json_features.AsArray().Where(f => f != null).Select(f => f!).Where(f => f["geometry"] != null).ToArray();//geometry:nullを除く

var endDic = new List<DataClass>();
var options = new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)/*,
    WriteIndented = true*/
};
foreach (var feature in json_features_valid)
{
    var geoType = feature["geometry"]!["type"]!.ToString();
    double[] lats, lons;
    if (geoType == "Polygon")
    {
        lons = feature["geometry"]!["coordinates"]![0]!.AsArray().Select(x => double.Parse(x![0]!.ToString())).ToArray();
        lats = feature["geometry"]!["coordinates"]![0]!.AsArray().Select(x => double.Parse(x![1]!.ToString())).ToArray();
    }
    else if (geoType == "MultiPolygon")
    {
        lons = feature["geometry"]!["coordinates"]!.AsArray().SelectMany((JsonNode? coordinate) => coordinate![0]!.AsArray()).Select(x => double.Parse(x![0]!.ToString())).ToArray();
        lats = feature["geometry"]!["coordinates"]!.AsArray().SelectMany((JsonNode? coordinate) => coordinate![0]!.AsArray()).Select(x => double.Parse(x![1]!.ToString())).ToArray();
    }
    else
    {
        Console.WriteLine("警告:typeが正しくありません:" + geoType);
        continue;
    }
    if (lats.Length == 0 || lons.Length == 0)
        throw new Exception("JSONの処理に失敗しました。", new Exception("緯度経度リストの取得に失敗しました。"));
    int? code;
    if (int.TryParse(feature["properties"]!["code"]!.ToString(), out int c))
        code = c;
    else
        code = null;
    var end = new DataClass
    {
        Top = lats.Max(),
        Right = lons.Max(),
        Bottom = lats.Min(),
        Left = lons.Min(),
        Properties = new DataClass.Properties_
        {
            Code = code,
            Name = feature["properties"]!["name"]!.ToString(),
            NameKana = feature["properties"]!["namekana"]!.ToString()
        }
    };
    Console.WriteLine(JsonSerializer.Serialize(end, options));
    endDic.Add(end);
    //throw new Exception();
}
Directory.CreateDirectory("output");
var savePath = $"output\\{DateTime.Now:yyyyMMddHHmmss}.json";
File.WriteAllText(savePath, JsonSerializer.Serialize(endDic, options));
Console.WriteLine(savePath + " に保存しました。");