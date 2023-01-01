using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FlowUi.Runtime
{
    public class FlowMessageDialog : FlowCustomDialog
    {
        [SerializeField] private TextMeshProUGUI messageTextmesh = null;

        public async Task<DialogResult> Show(string message, string title, bool centered = true)
        {
            messageTextmesh.text = message;
            messageTextmesh.alignment = centered ? TextAlignmentOptions.Center : TextAlignmentOptions.TopLeft;
            return await Show(title);
        }
    }
}