import os
import Adafruit_DHT
import datetime
import display
import strage
import web_api
import settings
from logging import Formatter, getLogger, INFO
from logging.handlers import RotatingFileHandler
 
logger = getLogger()
 
def init_logging():
    """ ログのローテーション設定 """
    global logger
 
    # ローテーションのタイミングを100キロバイト
    max_bytes = 100 * 1024
    # 保持する旧ファイル数
    backup_count = 4
    handler = RotatingFileHandler(
        settings.log_dir, 
        maxBytes=max_bytes,
        backupCount=backup_count,
        encoding='utf-8')
 
    formatter = Formatter('{asctime} {name:<8s} {levelname:<8s} {message}', style='{')
    handler.setFormatter(formatter)
    logger.addHandler(handler)
 
    logger.setLevel(INFO)


def main():
    now = datetime.datetime.now()

    # 温湿度センサーから計測値を取得
    sensor = Adafruit_DHT.DHT22
    humidity, temperature = Adafruit_DHT.read_retry(sensor, settings.sensor_gpio)

    hostname = os.uname()[1]

    # リモートDBに登録
    webApi = web_api.WebApi(settings.remote_address)

    result_code = 0
    try:
        api_result = webApi.store(now.isoformat(timespec='milliseconds')+ "Z", hostname, temperature, humidity)
        if request.status_code >= 400:
            logger.warning("failed to send temprature data : status=%s", request.status_code)
        result_code = api_result.status_code
    except Exception as e:
        logger.warning("failed to send temprature data.  \n%s", e)

    # ローカルDBに登録
    stg = strage.Strage(settings.db_path)
    stg.store(now, hostname, temperature, humidity, result_code)
    stg.close()

    # LCDディスプレイに温湿度を表示
    status = "OK"
    if result_code==0 or result_code>=400:
        status = "NG"

    disp = display.Display()
    disp.put(now.strftime('%Y/%m/%d %H:%M')+ " "+status, 0)

    thermoText = "気温:{0:.1f}°C 湿度:{1:.1f}%".format(temperature, humidity)
    disp.put(thermoText, 1)
    disp.show()



if __name__ == '__main__':
    init_logging()
    main()
