// FanucCncScreenDisplay.h : FanucCncScreenDisplay DLL ����ͷ�ļ�
//

#pragma once

#ifndef __AFXWIN_H__
	#error "�ڰ������ļ�֮ǰ������stdafx.h�������� PCH �ļ�"
#endif

#include "resource.h"		// ������

extern "C" _declspec(dllexport) void WINAPI _CreateCncScreenDisplay(HWND hWnd, char* ip,
	int xSrc, int ySrc , int xDest, int yDest, int nSrcWidth, int nSrcHeight, int nDestWidth,int nDestHeight);

extern "C" _declspec(dllexport) void WINAPI _StartRefresh(void);

extern "C" _declspec(dllexport) void WINAPI _StopRefresh(void);

extern "C" _declspec(dllexport) void WINAPI _SendKey(WORD state, WORD key);


// CFanucCncScreenDisplayApp
// �йش���ʵ�ֵ���Ϣ������� FanucCncScreenDisplay.cpp
//

class CFanucCncScreenDisplayApp : public CWinApp
{
public:
	CFanucCncScreenDisplayApp();

// ��д
public:
	virtual BOOL InitInstance();
	

private:
	

	DECLARE_MESSAGE_MAP()
};
