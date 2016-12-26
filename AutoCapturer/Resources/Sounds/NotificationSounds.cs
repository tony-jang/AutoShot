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
            System.Media.SoundPlayer player;
            switch (soundtype)
            {
                case SoundType.AuCaModeOn:
                    player = new System.Media.SoundPlayer($"snd2.wav");
                    player.Play();
                    break;
                case SoundType.AuCaModeOff:
                    player = new System.Media.SoundPlayer($"snd1.wav");
                    player.Play();
                    break;
                case SoundType.Captured:
                    player = new System.Media.SoundPlayer("CamSound.wav");
                    player.Play();
                    break;
            }
        }
        
        public enum SoundType
        {
            AuCaModeOn, AuCaModeOff, Captured
        }
    }
}
