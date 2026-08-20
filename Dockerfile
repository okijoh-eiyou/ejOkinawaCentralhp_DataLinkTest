# 電文確認Webアプリ（lw_Confirmation_of_received_telegram）のコンテナ定義
# ビルド: docker build -t lw-denbun-check .
# 実行例: docker run -d --name lw-denbun-check --restart unless-stopped -p 80:8080 \
#           -e ConnectionStrings__Default="Host=…;Port=5432;Database=okicenhp-db;Username=…;Password=…" \
#           lw-denbun-check

# --- ステージ1: SDKイメージでビルド（成果物だけ次へ渡す） ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish lw_Confirmation_of_received_telegram/lw_Confirmation_of_received_telegram.csproj \
    -c Release -o /app/publish

# --- ステージ2: 実行専用の軽量イメージ ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# 日時表示・ログを日本時間にする（コンテナ既定はUTC）
ENV TZ=Asia/Tokyo

# .NET 8 の既定リッスンポート（コンテナ内 8080）
EXPOSE 8080

ENTRYPOINT ["dotnet", "lw_Confirmation_of_received_telegram.dll"]
