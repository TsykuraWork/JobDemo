#include <windows.h> // заголовочный файл, содержащий функции API
#include <string>
#include <atlconv.h>
#include <sstream>
#include <time.h>
#define N 8
using namespace std;
// Прототип функции обработки сообщений с пользовательским названием:
LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);
BOOL RegClass(WNDPROC, LPCTSTR, UINT);//Регистрация класса
HINSTANCE main_hInst;// создаём дескриптор основного окошка
HWND hwnd;//первое окно
HWND hList1, hList2, hwndStatic;
float width = 500;
float height = 500;
HMENU hMainMenu;
float curX, curY;
HDC hdc,hCompatibleDC;
HANDLE thread;
string path = "D:\\Lannica's things\\Desktop\\ДонНТУ\\7 семестр (IV курс)\\Лабы + отчеты\\СП\\laba 6\\x64\\Debugx64\\Debug\\scrn.bmp";
char* str = new char[1024];
HANDLE hMailSlot, hEvent, hEvent2;
BOOL ret;
DWORD dwMes, dwMsgNum;
DWORD dwRes, dwSize;


static HANDLE hbitmap = 0;
HANDLE hOldBitmap;
BITMAP Bitmap;

OPENFILENAME openFileName;
CHAR fileName[255];
HANDLE hFile, hFileNew;


int matrica[N][N];
int fromServer[N][N];
static HANDLE hMap = 0;
int nArSize, i, j;
int* pAr;

TCHAR szClassName[] = "WinAppClass"; // строка с именем класса
// Основная функция - аналог int main() в консольном приложении:
int WINAPI WinMain(HINSTANCE hInst, //дескриптор экземпляра приложения. 
									//Этот дескриптор содержит адрес начала кода программы 
									//в ее адресном пространстве. //
									//Дескриптор hInstance чаще всего требуется функциям, 
									//работающим с ресурсами программы.
	HINSTANCE hPrevInst, //дескриптор предыдущего экземпляра приложения. 
						 //Этот дескриптор остался от старых версий Windows,
						 //он никогда не пригодится.
	LPSTR lpCmdLine, //указатель на начало командной строки, 
					 //введенной при запуске программы.
	int nCmdShow) // это значение содержит желаемый вид окна 
				  //(например, свернутый или развернутый)
{
	MSG msg; // создём экземпляр структуры MSG для обработки сообщений
	main_hInst = hInst;

	if (!RegClass(WndProc, szClassName, COLOR_WINDOW)) {
		MessageBox(NULL, "Не получилось зарегистрировать класс!", "Ошибка", MB_OK);
		return NULL; // возвращаем, следовательно, выходим из WinMain
	}
	srand(time(NULL));
	for (int i = 0; i < 8; i++) {
		for (int j = 0; j < 8; j++) {
			matrica[i][j] = -9 + rand() % (9 - (-9) + 1);
		}
	}

	// Функция, создающая окошко:
	hwnd = CreateWindowEx(
		0,//Расширенные возможности для вариации
		szClassName, // имя класса
		"Клиент_ЛР6_СистПрогр_Фролов_ПИ18в", // имя окошка (то что сверху)
		WS_OVERLAPPEDWINDOW, // режимы отображения окошка
		100, // позиция окошка по оси х
		100, // позиция окошка по оси у (раз дефолт в х, то писать не нужно)
		width, // ширина окошка
		height, // высота окошка (раз дефолт в ширине, то писать не нужно)
		NULL, // дескриптор родительского окна
		NULL, // дескриптор меню
		hInst, // дескриптор экземпляра приложения
		NULL); // ничего не передаём из WndProc

	if (!hwnd) {
		// в случае некорректного создания окошка (неверные параметры и тп):
		MessageBox(NULL, "Не получилось создать окно!", "Ошибка", MB_OK);
		return NULL;
	}

	ShowWindow(hwnd, nCmdShow); // отображаем окошко
	UpdateWindow(hwnd); // обновляем окошко
	// извлекаем сообщения из очереди, посылаемые фу-циями, ОС
	while (GetMessage(&msg, //указатель на структуру сообщения, 
							//в которую GetMessage вернет результат
		NULL, //описатель окна, от которого GetMessage примет сообщение 
			  //(NULL означает, что GetMessage принимает сообщения от всех окон, 
			  //принадлежащих потоку)
		NULL, //наименьший идентификатор сообщения, которое примет GetMessage
		NULL)) { //наибольший идентификатор сообщения, которое примет GetMessage

		TranslateMessage(&msg); // интерпретируем сообщения
		DispatchMessage(&msg); // передаём сообщения обратно ОС
	}
	//return 0; // возвращаем код выхода из приложения
	return msg.wParam; // возвращаем код выхода из приложения
}

