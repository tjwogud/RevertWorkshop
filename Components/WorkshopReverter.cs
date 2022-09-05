using DG.Tweening;
using RevertWorkshop.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RevertWorkshop.Components
{
    public class WorkshopReverter : MonoBehaviour
    {
		private scnCLS cls;

		private void Awake()
        {
			cls = GetComponent<scnCLS>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RevertLevel();
                SetText();
            }
        }

        public void RevertLevel()
        {
			Dictionary<string, bool> deletedLevels = cls.Get<Dictionary<string, bool>>("loadedLevelIsDeleted");
			string levelToSelect = cls.Get<string>("levelToSelect");
			if (!deletedLevels[levelToSelect])
				return;
            Dictionary<string, bool> isWorkshopLevel = cls.Get<Dictionary<string, bool>>("isWorkshopLevel");
            if (!isWorkshopLevel.TryGetValue(levelToSelect, out bool b) || !b)
                return;
			scrFlash.Flash(Color.white, 0.5f);
			cls.Set("changingLevel", true);
			cls.Set("levelTransitionTimer", 0f);
            CustomLevelTile tile = cls.Get<Dictionary<string, CustomLevelTile>>("loadedLevelTiles")[levelToSelect];
            tile.title.gameObject.SetActive(true);
            tile.artist.gameObject.SetActive(true);
            tile.removedText.gameObject.SetActive(false);
            tile.image.gameObject.SetActive(true);
            tile.GetComponent<SpriteRenderer>().color = Color.white;
            if (ulong.TryParse(levelToSelect, out ulong num))
            {
                using (List<SteamWorkshop.ResultItem>.Enumerator enumerator = SteamWorkshop.resultItems.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        SteamWorkshop.ResultItem resultItem = enumerator.Current;
                        object id = resultItem.Get("id");
                        if (id.Get<ulong>("m_PublishedFileId") == num)
                            typeof(SteamWorkshop).Method("Subscribe", new object[] { id });
                    }
                    goto end;
                }
            }
        end:
			deletedLevels[levelToSelect] = false;
			cls.DisplayLevel(levelToSelect);
            SetText();
        }

        private static Color deleteColor = new Color(0.8962f, 0.2156f, 0.2156f);
        private static Color revertColor = new Color(0.2863f, 0.8863f, 0.0863f);

        public void SetText()
        {
            if (Main.optionsPanelsCLS == null)
                return;
            string levelToSelect = scnCLS.instance.Get<string>("levelToSelect");
            if (levelToSelect != null)
            {
                bool deleted = scnCLS.instance.Get<Dictionary<string, bool>>("loadedLevelIsDeleted")[levelToSelect];
                RectTransform rightPanel = scnCLS.instance.Get("optionsPanels").Get<RectTransform>("rightPanel");
                Transform content = rightPanel.Find("Content");
                Transform deleteOrRevert = content.Find("Delete") ?? content.Find("Revert");
                Text text = deleteOrRevert.GetComponentInChildren<Text>();
                text.DOKill(false);
                if (deleted)
                {
                    deleteOrRevert.name = "Revert";
                    text.color = revertColor;
                    text.text = RDString.language == SystemLanguage.English ? "Revert Level" : "레벨 복구";
                }
                else
                {
                    deleteOrRevert.name = "Delete";
                    text.color = deleteColor;
                    text.text = RDString.Get("cls.delete");
                }
            }
        }
    }
}
