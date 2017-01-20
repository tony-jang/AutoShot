using AutoCapturer.Converter;
using AutoCapturer.Effectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoCapturer.Setting
{
    public class Setting
    {
        public delegate void SettingChangeHandler();

        public event SettingChangeHandler SettingChange;
        

        public Setting()
        {

        }

        #region [ 캡쳐 설정 ]
        
        #region [ 캡쳐 설정 - 자동 캡쳐 활성화 시 ]

        private AuCaEnableSelection _AutoCaptureEnableSelection;
        /// <summary>
        /// 자동 캡쳐 활성화 시 알림 선택 방식을 말합니다.
        /// </summary>
        public AuCaEnableSelection AutoCaptureEnableSelection
        {
            get { return _AutoCaptureEnableSelection; }
            set { _AutoCaptureEnableSelection = value; SettingChange(); }
        }

        private LimitInt _PopupCountSec = new LimitInt(1, 3, 1);
        /// <summary>
        /// 팝업으로 띄우기시 나타낼 초를 나타냅니다 최소 1, 최대 3입니다.
        /// </summary>
        public int PopupCountSecond
        {
            get { return _PopupCountSec.Value; }
            set { _PopupCountSec.Value = value; SettingChange(); }
        }

        #endregion

        private LimitInt _AllCaptureCountDown = new LimitInt(0, 5, 0);
        /// <summary>
        /// 전체 캡쳐 시 카운트 다운
        /// </summary>
        public int AllCaptureCountDown
        {
            get { return _AllCaptureCountDown.Value; }
            set { _AllCaptureCountDown.Value = value; SettingChange(); }
        }

        private HowtoSaveGetPicture _ImageFromURLSave = HowtoSaveGetPicture.Ask;
        /// <summary>
        /// 이미지를 URL로 부터 가져올때 저장 방법입니다.
        /// </summary>
        public HowtoSaveGetPicture ImageFromURLSave
        {
            get { return _ImageFromURLSave; }
            set { _ImageFromURLSave = value; SettingChange(); }
        }

        private HowtoSaveGetPicture _ImageFromImageTag = HowtoSaveGetPicture.Ask;
        /// <summary>
        /// 이미지를 Image Tag로 부터 가져올때 저장 방법입니다.
        /// </summary>
        public HowtoSaveGetPicture ImageFromImageTag
        {
            get { return _ImageFromImageTag; }
            set { _ImageFromImageTag = value; SettingChange(); }
        }
        #endregion


        #region [ 개인 설정 ]

        private User _ActivatedUser;
        /// <summary>
        /// 현재 활성화된 유저를 나타냅니다.
        /// </summary>
        public User ActivatedUser
        {
            get { return _ActivatedUser; }
            set { _ActivatedUser = value; SettingChange(); }
        }
        private User[] _Users;
        /// <summary>
        /// 설정에 저장된 유저 목록을 나타냅니다.
        /// </summary>
        public User[] Users
        {
            get { return _Users; }
            set { _Users = value; SettingChange(); }
        }
        #endregion







        #region [ 패턴 관리 ]


        public SavePattern DefaultPattern { get; set; } = new SavePattern("저장본 (%s)", "%d");

        List<SavePattern> _Patterns = new List<SavePattern>();
        public List<SavePattern> Patterns
        {
            get { return _Patterns; }
            set { _Patterns = value; }
        }

        #endregion

    }


    /// <summary>
    /// 자동 캡쳐 활성화 시 안내 방법입니다.
    /// </summary>
    public enum AuCaEnableSelection
    {
        None = 0,
        SoundPlay = 1,
        OpenPopup = 2,
        SoundAndPopUp= 3,
        NoRing = 4
    }

    /// <summary>
    /// 사진을 저장할때에 어떻게 저장할지에 대한 방법입니다.
    /// </summary>
    public enum HowtoSaveGetPicture
    {
        None = 0,
        Ask = 1,
        PatternSave = 2,
        NoUse = 3
    }

    /// <summary>
    /// AutoCapture를 사용할 유저에 대해서 나타냅니다.
    /// </summary>
    public struct User
    {
        public string UserName;
        public BitmapSource UserImg;
    }

    /// <summary>
    /// 최대, 최솟값이 있는 int 형식입니다.
    /// </summary>
    public struct LimitInt :IComparable, IComparable<Int32>, IEquatable<Int32>
    {
        public LimitInt(int min = 0, int max = 10, int value = 5)
        {
            _Minimum = int.MinValue;
            _Maximum = int.MaxValue;
            _Value = 0;

            
            Minimum = min;
            Maximum = max;
            Value = value;
        }
        public static bool operator ==(LimitInt LeftExp, LimitInt RightExp)
        {
            if (LeftExp.Value == RightExp.Value && LeftExp.Minimum == RightExp.Minimum && LeftExp.Maximum == RightExp.Maximum) return true;

            return false;
        }
        public static bool operator !=(LimitInt LeftExp, LimitInt RightExp)
        {
            if (LeftExp.Value != RightExp.Value || LeftExp.Minimum != RightExp.Minimum || LeftExp.Maximum != RightExp.Maximum) return true;

            return false;
        }
        public static bool operator ==(LimitInt lint, int intdata)
        {
            if (lint.Value == intdata) return true;

            return false;
        }
        public static bool operator !=(LimitInt lint, int intdata)
        {
            if (lint.Value != intdata) return true;

            return false;
        }
        

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() == typeof(int))
            {
                if (Value == (int)obj) return Value.CompareTo(obj);
            }
            if (obj.GetType() == typeof(LimitInt))
            {
                if (this == (LimitInt)obj) return Value.CompareTo(((LimitInt)obj).Value);
            }

            throw new ArgumentException("Object는 int나 LimitInt가 아닙니다.");
        }

        public bool Equals(int other)
        {
            this.Value = other;
            return true;
        }

        public int CompareTo(int other)
        {
            if (Value == other) return Value.CompareTo(other);
            throw new ArgumentException("Object는 int가 아닙니다.");
        }

        private int _Value;
        public int Value
        {
            get { return _Value; }
            set
            {
                if (value > Maximum) value = Maximum;
                if (value < Minimum) value = Minimum;

                _Value = value;
            }
        }


        private int _Minimum;
        public int Minimum
        {
            get { return _Minimum; }
            set
            {
                if (value > _Maximum) value = _Maximum;
                _Minimum = value;
            }
        }

        private int _Maximum;
        public int Maximum
        {
            get { return _Maximum; }
            set
            {
                if (value < _Minimum) value = _Minimum;
                _Maximum = value;
            }
        }
    }
    

    public struct SavePattern
    {
        
        public SavePattern(string Name, string Location, bool openeffector = false, bool overwrite = false,
                           bool putlogo = false)
        {
            PatternName = Name;
            SaveLocation = Location;
            OpenEffector = openeffector;
            OverWrite = overwrite;
            PutLogo = putlogo;
            SetAutoEffect = new NullBaseEffector();
        }

        public string RealSaveName
        {
            get
            {
                string str;
                PatternConverter.ConvertAll(PatternName, out str);
                return str;
            }
        }
        public string RealSaveLocation
        {
            get
            {
                string str;
                LocationConverter.ConvertAll(SaveLocation, out str);
                return str;
            }
        }

        public string PatternName;
        public string SaveLocation;
        
        public bool OpenEffector;
        public bool OverWrite;
        public bool PutLogo;
        BaseEffector SetAutoEffect;


    }

}
