using UnityEngine;
using UnityEngine.UIElements;

namespace Visus.Robotics.RosBridge
{
    public class StatusWidget: BindableElement, INotifyValueChanged<int>
    {
        private VisualElement container;
        
        private Label nameLabel;
        private Label valueLabel;

        private string[] texts;
        private Color[] colors;

        private int m_value;

        public StatusWidget(string name, float font_size, string[] texts, Color[] colors)
        {
            this.texts = texts;
            this.colors = colors;
            
            this.style.flexDirection = FlexDirection.Row;
            this.style.flexWrap = Wrap.NoWrap;

            nameLabel = new Label(name);
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            nameLabel.style.flexBasis = 140;

            valueLabel = new Label(texts[0]);
            valueLabel.style.unityFontStyleAndWeight = FontStyle.Normal;

            valueLabel.style.color = colors[0];
            valueLabel.style.fontSize = font_size;
            
            this.Insert(0, nameLabel);
            this.Insert(1, valueLabel);
            
            valueLabel.PlaceInFront(nameLabel);
        }

        public void SetValueWithoutNotify(int newValue)
        {
            this.m_value = newValue;
            valueLabel.text = texts[newValue];
            valueLabel.style.color = colors[newValue];
        }

        public int value
        {
            get => m_value;
            set
            {
                if (value == this.m_value)
                {
                    return;
                }

                int previousValue = this.m_value;
                SetValueWithoutNotify(value);
                using (var evt = ChangeEvent<int>.GetPooled(previousValue, value))
                {
                    evt.target = this;
                    SendEvent(evt);
                }
            }
        }
    }
}