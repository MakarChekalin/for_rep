using OpenTelemetry;
using System.Diagnostics;

namespace Atm.ServiceDefaults;

public class BaggageTagActivityProcessor : BaseProcessor<Activity>
{
    public override void OnStart(Activity activity)
    {
        foreach (KeyValuePair<string, string?> entry in activity.Baggage)
        {
            if (entry.Value != null)
                activity.SetTag(entry.Key, entry.Value);
        }
    }
}
