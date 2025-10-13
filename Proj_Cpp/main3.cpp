#include <windows.h> // ������������ ����, ���������� ������� API
#include <string>
#include <atlconv.h>
#include <tchar.h>
#include <iostream>
#include <clocale>
#include <locale>
#include <vector>
using namespace std;
// �������� ������� ��������� ��������� � ���������������� ���������:
LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);
BOOL RegClass(WNDPROC, LPCTSTR, UINT);//����������� ������

HINSTANCE main_hInst;// ������ ���������� ��������� ������
HWND hwnd;//������ ����
float width = 500;
float height = 500;
HMENU hMainMenu;
float curX, curY;
HDC hdc;

HWND hwndStatic;
HWND hwndEdit;
BOOL sectors, fileSize;
DWORD dwordSectorsPerCluster, dwordBytesPerSector, dwordNumberOfFreeClusters, dwordTotalNumberOfClusters; //��� ��, ��� ���� ��������
OPENFILENAME openFileName;
wstring str, virtualInfo;
HANDLE hFile,hFileNew;
MEMORYSTATUS memorystatus;
SYSTEM_INFO sinfo;

STARTUPINFO startupinfo;
PROCESS_INFORMATION processinformation;
string res;

TCHAR szClassName[] = L"WinAppClass"; // ������ � ������ ������
// �������� ������� - ������ int main() � ���������� ����������:
int WINAPI WinMain(HINSTANCE hInst, //���������� ���������� ����������. 
									//���� ���������� �������� ����� ������ ���� ��������� 
									//� �� �������� ������������. //
									//���������� hInstance ���� ����� ��������� ��������, 
									//���������� � ��������� ���������.
	HINSTANCE hPrevInst, //���������� ����������� ���������� ����������. 
						 //���� ���������� ������� �� ������ ������ Windows,
						 //�� ������� �� ����������.
	LPSTR lpCmdLine, //��������� �� ������ ��������� ������, 
					 //��������� ��� ������� ���������.
	int nCmdShow) // ��� �������� �������� �������� ��� ���� 
				  //(��������, ��������� ��� �����������)
{
	MSG msg; // ����� ��������� ��������� MSG ��� ��������� ���������
	main_hInst = hInst;

	if (!RegClass(WndProc, szClassName, COLOR_WINDOW)) {
		MessageBox(NULL, L"�� ���������� ���������������� �����!", L"������", MB_OK);
		return NULL; // ����������, �������������, ������� �� WinMain
	}

	// �������, ��������� ������:
	hwnd = CreateWindowEx(
		0,
		szClassName,
		L"��7_���������_������_��18�", 
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
		MessageBox(NULL, L"�� ���������� ������� ����!", L"������", MB_OK);
		return NULL;
	}
	ShowWindow(hwnd, nCmdShow); 
	UpdateWindow(hwnd); 
	// ��������� ��������� �� �������, ���������� ��-�����, ��
	while (GetMessage(&msg, 
		NULL, //(NULL ��������, ��� GetMessage ��������� ��������� �� ���� ����, ������������� ������)
		NULL, //���������� ������������� ���������, ������� ������ GetMessage
		NULL)) { //���������� ������������� ���������, ������� ������ GetMessage

		TranslateMessage(&msg); // �������������� ���������
		DispatchMessage(&msg); // ������� ��������� ��
	}
	return msg.wParam; // ���������� ��� ������ �� ����������
}

//�������� ������� ����������� �������
BOOL RegClass(WNDPROC Proc, LPCTSTR szName, UINT brBackground) {
	WNDCLASSEX wc; // ������ ���������, ��� ��������� � ������ ������ WNDCLASS
	wc.style = CS_HREDRAW | CS_VREDRAW; // ����� ������ ������
	wc.lpfnWndProc = Proc; // ��������� �� ���������������� �������
	wc.hInstance = main_hInst; // ��������� �� ������, ���������� ��� ����, ������������ ��� ������
	wc.lpszClassName = szName; // ��������� �� ��� ������
	wc.cbSize = sizeof(WNDCLASSEX); // ������ ��������� (� ������)

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
	AppendMenu(h1Menu, MF_STRING, 1, L"�������");
	AppendMenu(h1Menu, MF_STRING, 2, L"���������");

	AppendMenu(hMainMenu, MF_POPUP, (UINT_PTR)h1Menu, L"����");

	SetMenu(hWnd, hMainMenu); DrawMenuBar(hWnd);
}

LRESULT CALLBACK WndProc(HWND hWnd,//��������� ����, �� �������� ������ ���������.
	UINT uMsg,//������������� ���������
	WPARAM wParam, LPARAM lParam) {//��������� ���������
	// ������� � ��������� ���������
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
			L"���������� �������� � ����� ��������:",
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
				L"���������� ��������� ���������� �������� � ����� ��������!",
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

					MessageBox(hWnd, L"������ �������� �����!", L"�����������", MB_OK);
					return 0;
				}
				else {
					wsprintf(cBuffer, L"������ ����� = %d ����", (GetFileSize(hFile, NULL)));
					wstring w = wstring(cBuffer);//.c_str()
					MessageBox(hWnd, w.c_str(), L"������ �����", MB_OK);
				}
			}
			break;
		case 2:
			WCHAR cBuffer2[255];

			GetSystemInfo(&sinfo);
			GlobalMemoryStatus(&memorystatus);

			virtualInfo = L"����������� ����� ������ ��������� ������������ �������� = ";
			virtualInfo += to_wstring(wsprintf(cBuffer2, TEXT("%p"), sinfo.lpMinimumApplicationAddress));
			virtualInfo += L"\n";
			virtualInfo += L"����� ���������� ������, ���������� ��� ��������� ������������ �������� = ";
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
			// ������ ��������� ��������
			if (!CreateProcess(NULL,
				path,			// ��� ������ ������������ ��������
				NULL,					// ���������� �������� �� �����������
				NULL,					// ���������� ������ �� �����������
				FALSE,					// ����������� �� �����������
				0,						// ������ �������� ���
				NULL,					// ������������ environment block
				NULL,					// ������������ ������� ������� 
				&startupinfo,			// ��������� �� ��������� STARTUPINFO
				&processinformation)	// ��������� �� PROCESS_INFORMATION
				)
			{
				MessageBox(NULL, L"���������� ������� ����", L"����������", MB_OK | MB_ICONINFORMATION);
			}

			// �������, ���� ��������� ������� �� ����������
			WaitForSingleObject(processinformation.hProcess, INFINITE);

			// ��������� ����������� �������� � ������ 
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