using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;

public class TransparentWindow : MonoBehaviour
{
    [Tooltip("是否启动优先显示")]
    public bool isPriority = true;

    struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    #region 定义外部函数

    // 获取显示在最上面的窗口
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    // 注意要编写与 32 位和 64 位版本的 Windows 兼容的代码 请使用SetWindowLongPtr
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

    // 窗口句柄
    private IntPtr Handle;

    private void Start()
    {
#if !UNITY_EDITOR

        // 获取窗口句柄
        Handle = GetForegroundWindow();

        // 设置窗口的属性
        SetWindowLong(Handle, -16, 0x80000000);

        var margins = new MARGINS() { cxLeftWidth = -1 };

        // 将窗口框架扩展到工作区
        DwmExtendFrameIntoClientArea(Handle, ref margins);

        // 设置窗口位置
        SetWindowPos(Handle, -1, 0, 0, 0, 0, 2 | 1 | 64);
#endif
    }

    private void Update()
    {
#if !UNITY_EDITOR
        // 保持窗口始终在最前面
        if (Handle != GetForegroundWindow() && isPriority) SetForegroundWindow(Handle);

        // 左键拖动
        if (Input.GetMouseButtonDown(0))
        {
            ReleaseCapture();
            SendMessage(Handle, 0xA1, 0x02, 0);
            SendMessage(Handle, 0x0202, 0, 0);
        }
#endif
    }

    //更改屏幕分辨率
    public void SetResolutionByConfig()
    {
        int width = Convert.ToInt32(GameController.appConfigs["Width"]);
        int height = Convert.ToInt32(GameController.appConfigs["Height"]);

        Screen.SetResolution(width, height, FullScreenMode.Windowed, 1);
    }
}
