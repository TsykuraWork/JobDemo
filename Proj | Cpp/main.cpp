#include <windows.h>
#include <stdio.h>
#include <math.h>
#include "resource.h"
#include "atlstr.h"

LRESULT MyWinP(HWND, UINT, WPARAM, LPARAM);
LRESULT CALLBACK ParFun(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam);
void Graph(HWND hWnd, HDC dc, double xmin, double xmax); 

double fn(double x)
{
	return 1-cos(x);
}

double XMIN, XMAX;

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
	TCHAR ProgName[] = TEXT("My Window");
	TCHAR Title[] = TEXT("Заголовок окна");
	HWND hWnd; MSG msg;
	HMENU menu;
	WNDCLASS w;

	w.lpszClassName = (LPCWSTR)ProgName;
	w.hInstance = hInstance;
	w.lpfnWndProc = (WNDPROC)MyWinP;
	w.hCursor = LoadCursor(NULL, IDC_ARROW);
	w.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	w.lpszMenuName = NULL;
	w.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
	w.style = CS_HREDRAW | CS_VREDRAW;
	w.cbClsExtra = 0;
	w.cbWndExtra = 0;

	if (!RegisterClass(&w))
	{
		return 1;
	}

	menu = LoadMenu(NULL, (LPCWSTR)IDR_MENU1);
	hWnd = CreateWindow((LPCWSTR)ProgName, (LPCWSTR)Title, WS_OVERLAPPEDWINDOW, 0, 0, CW_USEDEFAULT, CW_USEDEFAULT, NULL, menu, hInstance, NULL);

	if (!hWnd)
	{
		return 2;
	}

	ShowWindow(hWnd, nCmdShow);

	while (GetMessage(&msg, NULL, 0, 0))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
	return msg.wParam;
}

LRESULT MyWinP(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam) {
	HDC dc;
	PAINTSTRUCT ps;

	switch (msg)
	{
	case WM_COMMAND:
		if (LOWORD(wParam) == ID_INPUT)
		{
			if (DialogBox(NULL, (LPCTSTR)IDD_PARAMETERS, hWnd, (DLGPROC)ParFun) == IDOK)
			{
				InvalidateRect(hWnd, NULL, TRUE);
			}
		}
		break;
	case WM_CREATE:
		XMIN = -10.0;
		XMAX = 10.0;
		break;
	case WM_PAINT:
		dc = BeginPaint(hWnd, &ps);
		Graph(hWnd, dc, XMIN, XMAX);
		EndPaint(hWnd, &ps);
		break;
	case WM_LBUTTONDOWN:
		if (DialogBox(NULL, (LPCTSTR)IDD_PARAMETERS, hWnd, (DLGPROC)ParFun) == IDOK)
		{
			InvalidateRect(hWnd, NULL, TRUE);
		}
		break;
	case WM_DESTROY: //сообщение на уничтожение окна
		PostQuitMessage(0);
		break;
	default:
		return LONG(DefWindowProc(hWnd, msg, wParam, lParam));
	}
	return 0;
}

LRESULT CALLBACK ParFun(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam) {
	wchar_t minim[100], maxim[100];

	switch (message)
	{
		//инициализация диалогового окна перед отображением
	case WM_INITDIALOG:
		// запись значений максимума и минимума в переменные в виде строк
		swprintf(minim, sizeof(minim), L"%.2f", XMIN); 
		swprintf(maxim, sizeof(maxim), L"%.2f", XMAX);

		// заполнение окон редактирования вышеуказанными строками
		SetDlgItemText(hDlg, IDC_XMIN, minim);
		SetDlgItemText(hDlg, IDC_XMAX, maxim);
		return TRUE;
		//обработка нажатия кнопок диалогового окна
	case WM_COMMAND:
		if (LOWORD(wParam) == IDOK)
		{
			GetDlgItemText(hDlg, IDC_XMIN, minim, sizeof(minim));
			GetDlgItemText(hDlg, IDC_XMAX, maxim, sizeof(maxim));

			XMIN = _wtof(minim);
			XMAX = _wtof(maxim);

			if (fabs(XMIN - XMAX) < 0.001)
			{
				MessageBox(NULL, L"Зачения Xmin и Xmax равны", L"Ошибка!!!", 0);
				return 0;
			}

			if (XMIN > XMAX)
			{
				double d;
				d = XMIN;
				XMIN = XMAX;
				XMAX = d;
			}

			EndDialog(hDlg, IDOK);
			return TRUE;
		}
		if (LOWORD(wParam) == IDCANCEL)
		{
			EndDialog(hDlg, IDCANCEL);
			return TRUE;
		}
	}
	return 0;
}

