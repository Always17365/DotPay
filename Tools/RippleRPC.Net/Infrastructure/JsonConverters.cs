using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;
using RippleRPC.Net.Model;
using RippleRPC.Net.Model.Paths;

namespace Newtonsoft.Json.Converters
{
    public class UnixDateTimeConverter : DateTimeConverterBase
    {
        private DateTime initialTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                DateTime currentValue = ((DateTime)value).ToUniversalTime();
                TimeSpan span = currentValue - initialTime;
                writer.WriteValue(span.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            }
            else
                throw new ArgumentException("Must provide a valid datetime struct", "value");

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            long ticks = Convert.ToInt64(reader.Value);
            return initialTime.AddSeconds(ticks);
        }

    }
   
    public class PathConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is PathSummary)
            {
                if (value != null)
                    writer.WriteValue(JsonConvert.SerializeObject(value));
            }

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            var vals = serializer.Deserialize(reader);
            if (vals is JArray)
            {
                if (((JArray)vals).Count > 0)
                {
                    var retObjects = new List<List<object>>();
                    JArray jArray = (JArray)vals;

                    foreach (JToken val in jArray)
                    {
                        if (val is JArray)
                        {
                            var subObjects = new List<object>();

                            foreach (JToken subval in val)
                            {
                                var op_success = false;

                                if (!op_success)
                                    try
                                    {
                                        var obj = subval.ToObject<PathAccount>();
                                        if (obj != null && !string.IsNullOrEmpty(obj.Account))
                                        {
                                            subObjects.Add(obj);
                                            op_success = true;
                                        }
                                    }
                                    catch (JsonSerializationException) { }

                                if (!op_success)
                                    try
                                    {
                                        var obj = subval.ToObject<PathCurrency>();
                                        if (obj != null && !string.IsNullOrEmpty(obj.Currency))
                                            subObjects.Add(obj);
                                    }
                                    catch (JsonSerializationException) { }
                            }

                            retObjects.Add(subObjects);
                        }
                    }

                    return retObjects;
                }
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }

    public class RippleValueConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            double val = ((double)value) * 1000000;
            writer.WriteValue("\"" + val.ToString(CultureInfo.InvariantCulture) + "\"");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            double val = Convert.ToDouble(reader.Value);
            return val / 1000000;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(double))
                return true;
            return false;
        }
    }
    public class RippleCurrencyValueConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var cv = value as RippleCurrencyValue;
            if (cv != null)
            {
                if (cv.Currency.ToUpper() != "XRP")
                    writer.WriteValue(JsonConvert.SerializeObject(cv));
                else
                {
                    var val = cv.Value * 1000000;
                    writer.WriteValue(val.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = default(RippleCurrencyValue);

            if (reader.Value != null)
            {
                decimal val = 0;
                if (decimal.TryParse(reader.Value.ToString(), out val))
                {
                    result = new RippleCurrencyValue { _Value = (val / 1000000).ToString(), Currency = "XRP", Issuer = string.Empty };
                }
            }
            else
            {
                result = serializer.Deserialize<RippleCurrencyValue>(reader);
            }
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(double))
                return true;
            return false;
        }
    }
    public class OfferItemConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return serializer.Deserialize<RippleCurrencyValue>(reader);
            }
            catch (JsonSerializationException) { }

            try
            {
                double val = serializer.Deserialize<double>(reader);
                return val / 1000000;
            }
            catch (JsonSerializationException) { }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(object))
                return true;
            return false;
        }
    }

    public class JsonEnumTypeConverter<T> : JsonConverter
    {

        //adapted from: http://stackoverflow.com/questions/794838/datacontractjsonserializer-and-enums/794962#794962
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((T)value).ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Enum.Parse(typeof(T), reader.Value.ToString(), true);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }
    }
}
