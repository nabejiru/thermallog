# Thermal Log

温湿度を計測してグラフ表示するためのプログラム群です。  
計測はraspberry PIとセンサで行い、WebAPIサーバに記録・蓄積します。また、記録した温湿度はWebブラウザで確認することができます。

## ディレクトリ構成

* backend: WebAPIサーバ
* frontend: 記録した温湿度をグラフ表示する画面
* sensor-scripts: 温湿度を計測するスクリプト

## 開発環境

### フロントエンド

* Node.js v20.5.1
* npm 9.8.0
* yarn 1.22.19

### バックエンド

* VisualStudio 2022
* ASP.NET Core 6.0（c#）
* SQL Server 2019 express
* Docker 24.0.6

### センサ機器

* Raspberry Pi zero
    * Linux raspberrypi 6.1.21+
    * Python 3.9.2
* DHT-22（温湿度センサ）
* i2c LCDディスプレイモジュール

## 導入方法

### WebAPIサーバ

1. （リバースプロキシ経由でサブディレクトリに配置したい場合）フロントエンドの.envファイルを環境に合わせて編集します。
2. フロントエンドをビルドし、出力されたファイルをバックエンドの静的コンテンツに設置します。
    ``` bash
    $ cd frontend
    $ yarn install
    $ yarn generate
    $ cp -rf .output/public/* ../backend/ThermalLog/ThermalLog/wwwroot/
    ```
3. 初期データ（CSVファイル）を登録したい場合は、`backend/Resources/initialData`フォルダにファイルを設置しておきます。
4. バックエンドのコンテナを起動します。起動後は http://{docker実行環境のアドレス}:5000 がグラフ画面（フロントエンド）となります。
    ``` bash
    $ cd backend
    $ docker-compose build
    $ docker-compose up -d
    ```

### 温湿度センサ（RaspberryPI）

1. sensor-scriptsフォルダをRaspberryPIの`/home/pi`ディレクトリに配置します。
2. RaspberryPIで下記コマンドを実行します。
    ```
    cd /home/pi/sensor-scripts
    chmod +x setup.sh
    sh setup.sh
    ```
3. `settings.py`ファイルを編集し、`remote_address`をWebAPIサーバのアドレスに変更します。
4. 計測スクリプトが動作するか確認します。
    ```
    python thermo.py
    ```
    上記コマンドを実行後、LCDディスプレイに温湿度・日付が表示され、エラーメッセージが無無ければ概ね正常です。
    
    ディスプレイの日付の後ろにNGと出ている場合はWebAPIサーバへの問合せに失敗しています（温湿度が記録できていない）。この場合、settings.pyやWebAPIサーバ（アドレスや起動しているかなど）を適宜見直してください。
5. 手順4のコマンドをcronで自動起動するようにします。私は5分おきに実行するようにしました。

## DBの初期登録データについて

ディレクトリ`backend/ThermalLog/ThermalLog/Resources/initialData`に温湿度のCSVファイルを配置してサーバを起動すると自動インポートします。（テーブルに登録が無い時だけ）

設置の際、ファイル名は拡張子がcsvであれば何でもOKですが、書式が下表のようになっている必要があります。


| 列名 | データ型 | 説明 |
| --- | --- | --- |
| HostName | 文字列 | 計測したホスト名。"raspberrypi"とします。 |
| MeasuredAt | 日付（yyyy/mm/dd HH:MM:SS.zzz） | 計測日時 |
| Temperature | 小数点数 | 温度 |
| Humidity | 小数点数 | 湿度 |
※カンマ区切り

## ライセンス・免責

ThermalLog は[MIT license](LICENSE.md)で利用できるものとします。

本プログラムは自己責任の元利用ください。もし何らかの損害が発生することがあっても、制作者は一切の責任を負いません。  
個人が勉強用に制作したものなので、細かい所まで詰めて作っていません。（プログラムや導入手順にアラがあるかもしれません。） 
