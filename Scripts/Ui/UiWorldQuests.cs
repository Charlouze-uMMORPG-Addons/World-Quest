using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace WorldQuest.Ui
{
    public class UiWorldQuests : MonoBehaviour
    {
        public GameObject panel;
        public Transform content;
        public UIQuestSlot slotPrefab;

        public string expandPrefix = "[+] ";
        public string hidePrefix = "[-] ";

        private void Update()
        {
            var player = Player.localPlayer;
            if (player == null)
            {
                panel.SetActive(false);
                return;
            }

            var pwq = player.GetComponent<PlayerWorldQuests>();

            if (pwq == null || !pwq.participating)
            {
                panel.SetActive(false);
                return;
            }

            if (!panel.activeSelf)
            {
                panel.SetActive(true);
            }

            var descriptors = pwq.managers;
            
            UIUtils.BalancePrefabs(slotPrefab.gameObject, descriptors.Count, content);

            for (int i = 0; i < descriptors.Count; i++)
            {
                var slot = content.GetChild(i).GetComponent<UIQuestSlot>();
                var descriptor = descriptors[i];

                // name button
                GameObject descriptionPanel = slot.descriptionText.gameObject;
                string prefix = descriptionPanel.activeSelf ? hidePrefix : expandPrefix;
                slot.nameButton.GetComponentInChildren<Text>().text = prefix + descriptor.name;
                slot.nameButton.onClick.SetListener(() => {
                    descriptionPanel.SetActive(!descriptionPanel.activeSelf);
                });

                // description
                slot.descriptionText.text = descriptor.Description;
            }
        }
    }
}