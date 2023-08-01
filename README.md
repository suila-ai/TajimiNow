# TajimiNow

気象情報をMisskeyに投げ続けるBot

## デモ
* [多治見](https://submarin.online/@tajimi_now)
* [稚内](https://submarin.online/@wakkanai_now)

## 使い方 (Docker)

### 前提条件

* Docker Engine 20.10.13以降

### インストール

```sh
git clone https://github.com/suila-ai/TajimiNow.git
cd TajimiNow

# 環境変数を設定
cp compose_example.yaml compose.yaml
vim compose.yaml

docker compose build
```

### 起動

```sh
docker compose up -d
```

### 更新

```sh
docker compose down
git pull --ff-only

# 必要に応じて環境変数を編集
vim compose.yaml

docker compose build
```

## ライセンス
[MIT License](https://github.com/suila-ai/TajimiNow/blob/main/LICENSE.txt)