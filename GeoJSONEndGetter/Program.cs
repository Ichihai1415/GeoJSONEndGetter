using GeoJSONEndGetter;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;

//コンソール色
var c_w = Console.ForegroundColor;
var c_r = ConsoleColor.Red;
var c_g = ConsoleColor.Green;
var c_b = ConsoleColor.Blue;
var c_y = ConsoleColor.Yellow;
//日本語のまま出力するためのオプション
//一回しか使わない時CA1869が出る
#pragma warning disable CA1869 // 'JsonSerializerOptions' インスタンスをキャッシュして再利用する
var options = new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)/*,
    WriteIndented = true*///インデントするか
};
#pragma warning restore CA1869 // 'JsonSerializerOptions' インスタンスをキャッシュして再利用する

restart:
try
{
    //メイン
    Console.ForegroundColor = c_w;
    Console.WriteLine("GeoJSONのパスを入力してください。");
    Console.ForegroundColor = c_b;
    var path = Console.ReadLine() ?? throw new Exception("パスが入力されていません。");
    path = path.Replace("\"", "");
    if (!File.Exists(path)) throw new FileNotFoundException("ファイルが見つかりません。");
    var rawJson = File.ReadAllText(path) ?? throw new Exception("ファイルの読み込みに失敗しました。");
    Console.ForegroundColor = c_g;
    Console.WriteLine("ファイル読み込み完了");
    var mapJson = JsonNode.Parse(rawJson) ?? throw new Exception("JSONの変換に失敗しました。");
    Console.WriteLine("JSON一次解析完了");
    var json_type = mapJson["type"] ?? throw new Exception("JSONの認識に失敗しました。");
    if (json_type.ToString() != "FeatureCollection") throw new Exception("typeがFeatureCollectionではありません。");
    var json_features = mapJson["features"] ?? throw new Exception("JSONの処理に失敗しました。", new Exception("featuresが見つかりません。"));
    //geometry:nullを除く
    //.Select(f => f!).Where(f => f["geometry"]...を.Where(f => f!["geometry"]にするとJsonNode?[]?となり、後で null可能性警告が出るためこのままで
    var json_features_valid = json_features.AsArray().Where(f => f != null).Select(f => f!).Where(f => f["geometry"] != null).ToArray();
    Console.WriteLine("JSON選別完了");
    Console.WriteLine("各地物処理に移ります...");
    Console.WriteLine();
    Console.WriteLine();

    //各地物のデータの抽出
    //それぞれ緯度経度の配列に変換し最大最小を求めます
    var endDic = new List<DataClass>();
    foreach (var feature in json_features_valid)
    {
        var name = feature["properties"]!["name"]!.ToString();
        Console.WriteLine("name=" + name);
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
            Console.ForegroundColor = c_y;
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
                Name = name,
                NameKana = feature["properties"]!["namekana"]!.ToString()
            }
        };
        Console.WriteLine("  top:" + end.Top + ", right=" + end.Right + ", bottom" + end.Bottom + ", left" + end.Left);
        //Console.WriteLine(JsonSerializer.Serialize(end, options));
        endDic.Add(end);
        //throw new Exception();
        Console.WriteLine();
    }
    Directory.CreateDirectory("output");
    var savePath = $"output\\{DateTime.Now:yyyyMMddHHmmss}.json";
    File.WriteAllText(savePath, JsonSerializer.Serialize(endDic, options));
    Console.WriteLine(Path.GetFullPath(savePath) + " に保存しました。");



}
catch (Exception ex)
{
    Console.ForegroundColor = c_r;
    Console.WriteLine(ex);
}

Console.ForegroundColor = c_w;
Console.WriteLine();
Console.WriteLine("さらに実行する場合Yキーを押してください。他のキーを押すと終了します。");
if (Console.ReadKey().Key.ToString() == "Y")
{
    Console.Clear();
    GC.Collect();
    goto restart;
}
