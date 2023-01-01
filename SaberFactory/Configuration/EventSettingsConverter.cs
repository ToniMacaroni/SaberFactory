using System.IO;
using System.Text;
using IPA.Config.Data;
using IPA.Config.Stores;
using Newtonsoft.Json;
using SaberFactory.Serialization;
using SaberFactory.UI.Flow;

namespace SaberFactory.Configuration;

internal class EventSettingsConverter : ValueConverter<EventSettings>
{
    public override Value ToValue(EventSettings obj, object parent)
    {
        // serialize object using JsonSerializer
        //var str = new StringBuilder()
        var str = new StringBuilder();
        using var writer = new JsonTextWriter(new StringWriter(str));
        Serializer.JsonSerializer.Serialize(writer, obj);
        return new Text(str.ToString());
    }

    public override EventSettings FromValue(Value value, object parent)
    {
        if (!(value is Text text))
        {
            return new();
        }

        using var reader = new JsonTextReader(new StringReader(text.Value));
        return Serializer.JsonSerializer.Deserialize<EventSettings>(reader);
    }
}