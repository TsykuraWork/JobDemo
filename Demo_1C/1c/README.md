# 1C Orders Project (Console)

## Описание
Проект для демонстрации REST-запросов из 1С.  
Подключается к DummyJSON (`https://dummyjson.com/carts`) и формирует читаемый JSON заказов.

## Структура
- 1c/HTTPService.bsl — основной REST-модуль
- utils.bsl — вспомогательные функции
- main.bsl — точка входа
- README.md — инструкции

## Запуск
1. Открыть 1C:Enterprise (консольный режим)
2. Выполнить main.bsl
3. Результат — читаемый JSON с заказами

