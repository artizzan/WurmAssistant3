namespace AldursLab.WurmApi
{
    public enum LogType
    {
        Unspecified = 0,
        Combat,
        Event,
        Friends,
        Local,
        Skills,
        Alliance,
        CaHelp,
        Freedom,
        GlFreedom,
        Mgmt,
        Pm,
        Team,
        Village,
        Deaths,
        MolRehan,
        JennKellon,
        GlMolRehan,
        GlJennKellon,
        Hots,
        GlHots,
        Trade,
        Support,
        Help,
        AllLogs // special flag, should indicate all other log types except Unspecified
    }
}