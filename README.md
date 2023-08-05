# TajimiNow

気象情報をMisskeyに投げ続けるBot

## デモ
* [多治見](https://submarin.online/@tajimi_now)
* [稚内](https://submarin.online/@wakkanai_now)

## 使い方 (Docker)

### 前提条件

* curl または wget
* Docker Engine 20.10.13以降

### インストール

```sh
mkdir TajimiNow
cd TajimiNow

curl -o compose.yaml https://raw.githubusercontent.com/suila-ai/TajimiNow/main/compose_example.yaml
# wget -O compose.yaml https://raw.githubusercontent.com/suila-ai/TajimiNow/main/compose_example.yaml

# 環境変数を設定
vim compose.yaml

docker compose pull
```

### 起動

```sh
docker compose up -d
```

### 更新

```sh
docker compose down

# 必要に応じて環境変数を編集
vim compose.yaml

docker compose pull
```

## ライセンス
[MIT License](https://github.com/suila-ai/TajimiNow/blob/main/LICENSE.txt)