//Описание функции регистрации классов
BOOL RegClass(WNDPROC Proc, LPCTSTR szName, UINT brBackground) {
	WNDCLASSEX wc; // создаём экземпляр, для обращения к членам класса WNDCLASS
	wc.style = CS_HREDRAW | CS_VREDRAW; // стиль класса окошка
	wc.lpfnWndProc = Proc; // указатель на пользовательскую функцию
	wc.hInstance = main_hInst; // указатель на строку, содержащую имя меню, применяемого для класса
	wc.lpszClassName = szName; // указатель на имя класса
	wc.cbSize = sizeof(WNDCLASSEX); // размер структуры (в байтах)

	wc.hIcon = LoadIcon(NULL, IDI_APPLICATION); // декриптор пиктограммы
	wc.hIconSm = LoadIcon(NULL, IDI_APPLICATION); // дескриптор маленькой пиктограммы (в трэе)
	wc.hCursor = LoadCursor(NULL, IDC_ARROW); // дескриптор курсора
	wc.lpszMenuName = NULL; // указатель на имя меню (у нас его нет)
	wc.cbWndExtra = NULL; // число освобождаемых байтов в конце структуры
	wc.cbClsExtra = NULL; // число освобождаемых байтов при создании экземпляра приложения
	wc.hbrBackground = CreateSolidBrush(RGB(240, 240, 240));//(HBRUSH)(brBackground + 1); // дескриптор кисти для закраски фона окна
	return (RegisterClassEx(&wc) != 0);
}

void AddMenus(HWND hWnd) {
	hMainMenu = CreateMenu();

	HMENU h1Menu = CreateMenu();
	AppendMenu(h1Menu, MF_STRING, 1, "Запуск");

	AppendMenu(hMainMenu, MF_POPUP, (UINT_PTR)h1Menu, "Сервер");

	SetMenu(hWnd, hMainMenu); DrawMenuBar(hWnd);
}

void toList(int matr[8][8], HWND h) {
	SendMessage(h, LB_RESETCONTENT, 0, 0);
	string s[8];
	for (int i = 0; i < 8; i++) {
		stringstream ss;
		ss << " {";
		for (int j = 0; j < 8; j++) {
			if (matr[i][j] >= 0) {
				ss << " ";
			}
			ss << matr[i][j];
			if (j != 7) {
				ss << ", ";
			}
			else {
				ss << "}";
			}
		}
		s[i] = ss.str();
		SendMessage(h, LB_ADDSTRING, 0, (LPARAM)(s[i].c_str()));
	}
}

DWORD WINAPI myMailThread(LPVOID t) {
	
	hEvent = CreateEvent(NULL, FALSE, FALSE, "Event1");
	hEvent2 = CreateEvent(NULL, FALSE, FALSE, "Event2");

	hMailSlot = CreateMailslot("\\\\.\\mailslot\\MyMailSlot", 0, MAILSLOT_WAIT_FOREVER, NULL);
	if (hMailSlot == INVALID_HANDLE_VALUE) {
		MessageBox(NULL, "Почтовый ящик в сервере не создан!", "Ошибка!!!", 0);
		return 0;
	}

	SetEvent(hEvent);
	dwRes = WaitForSingleObject(hEvent2, 50000);
	if (dwRes == WAIT_TIMEOUT) {
		MessageBox(NULL, "Время истекло", "", 0);
		return 0;
	}
	ret = GetMailslotInfo(hMailSlot, NULL, &dwMes, &dwMsgNum, NULL);

	if (ret == FALSE) {
		MessageBox(NULL, "Ошибка при работе с почтовым ящиком", "", 0);
		return 0;
	}

	if (dwMsgNum == 0) {
		MessageBox(NULL, "Сообщений нет", "", 0);
		return 0;
	}

	ret = ReadFile(hMailSlot, fromServer, 256, &dwSize, NULL);

	if (ret == FALSE)
		MessageBox(NULL, "Чтение не прошло", "", 0);
	else {
		toList(fromServer,hList2);
		MessageBox(NULL, "Матрица вернулась с сервера", "Уведомление", 0);
	}

	CloseHandle(hMailSlot);
	return 0;
}

