import requests

api_headers = {
    'user-agent':'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36',
    'accept': '*/*',
    'x-requested-with': 'XMLHttpRequest'
}

res = requests.get("https://bankiros.ru/currency/get-chat-rate-history-cbrf?block=view&currency_id=1&period=90", headers=api_headers)

for day in res.json():
    print(day)