﻿namespace ReClip.Interop
{
    internal static partial class NativeMethods
    {
        public const int MONITOR_DEFAULTTONULL = 0;
        public const int MONITOR_DEFAULTTOPRIMARY = 1;
        public const int MONITOR_DEFAULTTONEAREST = 2;

        public const int WH_JOURNALPLAYBACK = 1,
        WH_GETMESSAGE = 3,
        WH_MOUSE = 7,
        WSF_VISIBLE = 0x0001,
        WM_NULL = 0x0000,
        WM_CREATE = 0x0001,
        WM_DELETEITEM = 0x002D,
        WM_DESTROY = 0x0002,
        WM_MOVE = 0x0003,
        WM_SIZE = 0x0005,
        WM_ACTIVATE = 0x0006,
        WA_INACTIVE = 0,
        WA_ACTIVE = 1,
        WA_CLICKACTIVE = 2,
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008,
        WM_ENABLE = 0x000A,
        WM_SETREDRAW = 0x000B,
        WM_SETTEXT = 0x000C,
        WM_GETTEXT = 0x000D,
        WM_GETTEXTLENGTH = 0x000E,
        WM_PAINT = 0x000F,
        WM_CLOSE = 0x0010,
        WM_QUERYENDSESSION = 0x0011,
        WM_QUIT = 0x0012,
        WM_QUERYOPEN = 0x0013,
        WM_ERASEBKGND = 0x0014,
        WM_SYSCOLORCHANGE = 0x0015,
        WM_ENDSESSION = 0x0016,
        WM_SHOWWINDOW = 0x0018,
        WM_WININICHANGE = 0x001A,
        WM_SETTINGCHANGE = 0x001A,
        WM_DEVMODECHANGE = 0x001B,
        WM_ACTIVATEAPP = 0x001C,
        WM_FONTCHANGE = 0x001D,
        WM_TIMECHANGE = 0x001E,
        WM_CANCELMODE = 0x001F,
        WM_SETCURSOR = 0x0020,
        WM_MOUSEACTIVATE = 0x0021,
        WM_CHILDACTIVATE = 0x0022,
        WM_QUEUESYNC = 0x0023,
        WM_GETMINMAXINFO = 0x0024,
        WM_PAINTICON = 0x0026,
        WM_ICONERASEBKGND = 0x0027,
        WM_NEXTDLGCTL = 0x0028,
        WM_SPOOLERSTATUS = 0x002A,
        WM_DRAWITEM = 0x002B,
        WM_MEASUREITEM = 0x002C,
        WM_VKEYTOITEM = 0x002E,
        WM_CHARTOITEM = 0x002F,
        WM_SETFONT = 0x0030,
        WM_GETFONT = 0x0031,
        WM_SETHOTKEY = 0x0032,
        WM_GETHOTKEY = 0x0033,
        WM_QUERYDRAGICON = 0x0037,
        WM_COMPAREITEM = 0x0039,
        WM_GETOBJECT = 0x003D,
        WM_COMPACTING = 0x0041,
        WM_COMMNOTIFY = 0x0044,
        WM_WINDOWPOSCHANGING = 0x0046,
        WM_WINDOWPOSCHANGED = 0x0047,
        WM_POWER = 0x0048,
        WM_COPYDATA = 0x004A,
        WM_CANCELJOURNAL = 0x004B,
        WM_NOTIFY = 0x004E,
        WM_INPUTLANGCHANGEREQUEST = 0x0050,
        WM_INPUTLANGCHANGE = 0x0051,
        WM_TCARD = 0x0052,
        WM_HELP = 0x0053,
        WM_USERCHANGED = 0x0054,
        WM_NOTIFYFORMAT = 0x0055,
        WM_CONTEXTMENU = 0x007B,
        WM_STYLECHANGING = 0x007C,
        WM_STYLECHANGED = 0x007D,
        WM_DISPLAYCHANGE = 0x007E,
        WM_GETICON = 0x007F,
        WM_SETICON = 0x0080,
        WM_NCCREATE = 0x0081,
        WM_NCDESTROY = 0x0082,
        WM_NCCALCSIZE = 0x0083,
        WM_NCHITTEST = 0x0084,
        WM_NCPAINT = 0x0085,
        WM_NCACTIVATE = 0x0086,
        WM_GETDLGCODE = 0x0087,
        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCMOUSELEAVE = 0x02A2,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_NCRBUTTONDBLCLK = 0x00A6,
        WM_NCMBUTTONDOWN = 0x00A7,
        WM_NCMBUTTONUP = 0x00A8,
        WM_NCMBUTTONDBLCLK = 0x00A9,
        WM_NCXBUTTONDOWN = 0x00AB,
        WM_NCXBUTTONUP = 0x00AC,
        WM_NCXBUTTONDBLCLK = 0x00AD,
        WM_KEYFIRST = 0x0100,
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_CHAR = 0x0102,
        WM_DEADCHAR = 0x0103,
        WM_CTLCOLOR = 0x0019,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,
        WM_SYSCHAR = 0x0106,
        WM_SYSDEADCHAR = 0x0107,
        WM_KEYLAST = 0x0108,
        WM_IME_STARTCOMPOSITION = 0x010D,
        WM_IME_ENDCOMPOSITION = 0x010E,
        WM_IME_COMPOSITION = 0x010F,
        WM_IME_KEYLAST = 0x010F,
        WM_INITDIALOG = 0x0110,
        WM_COMMAND = 0x0111,
        WM_SYSCOMMAND = 0x0112,
        WM_TIMER = 0x0113,
        WM_HSCROLL = 0x0114,
        WM_VSCROLL = 0x0115,
        WM_INITMENU = 0x0116,
        WM_INITMENUPOPUP = 0x0117,
        WM_MENUSELECT = 0x011F,
        WM_MENUCHAR = 0x0120,
        WM_ENTERIDLE = 0x0121,
        WM_UNINITMENUPOPUP = 0x0125,
        WM_CHANGEUISTATE = 0x0127,
        WM_UPDATEUISTATE = 0x0128,
        WM_QUERYUISTATE = 0x0129,
        WM_CTLCOLORMSGBOX = 0x0132,
        WM_CTLCOLOREDIT = 0x0133,
        WM_CTLCOLORLISTBOX = 0x0134,
        WM_CTLCOLORBTN = 0x0135,
        WM_CTLCOLORDLG = 0x0136,
        WM_CTLCOLORSCROLLBAR = 0x0137,
        WM_CTLCOLORSTATIC = 0x0138,
        WM_MOUSEFIRST = 0x0200,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_XBUTTONDOWN = 0x020B,
        WM_XBUTTONUP = 0x020C,
        WM_XBUTTONDBLCLK = 0x020D,
        WM_MOUSEWHEEL = 0x020A,
        WM_MOUSELAST = 0x020A;



        public const int WHEEL_DELTA = 120,
        WM_PARENTNOTIFY = 0x0210,
        WM_ENTERMENULOOP = 0x0211,
        WM_EXITMENULOOP = 0x0212,
        WM_NEXTMENU = 0x0213,
        WM_SIZING = 0x0214,
        WM_CAPTURECHANGED = 0x0215,
        WM_MOVING = 0x0216,
        WM_POWERBROADCAST = 0x0218,
        WM_DEVICECHANGE = 0x0219,
        WM_IME_SETCONTEXT = 0x0281,
        WM_IME_NOTIFY = 0x0282,
        WM_IME_CONTROL = 0x0283,
        WM_IME_COMPOSITIONFULL = 0x0284,
        WM_IME_SELECT = 0x0285,
        WM_IME_CHAR = 0x0286,
        WM_IME_KEYDOWN = 0x0290,
        WM_IME_KEYUP = 0x0291,
        WM_MDICREATE = 0x0220,
        WM_MDIDESTROY = 0x0221,
        WM_MDIACTIVATE = 0x0222,
        WM_MDIRESTORE = 0x0223,
        WM_MDINEXT = 0x0224,
        WM_MDIMAXIMIZE = 0x0225,
        WM_MDITILE = 0x0226,
        WM_MDICASCADE = 0x0227,
        WM_MDIICONARRANGE = 0x0228,
        WM_MDIGETACTIVE = 0x0229,
        WM_MDISETMENU = 0x0230,
        WM_ENTERSIZEMOVE = 0x0231,
        WM_EXITSIZEMOVE = 0x0232,
        WM_DROPFILES = 0x0233,
        WM_MDIREFRESHMENU = 0x0234,
        WM_MOUSEHOVER = 0x02A1,
        WM_MOUSELEAVE = 0x02A3,
        WM_CUT = 0x0300,
        WM_COPY = 0x0301,
        WM_PASTE = 0x0302,
        WM_CLEAR = 0x0303,
        WM_UNDO = 0x0304,
        WM_RENDERFORMAT = 0x0305,
        WM_RENDERALLFORMATS = 0x0306,
        WM_DESTROYCLIPBOARD = 0x0307,
        WM_DRAWCLIPBOARD = 0x0308,
        WM_PAINTCLIPBOARD = 0x0309,
        WM_VSCROLLCLIPBOARD = 0x030A,
        WM_SIZECLIPBOARD = 0x030B,
        WM_ASKCBFORMATNAME = 0x030C,
        WM_CHANGECBCHAIN = 0x030D,
        WM_HSCROLLCLIPBOARD = 0x030E,
        WM_QUERYNEWPALETTE = 0x030F,
        WM_PALETTEISCHANGING = 0x0310,
        WM_PALETTECHANGED = 0x0311,
        WM_HOTKEY = 0x0312,
        WM_PRINT = 0x0317,
        WM_PRINTCLIENT = 0x0318,
        WM_THEMECHANGED = 0x031A,
        WM_HANDHELDFIRST = 0x0358,
        WM_HANDHELDLAST = 0x035F,
        WM_AFXFIRST = 0x0360,
        WM_AFXLAST = 0x037F,
        WM_PENWINFIRST = 0x0380,
        WM_PENWINLAST = 0x038F,
        WM_APP = unchecked((int)0x8000),
        WM_USER = 0x0400,
        WM_REFLECT = NativeMethods.WM_USER + 0x1C00,
        WS_OVERLAPPED = 0x00000000,
        WS_POPUP = unchecked((int)0x80000000),
        WS_CHILD = 0x40000000,
        WS_MINIMIZE = 0x20000000,
        WS_VISIBLE = 0x10000000,
        WS_DISABLED = 0x08000000,
        WS_CLIPSIBLINGS = 0x04000000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_MAXIMIZE = 0x01000000,
        WS_CAPTION = 0x00C00000,
        WS_BORDER = 0x00800000,
        WS_DLGFRAME = 0x00400000,
        WS_VSCROLL = 0x00200000,
        WS_HSCROLL = 0x00100000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000,
        WS_TABSTOP = 0x00010000,
        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,
        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_MDICHILD = 0x00000040,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_CONTEXTHELP = 0x00000400,
        WS_EX_RIGHT = 0x00001000,
        WS_EX_LEFT = 0x00000000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_APPWINDOW = 0x00040000,
        WS_EX_LAYERED = 0x00080000,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_LAYOUTRTL = 0x00400000,
        WS_EX_NOINHERITLAYOUT = 0x00100000,
        WPF_SETMINPOSITION = 0x0001,
        WM_CHOOSEFONT_GETLOGFONT = (0x0400 + 1);

        public const int cmb4 = 0x0473,
        CS_DBLCLKS = 0x0008,
        CS_DROPSHADOW = 0x00020000,
        CS_SAVEBITS = 0x0800,
        CF_TEXT = 1,
        CF_BITMAP = 2,
        CF_METAFILEPICT = 3,
        CF_SYLK = 4,
        CF_DIF = 5,
        CF_TIFF = 6,
        CF_OEMTEXT = 7,
        CF_DIB = 8,
        CF_PALETTE = 9,
        CF_PENDATA = 10,
        CF_RIFF = 11,
        CF_WAVE = 12,
        CF_UNICODETEXT = 13,
        CF_ENHMETAFILE = 14,
        CF_HDROP = 15,
        CF_LOCALE = 16,
        CLSCTX_INPROC_SERVER = 0x1,
        CLSCTX_LOCAL_SERVER = 0x4,
        CW_USEDEFAULT = (unchecked((int)0x80000000)),
        CWP_SKIPINVISIBLE = 0x0001,
        COLOR_WINDOW = 5,
        CB_ERR = (-1),
        CBN_SELCHANGE = 1,
        CBN_DBLCLK = 2,
        CBN_EDITCHANGE = 5,
        CBN_EDITUPDATE = 6,
        CBN_DROPDOWN = 7,
        CBN_CLOSEUP = 8,
        CBN_SELENDOK = 9,
        CBS_SIMPLE = 0x0001,
        CBS_DROPDOWN = 0x0002,
        CBS_DROPDOWNLIST = 0x0003,
        CBS_OWNERDRAWFIXED = 0x0010,
        CBS_OWNERDRAWVARIABLE = 0x0020,
        CBS_AUTOHSCROLL = 0x0040,
        CBS_HASSTRINGS = 0x0200,
        CBS_NOINTEGRALHEIGHT = 0x0400,
        CB_GETEDITSEL = 0x0140,
        CB_LIMITTEXT = 0x0141,
        CB_SETEDITSEL = 0x0142,
        CB_ADDSTRING = 0x0143,
        CB_DELETESTRING = 0x0144,
        CB_GETCURSEL = 0x0147,
        CB_GETLBTEXT = 0x0148,
        CB_GETLBTEXTLEN = 0x0149,
        CB_INSERTSTRING = 0x014A,
        CB_RESETCONTENT = 0x014B,
        CB_FINDSTRING = 0x014C,
        CB_SETCURSEL = 0x014E,
        CB_SHOWDROPDOWN = 0x014F,
        CB_GETITEMDATA = 0x0150,
        CB_SETITEMHEIGHT = 0x0153,
        CB_GETITEMHEIGHT = 0x0154,
        CB_GETDROPPEDSTATE = 0x0157,
        CB_FINDSTRINGEXACT = 0x0158,
        CB_GETDROPPEDWIDTH = 0x015F,
        CB_SETDROPPEDWIDTH = 0x0160,
        CDRF_DODEFAULT = 0x00000000,
        CDRF_NEWFONT = 0x00000002,
        CDRF_SKIPDEFAULT = 0x00000004,
        CDRF_NOTIFYPOSTPAINT = 0x00000010,
        CDRF_NOTIFYITEMDRAW = 0x00000020,
        CDRF_NOTIFYSUBITEMDRAW = CDRF_NOTIFYITEMDRAW,
        CDDS_PREPAINT = 0x00000001,
        CDDS_POSTPAINT = 0x00000002,
        CDDS_ITEM = 0x00010000,
        CDDS_SUBITEM = 0x00020000,
        CDDS_ITEMPREPAINT = (0x00010000 | 0x00000001),
        CDDS_ITEMPOSTPAINT = (0x00010000 | 0x00000002),
        CDIS_SELECTED = 0x0001,
        CDIS_GRAYED = 0x0002,
        CDIS_DISABLED = 0x0004,
        CDIS_CHECKED = 0x0008,
        CDIS_FOCUS = 0x0010,
        CDIS_DEFAULT = 0x0020,
        CDIS_HOT = 0x0040,
        CDIS_MARKED = 0x0080,
        CDIS_INDETERMINATE = 0x0100,
        CDIS_SHOWKEYBOARDCUES = 0x0200,
        CLR_NONE = unchecked((int)0xFFFFFFFF),
        CLR_DEFAULT = unchecked((int)0xFF000000),
        CCM_SETVERSION = (0x2000 + 0x7),
        CCM_GETVERSION = (0x2000 + 0x8),
        CCS_NORESIZE = 0x00000004,
        CCS_NOPARENTALIGN = 0x00000008,
        CCS_NODIVIDER = 0x00000040,
        CBEM_INSERTITEMA = (0x0400 + 1),
        CBEM_GETITEMA = (0x0400 + 4),
        CBEM_SETITEMA = (0x0400 + 5),
        CBEM_INSERTITEMW = (0x0400 + 11),
        CBEM_SETITEMW = (0x0400 + 12),
        CBEM_GETITEMW = (0x0400 + 13),
        CBEN_ENDEDITA = ((0 - 800) - 5),
        CBEN_ENDEDITW = ((0 - 800) - 6),
        CONNECT_E_NOCONNECTION = unchecked((int)0x80040200),
        CONNECT_E_CANNOTCONNECT = unchecked((int)0x80040202),
        CTRLINFO_EATS_RETURN = 1,
        CTRLINFO_EATS_ESCAPE = 2,
        CSIDL_DESKTOP = 0x0000,        // <desktop>
        CSIDL_INTERNET = 0x0001,        // Internet Explorer (icon on desktop)
        CSIDL_PROGRAMS = 0x0002,        // Start Menu\Programs
        CSIDL_PERSONAL = 0x0005,        // My Documents
        CSIDL_FAVORITES = 0x0006,        // <user name>\Favorites
        CSIDL_STARTUP = 0x0007,        // Start Menu\Programs\Startup
        CSIDL_RECENT = 0x0008,        // <user name>\Recent
        CSIDL_SENDTO = 0x0009,        // <user name>\SendTo
        CSIDL_STARTMENU = 0x000b,        // <user name>\Start Menu
        CSIDL_DESKTOPDIRECTORY = 0x0010,        // <user name>\Desktop
        CSIDL_TEMPLATES = 0x0015,
        CSIDL_APPDATA = 0x001a,        // <user name>\Application Data
        CSIDL_LOCAL_APPDATA = 0x001c,        // <user name>\Local Settings\Applicaiton Data (non roaming)
        CSIDL_INTERNET_CACHE = 0x0020,
        CSIDL_COOKIES = 0x0021,
        CSIDL_HISTORY = 0x0022,
        CSIDL_COMMON_APPDATA = 0x0023,        // All Users\Application Data
        CSIDL_SYSTEM = 0x0025,        // GetSystemDirectory()
        CSIDL_PROGRAM_FILES = 0x0026,        // C:\Program Files
        CSIDL_PROGRAM_FILES_COMMON = 0x002b;        // C:\Program Files\Common

        public const int DUPLICATE = 0x06,
        DISPID_UNKNOWN = (-1),
        DISPID_PROPERTYPUT = (-3),
        DISPATCH_METHOD = 0x1,
        DISPATCH_PROPERTYGET = 0x2,
        DISPATCH_PROPERTYPUT = 0x4,
        DV_E_DVASPECT = unchecked((int)0x8004006B),
        DISP_E_MEMBERNOTFOUND = unchecked((int)0x80020003),
        DISP_E_PARAMNOTFOUND = unchecked((int)0x80020004),
        DISP_E_EXCEPTION = unchecked((int)0x80020009),
        DEFAULT_GUI_FONT = 17,
        DIB_RGB_COLORS = 0,
        DRAGDROP_E_NOTREGISTERED = unchecked((int)0x80040100),
        DRAGDROP_E_ALREADYREGISTERED = unchecked((int)0x80040101),
        DUPLICATE_SAME_ACCESS = 0x00000002,
        DFC_CAPTION = 1,
        DFC_MENU = 2,
        DFC_SCROLL = 3,
        DFC_BUTTON = 4,
        DFCS_CAPTIONCLOSE = 0x0000,
        DFCS_CAPTIONMIN = 0x0001,
        DFCS_CAPTIONMAX = 0x0002,
        DFCS_CAPTIONRESTORE = 0x0003,
        DFCS_CAPTIONHELP = 0x0004,
        DFCS_MENUARROW = 0x0000,
        DFCS_MENUCHECK = 0x0001,
        DFCS_MENUBULLET = 0x0002,
        DFCS_SCROLLUP = 0x0000,
        DFCS_SCROLLDOWN = 0x0001,
        DFCS_SCROLLLEFT = 0x0002,
        DFCS_SCROLLRIGHT = 0x0003,
        DFCS_SCROLLCOMBOBOX = 0x0005,
        DFCS_BUTTONCHECK = 0x0000,
        DFCS_BUTTONRADIO = 0x0004,
        DFCS_BUTTON3STATE = 0x0008,
        DFCS_BUTTONPUSH = 0x0010,
        DFCS_INACTIVE = 0x0100,
        DFCS_PUSHED = 0x0200,
        DFCS_CHECKED = 0x0400,
        DFCS_FLAT = 0x4000,
        DCX_WINDOW = 0x00000001,
        DCX_CACHE = 0x00000002,
        DCX_LOCKWINDOWUPDATE = 0x00000400,
        DCX_INTERSECTRGN = 0x00000080,
        DI_NORMAL = 0x0003,
        DLGC_WANTARROWS = 0x0001,
        DLGC_WANTTAB = 0x0002,
        DLGC_WANTALLKEYS = 0x0004,
        DLGC_WANTCHARS = 0x0080,
        DLGC_WANTMESSAGE = 0x0004,      /* Pass message to control          */
        DLGC_HASSETSEL = 0x0008,      /* Understands EM_SETSEL message    */
        DTM_GETSYSTEMTIME = (0x1000 + 1),
        DTM_SETSYSTEMTIME = (0x1000 + 2),
        DTM_SETRANGE = (0x1000 + 4),
        DTM_SETFORMATA = (0x1000 + 5),
        DTM_SETFORMATW = (0x1000 + 50),
        DTM_SETMCCOLOR = (0x1000 + 6),
        DTM_GETMONTHCAL = (0x1000 + 8),
        DTM_SETMCFONT = (0x1000 + 9),
        DTS_UPDOWN = 0x0001,
        DTS_SHOWNONE = 0x0002,
        DTS_LONGDATEFORMAT = 0x0004,
        DTS_TIMEFORMAT = 0x0009,
        DTS_RIGHTALIGN = 0x0020,
        DTN_DATETIMECHANGE = ((0 - 760) + 1),
        DTN_USERSTRINGA = ((0 - 760) + 2),
        DTN_USERSTRINGW = ((0 - 760) + 15),
        DTN_WMKEYDOWNA = ((0 - 760) + 3),
        DTN_WMKEYDOWNW = ((0 - 760) + 16),
        DTN_FORMATA = ((0 - 760) + 4),
        DTN_FORMATW = ((0 - 760) + 17),
        DTN_FORMATQUERYA = ((0 - 760) + 5),
        DTN_FORMATQUERYW = ((0 - 760) + 18),
        DTN_DROPDOWN = ((0 - 760) + 6),
        DTN_CLOSEUP = ((0 - 760) + 7),
        DVASPECT_CONTENT = 1,
        DVASPECT_TRANSPARENT = 32,
        DVASPECT_OPAQUE = 16;

        public const int
        HC_ACTION = 0,
        HC_GETNEXT = 1,
        HC_SKIP = 2,
        HTCAPTION = 2,
        HTTRANSPARENT = (-1),
        HTNOWHERE = 0,
        HTCLIENT = 1,
        HTLEFT = 10,
        HTBOTTOM = 15,
        HTBOTTOMLEFT = 16,
        HTBOTTOMRIGHT = 17,
        HTBORDER = 18,
        HELPINFO_WINDOW = 0x0001,
        HCF_HIGHCONTRASTON = 0x00000001,
        HDI_ORDER = 0x0080,
        HDI_WIDTH = 0x0001,
        HDM_GETITEMCOUNT = (0x1200 + 0),
        HDM_INSERTITEMA = (0x1200 + 1),
        HDM_INSERTITEMW = (0x1200 + 10),
        HDM_GETITEMA = (0x1200 + 3),
        HDM_GETITEMW = (0x1200 + 11),
        HDM_LAYOUT = (0x1200 + 5),
        HDM_SETITEMA = (0x1200 + 4),
        HDM_SETITEMW = (0x1200 + 12),
        HDN_ITEMCHANGINGA = ((0 - 300) - 0),
        HDN_ITEMCHANGINGW = ((0 - 300) - 20),
        HDN_ITEMCHANGEDA = ((0 - 300) - 1),
        HDN_ITEMCHANGEDW = ((0 - 300) - 21),
        HDN_ITEMCLICKA = ((0 - 300) - 2),
        HDN_ITEMCLICKW = ((0 - 300) - 22),
        HDN_ITEMDBLCLICKA = ((0 - 300) - 3),
        HDN_ITEMDBLCLICKW = ((0 - 300) - 23),
        HDN_DIVIDERDBLCLICKA = ((0 - 300) - 5),
        HDN_DIVIDERDBLCLICKW = ((0 - 300) - 25),
        HDN_BEGINTDRAG = ((0 - 300) - 10),
        HDN_BEGINTRACKA = ((0 - 300) - 6),
        HDN_BEGINTRACKW = ((0 - 300) - 26),
        HDN_ENDDRAG = ((0 - 300) - 11),
        HDN_ENDTRACKA = ((0 - 300) - 7),
        HDN_ENDTRACKW = ((0 - 300) - 27),
        HDN_TRACKA = ((0 - 300) - 8),
        HDN_TRACKW = ((0 - 300) - 28),
        HDN_GETDISPINFOA = ((0 - 300) - 9),
        HDN_GETDISPINFOW = ((0 - 300) - 29);

        public enum IDC : uint
        {
            ARROW = 32512,
            IBEAM = 32513,
            WAIT = 32514,
            CROSS = 32515,
            UPARROW = 32516,
            SIZE = 32640,
            ICON = 32641,
            SIZENWSE = 32642,
            SIZENESW = 32643,
            SIZEWE = 32644,
            SIZENS = 32645,
            SIZEALL = 32646,
            NO = 32648,
            HAND = 32649,
            APPSTARTING = 32650,
            HELP = 32651
        }
    }
}