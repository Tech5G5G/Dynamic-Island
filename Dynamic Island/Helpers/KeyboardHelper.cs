using System.Runtime.InteropServices;
using Windows.System;

namespace Dynamic_Island.Helpers
{
    /// <summary>Processes keyboard inputs and allows for skipping them entirely.</summary>
    public class KeyboardHelper
    {
        const int WH_KEYBOARD_LL = 13;
        const int HC_ACTION = 0;

        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;

        IntPtr hookId;
        HookProc hookProc;

        /// <summary>Invoked when the system detects a key has been pressed.</summary>
        public event KeyEventHandler KeyPressed;

        public KeyboardHelper()
        {
            hookProc = KeyboardProc;
            var module = Process.GetCurrentProcess().MainModule;
            IntPtr handle = PInvoke.GetModuleHandle(module.ModuleName);
            hookId = PInvoke.SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, handle, 0);
        }
        ~KeyboardHelper() => PInvoke.UnhookWindowsHookEx((int)hookId);

        private static VirtualKey LParamToVirtualKey(nint lParam) => (VirtualKey)((KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT))).vkCode;
        private int KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == HC_ACTION)
            {
                if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
                {
                    var args = new KeyEventArgs(LParamToVirtualKey(lParam));
                    KeyPressed?.Invoke(this, args);
                    if (args.Handled)
                        return 1;
                }
            }
            return PInvoke.CallNextHookEx((int)IntPtr.Zero, nCode, wParam, lParam);
        }
    }

    /// <summary>Event handler for <see cref="KeyboardHelper.KeyPressed"/> event.</summary>
    /// <param name="sender"><see cref="KeyboardHelper"/> sending the event.</param>
    /// <param name="args">Containing info about the event, such as the <see cref="VirtualKey"/> that is involved in the event.</param>
    public delegate void KeyEventHandler(KeyboardHelper sender, KeyEventArgs args);
    /// <summary>Event arguments for the <see cref="KeyEventHandler"/> delegate.</summary>
    /// <param name="key">The <see cref="VirtualKey"/> that is involved in the event.</param>
    public class KeyEventArgs(VirtualKey key)
    {
        /// <summary>The <see cref="VirtualKey"/> recived from the keyboard.</summary>
        public VirtualKey Key { get; } = key;

        /// <summary>Mark this as <see langword="true"/> to avoid sending <see cref="KeyEventArgs.Key"/>.</summary>
        public bool Handled { get; set; } = false;
    }

    delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
#pragma warning disable CS0649
    struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public KBDLLHOOKSTRUCTFlags flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }
#pragma warning restore CS0649
    [Flags]
    enum KBDLLHOOKSTRUCTFlags : uint
    {
        LLKHF_EXTENDED = 0x1,
        LLKHF_INJECTED = 0x10,
        LLKHF_ALTDOWN = 0x20,
        LLKHF_UP = 0x80
    }
}
