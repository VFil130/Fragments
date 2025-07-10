using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Dialogue
{
    public class DialogueStory : MonoBehaviour
    {
        [SerializeField] private Story[] _stories;
        private Dictionary<string, Story> _storiesDictionary;
        public event Action<Story> ChangedStory;

        [Serializable]
        public struct Story
        {
            [field: SerializeField] public string Tag { get; private set; }
            [field: SerializeField] public string Text { get; private set; }
            [field: SerializeField] public Answer[] Answers { get; private set; }
        }

        [Serializable]
        public class Answer
        {
            [field: SerializeField] public string Text { get; private set; }
            [field: SerializeField] public string ReposeText { get; private set; }
        }

        private void Start()
        {
            // Исправленный способ создания словаря
            _storiesDictionary = _stories.ToDictionary(story => story.Tag, story => story);

            // Важно: Проверяйте, что массив _stories не пустой.
            if (_stories.Length > 0)
            {
                ChangedStory(_stories[0]);
            }
            else
            {
                Debug.LogError("Массив _stories пустой!");
            }
        }


        public void ChangeStory(string tag)
        {
            if (_storiesDictionary.TryGetValue(tag, out var story))
            {
                ChangedStory?.Invoke(story);
            }
            else
            {
                Debug.LogError($"Не найдено истории с тегом: {tag}");
            }
        }
    }
}