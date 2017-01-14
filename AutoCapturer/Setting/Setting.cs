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
    class Setting
    {
        public Setting()
        {

        }

        // 캡쳐 설정

        // 자동 캡쳐 활성화 시
        AuCaEnableSelection AutoCaptureEnableSelection;

        // => 팝업으로 띄우기
        int PopupCountSec;

        // 개인 설정
        User ActivatedUser;
        User[] Users;   
    }


    public enum AuCaEnableSelection
    {
        None = 0,
        SoundPlay = 1,
        OpenPopup = 2,
        NoAlarm = 3
    }

    public enum HowtoSaveGetPicture
    {
        None = 0,
        Ask = 1,
        PatternEnd = 2,
        NoUse = 3
    }

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
            if (obj.GetType() == typeof(Int32))
            {
                if (Value == (Int32)obj) return Value.CompareTo(obj);
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
            if (Value == (Int32)other) return Value.CompareTo(other);
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
    

}