LRESULT CALLBACK WndProc(HWND hWnd,//описатель окна, от которого пришло сообщение.
	UINT uMsg,//идентификатор сообщения
	WPARAM wParam, LPARAM lParam) {//параметры сообщения

	PAINTSTRUCT ps;
	RECT Rect;
	// выборка и обработка сообщений
	switch (uMsg)
	{
	case WM_SIZE:
		curX = LOWORD(lParam); curY = HIWORD(lParam);
		break;
	case WM_CREATE:
		AddMenus(hWnd);


		thread = CreateThread(NULL, 0, myMailThread, hWnd, 0, NULL);

		hList1 = CreateWindow(
			"listbox",
			"1 матрица",
			WS_CHILD | WS_VISIBLE | WS_BORDER,
			10,
			10,
			175,
			135,
			hWnd,
			(HMENU)10,
			main_hInst,
			NULL);

		hList2 = CreateWindow(
			"listbox",
			"2 матрица",
			WS_CHILD | WS_VISIBLE | WS_BORDER,
			195,
			10,
			175,
			135,
			hWnd,
			(HMENU)10,
			main_hInst,
			NULL);

		toList(matrica, hList1);


		//hbitmap = LoadBitmap(main_hInst, path.c_str());
		hbitmap = LoadImage(NULL, path.c_str(), IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE);
		OpenClipboard(hWnd);
		SetClipboardData(CF_BITMAP, hbitmap);
		CloseClipboard();


		break;
	case WM_COMMAND:
		switch (LOWORD(wParam))
		{
		case 1:
			WCHAR cBuffer[255];
			openFileName.lStructSize = sizeof(openFileName);
			openFileName.hwndOwner = hWnd;
			openFileName.hInstance = main_hInst;
			openFileName.lpstrFilter = "Исполняемые файлы (*.exe)\0*.exe\0";
			openFileName.lpstrCustomFilter = NULL;
			openFileName.nFilterIndex = 0;
			openFileName.lpstrFile = fileName;
			openFileName.nMaxFile = sizeof(fileName);
			openFileName.lpstrFileTitle = NULL;
			openFileName.lpstrInitialDir = NULL;

			openFileName.lpstrFile[0] = '\0';
			//openFileName.nMaxFileTitle = 0;
			openFileName.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST;

			if (GetOpenFileName(&openFileName))
			{
				PROCESS_INFORMATION pi;
				STARTUPINFO si;
				ZeroMemory(&si, sizeof(si));
				si.cb = sizeof(si);
				ZeroMemory(&pi, sizeof(pi));
				CHAR path[1024];
				wsprintf(path, openFileName.lpstrFile);

				MessageBox(hWnd, "Сервер запустился.", "Уведомление.", MB_OK);

				DWORD test = CreateProcess(NULL, path, NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi);
			}
			//TODO запуск сервера


			nArSize = N * N * sizeof(int);
			hMap = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, nArSize, "MyMap");
			if (hMap == 0) {
				MessageBox(NULL, "Передача невозможна", NULL, 0);
			}
			else {
				pAr = (int*)MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, nArSize);

				if (pAr) {
					for (int i = 0; i < N; i++) {
						for (int j = 0; j < N; j++) {
							pAr[i*N+j] = matrica[i][j];
						}
					}
				}
			}

			break;
		}
		break;
	case WM_LBUTTONDOWN:
		//InvalidateRect(hWnd, NULL, TRUE);
		break;
	case WM_PAINT:
		hdc = BeginPaint(hWnd, &ps);
		
		GetObject(hbitmap, sizeof(BITMAP), &Bitmap);
		hCompatibleDC = CreateCompatibleDC(hdc);
		hOldBitmap=SelectObject(hCompatibleDC, hbitmap);
		GetClientRect(hWnd, &Rect);
		StretchBlt(hdc, 0, 0, Rect.right, Rect.bottom, hCompatibleDC, 0, 0, Bitmap.bmWidth, Bitmap.bmHeight, SRCCOPY);
		SelectObject(hCompatibleDC, hOldBitmap);
		DeleteObject(hbitmap);
		DeleteDC(hCompatibleDC);
		EndPaint(hWnd, &ps); 

		break;
	case WM_DESTROY:
		UnmapViewOfFile(pAr);
		CloseHandle(hMap);
		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hWnd, uMsg, wParam, lParam);
	}
	return 0;
}