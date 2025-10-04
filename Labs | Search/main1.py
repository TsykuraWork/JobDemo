import os
import binascii
import random
import matplotlib.pyplot as plt
from collections import Counter
from difflib import SequenceMatcher

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

# Функция для оценки качества алгоритма на разных количествах документов
def evaluate_quality(texts, num_documents_list):
    precision_results = []
    recall_results = []
    fallout_results = []
    f1_score_results = []

    for num_documents in num_documents_list:
        if num_documents > len(texts):
            print(f"Недостаточно текстовых документов для оценки с {num_documents} документами.")
            continue

        # Перемешайте тексты случайным образом
        random.shuffle(texts)

        relevant_documents = texts[:num_documents]  # Берем первые num_documents текстов как релевантные
        retrieved_documents = texts[num_documents:num_documents * 2]  # Берем следующие num_documents текстов как извлекаемые

        true_positive = len(set(relevant_documents) & set(retrieved_documents))
        false_positive = len(set(retrieved_documents) - set(relevant_documents))
        false_negative = len(set(relevant_documents) - set(retrieved_documents))

        precision = true_positive / (true_positive + false_positive) if (true_positive + false_positive) != 0 else 0
        recall = true_positive / (true_positive + false_negative) if (true_positive + false_negative) != 0 else 0
        fallout = false_positive / (false_positive + len(texts) - 2 * num_documents) if (false_positive + len(texts) - 2 * num_documents) != 0 else 0
        f1_score = 2 * precision * recall / (precision + recall) if (precision + recall) != 0 else 0

        precision_results.append(precision)
        recall_results.append(recall)
        fallout_results.append(fallout)
        f1_score_results.append(f1_score)

    return precision_results, recall_results, fallout_results, f1_score_results


def main():
    texts = []  # Переменная для хранения загруженных текстов

    print("Лабораторная работа №4")
    print("Вариант <4>, алгоритм хэширования: CRC32, кол-во слов в шинглах: 10, язык: Английский")
    print("1) Загрузить тексты")
    print("2) Проанализировать тексты")

    while True:
        option = input("Выберите опцию: ")

        if option == "1":
            directory = input("Введите путь к папке с текстовыми файлами: ")
            texts = load_text_files(directory)
            print("Тексты успешно загружены.")
        elif option == "2":
            if len(texts) < 4:
                print("Необходимо загрузить как минимум 4 текста.")
            else:
                num_documents_list = [2, 4, 6, 8, 10]
                precision_results, recall_results, fallout_results, f1_score_results = evaluate_quality(texts, num_documents_list)

                for i, num_documents in enumerate(num_documents_list):
                    print(f"Результаты для {num_documents} документов:")
                    print(f"Точность (Precision): {precision_results[i]}")
                    print(f"Полнота (Recall): {recall_results[i]}")
                    print(f"Выпадение (Fallout): {fallout_results[i]}")
                    print(f"F1-мера: {f1_score_results[i]}")

                    # Строим графики для каждого количества документов
                    plt.figure()
                    plt.plot(num_documents_list, f1_score_results, marker='o', label='F1-мера')
                    plt.plot(num_documents_list, precision_results, marker='x', label='Точность (Precision)')
                    plt.plot(num_documents_list, recall_results, marker='s', label='Полнота (Recall)')
                    plt.plot(num_documents_list, fallout_results, marker='^', label='Выпадение (Fallout)')
                    plt.xlabel('Количество документов')
                    plt.ylabel('Значение')
                    plt.title(f'Оценка качества алгоритма (Документы: {num_documents})')
                    plt.legend()
                    plt.grid(True)
                    plt.show()
        else:
            print("Некорректный выбор опции. Попробуйте ещё раз.")

if __name__ == "__main__":
    main()
