#include <windows.h> // ������������ ����, ���������� ������� API
#include <string>
#include <atlconv.h>
#include <sstream>
#include <time.h>
#define N 8
using namespace std;
// �������� ������� ��������� ��������� � ���������������� ���������:
LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);
BOOL RegClass(WNDPROC, LPCTSTR, UINT);//����������� ������
HINSTANCE main_hInst;// ������ ���������� ��������� ������
HWND hwnd;//������ ����
HWND hList1, hList2, hwndStatic;
float width = 500;
float height = 500;
HMENU hMainMenu;
float curX, curY;
HDC hdc,hCompatibleDC;
HANDLE thread;
string path = "D:\\Lannica's things\\Desktop\\������\\7 ������� (IV ����)\\���� + ������\\��\\laba 6\\x64\\Debugx64\\Debug\\scrn.bmp";
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

TCHAR szClassName[] = "WinAppClass"; // ������ � ������ ������
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
		MessageBox(NULL, "�� ���������� ���������������� �����!", "������", MB_OK);
		return NULL; // ����������, �������������, ������� �� WinMain
	}
	srand(time(NULL));
	for (int i = 0; i < 8; i++) {
		for (int j = 0; j < 8; j++) {
			matrica[i][j] = -9 + rand() % (9 - (-9) + 1);
		}
	}

	// �������, ��������� ������:
	hwnd = CreateWindowEx(
		0,//����������� ����������� ��� ��������
		szClassName, // ��� ������
		"������_��6_���������_������_��18�", // ��� ������ (�� ��� ������)
		WS_OVERLAPPEDWINDOW, // ������ ����������� ������
		100, // ������� ������ �� ��� �
		100, // ������� ������ �� ��� � (��� ������ � �, �� ������ �� �����)
		width, // ������ ������
		height, // ������ ������ (��� ������ � ������, �� ������ �� �����)
		NULL, // ���������� ������������� ����
		NULL, // ���������� ����
		hInst, // ���������� ���������� ����������
		NULL); // ������ �� ������� �� WndProc

	if (!hwnd) {
		// � ������ ������������� �������� ������ (�������� ��������� � ��):
		MessageBox(NULL, "�� ���������� ������� ����!", "������", MB_OK);
		return NULL;
	}

	ShowWindow(hwnd, nCmdShow); // ���������� ������
	UpdateWindow(hwnd); // ��������� ������
	// ��������� ��������� �� �������, ���������� ��-�����, ��
	while (GetMessage(&msg, //��������� �� ��������� ���������, 
							//� ������� GetMessage ������ ���������
		NULL, //��������� ����, �� �������� GetMessage ������ ��������� 
			  //(NULL ��������, ��� GetMessage ��������� ��������� �� ���� ����, 
			  //������������� ������)
		NULL, //���������� ������������� ���������, ������� ������ GetMessage
		NULL)) { //���������� ������������� ���������, ������� ������ GetMessage

		TranslateMessage(&msg); // �������������� ���������
		DispatchMessage(&msg); // ������� ��������� ������� ��
	}
	//return 0; // ���������� ��� ������ �� ����������
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

	wc.hIcon = LoadIcon(NULL, IDI_APPLICATION); // ��������� �����������
	wc.hIconSm = LoadIcon(NULL, IDI_APPLICATION); // ���������� ��������� ����������� (� ����)
	wc.hCursor = LoadCursor(NULL, IDC_ARROW); // ���������� �������
	wc.lpszMenuName = NULL; // ��������� �� ��� ���� (� ��� ��� ���)
	wc.cbWndExtra = NULL; // ����� ������������� ������ � ����� ���������
	wc.cbClsExtra = NULL; // ����� ������������� ������ ��� �������� ���������� ����������
	wc.hbrBackground = CreateSolidBrush(RGB(240, 240, 240));//(HBRUSH)(brBackground + 1); // ���������� ����� ��� �������� ���� ����
	return (RegisterClassEx(&wc) != 0);
}

void AddMenus(HWND hWnd) {
	hMainMenu = CreateMenu();

	HMENU h1Menu = CreateMenu();
	AppendMenu(h1Menu, MF_STRING, 1, "������");

	AppendMenu(hMainMenu, MF_POPUP, (UINT_PTR)h1Menu, "������");

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
		MessageBox(NULL, "�������� ���� � ������� �� ������!", "������!!!", 0);
		return 0;
	}

	SetEvent(hEvent);
	dwRes = WaitForSingleObject(hEvent2, 50000);
	if (dwRes == WAIT_TIMEOUT) {
		MessageBox(NULL, "����� �������", "", 0);
		return 0;
	}
	ret = GetMailslotInfo(hMailSlot, NULL, &dwMes, &dwMsgNum, NULL);

	if (ret == FALSE) {
		MessageBox(NULL, "������ ��� ������ � �������� ������", "", 0);
		return 0;
	}

	if (dwMsgNum == 0) {
		MessageBox(NULL, "��������� ���", "", 0);
		return 0;
	}

	ret = ReadFile(hMailSlot, fromServer, 256, &dwSize, NULL);

	if (ret == FALSE)
		MessageBox(NULL, "������ �� ������", "", 0);
	else {
		toList(fromServer,hList2);
		MessageBox(NULL, "������� ��������� � �������", "�����������", 0);
	}

	CloseHandle(hMailSlot);
	return 0;
}

LRESULT CALLBACK WndProc(HWND hWnd,//��������� ����, �� �������� ������ ���������.
	UINT uMsg,//������������� ���������
	WPARAM wParam, LPARAM lParam) {//��������� ���������

	PAINTSTRUCT ps;
	RECT Rect;
	// ������� � ��������� ���������
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
			"1 �������",
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
			"2 �������",
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
			openFileName.lpstrFilter = "����������� ����� (*.exe)\0*.exe\0";
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

				MessageBox(hWnd, "������ ����������.", "�����������.", MB_OK);

				DWORD test = CreateProcess(NULL, path, NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi);
			}
			//TODO ������ �������


			nArSize = N * N * sizeof(int);
			hMap = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, nArSize, "MyMap");
			if (hMap == 0) {
				MessageBox(NULL, "�������� ����������", NULL, 0);
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