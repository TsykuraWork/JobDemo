import os
import binascii
from collections import Counter

# Функция для вычисления хэша CRC32 для текста
def calculate_crc32_hash(text):
    crc32_hash = binascii.crc32(text.encode('utf-8')) & 0xFFFFFFFF
    return crc32_hash

# Функция для создания шинглов из текста
def get_shingles(text, k=10):
    shingles = set()
    words = text.split()
    for i in range(len(words) - k + 1):
        shingle = ' '.join(words[i:i + k])
        shingles.add(calculate_crc32_hash(shingle))
    return shingles

def jaccard_similarity(set1, set2):
    intersection = len(set1 & set2)
    union = len(set1 | set2)
    return intersection / union

# Функция для загрузки текстов из указанной папки
def load_text_files(directory):
    texts = []
    for filename in os.listdir(directory):
        if filename.endswith(".txt"):
            with open(os.path.join(directory, filename), 'r', encoding='utf-8') as file:
                text = file.read()
                texts.append(text)
    return texts

texts = []

def main():
    global texts
    print("Лабораторная работа №3")
    print("Вариант <4>, алгоритм хэширования: CRC32, кол-во слов в шинглах: 10, язык: Английский")
    print("1) Загрузить тексты")
    print("2) Проанализировать тексты")

    option = input("Выберите опцию: ")

    if option == "1":
        directory = input("Введите путь к папке с текстовыми файлами: ")
        texts = load_text_files(directory)
        print("Тексты успешно загружены.")
        main()
    elif option == "2":
        if len(texts) < 4:
            print("Необходимо загрузить как минимум 4 текста.")
            main()
        else:
            similarities = []
            for i in range(len(texts)):
                for j in range(i + 1, len(texts)):
                    shingles1 = get_shingles(texts[i])
                    shingles2 = get_shingles(texts[j])
                    similarity = jaccard_similarity(shingles1, shingles2)
                    similarities.append(similarity)

            # Добавляем цикл для сравнения всех текстов между собой
            for i in range(len(texts)):
                for j in range(i + 1, len(texts)):
                    shingles1 = get_shingles(texts[i])
                    shingles2 = get_shingles(texts[j])
                    similarity = jaccard_similarity(shingles1, shingles2)
                    similarities.append(similarity)

            average_similarity = sum(similarities) / len(similarities)
            print(f"Схожесть всех текстов: {average_similarity * 100:.2f}%")
    else:
        print("Некорректный выбор опции. Попробуйте ещё раз.")
        main()

if __name__ == "__main__":
    main()
