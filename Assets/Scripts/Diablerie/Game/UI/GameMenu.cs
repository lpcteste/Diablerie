using System.Collections.Generic;
using Diablerie.Engine;
using Diablerie.Engine.Entities;
using Diablerie.Engine.IO.D2Formats;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Diablerie.Game.UI
{
    public class GameMenu
    {
        private static GameMenu instance;
        private GraphicRaycaster raycaster;
        private GameObject root;
        private GameObject layoutGroupObject;
        private GameObject leftStar;
        private GameObject rightStar;
        private List<MenuItem> items = new List<MenuItem>();
        private int selectedIndex = 0;
        private int itemHeight = 50; // Use it

        public static void Show()
        {
            GetInstance().ShowInternal();
        }

        public static void Hide()
        {
            GetInstance().HideInternal();
        }

        public static bool IsVisible()
        {
            return GetInstance().root.activeSelf;
        }

        private static GameMenu GetInstance()
        {
            if (instance == null)
                instance = new GameMenu();
            return instance;
        }

        private GameMenu()
        {
            root = new GameObject("Game Menu");
            var canvas = root.AddComponent<Canvas>();
            raycaster = root.AddComponent<GraphicRaycaster>();
            var behaviour = root.AddComponent<InternalBehaviour>();
            behaviour.menu = this;
            layoutGroupObject = new GameObject("Vertical Layout");
            layoutGroupObject.transform.SetParent(root.transform);
            var layoutGroup = layoutGroupObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 0;
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlHeight = true;
            var layoutGroupTransform = layoutGroupObject.GetComponent<RectTransform>();
            layoutGroupTransform.anchorMin = new Vector2(0, 0.5f);
            layoutGroupTransform.anchorMax = new Vector2(1, 0.5f);
            layoutGroupTransform.pivot = new Vector2(0.5f, 0.5f);
            layoutGroupTransform.anchoredPosition = new Vector2(0, 0);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            AddMenuItem("OPTIONS", enabled: false);
            AddMenuItem("EXIT GAME");
            AddMenuItem("RETURN TO GAME");
            layoutGroupTransform.sizeDelta = new Vector2(0, itemHeight * items.Count);
            HideInternal();
            leftStar = CreateStar(true);
            rightStar = CreateStar(false);
        }
        
        private void ShowInternal()
        {
            root.SetActive(true);
        }

        private void HideInternal()
        {
            root.SetActive(false);
        }

        private void AddMenuItem(string itemName, bool enabled = true)
        {
            var menuItem = new MenuItem(itemName, enabled);
            menuItem.rectTransform.SetParent(layoutGroupObject.transform);
            items.Add(menuItem);
        }

        private GameObject CreateStar(bool reversed)
        {
            Palette.LoadPalette(0);
            var spritesheet = Spritesheet.Load(@"data\global\ui\CURSOR\pentspin");
            var star = new GameObject("star");
            var animator = star.AddComponent<SpriteAnimator>();
            animator.sprites = spritesheet.GetSprites(0);
            animator.fps = 20;
            animator.reversed = reversed;
            animator.OffsetTime(Random.Range(0, 2));
            var spriteRenderer = star.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "UI";
            return star;
        }

        private class InternalBehaviour : MonoBehaviour
        {
            public GameMenu menu;
            
            void Update()
            {
                List<RaycastResult> results = new List<RaycastResult>();
                var pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Input.mousePosition;
                menu.raycaster.Raycast(pointerEventData, results);
                Debug.Log(Input.mousePosition + " raycast hits " + results.Count);
                if (results.Count > 0)
                    Debug.Log(results[0].gameObject);
                UpdateStarsPositions();
                if (Input.GetKeyDown(KeyCode.Escape))
                    Hide();
            }

            private void UpdateStarsPositions()
            {
                MenuItem selectedItem = menu.items[menu.selectedIndex];
                var leftPosition = Camera.main.ScreenToWorldPoint(selectedItem.rectTransform.position - new Vector3(100, 0));
                var rightPosition = Camera.main.ScreenToWorldPoint(selectedItem.rectTransform.position + new Vector3(100, 0));
                leftPosition.z = 0;
                rightPosition.z = 0;
                menu.leftStar.transform.position = leftPosition;
                menu.rightStar.transform.position = rightPosition;
            }
        }

        private class MenuItem
        {
            public GameObject gameObject;
            public RectTransform rectTransform;
            
            public MenuItem(string name, bool enabled)
            {
                gameObject = new GameObject(name);
                var text = gameObject.AddComponent<Text>();
                rectTransform = gameObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = new Vector2(0, 0);
                text.alignment = TextAnchor.LowerCenter;
                text.color = enabled ? Color.white : Color.grey;
                text.font = Fonts.GetFont42();
                text.text = name;
            }
        }
    }
}