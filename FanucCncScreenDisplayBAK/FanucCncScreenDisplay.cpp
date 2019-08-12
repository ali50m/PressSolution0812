// FanucCncScreenDisplay.cpp : ���� DLL �ĳ�ʼ�����̡�
//

#include "stdafx.h"
#include "FanucCncScreenDisplay.h"
#include "CSD.h"
#include "cncscrn_ctrl.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//TODO:  ����� DLL ����� MFC DLL �Ƕ�̬���ӵģ�
//		��Ӵ� DLL �������κε���
//		MFC �ĺ������뽫 AFX_MANAGE_STATE ����ӵ�
//		�ú�������ǰ�档
//
//		����: 
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// �˴�Ϊ��ͨ������
//		}
//
//		�˺������κ� MFC ����
//		������ÿ��������ʮ����Ҫ��  ����ζ��
//		��������Ϊ�����еĵ�һ�����
//		���֣������������ж������������
//		������Ϊ���ǵĹ��캯���������� MFC
//		DLL ���á�
//
//		�й�������ϸ��Ϣ��
//		����� MFC ����˵�� 33 �� 58��
//

// CFanucCncScreenDisplayApp

BEGIN_MESSAGE_MAP(CFanucCncScreenDisplayApp, CWinApp)
END_MESSAGE_MAP()


// CFanucCncScreenDisplayApp ����

CFanucCncScreenDisplayApp::CFanucCncScreenDisplayApp()
{
	// TODO:  �ڴ˴���ӹ�����룬
	// ��������Ҫ�ĳ�ʼ�������� InitInstance ��
}


// Ψһ��һ�� CFanucCncScreenDisplayApp ����

CFanucCncScreenDisplayApp theApp;
volatile BOOL m_bRun;
HWND m_hWnd;

HANDLE hThread;
DWORD ThreadID;


// CFanucCncScreenDisplayApp ��ʼ��

BOOL CFanucCncScreenDisplayApp::InitInstance()
{
	CWinApp::InitInstance();

	return TRUE;
}

void WINAPI _CreateCncScreenDisplay(HWND hWnd, char* ip)
{
	m_hWnd = hWnd;

	CCSD *pMyDlg = CCSD::GetInstance();
	pMyDlg->DestroyWindow();
	//memset(pMyDlg->ip, 0, sizeof(char) * 15);
	//memcpy(pMyDlg->ip, ip, strlen(ip));
	pMyDlg->Create(IDD_CSD, CWnd::FromHandle(m_hWnd));
	pMyDlg->ShowWindow(SW_NORMAL);
	pMyDlg->SetIp(ip);

}

void ThreadFunc()
{
	m_bRun = TRUE;
	while (m_bRun)
	{
		CCSD *pMyDlg = CCSD::GetInstance();
		pMyDlg->Refresh();
		
	}

}

void WINAPI _StartRefresh()
{

	hThread = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)ThreadFunc, NULL, 0, &ThreadID);
}

void WINAPI _StopRefresh()
{
	//CCSD *pMyDlg = CCSD::GetInstance();
	//pMyDlg->StopRefresh();

	m_bRun = false;

	//CCSD *pMyDlg = CCSD::GetInstance();
	//pMyDlg->DestroyWindow();
}

void WINAPI _SendKey(WORD state, WORD key)
{
	CCSD *pMyDlg = CCSD::GetInstance();

	pMyDlg->SendKey(state, key);
}
