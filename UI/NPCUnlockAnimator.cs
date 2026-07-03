using Microsoft.Xna.Framework;
using NPCUnlockAnnouncer.Systems;

namespace NPCUnlockAnnouncer.UI
{
    public enum BannerState
    {
        Hidden,
        Entering,
        Showing,
        Exiting
    }

    public class NPCUnlockAnimator
    {
        public BannerState State { get; private set; } = BannerState.Hidden;

        private const int EnterDuration = 20;
        private int ShowDuration => (AnnouncerConfig.Instance?.DurationSeconds ?? 5) * 60;
        private const int ExitDuration = 20;

        private int timer;
        private float progress;

        private readonly float hiddenY = -100f; 
        private readonly float visibleY = 30f;

        public float CurrentY { get; private set; }

        public bool IsActive => State != BannerState.Hidden;

        public void Start()
        {
            State = BannerState.Entering;
            timer = 0;
        }

        public void Update()
        {
            switch (State)
            {
                case BannerState.Entering:
                    timer++;
                    progress = timer / (float)EnterDuration;
                    progress = MathHelper.SmoothStep(0, 1, progress); 
                    CurrentY = MathHelper.Lerp(hiddenY, visibleY, progress);

                    if (timer >= EnterDuration)
                    {
                        State = BannerState.Showing;
                        timer = 0;
                    }
                    break;

                case BannerState.Showing:
                    timer++;
                    CurrentY = visibleY;

                    if (timer >= ShowDuration)
                    {
                        State = BannerState.Exiting;
                        timer = 0;
                    }
                    break;

                case BannerState.Exiting:
                    timer++;
                    progress = timer / (float)ExitDuration;
                    progress = MathHelper.SmoothStep(0, 1, progress);
                    CurrentY = MathHelper.Lerp(visibleY, hiddenY, progress);

                    if (timer >= ExitDuration)
                    {
                        State = BannerState.Hidden;
                    }
                    break;
            }
        }
    }
}