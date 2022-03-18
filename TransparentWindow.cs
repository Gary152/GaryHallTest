using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;

public class TransparentWindow : MonoBehaviour
{
    [Tooltip("�Ƿ�����������ʾ")]
    public bool isPriority = true;

    struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    #region �����ⲿ����

    // ��ȡ��ʾ��������Ĵ���
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    // ע��Ҫ��д�� 32 λ�� 64 λ�汾�� Windows ���ݵĴ��� ��ʹ��SetWindowLongPtr
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

    #endregion

    // ���ھ��
    private IntPtr Handle;

    private void Start()
    {
#if !UNITY_EDITOR

        // ��ȡ���ھ��
        Handle = GetForegroundWindow();

        // ���ô��ڵ�����
        SetWindowLong(Handle, -16, 0x80000000);

        var margins = new MARGINS() { cxLeftWidth = -1 };

        // �����ڿ����չ��������
        DwmExtendFrameIntoClientArea(Handle, ref margins);

        // ���ô���λ��
        SetWindowPos(Handle, -1, 0, 0, 0, 0, 2 | 1 | 64);
#endif
    }

    private void Update()
    {
#if !UNITY_EDITOR
        // ���ִ���ʼ������ǰ��
        if (Handle != GetForegroundWindow() && isPriority) SetForegroundWindow(Handle);

        // ����϶�
        if (Input.GetMouseButtonDown(0))
        {
            ReleaseCapture();
            SendMessage(Handle, 0xA1, 0x02, 0);
            SendMessage(Handle, 0x0202, 0, 0);
        }
#endif
    }

    //������Ļ�ֱ���
    public void SetResolutionByConfig()
    {
        int width = Convert.ToInt32(GameController.appConfigs["Width"]);
        int height = Convert.ToInt32(GameController.appConfigs["Height"]);

        Screen.SetResolution(width, height, FullScreenMode.Windowed, 1);
    }
}
