using System.Text;
using UnityEngine;

namespace SaberFactory.Helpers
{
    internal class MeshRendererModifier : BaseComponentModifier<MeshRenderer>
    {
        [ComponentValue]
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                if (Component != null)
                {
                    Component.enabled = value;
                }
            }
        }

        private bool _enabled;

        public MeshRendererModifier(Component component, int index) : base(component, index)
        {
        }

        protected override void Init(MeshRenderer component)
        {
            // Init values
            _enabled = component.enabled;
            var mat = component.material;
        }

        public override string DrawUi()
        {
            var str = new StringBuilder();
            str.AppendLine("<vertical>");
            str.AppendLine("<checkbox text='Enabled' value='Enabled' apply-on-change='true' pref-width='50'/>");
            str.AppendLine("</vertical>");
            return str.ToString();
        }
    }
}