using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.Dialogue;
using RPG.Core;

namespace RPG.UI.Dialogue
{
    public class DialogueUI : MonoBehaviour
    {
        #region --Fields-- (Inspector)
        [Header("Dialogue UI Panels")]
        [SerializeField] private GameObject[] _dialogueUIPanels;

        [Header("Talk Panels")]
        [SerializeField] private GameObject[] _talkPanels;

        [Header("Response Panels")]
        [SerializeField] private GameObject[] _responsePanels;

        [Space]
        [Space]

        [Header("Profile Stuffs")]
        [SerializeField] private TMP_Text _profileText;
        [SerializeField] private Image _profileImage;

        [Header("Dialogue Stuffs")]
        [SerializeField] private Button _quitButton;

        [Header("Talk Stuffs")]
        [SerializeField] private TMP_Text _talkText;
        [SerializeField] private Button _nextButton;

        [Header("Response Stuffs")]
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private GameObject _replyButtonPrefab;
        [SerializeField] private Transform _spawnParent;
        #endregion



        #region --Fields-- (In Class)
        private PlayerConversant _playerConversant;
        #endregion



        #region --Methods-- (Built In)
        private void Awake()
        {
            _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerConversant>();

            _nextButton.onClick.AddListener(Next);
            _quitButton.onClick.AddListener(Quit);
        }

        private void OnEnable()
        {
            UIDisplayManager.OnDialogueRefreshed += UpdateDialogueUI;
        }

        private void Start()
        {
            UpdateDialogueUI();
        }

        private void OnDisable()
        {
            UIDisplayManager.OnDialogueRefreshed -= UpdateDialogueUI;
        }
        #endregion



        #region --Methods-- (Custom PRIVATE)
        private void ClearReplyButton()
        {
            foreach (Transform eachButton in _spawnParent.transform)
            {
                Destroy(eachButton.gameObject);
            }
        }

        private void BuildChoiceList()
        {
            foreach (DialogueNode choiceNode in _playerConversant.GetChoices())
            {
                GameObject spawnedGameObject = Instantiate(_replyButtonPrefab, _spawnParent);
                spawnedGameObject.GetComponentInChildren<TMP_Text>().text = choiceNode.Text;
                spawnedGameObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    Pick(choiceNode);
                });
            }
        }

        private void UpdateProfilePanels()
        {
            _profileText.text = _playerConversant.GetAISpeakerName();
            _profileImage.overrideSprite = _playerConversant.GetAIProfileImage();
        }

        private void SetDialogueUIPanels(bool status)
        {
            foreach (GameObject eachPanel in _dialogueUIPanels)
                eachPanel.SetActive(status);
        }

        private void SetTalkPanels(bool status)
        {
            foreach (GameObject eachPanel in _talkPanels)
                eachPanel.SetActive(status);
        }

        private void SetResponsePanels(bool status)
        {
            foreach (GameObject eachPanel in _responsePanels)
                eachPanel.SetActive(status);
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void UpdateDialogueUI()
        {
            SetDialogueUIPanels(_playerConversant.IsActive());

            if (!_playerConversant.IsActive()) return;

            UpdateProfilePanels();

            if (_playerConversant.IsPlayerSpeaking())
            {
                SetTalkPanels(false);
                SetResponsePanels(true);

                _questionText.text = _playerConversant.GetQuestionText();

                ClearReplyButton();
                BuildChoiceList();
            }
            else
            {
                SetResponsePanels(false);
                SetTalkPanels(true);

                _talkText.text = _playerConversant.GetText();

                _nextButton.gameObject.SetActive(_playerConversant.HasNext());
            }
        }
        #endregion



        #region --Methods-- (Subscriber)
        private void Next()
        {
            _playerConversant.GetNextNode();
        }

        private void Pick(DialogueNode selectedNode)
        {
            _playerConversant.GetChoiceNode(selectedNode);
        }

        private void Quit()
        {
            _playerConversant.QuitDialogue();
        }
        #endregion
    }
}