namespace tweetz5.Utilities.System
{
    public enum PowerMgmt
    {
        StandBy,
        Off,
        On
    }

    public class ScreenPowerMgmtEventArgs
    {
        public ScreenPowerMgmtEventArgs(PowerMgmt powerStat)
        {
            PowerStatus = powerStat;
        }
        public PowerMgmt PowerStatus { get; }
    }

    public class ScreenPowerManagement
    {
        public delegate void ScreenPowerMgmtEventHandler(object sender, ScreenPowerMgmtEventArgs e);
        public event ScreenPowerMgmtEventHandler ScreenPower;
        private void OnScreenPowerMgmtEvent(ScreenPowerMgmtEventArgs args)
        {
            ScreenPower?.Invoke(this, args);
        }
        public void SwitchMonitorOff()
        {
            /* The code to switch off */
            OnScreenPowerMgmtEvent(new ScreenPowerMgmtEventArgs(PowerMgmt.Off));
        }
        public void SwitchMonitorOn()
        {
            /* The code to switch on */
            OnScreenPowerMgmtEvent(new ScreenPowerMgmtEventArgs(PowerMgmt.On));
        }
        public void SwitchMonitorStandby()
        {
            /* The code to switch standby */
            OnScreenPowerMgmtEvent(new ScreenPowerMgmtEventArgs(PowerMgmt.StandBy));
        }
    }
}
