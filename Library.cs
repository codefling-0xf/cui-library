using Network;
using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Oxide.Plugins
{
    #region 0xF UI Library 2.3.1
    partial class YourPartialPluginClassName
    {
        public class CUI
        {
            public enum Font
            {
                RobotoCondensedBold,
                RobotoCondensedRegular,
                RobotoMonoRegular,
                DroidSansMono,
                PermanentMarker,
                PressStart2PRegular,
                NotoSansArabicBold
            }

            private static readonly Dictionary<Font, string> FontToString = new Dictionary<Font, string>
            {
                { Font.RobotoCondensedBold, "RobotoCondensed-Bold.ttf" },
                { Font.RobotoCondensedRegular, "RobotoCondensed-Regular.ttf" },
                { Font.RobotoMonoRegular, "RobotoMono-Regular.ttf" },
                { Font.DroidSansMono, "DroidSansMono.ttf" },
                { Font.PermanentMarker, "PermanentMarker.ttf" },
                { Font.PressStart2PRegular, "PressStart2P-Regular.ttf" },
                { Font.NotoSansArabicBold, "nonenglish/notosansarabic-bold.ttf" }
            };

            public enum InputType
            {
                None,
                Default,
                HudMenuInput
            }

            private static readonly Dictionary<TextAnchor, string> TextAnchorToString = new Dictionary<TextAnchor, string>
            {
                { TextAnchor.UpperLeft, TextAnchor.UpperLeft.ToString() },
                { TextAnchor.UpperCenter, TextAnchor.UpperCenter.ToString() },
                { TextAnchor.UpperRight, TextAnchor.UpperRight.ToString() },
                { TextAnchor.MiddleLeft, TextAnchor.MiddleLeft.ToString() },
                { TextAnchor.MiddleCenter, TextAnchor.MiddleCenter.ToString() },
                { TextAnchor.MiddleRight, TextAnchor.MiddleRight.ToString() },
                { TextAnchor.LowerLeft, TextAnchor.LowerLeft.ToString() },
                { TextAnchor.LowerCenter, TextAnchor.LowerCenter.ToString() },
                { TextAnchor.LowerRight, TextAnchor.LowerRight.ToString() }
            };

            private static readonly Dictionary<VerticalWrapMode, string> VWMToString = new Dictionary<VerticalWrapMode, string>
            {
                { VerticalWrapMode.Truncate, VerticalWrapMode.Truncate.ToString() },
                { VerticalWrapMode.Overflow, VerticalWrapMode.Overflow.ToString() },
            };

            private static readonly Dictionary<Image.Type, string> ImageTypeToString = new Dictionary<Image.Type, string>
            {
                { Image.Type.Simple, Image.Type.Simple.ToString() },
                { Image.Type.Sliced, Image.Type.Sliced.ToString() },
                { Image.Type.Tiled, Image.Type.Tiled.ToString() },
                { Image.Type.Filled, Image.Type.Filled.ToString() },
            };

            private static readonly Dictionary<InputField.LineType, string> LineTypeToString = new Dictionary<InputField.LineType, string>
            {
                { InputField.LineType.MultiLineNewline, InputField.LineType.MultiLineNewline.ToString() },
                { InputField.LineType.MultiLineSubmit, InputField.LineType.MultiLineSubmit.ToString() },
                { InputField.LineType.SingleLine, InputField.LineType.SingleLine.ToString() },
            };

            private static readonly Dictionary<ScrollRect.MovementType, string> MovementTypeToString = new Dictionary<ScrollRect.MovementType, string>
            {
                { ScrollRect.MovementType.Unrestricted, ScrollRect.MovementType.Unrestricted.ToString() },
                { ScrollRect.MovementType.Elastic, ScrollRect.MovementType.Elastic.ToString() },
                { ScrollRect.MovementType.Clamped, ScrollRect.MovementType.Clamped.ToString() },
            };


            private static readonly Dictionary<TimerFormat, string> TimerFormatToString = new Dictionary<TimerFormat, string>
            {
                { TimerFormat.None, TimerFormat.None.ToString() },
                { TimerFormat.SecondsHundreth, TimerFormat.SecondsHundreth.ToString() },
                { TimerFormat.MinutesSeconds, TimerFormat.MinutesSeconds.ToString() },
                { TimerFormat.MinutesSecondsHundreth, TimerFormat.MinutesSecondsHundreth.ToString() },
                { TimerFormat.HoursMinutes, TimerFormat.HoursMinutes.ToString() },
                { TimerFormat.HoursMinutesSeconds, TimerFormat.HoursMinutesSeconds.ToString() },
                { TimerFormat.HoursMinutesSecondsMilliseconds, TimerFormat.HoursMinutesSecondsMilliseconds.ToString() },
                { TimerFormat.HoursMinutesSecondsTenths, TimerFormat.HoursMinutesSecondsTenths.ToString() },
                { TimerFormat.DaysHoursMinutes, TimerFormat.DaysHoursMinutes.ToString() },
                { TimerFormat.DaysHoursMinutesSeconds, TimerFormat.DaysHoursMinutesSeconds.ToString() },
                { TimerFormat.Custom, TimerFormat.Custom.ToString() },
            };

            public static class Defaults
            {
                public const string VectorZero = "0 0";
                public const string VectorOne = "1 1";
                public const string Color = "1 1 1 1";
                public const string OutlineColor = "0 0 0 1";
                public const string Sprite = "assets/content/ui/ui.background.tile.psd";
                public const string Material = "assets/content/ui/namefontmaterial.mat";
                public const string IconMaterial = "assets/icons/iconmaterial.mat";
                public const Image.Type ImageType = Image.Type.Simple;
                public const CUI.Font Font = CUI.Font.RobotoCondensedRegular;
                public const int FontSize = 14;
                public const TextAnchor Align = TextAnchor.UpperLeft;
                public const VerticalWrapMode VerticalOverflow = VerticalWrapMode.Overflow;
                public const InputField.LineType LineType = InputField.LineType.SingleLine;
            }

            public static Color GetColor(string colorStr)
            {
                return ColorEx.Parse(colorStr);
            }

            public static string GetColorString(Color color)
            {
                return string.Format("{0} {1} {2} {3}", color.r, color.g, color.b, color.a);
            }

            public static void AddUI(Connection connection, string json)
            {
                CommunityEntity.ServerInstance.ClientRPCEx<string>(new SendInfo
                {
                    connection = connection
                }, null, "AddUI", json);
            }

            private static void SerializeType(ICuiComponent component, JsonWriter jsonWriter)
            {
                jsonWriter.WritePropertyName("type");
                jsonWriter.WriteValue(component.Type);
            }

            private static void SerializeField(string key, object value, object defaultValue, JsonWriter jsonWriter)
            {
                if (value != null && !value.Equals(defaultValue))
                {
                    if (value is string && defaultValue != null && string.IsNullOrEmpty(value as string))
                        return;

                    jsonWriter.WritePropertyName(key);

                    if (value is ICuiComponent)
                        SerializeComponent(value as ICuiComponent, jsonWriter);
                    else
                        jsonWriter.WriteValue(value ?? defaultValue);
                }
            }


            private static void SerializeField(string key, CuiScrollbar scrollbar, JsonWriter jsonWriter)
            {
                const string defaultHandleSprite = "assets/content/ui/ui.rounded.tga";
                const string defaultHandleColor = "0.15 0.15 0.15 1";
                const string defaultHighlightColor = "0.17 0.17 0.17 1";
                const string defaultPressedColor = "0.2 0.2 0.2 1";
                const string defaultTrackSprite = "assets/content/ui/ui.background.tile.psd";
                const string defaultTrackColor = "0.09 0.09 0.09 1";

                if (scrollbar == null)
                    return;

                jsonWriter.WritePropertyName(key);
                jsonWriter.WriteStartObject();
                SerializeField("invert", scrollbar.Invert, false, jsonWriter);
                SerializeField("autoHide", scrollbar.AutoHide, false, jsonWriter);
                SerializeField("handleSprite", scrollbar.HandleSprite, defaultHandleSprite, jsonWriter);
                SerializeField("size", scrollbar.Size, 20f, jsonWriter);
                SerializeField("handleColor", scrollbar.HandleColor, defaultHandleColor, jsonWriter);
                SerializeField("highlightColor", scrollbar.HighlightColor, defaultHighlightColor, jsonWriter);
                SerializeField("pressedColor", scrollbar.PressedColor, defaultPressedColor, jsonWriter);
                SerializeField("trackSprite", scrollbar.TrackSprite, defaultTrackSprite, jsonWriter);
                SerializeField("trackColor", scrollbar.TrackColor, defaultTrackColor, jsonWriter);
                jsonWriter.WriteEndObject();
            }

            private static void SerializeComponent(ICuiComponent IComponent, JsonWriter jsonWriter)
            {
                const string vector2zero = "0 0";
                const string vector2one = "1 1";
                const string colorWhite = "1 1 1 1";
                const string backgroundTile = "assets/content/ui/ui.background.tile.psd";
                const string iconMaterial = "assets/icons/iconmaterial.mat";
                const string fontBold = "RobotoCondensed-Bold.ttf";
                const string defaultOutlineDistance = "1.0 -1.0";

                void SerializeType() => CUI.SerializeType(IComponent, jsonWriter);
                void SerializeField(string key, object value, object defaultValue) => CUI.SerializeField(key, value, defaultValue, jsonWriter);
                void SerializeScrollbar(string key, CuiScrollbar value) => CUI.SerializeField(key, value, jsonWriter);

                switch (IComponent.Type)
                {
                    case "RectTransform":
                        {
                            CuiRectTransformComponent component = IComponent as CuiRectTransformComponent;
                            jsonWriter.WriteStartObject();
                            SerializeType();
                            SerializeField("anchormin", component.AnchorMin, vector2zero);
                            SerializeField("anchormax", component.AnchorMax, vector2one);
                            SerializeField("offsetmin", component.OffsetMin, vector2zero);
                            SerializeField("offsetmax", component.OffsetMax, vector2zero);
                            jsonWriter.WriteEndObject();
                            break;
                        }
                    case "UnityEngine.UI.Image":
                        {
                            CuiImageComponent component = IComponent as CuiImageComponent;
                            jsonWriter.WriteStartObject();
                            SerializeType();
                            SerializeField("color", component.Color, colorWhite);
                            SerializeField("sprite", component.Sprite, backgroundTile);
                            SerializeField("material", component.Material, iconMaterial);
                            SerializeField("imagetype", ImageTypeToString[component.ImageType], ImageTypeToString[Image.Type.Simple]);
                            SerializeField("png", component.Png, null);
                            SerializeField("itemid", component.ItemId, 0);
                            SerializeField("skinid", component.SkinId, 0UL);
                            SerializeField("fadeIn", component.FadeIn, 0f);
                            jsonWriter.WriteEndObject();
                            break;
                        }
                    case "UnityEngine.UI.RawImage":
                        {
                            CuiRawImageComponent component = IComponent as CuiRawImageComponent;
                            jsonWriter.WriteStartObject();
                            SerializeType();
                            SerializeField("color", component.Color, colorWhite);
                            SerializeField("sprite", component.Sprite, backgroundTile);
                            SerializeField("material", component.Material, iconMaterial);
                            SerializeField("url", component.Url, null);
                            SerializeField("png", component.Png, null);
                            SerializeField("fadeIn", component.FadeIn, 0f);
                            jsonWriter.WriteEndObject();
                            break;
                        }
                    case "UnityEngine.UI.Text":
                        {
                            CuiTextComponent component = IComponent as CuiTextComponent;
                            jsonWriter.WriteStartObject();
                            SerializeType();
                            SerializeField("text", component.Text, null);
                            SerializeField("font", component.Font, fontBold);
                            SerializeField("fontSize", component.FontSize, 14);
                            SerializeField("align", TextAnchorToString[component.Align], TextAnchorToString[TextAnchor.UpperLeft]);
                            SerializeField("color", component.Color, colorWhite);
                            SerializeField("verticalOverflow", VWMToString[component.VerticalOverflow], VWMToString[VerticalWrapMode.Truncate]);
                            SerializeField("fadeIn", component.FadeIn, 0f);
                            jsonWriter.WriteEndObject();
                            break;
                        }
                    case "UnityEngine.UI.Button":
                        {
                            CuiButtonComponent component = IComponent as CuiButtonComponent;
                            jsonWriter.WriteStartObject();
                            SerializeType();
                            SerializeField("color", component.Color, colorWhite);
                            SerializeField("sprite", component.Sprite, backgroundTile);
                            SerializeField("material", component.Material, iconMaterial);
                            SerializeField("imagetype", ImageTypeToString[component.ImageType], ImageTypeToString[Image.Type.Simple]);
                            SerializeField("command", component.Command, null);
                            SerializeField("close", component.Close, null);
                            SerializeField("fadeIn", component.FadeIn, 0f);
                            jsonWriter.WriteEndObject();
                            break;
                        }
                    case "UnityEngine.UI.InputField":
                        {
                            CuiInputFieldComponent component = IComponent as CuiInputFieldComponent;
                            jsonWriter.WriteStartObject();
                            SerializeType();
                            SerializeField("text", component.Text, null);
                            SerializeField("font", component.Font, fontBold);
                            SerializeField("fontSize", component.FontSize, 14);
                            SerializeField("align", TextAnchorToString[component.Align], TextAnchorToString[TextAnchor.UpperLeft]);
                            SerializeField("color", component.Color, colorWhite);
                            SerializeField("command", component.Command, null);
                            SerializeField("characterLimit", component.CharsLimit, 0);
                            SerializeField("lineType", LineTypeToString[component.LineType], LineTypeToString[InputField.LineType.SingleLine]);
                            SerializeField("readOnly", component.ReadOnly, false);
                            SerializeField("password", component.IsPassword, false);
                            SerializeField("needsKeyboard", component.NeedsKeyboard, false);
                            SerializeField("hudMenuInput", component.HudMenuInput, false);
                            SerializeField("autofocus", component.Autofocus, false);
                            jsonWriter.WriteEndObject();
                            break;
                        }
                    case "UnityEngine.UI.ScrollView":
                        {
                            CuiScrollViewComponent component = IComponent as CuiScrollViewComponent;
                            jsonWriter.WriteStartObject();
                            SerializeType();
                            SerializeField("contentTransform", component.ContentTransform, null);
                            SerializeField("horizontal", component.Horizontal, false);
                            SerializeField("vertical", component.Vertical, false);
                            SerializeField("movementType", MovementTypeToString[component.MovementType], MovementTypeToString[ScrollRect.MovementType.Clamped]);
                            SerializeField("elasticity", component.Elasticity, 0.1f);
                            SerializeField("inertia", component.Inertia, false);
                            SerializeField("decelerationRate", component.DecelerationRate, 0.135f);
                            SerializeField("scrollSensitivity", component.ScrollSensitivity, 1f);
                            SerializeScrollbar("horizontalScrollbar", component.HorizontalScrollbar);
                            SerializeScrollbar("verticalScrollbar", component.VerticalScrollbar);
                            jsonWriter.WriteEndObject();
                            break;
                        }
                    case "UnityEngine.UI.Outline":
                        {
                            CuiOutlineComponent component = IComponent as CuiOutlineComponent;
                            jsonWriter.WriteStartObject();
                            SerializeType();
                            SerializeField("color", component.Color, colorWhite);
                            SerializeField("distance", component.Distance, defaultOutlineDistance);
                            SerializeField("useGraphicAlpha", component.UseGraphicAlpha, false);
                            jsonWriter.WriteEndObject();
                            break;
                        }
                    case "Countdown":
                        {
                            CuiCountdownComponent component = IComponent as CuiCountdownComponent;
                            jsonWriter.WriteStartObject();
                            SerializeType();
                            SerializeField("endTime", component.EndTime, 0f);
                            SerializeField("startTime", component.StartTime, 0f);
                            SerializeField("step", component.Step, 1f);
                            SerializeField("interval", component.Interval, 1f);
                            SerializeField("timerFormat", TimerFormatToString[component.TimerFormat], TimerFormatToString[TimerFormat.None]);
                            SerializeField("numberFormat", component.NumberFormat, "0.####");
                            SerializeField("destroyIfDone", component.DestroyIfDone, true);
                            SerializeField("command", component.Command, null);
                            SerializeField("fadeIn", component.FadeIn, 0f);
                            jsonWriter.WriteEndObject();
                            break;
                        }
                    case "NeedsKeyboard":
                    case "NeedsCursor":
                        {
                            jsonWriter.WriteStartObject();
                            SerializeType();
                            jsonWriter.WriteEndObject();
                            break;
                        }
                }
            }


            [JsonObject(MemberSerialization.OptIn)]
            public class Element : CuiElement
            {
                public new string Name { get; set; } = null;

                public Element ParentElement { get; set; }
                public virtual List<Element> Container => ParentElement?.Container;
                public ComponentList Components { get; set; } = new ComponentList();

                [JsonProperty("name")]
                public string JsonName
                {
                    get
                    {
                        if (Name == null)
                        {
                            string result = this.GetHashCode().ToString();
                            if (ParentElement != null)
                                result.Insert(0, ParentElement.JsonName);
                            return result.GetHashCode().ToString();
                        }
                        return Name;
                    }
                }

                public Element() { }
                public Element(Element parent)
                {
                    AssignParent(parent);
                }

                public CUI.Element AssignParent(Element parent)
                {
                    if (parent == null)
                        return this;

                    ParentElement = parent;
                    Parent = ParentElement.JsonName;
                    return this;
                }

                public Element AddDestroy(string elementName)
                {
                    this.DestroyUi = elementName;
                    return this;
                }

                public Element AddDestroySelfAttribute()
                {
                    return AddDestroy(this.Name);
                }

                public virtual void WriteJson(JsonWriter jsonWriter)
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("name");
                    jsonWriter.WriteValue(this.JsonName);
                    if (!string.IsNullOrEmpty(Parent))
                    {
                        jsonWriter.WritePropertyName("parent");
                        jsonWriter.WriteValue(this.Parent);
                    }
                    if (!string.IsNullOrEmpty(this.DestroyUi))
                    {
                        jsonWriter.WritePropertyName("destroyUi");
                        jsonWriter.WriteValue(this.DestroyUi);
                    }
                    if (this.Update)
                    {
                        jsonWriter.WritePropertyName("update");
                        jsonWriter.WriteValue(this.Update);
                    }
                    if (this.FadeOut > 0f)
                    {
                        jsonWriter.WritePropertyName("fadeOut");
                        jsonWriter.WriteValue(this.FadeOut);
                    }
                    jsonWriter.WritePropertyName("components");
                    jsonWriter.WriteStartArray();
                    for (int i = 0; i < this.Components.Count; i++)
                    {
                        SerializeComponent(this.Components[i], jsonWriter);
                    }
                    jsonWriter.WriteEndArray();
                    jsonWriter.WriteEndObject();
                }

                public Element Add(Element element)
                {
                    if (element.ParentElement == null)
                        element.AssignParent(this);
                    Container.Add(element);
                    return element;
                }

                public Element AddEmpty(string name = null)
                {
                    return Add(new Element(this) { Name = name });
                }

                public Element AddUpdateElement(string name = null)
                {
                    Element element = AddEmpty(name);
                    element.Parent = null;
                    element.Update = true;
                    return element;
                }

                public Element AddText(
                    string text,
                    string color = Defaults.Color,
                    CUI.Font font = Defaults.Font,
                    int fontSize = Defaults.FontSize,
                    TextAnchor align = Defaults.Align,
                    VerticalWrapMode overflow = Defaults.VerticalOverflow,
                    string anchorMin = Defaults.VectorZero,
                    string anchorMax = Defaults.VectorOne,
                    string offsetMin = Defaults.VectorZero,
                    string offsetMax = Defaults.VectorZero,
                    string name = null)
                {
                    return Add(ElementContructor.CreateText(text, color, font, fontSize, align, overflow, anchorMin, anchorMax, offsetMin, offsetMax, name));
                }

                public Element AddOutlinedText(
                   string text,
                   string color = Defaults.Color,
                   CUI.Font font = Defaults.Font,
                   int fontSize = Defaults.FontSize,
                   TextAnchor align = Defaults.Align,
                   VerticalWrapMode overflow = Defaults.VerticalOverflow,
                   string outlineColor = Defaults.OutlineColor,
                   int outlineWidth = 1,
                   string anchorMin = Defaults.VectorZero,
                   string anchorMax = Defaults.VectorOne,
                   string offsetMin = Defaults.VectorZero,
                   string offsetMax = Defaults.VectorZero,
                   string name = null)
                {
                    return Add(ElementContructor.CreateOutlinedText(text, color, font, fontSize, align, overflow, outlineColor, outlineWidth, anchorMin, anchorMax, offsetMin, offsetMax, name));
                }

                public Element AddInputfield(
                    string command = null,
                    string text = "",
                    string color = Defaults.Color,
                    CUI.Font font = Defaults.Font,
                    int fontSize = Defaults.FontSize,
                    TextAnchor align = Defaults.Align,
                    InputField.LineType lineType = Defaults.LineType,
                    CUI.InputType inputType = CUI.InputType.Default,
                    bool @readonly = false,
                    bool autoFocus = false,
                    bool isPassword = false,
                    int charsLimit = 0,
                    string anchorMin = Defaults.VectorZero,
                    string anchorMax = Defaults.VectorOne,
                    string offsetMin = Defaults.VectorZero,
                    string offsetMax = Defaults.VectorZero,
                    string name = null)
                {
                    return Add(ElementContructor.CreateInputfield(command, text, color, font, fontSize, align, lineType, inputType, @readonly, autoFocus, isPassword, charsLimit, anchorMin, anchorMax, offsetMin, offsetMax, name));
                }

                public Element AddPanel(
                    string color = Defaults.Color,
                    string sprite = Defaults.Sprite,
                    string material = Defaults.Material,
                    Image.Type imageType = Defaults.ImageType,
                    string anchorMin = Defaults.VectorZero,
                    string anchorMax = Defaults.VectorOne,
                    string offsetMin = Defaults.VectorZero,
                    string offsetMax = Defaults.VectorZero,
                    bool cursorEnabled = false,
                    bool keyboardEnabled = false,
                    string name = null)
                {
                    return Add(ElementContructor.CreatePanel(color, sprite, material, imageType, anchorMin, anchorMax, offsetMin, offsetMax, cursorEnabled, keyboardEnabled, name));
                }

                public Element AddButton(
                   string command = null,
                   string close = null,
                   string color = Defaults.Color,
                   string sprite = Defaults.Sprite,
                   string material = Defaults.Material,
                   Image.Type imageType = Defaults.ImageType,
                   string anchorMin = Defaults.VectorZero,
                   string anchorMax = Defaults.VectorOne,
                   string offsetMin = Defaults.VectorZero,
                   string offsetMax = Defaults.VectorZero,
                   string name = null)
                {
                    return Add(ElementContructor.CreateButton(command, close, color, sprite, material, imageType, anchorMin, anchorMax, offsetMin, offsetMax, name));
                }

                public Element AddImage(
                    string content,
                    string color = Defaults.Color,
                    string material = null,
                    string anchorMin = Defaults.VectorZero,
                    string anchorMax = Defaults.VectorOne,
                    string offsetMin = Defaults.VectorZero,
                    string offsetMax = Defaults.VectorZero,
                    string name = null)
                {
                    return Add(ElementContructor.CreateImage(content, color, material, anchorMin, anchorMax, offsetMin, offsetMax, name));
                }

                public Element AddHImage(
                    string content,
                    string color = Defaults.Color,
                    string anchorMin = Defaults.VectorZero,
                    string anchorMax = Defaults.VectorOne,
                    string offsetMin = Defaults.VectorZero,
                    string offsetMax = Defaults.VectorZero,
                    string name = null)
                {
                    return AddImage(content, color, Defaults.IconMaterial, anchorMin, anchorMax, offsetMin, offsetMax, name);
                }

                public Element AddIcon(
                    int itemId,
                    ulong skin = 0,
                    string color = Defaults.Color,
                    string sprite = Defaults.Sprite,
                    string material = Defaults.IconMaterial,
                    Image.Type imageType = Defaults.ImageType,
                    string anchorMin = Defaults.VectorZero,
                    string anchorMax = Defaults.VectorOne,
                    string offsetMin = Defaults.VectorZero,
                    string offsetMax = Defaults.VectorZero,
                    string name = null)
                {
                    return Add(ElementContructor.CreateIcon(itemId, skin, color, sprite, material, imageType, anchorMin, anchorMax, offsetMin, offsetMax, name));
                }

                public Element AddContainer(
                    string anchorMin = Defaults.VectorZero,
                    string anchorMax = Defaults.VectorOne,
                    string offsetMin = Defaults.VectorZero,
                    string offsetMax = Defaults.VectorZero,
                    string name = null)
                {
                    return Add(ElementContructor.CreateContainer(anchorMin, anchorMax, offsetMin, offsetMax, name));
                }

                public CUI.Element WithRect(
                    string anchorMin = Defaults.VectorZero,
                    string anchorMax = Defaults.VectorOne,
                    string offsetMin = Defaults.VectorZero,
                    string offsetMax = Defaults.VectorZero)
                {
                    if (this.Components.Count > 0)
                        this.Components.RemoveAll(c => c is CuiRectTransformComponent);
                    this.Components.Add(new CuiRectTransformComponent()
                    {
                        AnchorMin = anchorMin,
                        AnchorMax = anchorMax,
                        OffsetMin = offsetMin,
                        OffsetMax = offsetMax
                    });
                    return this;
                }

                public CUI.Element WithFade(
                    float @in = 0f,
                    float @out = 0f)
                {
                    this.FadeOut = @out;
                    foreach (ICuiComponent component in this.Components)
                    {
                        if (component is CuiRawImageComponent rawImage)
                            rawImage.FadeIn = @in;
                        else if (component is CuiImageComponent image)
                            image.FadeIn = @in;
                        else if (component is CuiButtonComponent button)
                            button.FadeIn = @in;
                        else if (component is CuiTextComponent text)
                            text.FadeIn = @in;
                        else if (component is CuiCountdownComponent countdown)
                            countdown.FadeIn = @in;
                    }
                    return this;
                }

                public void AddComponents(params ICuiComponent[] components)
                {
                    this.Components.AddRange(components);
                }

                public CUI.Element WithComponents(params ICuiComponent[] components)
                {
                    AddComponents(components);
                    return this;
                }

                public CUI.Element CreateChild(string name = null, params ICuiComponent[] components)
                {
                    return CUI.Element.Create(name, components).AssignParent(this);
                }

                public static CUI.Element Create(string name = null, params ICuiComponent[] components)
                {
                    return new CUI.Element()
                    {
                        Name = name
                    }.WithComponents(components);
                }

                public class ComponentList : List<ICuiComponent>
                {
                    private Dictionary<Type, ICuiComponent> typeToComponent = new Dictionary<Type, ICuiComponent>();

                    public T Get<T>() where T : ICuiComponent
                    {
                        if (typeToComponent.TryGetValue(typeof(T), out ICuiComponent component))
                            return (T)component;
                        return default(T);
                    }

                    public new void Add(ICuiComponent item)
                    {
                        base.Add(item);
                        typeToComponent.Add(item.GetType(), item);
                    }

                    public new void Remove(ICuiComponent item)
                    {
                        base.Remove(item);
                        typeToComponent.Remove(item.GetType());
                    }

                    public new void Clear()
                    {
                        base.Clear();
                        typeToComponent.Clear();
                    }


                    public ComponentList AddImage(
                        string color = Defaults.Color,
                        string sprite = Defaults.Sprite,
                        string material = Defaults.Material,
                        Image.Type imageType = Defaults.ImageType,
                        int itemId = 0,
                        ulong skinId = 0UL)
                    {
                        Add(new CuiImageComponent
                        {
                            Color = color,
                            Sprite = sprite,
                            Material = material,
                            ImageType = imageType,
                            ItemId = itemId,
                            SkinId = skinId,
                        });
                        return this;
                    }

                    public ComponentList AddRawImage(
                        string content,
                        string color = Defaults.Color,
                        string sprite = Defaults.Sprite,
                        string material = Defaults.IconMaterial)
                    {
                        CuiRawImageComponent rawImageComponent = new CuiRawImageComponent
                        {
                            Color = color,
                            Sprite = sprite,
                            Material = material,
                        };
                        if (!string.IsNullOrEmpty(content))
                        {
                            if (content.Contains("://"))
                                rawImageComponent.Url = content;
                            else if (content.IsNumeric())
                                rawImageComponent.Png = content;
                        }
                        Add(rawImageComponent);
                        return this;
                    }

                    public ComponentList AddButton(
                        string command = null,
                        string close = null,
                        string color = Defaults.Color,
                        string sprite = Defaults.Sprite,
                        string material = Defaults.Material,
                        Image.Type imageType = Defaults.ImageType)
                    {
                        Add(new CuiButtonComponent
                        {
                            Command = command,
                            Close = close,
                            Color = color,
                            Sprite = sprite,
                            Material = material,
                            ImageType = imageType,
                        });
                        return this;
                    }

                    public ComponentList AddText(
                        string text,
                        string color = Defaults.Color,
                        CUI.Font font = Defaults.Font,
                        int fontSize = Defaults.FontSize,
                        TextAnchor align = Defaults.Align,
                        VerticalWrapMode overflow = Defaults.VerticalOverflow)
                    {
                        Add(new CuiTextComponent
                        {
                            Text = text,
                            Color = color,
                            Font = FontToString[font],
                            FontSize = fontSize,
                            Align = align,
                            VerticalOverflow = overflow
                        });
                        return this;
                    }

                    public ComponentList AddInputfield(
                        string command = null,
                        string text = "",
                        string color = Defaults.Color,
                        CUI.Font font = Defaults.Font,
                        int fontSize = Defaults.FontSize,
                        TextAnchor align = Defaults.Align,
                        InputField.LineType lineType = Defaults.LineType,
                        CUI.InputType inputType = CUI.InputType.Default,
                        bool @readonly = false,
                        bool autoFocus = false,
                        bool isPassword = false,
                        int charsLimit = 0)
                    {
                        Add(new CuiInputFieldComponent
                        {
                            Command = command,
                            Text = text,
                            Color = color,
                            Font = FontToString[font],
                            FontSize = fontSize,
                            Align = align,
                            NeedsKeyboard = inputType == InputType.Default,
                            HudMenuInput = inputType == InputType.HudMenuInput,
                            Autofocus = autoFocus,
                            ReadOnly = @readonly,
                            CharsLimit = charsLimit,
                            IsPassword = isPassword,
                            LineType = lineType
                        });
                        return this;
                    }

                    public ComponentList AddScrollView(
                        bool horizontal = false,
                        CuiScrollbar horizonalScrollbar = null,
                        bool vertical = false,
                        CuiScrollbar verticalScrollbar = null,
                        bool inertia = false,
                        ScrollRect.MovementType movementType = ScrollRect.MovementType.Clamped,
                        float decelerationRate = 0.135f,
                        float elasticity = 0.1f,
                        float scrollSensitivity = 1f,
                        string anchorMin = "0 0",
                        string anchorMax = "1 1",
                        string offsetMin = "0 0",
                        string offsetMax = "0 0")
                    {
                        Add(new CuiScrollViewComponent()
                        {
                            ContentTransform =
                                         new CuiRectTransformComponent()
                                         {
                                             AnchorMin = anchorMin,
                                             AnchorMax = anchorMax,
                                             OffsetMin = offsetMin,
                                             OffsetMax = offsetMax
                                         },
                            Horizontal = horizontal,
                            HorizontalScrollbar = horizonalScrollbar,
                            Vertical = vertical,
                            VerticalScrollbar = verticalScrollbar,
                            Inertia = inertia,
                            DecelerationRate = decelerationRate,
                            Elasticity = elasticity,
                            ScrollSensitivity = scrollSensitivity,
                            MovementType = movementType,
                        });
                        return this;
                    }

                    public ComponentList AddOutline(
                        string color = Defaults.OutlineColor,
                        int width = 1)
                    {
                        Add(new CuiOutlineComponent
                        {
                            Color = color,
                            Distance = string.Format("{0} -{0}", width)
                        });
                        return this;
                    }
                    public ComponentList AddNeedsKeyboard()
                    {
                        Add(new CuiNeedsKeyboardComponent());
                        return this;
                    }

                    public ComponentList AddNeedsCursor()
                    {
                        Add(new CuiNeedsCursorComponent());
                        return this;
                    }

                    public ComponentList AddCountdown(
                        string command = null,
                        float endTime = 0,
                        float startTime = 0,
                        float step = 1,
                        float interval = 1f,
                        TimerFormat timerFormat = TimerFormat.None,
                        string numberFormat = "0.####",
                        bool destroyIfDone = true)
                    {
                        Add(new CuiCountdownComponent
                        {
                            Command = command,
                            EndTime = endTime,
                            StartTime = startTime,
                            Step = step,
                            Interval = interval,
                            TimerFormat = timerFormat,
                            NumberFormat = numberFormat,
                            DestroyIfDone = destroyIfDone
                        });
                        return this;
                    }
                }
            }

            public static class ElementContructor
            {
                public static CUI.Element CreateText(
                 string text,
                 string color = Defaults.Color,
                 CUI.Font font = Defaults.Font,
                 int fontSize = Defaults.FontSize,
                 TextAnchor align = Defaults.Align,
                 VerticalWrapMode overflow = Defaults.VerticalOverflow,
                 string anchorMin = Defaults.VectorZero,
                 string anchorMax = Defaults.VectorOne,
                 string offsetMin = Defaults.VectorZero,
                 string offsetMax = Defaults.VectorZero,
                 string name = null)
                {
                    CUI.Element element = CreateContainer(anchorMin, anchorMax, offsetMin, offsetMax, name);
                    element.Components.AddText(text, color, font, fontSize, align, overflow);
                    return element;
                }

                public static CUI.Element CreateOutlinedText(
                   string text,
                   string color = Defaults.Color,
                   CUI.Font font = Defaults.Font,
                   int fontSize = Defaults.FontSize,
                   TextAnchor align = Defaults.Align,
                   VerticalWrapMode overflow = Defaults.VerticalOverflow,
                   string outlineColor = Defaults.OutlineColor,
                   int outlineWidth = 1,
                   string anchorMin = Defaults.VectorZero,
                   string anchorMax = Defaults.VectorOne,
                   string offsetMin = Defaults.VectorZero,
                   string offsetMax = Defaults.VectorZero,
                   string name = null)
                {
                    CUI.Element element = CreateText(text, color, font, fontSize, align, overflow, anchorMin, anchorMax, offsetMin, offsetMax, name);
                    element.Components.AddOutline(outlineColor, outlineWidth);
                    return element;
                }

                public static CUI.Element CreateInputfield(
                      string command = null,
                      string text = "",
                      string color = Defaults.Color,
                      CUI.Font font = Defaults.Font,
                      int fontSize = Defaults.FontSize,
                      TextAnchor align = Defaults.Align,
                      InputField.LineType lineType = Defaults.LineType,
                      CUI.InputType inputType = CUI.InputType.Default,
                      bool @readonly = false,
                      bool autoFocus = false,
                      bool isPassword = false,
                      int charsLimit = 0,
                      string anchorMin = Defaults.VectorZero,
                      string anchorMax = Defaults.VectorOne,
                      string offsetMin = Defaults.VectorZero,
                      string offsetMax = Defaults.VectorZero,
                      string name = null)
                {
                    CUI.Element element = CreateContainer(anchorMin, anchorMax, offsetMin, offsetMax, name);
                    element.Components.AddInputfield(command, text, color, font, fontSize, align, lineType, inputType, @readonly, autoFocus, isPassword, charsLimit);
                    return element;
                }

                public static CUI.Element CreateButton(
                        string command = null,
                        string close = null,
                        string color = Defaults.Color,
                        string sprite = Defaults.Sprite,
                        string material = Defaults.Material,
                        Image.Type imageType = Defaults.ImageType,
                        string anchorMin = Defaults.VectorZero,
                        string anchorMax = Defaults.VectorOne,
                        string offsetMin = Defaults.VectorZero,
                        string offsetMax = Defaults.VectorZero,
                        string name = null)
                {
                    CUI.Element element = CreateContainer(anchorMin, anchorMax, offsetMin, offsetMax, name);
                    element.Components.AddButton(command, close, color, sprite, material, imageType);
                    return element;
                }

                public static CUI.Element CreatePanel(
                        string color = Defaults.Color,
                        string sprite = Defaults.Sprite,
                        string material = Defaults.Material,
                        Image.Type imageType = Defaults.ImageType,
                        string anchorMin = Defaults.VectorZero,
                        string anchorMax = Defaults.VectorOne,
                        string offsetMin = Defaults.VectorZero,
                        string offsetMax = Defaults.VectorZero,
                        bool cursorEnabled = false,
                        bool keyboardEnabled = false,
                        string name = null)
                {

                    Element element = CreateContainer(anchorMin, anchorMax, offsetMin, offsetMax, name);
                    element.Components.AddImage(color, sprite, material, imageType);
                    if (cursorEnabled)
                        element.Components.AddNeedsCursor();
                    if (keyboardEnabled)
                        element.Components.AddNeedsKeyboard();
                    return element;
                }

                public static CUI.Element CreateImage(
                    string content,
                    string color = Defaults.Color,
                    string material = null,
                    string anchorMin = Defaults.VectorZero,
                    string anchorMax = Defaults.VectorOne,
                    string offsetMin = Defaults.VectorZero,
                    string offsetMax = Defaults.VectorZero,
                    string name = null)
                {
                    Element element = CreateContainer(anchorMin, anchorMax, offsetMin, offsetMax, name);
                    element.Components.AddRawImage(content, color, material: material);
                    return element;
                }

                public static CUI.Element CreateIcon(
                        int itemId,
                        ulong skin = 0,
                        string color = Defaults.Color,
                        string sprite = Defaults.Sprite,
                        string material = Defaults.IconMaterial,
                        Image.Type imageType = Defaults.ImageType,
                        string anchorMin = Defaults.VectorZero,
                        string anchorMax = Defaults.VectorOne,
                        string offsetMin = Defaults.VectorZero,
                        string offsetMax = Defaults.VectorZero,
                        string name = null)
                {
                    Element element = CreateContainer(anchorMin, anchorMax, offsetMin, offsetMax, name);
                    element.Components.AddImage(color, sprite, material, imageType, itemId, skin);
                    return element;
                }

                public static Element CreateContainer(
                       string anchorMin = Defaults.VectorZero,
                       string anchorMax = Defaults.VectorOne,
                       string offsetMin = Defaults.VectorZero,
                       string offsetMax = Defaults.VectorZero,
                       string name = null)
                {
                    return Element.Create(name).WithRect(anchorMin, anchorMax, offsetMin, offsetMax);
                }
            }


            public class Root : Element
            {
                public bool wasRendered = false;
                private static StringBuilder stringBuilder = new StringBuilder();

                public Root()
                {
                    Name = string.Empty;
                }

                public Root(string rootObjectName = "Overlay")
                {
                    Name = rootObjectName;
                }

                public override List<Element> Container { get; } = new List<Element>();

                public string ToJson(List<Element> elements)
                {
                    stringBuilder.Clear();
                    try
                    {
                        using (StringWriter stringWriter = new StringWriter(stringBuilder))
                        {
                            using (JsonWriter jsonWriter = new JsonTextWriter(stringWriter))
                            {
                                jsonWriter.WriteStartArray();
                                foreach (Element element in elements)
                                    element.WriteJson(jsonWriter);
                                jsonWriter.WriteEndArray();
                            }
                        }
                        return stringBuilder.ToString().Replace("\\n", "\n");
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError(ex.Message + "\n" + ex.StackTrace);
                        return string.Empty;
                    }
                }

                public string ToJson()
                {
                    return ToJson(Container);
                }

                public void Render(Connection connection)
                {
                    if (connection == null || !connection.connected)
                        return;

                    wasRendered = true;
                    CUI.AddUI(connection, ToJson(Container));
                }

                public void Render(BasePlayer player)
                {
                    Render(player.Connection);
                }

                public void Update(Connection connection)
                {
                    foreach (Element element in Container)
                        element.Update = true;
                    CUI.AddUI(connection, ToJson(Container));
                }

                public void Update(BasePlayer player)
                {
                    Update(player.Connection);
                }

            }
        }
    }
    #endregion
}
