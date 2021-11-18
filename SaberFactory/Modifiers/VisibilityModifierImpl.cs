using System.Text;
using SaberFactory.ProjectComponents;

namespace SaberFactory.Modifiers
{
    internal class VisibilityModifierImpl : BaseModifierImpl
    {
        [HandledValue]
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                SetVisibility(value);
            }
        }

        public override string Name { get; }

        public override string TypeName => "Visibility Modifier";

        private string DefaultValueText => "<color=#ffffff80>Visible by default:</color> " + _visibilityModifier.DefaultValue;

        private bool _visible;
        private VisibilityModifier _visibilityModifier;

        public VisibilityModifierImpl(VisibilityModifier visibilityModifier) : base(visibilityModifier.Id)
        {
            _visible = visibilityModifier.DefaultValue;
            Name = visibilityModifier.Name;
        }

        public override void SetInstance(object instance)
        {
            _visibilityModifier = (VisibilityModifier)instance;
            Update();
        }

        public override void Reset()
        {
            Visible = _visibilityModifier.DefaultValue;
        }

        public override string DrawUi()
        {
            var str = new StringBuilder();
            str.AppendLine("<vertical>");
            
            str.AppendLine("<vertical bg='round-rect-panel' custom-color='#777' vertical-fit='PreferredSize' horizontal-fit='Unconstrained' pad='2'>");
            str.AppendLine("<text text='" + _visibilityModifier.Name + "' align='Center'/>");
            str.AppendLine("</vertical>");
            
            str.AppendLine("<text text='~DefaultValueText' align='Center'/>");
            str.AppendLine("<checkbox text='Visible' value='Visible' apply-on-change='true' pref-width='50'/>");
            str.AppendLine("</vertical>");
            return str.ToString();
        }

        private void SetVisibility(bool visible)
        {
            if (_visibilityModifier?.Objects == null)
            {
                return;
            }
            
            foreach (var obj in _visibilityModifier.Objects)
            {
                if (obj == null)
                {
                    continue;
                }

                obj.SetActive(visible);
            }
        }
    }
}