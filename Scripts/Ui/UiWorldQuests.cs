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

            if (!panel.activeSelf) panel.SetActive(true);

            UIUtils.BalancePrefabs(slotPrefab.gameObject, pwq.Count, content);

            for (var i = 0; i < pwq.Count; i++)
            {
                var slot = content.GetChild(i).GetComponent<UIQuestSlot>();
                var descriptor = pwq.GetDescriptionManager(i);

                // name button
                var descriptionPanel = slot.descriptionText.gameObject;
                var prefix = descriptionPanel.activeSelf ? hidePrefix : expandPrefix;
                slot.nameButton.GetComponentInChildren<Text>().text = prefix + descriptor.Name;
                slot.nameButton.onClick.SetListener(() => { descriptionPanel.SetActive(!descriptionPanel.activeSelf); });

                // description
                slot.descriptionText.text = descriptor.Description;
            }
        }
    }
}