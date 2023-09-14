using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.Internal;

namespace RPCServer
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WriteEndObject();
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                float x = 0, y = 0;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = reader.Value.ToString();
                        if (propertyName.Equals("x"))
                        {
                            reader.Read();
                            x = Convert.ToSingle(reader.Value);
                        }
                        else if (propertyName.Equals("y"))
                        {
                            reader.Read();
                            y = Convert.ToSingle(reader.Value);
                        }
                    }
                    else if (reader.TokenType == JsonToken.EndObject)
                    {
                        return new Vector2(x, y);
                    }
                }
            }

            return Vector2.zero;
        }
    }
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("z");
            writer.WriteValue(value.z);
            writer.WriteEndObject();
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                float x = 0, y = 0, z = 0;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = reader.Value.ToString();
                        if (propertyName.Equals("x"))
                        {
                            reader.Read();
                            x = Convert.ToSingle(reader.Value);
                        }
                        else if (propertyName.Equals("y"))
                        {
                            reader.Read();
                            y = Convert.ToSingle(reader.Value);
                        }
                        else if (propertyName.Equals("z"))
                        {
                            reader.Read();
                            z = Convert.ToSingle(reader.Value);
                        }
                    }
                    else if (reader.TokenType == JsonToken.EndObject)
                    {
                        return new Vector3(x, y, z);
                    }
                }
            }

            return Vector3.zero;
        }
    }
    public class Vector4Converter : JsonConverter<Vector4>
    {
        public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("z");
            writer.WriteValue(value.z);
            writer.WritePropertyName("w");
            writer.WriteValue(value.w);
            writer.WriteEndObject();
        }

        public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                float x = 0, y = 0, z = 0, w = 0;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = reader.Value.ToString();
                        if (propertyName.Equals("x"))
                        {
                            reader.Read();
                            x = Convert.ToSingle(reader.Value);
                        }
                        else if (propertyName.Equals("y"))
                        {
                            reader.Read();
                            y = Convert.ToSingle(reader.Value);
                        }
                        else if (propertyName.Equals("z"))
                        {
                            reader.Read();
                            z = Convert.ToSingle(reader.Value);
                        }
                        else if (propertyName.Equals("w"))
                        {
                            reader.Read();
                            w = Convert.ToSingle(reader.Value);
                        }
                    }
                    else if (reader.TokenType == JsonToken.EndObject)
                    {
                        return new Vector4(x, y, z, w);
                    }
                }
            }

            return Vector4.zero;
        }
    }
    public class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("r");
            writer.WriteValue(value.r);
            writer.WritePropertyName("g");
            writer.WriteValue(value.g);
            writer.WritePropertyName("b");
            writer.WriteValue(value.b);
            writer.WritePropertyName("a");
            writer.WriteValue(value.a);
            writer.WriteEndObject();
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                float r = 0, g = 0, b = 0, a = 1;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = reader.Value.ToString();
                        if (propertyName.Equals("r"))
                        {
                            reader.Read();
                            r = Convert.ToSingle(reader.Value);
                        }
                        else if (propertyName.Equals("g"))
                        {
                            reader.Read();
                            g = Convert.ToSingle(reader.Value);
                        }
                        else if (propertyName.Equals("b"))
                        {
                            reader.Read();
                            b = Convert.ToSingle(reader.Value);
                        }
                        else if (propertyName.Equals("a"))
                        {
                            reader.Read();
                            a = Convert.ToSingle(reader.Value);
                        }
                    }
                    else if (reader.TokenType == JsonToken.EndObject)
                    {
                        return new Color(r, g, b, a);
                    }
                }
            }

            return Color.white;
        }
    }
    public class Color32Converter : JsonConverter<Color32>
    {
        public override void WriteJson(JsonWriter writer, Color32 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("r");
            writer.WriteValue(value.r);
            writer.WritePropertyName("g");
            writer.WriteValue(value.g);
            writer.WritePropertyName("b");
            writer.WriteValue(value.b);
            writer.WritePropertyName("a");
            writer.WriteValue(value.a);
            writer.WriteEndObject();
        }

        public override Color32 ReadJson(JsonReader reader, Type objectType, Color32 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                byte r = 0, g = 0, b = 0, a = 255;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = reader.Value.ToString();
                        if (propertyName.Equals("r"))
                        {
                            reader.Read();
                            r = Convert.ToByte(reader.Value);
                        }
                        else if (propertyName.Equals("g"))
                        {
                            reader.Read();
                            g = Convert.ToByte(reader.Value);
                        }
                        else if (propertyName.Equals("b"))
                        {
                            reader.Read();
                            b = Convert.ToByte(reader.Value);
                        }
                        else if (propertyName.Equals("a"))
                        {
                            reader.Read();
                            a = Convert.ToByte(reader.Value);
                        }
                    }
                    else if (reader.TokenType == JsonToken.EndObject)
                    {
                        return new Color32(r, g, b, a);
                    }
                }
            }

            return new Color32(0, 0, 0, 255);
        }
    }
    public class QuaternionConverter : JsonConverter<Quaternion>
    {
        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("z");
            writer.WriteValue(value.z);
            writer.WritePropertyName("w");
            writer.WriteValue(value.w);
            writer.WriteEndObject();
        }

        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                float x = 0, y = 0, z = 0, w = 1;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = reader.Value.ToString();
                        if (propertyName.Equals("x"))
                        {
                            reader.Read();
                            x = Convert.ToSingle(reader.Value);
                        }
                        else if (propertyName.Equals("y"))
                        {
                            reader.Read();
                            y = Convert.ToSingle(reader.Value);
                        }
                        else if (propertyName.Equals("z"))
                        {
                            reader.Read();
                            z = Convert.ToSingle(reader.Value);
                        }
                        else if (propertyName.Equals("w"))
                        {
                            reader.Read();
                            w = Convert.ToSingle(reader.Value);
                        }
                    }
                    else if (reader.TokenType == JsonToken.EndObject)
                    {
                        return new Quaternion(x, y, z, w);
                    }
                }
            }

            return Quaternion.identity;
        }
    }
    public class TransformConverter : JsonConverter<Transform>
    {
        public override void WriteJson(JsonWriter writer, Transform value, JsonSerializer serializer)
        {
            // 序列化 Transform 中的位置、旋转和缩放信息
            writer.WriteStartObject();
            writer.WritePropertyName("position");
            writer.WriteValue(value.position);
            writer.WritePropertyName("rotation");
            writer.WriteValue(value.rotation);
            writer.WritePropertyName("scale");
            writer.WriteValue(value.localScale);
            writer.WriteEndObject();
        }

        public override Transform ReadJson(JsonReader reader, Type objectType, Transform existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                Vector3 position = Vector3.zero;
                Quaternion rotation = Quaternion.identity;
                Vector3 scale = Vector3.one;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = reader.Value.ToString();
                        if (propertyName.Equals("position"))
                        {
                            reader.Read();
                            position = JsonConvert.DeserializeObject<Vector3>(reader.Value.ToString());
                        }
                        else if (propertyName.Equals("rotation"))
                        {
                            reader.Read();
                            rotation = JsonConvert.DeserializeObject<Quaternion>(reader.Value.ToString());
                        }
                        else if (propertyName.Equals("scale"))
                        {
                            reader.Read();
                            scale = JsonConvert.DeserializeObject<Vector3>(reader.Value.ToString());
                        }
                    }
                    else if (reader.TokenType == JsonToken.EndObject)
                    {
                        // 创建一个新的 Transform，并应用位置、旋转和缩放信息
                        GameObject gameObject = new GameObject();
                        Transform transform = gameObject.transform;
                        transform.position = position;
                        transform.rotation = rotation;
                        transform.localScale = scale;
                        return transform;
                    }
                }
            }

            return null;
        }
    }
    public class GameObjectConverter : JsonConverter<GameObject>
    {
        public override void WriteJson(JsonWriter writer, GameObject value, JsonSerializer serializer)
        {
            // 序列化 GameObject 的名称
            writer.WriteValue(value.name);
        }

        public override GameObject ReadJson(JsonReader reader, Type objectType, GameObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string gameObjectName = reader.Value.ToString();
                GameObject gameObject = new GameObject(gameObjectName);
                return gameObject;
            }

            return null;
        }
    }
}