// функция прорисовки графика
void Graph(HWND hWnd, HDC dc, double xmin, double xmax) {
	double y, ymax, ymin, hx, y0, x0, hscr, hval;
	int i, del;
	int n = 100; //количество точек
	HPEN pen, penOld;
	HBRUSH brush, brushOld;
	wchar_t s[80];
	RECT rect;

	// границы графика
	hx = (xmax - xmin) / (double)n;
	ymax = ymin = fn(xmin);

	for (i = 1; i < n; i++)
	{
		y = fn(xmin + i * hx);
		if (y > ymax) ymax = y;
		if (y < ymin) ymin = y;
	}

	GetClientRect(hWnd, &rect);
	brush = CreateSolidBrush(RGB(192, 192, 192));
	brushOld = (HBRUSH)SelectObject(dc, brush);
	Rectangle(dc, 0, 0, rect.right, rect.bottom);
	pen = CreatePen(PS_SOLID, 2, RGB(0, 0, 0));
	penOld = (HPEN)SelectObject(dc, pen);
	SetBkColor(dc, RGB(192, 192, 192));

	//ось OY
	x0 = xmax / (xmax - xmin);
	if (x0 > 1)
	{
		x0 = 2;
	}
	else
		if (x0 < 0) x0 = rect.right - 2;
		else
			x0 = (1 - x0) * rect.right;
	MoveToEx(dc, int(x0), 0, NULL);
	LineTo(dc, int(x0), rect.bottom);

	if (rect.bottom < 150) del = 2;
	if (rect.bottom >= 150 && rect.bottom < 350) del = 3;
	if (rect.bottom >= 350 && rect.bottom < 550) del = 5;
	if (rect.bottom >= 550) del = 10;

	hscr = (double)rect.bottom / double(del);
	hval = (ymax - ymin) / del;

	// значения на оси координат
	for (i = 0; i < del; i++)
	{
		MoveToEx(dc, int(x0) - 2, int(i * hscr), NULL);
		LineTo(dc, int(x0) + 3, int(i * hscr));
		swprintf(s, sizeof(s), L"%.3f", ymax - i * hval);

		if (x0 < rect.right - 50)
			TextOut(dc, int(x0) + 2, int(i * hscr), s, wcslen(s));
		else
			TextOut(dc, rect.right - 50, int(i * hscr), s, wcslen(s));
	}

	MoveToEx(dc, int(x0) - 2, rect.bottom - 1, NULL);
	LineTo(dc, int(x0) + 3, rect.bottom - 1);
	swprintf(s, sizeof(s), L"%.3f", ymin);

	if (x0 < rect.right - 50)
		TextOut(dc, int(x0) + 2, rect.bottom - 20, s, wcslen(s));
	else
		TextOut(dc, rect.right - 50, rect.bottom - 20, s, wcslen(s));

	//ось OX
	y0 = ymax / (ymax - ymin);
	if (y0 > 1) y0 = rect.bottom - 2;
	else if (y0 < 0) y0 = 2;
	else y0 *= rect.bottom;

	MoveToEx(dc, 0, int(y0), NULL);
	LineTo(dc, rect.right, int(y0));

	if (rect.right < 200) del = 2;
	if (rect.right >= 200 && rect.right < 350) del = 3;
	if (rect.right >= 350 && rect.right < 550) del = 5;
	if (rect.right >= 550) del = 10;

	hscr = (double)rect.right / double(del);
	hval = (xmax - xmin) / del;

	// значения на оси координат
	for (i = 0; i < del; i++)
	{
		MoveToEx(dc, int(i * hscr), int(y0 - 2), NULL);
		LineTo(dc, int(i * hscr), int(y0 + 3));
		swprintf(s, sizeof(s), L"%.3f", xmin + i * hval);

		if (y0 < rect.bottom - 30)
			TextOut(dc, int(i * hscr), int(y0 + 4), s, wcslen(s));
		else
			TextOut(dc, int(i * hscr), rect.bottom - 20, s, wcslen(s));
	}

	MoveToEx(dc, rect.right - 1, int(y0 - 2), NULL);
	LineTo(dc, rect.right - 1, int(y0 + 3));
	swprintf(s, sizeof(s), L"%.3f", xmax);

	if (y0 < rect.bottom - 20)
		TextOut(dc, rect.right - 50, int(y0 + 4), s, wcslen(s));
	else
		TextOut(dc, rect.right - 50, rect.bottom - 20, s, wcslen(s));

	DeleteObject(pen);
	pen = CreatePen(PS_SOLID, 1, RGB(255, 0, 0));
	SelectObject(dc, pen);
	hscr = double(rect.right) / double(n);

	MoveToEx(dc, 0, int(rect.bottom - (fn(xmin) - ymin) * rect.bottom / (ymax - ymin)),
		NULL);

	//график
	for (i = 1; i < n; i++) {
		y = fn(xmin + i * hx);
		LineTo(dc, int(i * hscr), int(rect.bottom - (y - ymin)*
			rect.bottom / (ymax - ymin)));
	}

	// удалеие объектов
	SelectObject(dc, penOld);
	DeleteObject(pen);
	SelectObject(dc, brushOld);
	DeleteObject(brush);
}