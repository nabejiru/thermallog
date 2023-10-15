import requests
import sys
import json

class WebApi:
    conn = None

    def  __init__(self, remote_address) :
        self.remote_address = remote_address

    def store(self, date, hostname, temperature, humidity):
        url = f"//{self.remote_address}/api/store"
        headers = {"Content-Type" : "application/json"}
        post_data = {
            'measuredAt': date,
            'hostName': hostname,
            'temperature': temperature,
            'humidity': humidity
        }
        json_data = json.dumps(post_data).encode("utf-8")

        print("request for "+url)
        print("payload = "+str(post_data))
        response = requests.post(url, headers=headers, data=json_data)
        return response
