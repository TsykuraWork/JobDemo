#include <windows.h> // заголовочный файл, содержащий функции API
#include <string>
#include <atlconv.h>
#include <tchar.h>
#include <iostream>
#include <clocale>
#include <locale>
#include <vector>
using namespace std;
// Прототип функции обработки сообщений с пользовательским названием:
LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);
BOOL RegClass(WNDPROC, LPCTSTR, UINT);//Регистрация класса

HINSTANCE main_hInst;// создаём дескриптор основного окошка
HWND hwnd;//первое окно
float width = 500;
float height = 500;
HMENU hMainMenu;
float curX, curY;
HDC hdc;

HWND hwndStatic;
HWND hwndEdit;
BOOL sectors, fileSize;
DWORD dwordSectorsPerCluster, dwordBytesPerSector, dwordNumberOfFreeClusters, dwordTotalNumberOfClusters; //все то, что надо выяснить
OPENFILENAME openFileName;
wstring str, virtualInfo;
HANDLE hFile,hFileNew;
MEMORYSTATUS memorystatus;
SYSTEM_INFO sinfo;

STARTUPINFO startupinfo;
PROCESS_INFORMATION processinformation;
string res;

TCHAR szClassName[] = L"WinAppClass"; // строка с именем класса
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
		MessageBox(NULL, L"Не получилось зарегистрировать класс!", L"Ошибка", MB_OK);
		return NULL; // возвращаем, следовательно, выходим из WinMain
	}

	// Функция, создающая окошко:
	hwnd = CreateWindowEx(
		0,
		szClassName,
		L"ЛР7_СистПрогр_Фролов_ПИ18в", 
		WS_OVERLAPPEDWINDOW, 
		100, 
		100, 
		width, 
		height, 
		NULL, 
		NULL, 
		hInst, 
		NULL); 

	if (!hwnd) {
		MessageBox(NULL, L"Не получилось создать окно!", L"Ошибка", MB_OK);
		return NULL;
	}
	ShowWindow(hwnd, nCmdShow); 
	UpdateWindow(hwnd); 
	// извлекаем сообщения из очереди, посылаемые фу-циями, ОС
	while (GetMessage(&msg, 
		NULL, //(NULL означает, что GetMessage принимает сообщения от всех окон, принадлежащих потоку)
		NULL, //наименьший идентификатор сообщения, которое примет GetMessage
		NULL)) { //наибольший идентификатор сообщения, которое примет GetMessage

		TranslateMessage(&msg); // интерпретируем сообщения
		DispatchMessage(&msg); // передаём сообщения ОС
	}
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

	wc.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	wc.hIconSm = LoadIcon(NULL, IDI_APPLICATION); 
	wc.hCursor = LoadCursor(NULL, IDC_ARROW); 
	wc.lpszMenuName = NULL; 
	wc.cbWndExtra = NULL; 
	wc.cbClsExtra = NULL; 
	wc.hbrBackground = (HBRUSH)(brBackground + 1); 
	return (RegisterClassEx(&wc) != 0);
}

void AddMenus(HWND hWnd) {
	hMainMenu = CreateMenu();

	HMENU h1Menu = CreateMenu();
	AppendMenu(h1Menu, MF_STRING, 1, L"Открыть");
	AppendMenu(h1Menu, MF_STRING, 2, L"Сохранить");

	AppendMenu(hMainMenu, MF_POPUP, (UINT_PTR)h1Menu, L"Файл");

	SetMenu(hWnd, hMainMenu); DrawMenuBar(hWnd);
}

