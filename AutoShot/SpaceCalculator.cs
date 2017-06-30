using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoShot
{
    class SpaceCalculator
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="volumeName">불륨 이름입니다.</param>
        /// <param name="expectsize">예상하는 사이즈입니다. MB단위로 계산됩니다.</param>
        /// <param name="maxShowNumber">보여줄 수 있는 최대 크기를 의미합니다. 초과시 +로 표시됩니다.</param>
        public SpaceCalculator(string volumeName, long expectsize, int maxShowNumber)
        {
            _ExpectImageSize = expectsize;
            Volume = volumeName;
            _MaxShowNumber = maxShowNumber;
            
        }
        public SpaceCalculator(string volumeName)
            : this(volumeName, 1024, 9999)
        { }
        public SpaceCalculator(string volumeName, long expectsize)
            : this(volumeName, expectsize, 9999)
        { }
        public SpaceCalculator(string volumeName, int maxShowNumber)
            : this(volumeName, 1024, maxShowNumber)
        { }



        private int _MaxShowNumber = 9999;


        private string _Volume = ""; // C:\ D:\ ...
        public string Volume
        {
            get { return _Volume; }
            set
            {
                _Volume = value;
                foreach (DriveInfo di in DriveInfo.GetDrives())
                {
                    if (_Volume == di.Name)
                    {
                        _RemainSpace = di.TotalFreeSpace;
                        _RemainPicNum = di.TotalFreeSpace / _ExpectImageSize;
                        break;
                    }
                }
            }
        }
        private long _RemainSpace = 0; // 볼륨의 남은 크기

        private long _ExpectImageSize = 1024;
        public long ExpectImageSize
        {
            get { return _ExpectImageSize; }
            set
            {
                _ExpectImageSize = value;
                _RemainPicNum = _RemainSpace / _ExpectImageSize;
            }
        }

        private long _RemainPicNum = 0;
        /// <summary>
        /// 예상된 남은 사진 장 수 입니다.
        /// </summary>
        public long RemainPicNum
        {
            get { return _RemainPicNum; }
        }

        public string RemainPicNumText
        {
            get
            {
                if (_MaxShowNumber < RemainPicNum) { return _MaxShowNumber.ToString() + "+"; }
                return _RemainPicNum.ToString();
            }
        }
        
    }
}
