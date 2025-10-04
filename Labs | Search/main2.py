import spacy


nlp = spacy.load("en")

# Функция для загрузки текста из файла
def load_text_from_file(file_path):
    try:
        with open(file_path, 'r', encoding='utf-8') as file:
            text = file.read()
            return text
    except FileNotFoundError:
        return None

# Функция для стемминга текста
def stem_text(text):
    doc = nlp(text)
    stemmed_text = " ".join([token.lemma_ for token in doc])
    return stemmed_text

# Главное меню
while True:
    print("Лабораторная работа №2")
    print("(<4> Стемминг алгоритмический , англ язык")
    print("1) Загрузить файл")
    print("Путь C:\\Users\\eldor\\OneDrive\\Desktop\\очередь 2\\боднар\\ConsoleApp4\\bin\\Debug\\TextFiles\\document2.txt")
    print("2) Проанализировать файл")
    print("0) Выход")

    choice = input("Выберите действие: ")

    if choice == '1':
        file_path = input("Введите путь к файлу: ")
        file_content = load_text_from_file(file_path)
        if file_content:
            show_file = input("Показать содержимое файла? (0 - да, 1 - нет): ")
            if show_file == '0':
                print("Содержимое файла:")
                print(file_content)
        else:
            print("Файл не найден.")
    elif choice == '2':
        if 'file_content' not in locals():
            print("Сначала загрузите файл (пункт 1).")
        else:
            stemmed_content = stem_text(file_content)
            print("Проанализированный текст:")
            print(stemmed_content)
    elif choice == '0':
        print("Выход из программы.")
        break
    else:
        print("Неверный выбор. Пожалуйста, выберите один из вариантов из меню.")