LRESULT CALLBACK WndProc(HWND hWnd,//описатель окна, от которого пришло сообщение.
	UINT uMsg,//идентификатор сообщения
	WPARAM wParam, LPARAM lParam) {//параметры сообщения
	// выборка и обработка сообщений
	WCHAR fileName[255];
	switch (uMsg)
	{
	case WM_SIZE:
		curX = LOWORD(lParam); curY = HIWORD(lParam);
		break;
	case WM_CREATE:
		AddMenus(hWnd);

		hwndStatic = CreateWindow(
			L"static",
			L"Количество секторов в одном кластере:",
			WS_CHILD | WS_VISIBLE,
			20,
			10,
			200,
			30,
			hWnd,
			(HMENU)110,
			main_hInst,
			NULL);

		sectors = GetDiskFreeSpace(L"C:\\", &dwordSectorsPerCluster, &dwordBytesPerSector, &dwordNumberOfFreeClusters, &dwordTotalNumberOfClusters);

		if (sectors)
		{
			hwndEdit = CreateWindow(L"EDIT",
				L"",
				WS_BORDER | WS_CHILD | WS_VISIBLE | WS_DISABLED,
				20,
				50,
				20,
				20,
				hWnd,
				(HMENU)111,
				main_hInst,
				NULL);

			WCHAR buffer[2];
			wsprintf(buffer, L"%d", dwordSectorsPerCluster);
			SetWindowText(hwndEdit, buffer);
		}
		else
		{
			hwndEdit = CreateWindow(L"EDIT",
				L"Невозможно вычислить количество секторов в одном кластере!",
				WS_BORDER | WS_CHILD | WS_VISIBLE | WS_DISABLED,
				20,
				50,
				20,
				20,
				hWnd,
				(HMENU)112,
				main_hInst,
				NULL);
		}
		break;
	case WM_COMMAND:
		switch (LOWORD(wParam))
		{
		case 1:
			WCHAR cBuffer[255];

			openFileName.lStructSize = sizeof(openFileName);
			openFileName.hwndOwner = hWnd;
			openFileName.hInstance = main_hInst;
			openFileName.lpstrFilter = L"Text Files (*.txt)\0*.txt\0";
			openFileName.lpstrCustomFilter = NULL;
			openFileName.nFilterIndex = 0;
			openFileName.lpstrFile = fileName;
			openFileName.nMaxFile = sizeof(fileName);
			openFileName.lpstrFileTitle = NULL;
			openFileName.lpstrInitialDir = NULL;

			openFileName.lpstrFile[0] = '\0';
			openFileName.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST;

			if (GetOpenFileName(&openFileName))
			{
				if (INVALID_HANDLE_VALUE == (hFile = CreateFile(openFileName.lpstrFile,
					GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL))) {

					MessageBox(hWnd, L"Ошибка открытия файла!", L"Уведомление", MB_OK);
					return 0;
				}
				else {
					wsprintf(cBuffer, L"Размер файла = %d байт", (GetFileSize(hFile, NULL)));
					wstring w = wstring(cBuffer);//.c_str()
					MessageBox(hWnd, w.c_str(), L"Размер файла", MB_OK);
				}
			}
			break;
		case 2:
			WCHAR cBuffer2[255];

			GetSystemInfo(&sinfo);
			GlobalMemoryStatus(&memorystatus);

			virtualInfo = L"Минимальный адрес памяти адресного пространства процесса = ";
			virtualInfo += to_wstring(wsprintf(cBuffer2, TEXT("%p"), sinfo.lpMinimumApplicationAddress));
			virtualInfo += L"\n";
			virtualInfo += L"Общее количество байтов, выделенное для адресного пространства процесса = ";
			virtualInfo += to_wstring(memorystatus.dwTotalVirtual);
			
			wstring ws = virtualInfo;

			const std::locale locale("");
			typedef std::codecvt<wchar_t, char, std::mbstate_t> converter_type;
			const converter_type& converter = std::use_facet<converter_type>(locale);
			std::vector<char> to(ws.length()* converter.max_length());
			std::mbstate_t state;
			const wchar_t* from_next; 
			char* to_next;
			const converter_type::result result = converter.out(state, ws.data(), ws.data() + ws.length(), from_next, &to[0], &to[0] + to.size(), to_next);
			if (result == converter_type::ok or result == converter_type::noconv) {
				const std::string s(&to[0], to_next);
				res = s;
			}
			
			ZeroMemory(&openFileName, sizeof(openFileName));
			openFileName.lStructSize = sizeof(openFileName);
			openFileName.hwndOwner = hWnd;
			openFileName.hInstance = main_hInst;
			openFileName.lpstrDefExt = L".txt";
			openFileName.lpstrFilter = L"Text Files (*.txt)\0*.txt\0";
			openFileName.lpstrCustomFilter = NULL;
			openFileName.nFilterIndex = 0;
			openFileName.lpstrFile = fileName;
			openFileName.nMaxFile = sizeof(fileName);
			openFileName.lpstrFileTitle = NULL;
			openFileName.lpstrInitialDir = NULL;

			openFileName.lpstrFile[0] = '\0';
			//openFileName.nMaxFileTitle = 0;
			openFileName.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST;

			if (GetSaveFileName(&openFileName))
			{
				hFileNew = CreateFile(fileName, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
				WriteFile(hFileNew, res.c_str(), res.length(), NULL, NULL);
				CloseHandle(hFileNew);
			}

			ZeroMemory(&startupinfo, sizeof(startupinfo));
			startupinfo.cb = sizeof(startupinfo);
			ZeroMemory(&processinformation, sizeof(processinformation));

			WCHAR path[275];
			wsprintf(path, L"notepad.exe %s", fileName);
			// Запуск дочернего процесса
			if (!CreateProcess(NULL,
				path,			// имя образа запускаемого процесса
				NULL,					// дескриптор процесса не наследуется
				NULL,					// дескриптор потока не наследуется
				FALSE,					// дескрипторы не наследуются
				0,						// флагов создания нет
				NULL,					// родительский environment block
				NULL,					// родительский текущий каталог 
				&startupinfo,			// указатель на структуру STARTUPINFO
				&processinformation)	// указатель на PROCESS_INFORMATION
				)
			{
				MessageBox(NULL, L"Невозможно открыть файл", L"Информация", MB_OK | MB_ICONINFORMATION);
			}

			// Ожидаем, пока созданный процесс не завершится
			WaitForSingleObject(processinformation.hProcess, INFINITE);

			// Закрываем дескрипторы процесса и потока 
			CloseHandle(processinformation.hProcess);
			CloseHandle(processinformation.hThread);
			break;
		}

		break;
	case WM_LBUTTONDOWN:
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hWnd, uMsg, wParam, lParam);

	}
	return 0;
}