using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace AutoShot.Sounds
{
    static class NotificationSounds
    {
        public static void PlayNotificationSound(SoundType soundtype)
        {
            SoundPlayer player;
            Stream str;

            switch (soundtype)
            {
                case SoundType.AuCaModeOn:
                    str = Properties.Resources.snd2;
                    break;
                case SoundType.AuCaModeOff:
                    str = Properties.Resources.snd1;
                    break;
                case SoundType.Captured:
                    str = Properties.Resources.CamSound;
                    break;
                default:
                    return;
            }
            player = new SoundPlayer(str);
            player.Play();
        }
        
        public enum SoundType
        {
            AuCaModeOn, AuCaModeOff, Captured
        }
    }
}
