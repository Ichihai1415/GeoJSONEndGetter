# GeoJSONEndGetter
GeoJSONデータから地物の端を求めます。

`sample`に例があります(作り次第追加します)。


## 注意
- 気象庁GISデータ・FeatureCollectionデータでの使用を前提としていますが、コードを修正することで対応することができるはずです。
- .NET8です。各ランタイム等が必要です。

## 使い方
- cloneしてビルドしてください。
- ファイルパスを入力すれば端が計算され`output`フォルダに出力されます。形式はJSONです。

### JSON出力例
実際はインデントされていません。`DataClass.cs`の`DataClass`の配列です。
```JSON:出力内容例
[
  {
    "top": 35.35223,
    "right": 135.79453,
    "bottom": 30.4513,
    "left": 131.312,
    "properties":
    {
      "code": 4364,
      "name":"名称",
      "namekana":"めいしょう"
    }
  }
]
```

## 更新履歴
### v1.0.1
2024/08/18
- コンソール進捗出力を追加
- 地震情報／細分区域のサンプルを追加

### v1.0.0
2024/08/17
- とりあえず仮
