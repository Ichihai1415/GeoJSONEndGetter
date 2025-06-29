using GeoJSONEndGetter;
using Ichihai1415.GeoJSON;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;

internal class Program
{
    //コンソール色
    internal static ConsoleColor c_w = Console.ForegroundColor;
    internal static ConsoleColor c_r = ConsoleColor.Red;
    internal static ConsoleColor c_g = ConsoleColor.Green;
    internal static ConsoleColor c_b = ConsoleColor.Blue;
    internal static ConsoleColor c_y = ConsoleColor.Yellow;
    internal static readonly JsonSerializerOptions options = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)/*,
        WriteIndented = true*/
    };

    private static void Main(string[] args)
    {
    //GetAll();
    //return;
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
            var mapJson = GeoJSONHelper.Deserialize<GeoJSONScheme.GeoJSON_JMA_Map>(rawJson) ?? throw new Exception("JSONの変換に失敗しました。");
            Console.WriteLine("JSON一次解析完了");
            var json_type = mapJson.Type ?? throw new Exception("JSONの認識に失敗しました。");
            if (json_type.ToString() != "FeatureCollection") throw new Exception("typeがFeatureCollectionではありません。");
            var json_features = mapJson.Features ?? throw new Exception("JSONの処理に失敗しました。", new Exception("featuresが見つかりません。"));
            //geometry:nullを除く
            //.Select(f => f!).Where(f => f["geometry"]...を.Where(f => f!["geometry"]にするとJsonNode?[]?となり、後で null可能性警告が出るためこのままで
            //var json_features_valid = json_features.AsArray().Where(f => f != null).Select(f => f!).Where(f => f["geometry"] != null).ToArray();
            Console.WriteLine("JSON選別完了");
            Console.WriteLine("各地物処理に移ります...");
            Console.WriteLine();
            Console.WriteLine();

            //各地物のデータの抽出
            //それぞれ緯度経度の配列に変換し最大最小を求めます
            var endDic = new List<DataClass>();
            foreach (var feature in json_features)
            {
                if (feature.Properties == null) continue;
                var name = feature.Properties.Name;
                Console.WriteLine("name=" + name);
                var geoType = feature.Geometry?.Type;
                float[] lats, lons;
                if (feature.Geometry == null)
                {
                    var end_null = new DataClass { Properties = feature.Properties };
                    endDic.Add(end_null);
                    Console.WriteLine("  no geometry.");
                    Console.WriteLine();
                    continue;
                }
                else
                    switch (geoType)
                    {
                        case "Polygon":
                        case "LineString":
                            lats = [.. feature.Geometry.Coordinates.Objects[0].MainPoints.Select(x => x.Lat)];
                            lons = [.. feature.Geometry.Coordinates.Objects[0].MainPoints.Select(x => x.Lon)];
                            break;
                        case "MultiPolygon":
                        case "MultiLineString":
                            lats = [.. feature.Geometry.Coordinates.Objects.SelectMany(c => c.MainPoints.Select(x => x.Lat))];
                            lons = [.. feature.Geometry.Coordinates.Objects.SelectMany(c => c.MainPoints.Select(x => x.Lon))];
                            break;
                        default:
                            Console.ForegroundColor = c_y;
                            Console.WriteLine("警告:typeが正しくありません:" + geoType);
                            Console.WriteLine();
                            continue;

                    }
                if (lats.Length == 0 || lons.Length == 0)
                    throw new Exception("JSONの処理に失敗しました。", new Exception("緯度経度リストの取得に失敗しました。"));
                var end = new DataClass
                {
                    Ends = new DataClass.C_Ends
                    {
                        Top = lats.Max(),
                        Right = lons.Max(),
                        Bottom = lats.Min(),
                        Left = lons.Min()
                    },
                    Properties = feature.Properties
                };
                Console.WriteLine("  top=" + end.Ends.Top + ", right=" + end.Ends.Right + ", bottom=" + end.Ends.Bottom + ", left=" + end.Ends.Left);
                //Console.WriteLine(JsonSerializer.Serialize(end, options));
                endDic.Add(end);
                //throw new Exception();
                Console.WriteLine();
            }
            Directory.CreateDirectory("output");
            var savePath = "output\\" + Path.GetFileName(path).Replace(".geo", ".");
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
    }

    private static void GetAll()
    {
        var dict = new List<GeoJsonEnds>();
        foreach (var path in Directory.GetFiles("C:\\Ichihai1415\\source\\vs\\JmaXmlViewer\\JmaXmlViewer\\bin\\x64\\Debug\\net9.0-windows\\Resources\\MapData", "*_1.geojson"))
        {
            Console.WriteLine(Path.GetFileName(path));
            var rawJson = File.ReadAllText(path);
            var mapJson = GeoJSONHelper.Deserialize<GeoJSONScheme.GeoJSON_JMA_Map>(rawJson)!;
            var json_type = mapJson!.Type ?? throw new Exception("JSONの認識に失敗しました。");
            if (json_type.ToString() != "FeatureCollection") throw new Exception("typeがFeatureCollectionではありません。");
            var json_features = mapJson.Features ?? throw new Exception("JSONの処理に失敗しました。", new Exception("featuresが見つかりません。"));
            var endDic = new List<DataClass>();
            foreach (var feature in json_features)
            {
                if (feature.Properties == null) continue;
                var name = feature.Properties.Name;
                var geoType = feature.Geometry?.Type;
                float[] lats, lons;
                if (feature.Geometry == null)
                {
                    var end_null = new DataClass { Properties = feature.Properties };
                    endDic.Add(end_null);
                    continue;
                }
                else
                    switch (geoType)
                    {
                        case "Polygon":
                        case "LineString":
                            lats = [.. feature.Geometry.Coordinates.Objects[0].MainPoints.Select(x => x.Lat)];
                            lons = [.. feature.Geometry.Coordinates.Objects[0].MainPoints.Select(x => x.Lon)];
                            break;
                        case "MultiPolygon":
                        case "MultiLineString":
                            lats = [.. feature.Geometry.Coordinates.Objects.SelectMany(c => c.MainPoints.Select(x => x.Lat))];
                            lons = [.. feature.Geometry.Coordinates.Objects.SelectMany(c => c.MainPoints.Select(x => x.Lon))];
                            break;
                        default:
                            Console.ForegroundColor = c_y;
                            Console.WriteLine("警告:typeが正しくありません:" + geoType);
                            continue;

                    }
                if (lats.Length == 0 || lons.Length == 0)
                    throw new Exception("JSONの処理に失敗しました。", new Exception("緯度経度リストの取得に失敗しました。"));
                var end = new DataClass
                {
                    Ends = new DataClass.C_Ends
                    {
                        Top = lats.Max(),
                        Right = lons.Max(),
                        Bottom = lats.Min(),
                        Left = lons.Min()
                    },
                    Properties = feature.Properties
                };
                //Console.WriteLine(JsonSerializer.Serialize(end, options));
                endDic.Add(end);
                //throw new Exception();
            }
            var p = Path.GetFileName(path).Split('_');
            dict.Add(new GeoJsonEnds { Name = Path.GetFileName(path).Replace("_GIS", "*").Split('*')[0], UpdateDate = p[^2], Data = [.. endDic] });
        }
        Directory.CreateDirectory("output");
        File.WriteAllText("output\\GeoJSONEnds.json", JsonSerializer.Serialize(dict, options));
    }
}