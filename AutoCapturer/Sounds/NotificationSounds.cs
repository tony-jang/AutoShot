using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCapturer.Sounds
{
    static class NotificationSounds
    {
        public static void PlayNotificationSound(SoundType soundtype)
        {
            if (soundtype == SoundType.AuCaModeOn)
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer($"snd2.wav");
                player.Play();
            }
            else if (soundtype == SoundType.AuCaModeOff)
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer($"snd1.wav");
                player.Play();
            }
        }
        
        public enum SoundType
        {
            AuCaModeOn, AuCaModeOff
        }
    }
}
