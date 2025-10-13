# 1C Project (Console + Nginx + SSH)

Локальный проект 1C, который получает заказы из DummyJSON через REST API.  
Nginx используется как reverse proxy, SSH для администрирования.

## Структура
1c/HTTPService.bsl — основной модуль REST
main.bsl — точка входа
utils.bsl — вспомогательные функции
nginx/1c_orders.conf — конфиг Nginx
ssh/ — ключи для SSH

## Запуск
   python3 -m http.server 8080
   sudo nginx -c /path/to/Demo_1C/nginx/1c_orders.conf
   curl http://localhost/api/orders

