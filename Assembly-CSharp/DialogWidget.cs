using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogWidget : MonoBehaviour
{
	private void Awake()
	{
		this.textCanvas = this.linesRoot.GetComponent<CanvasGroup>();
		this.longTextCanvas = this.longLinesRoot.GetComponent<CanvasGroup>();
		this.objectCanvas = this.objectRoot.GetComponent<CanvasGroup>();
		this.purgeCanvas = this.purgeRoot.GetComponent<CanvasGroup>();
		this.buyCanvas = this.buyRoot.GetComponent<CanvasGroup>();
		this.purgeGenericCanvas = this.purgeGenericRoot.GetComponent<CanvasGroup>();
		this.linesRoot.SetActive(false);
		this.longLinesRoot.SetActive(false);
		this.objectRoot.SetActive(false);
		this.purgeRoot.SetActive(false);
		this.purgeGenericRoot.SetActive(false);
		this.buyRoot.SetActive(false);
		this.animator = base.GetComponent<Animator>();
		this.animator.SetBool("SHOW_DIALOG", false);
		this.responsesUI = this.dialogResponse[0].transform.parent.gameObject;
		this.responsesUI.SetActive(false);
		this.state = DialogWidget.DialogState.Off;
		this.hasResponses = false;
		for (int i = 0; i < 2; i++)
		{
			this.dialogResponseSelection[i] = this.dialogResponse[i].transform.Find("Img").gameObject;
			this.dialogResponseSelection[i].SetActive(false);
		}
		RectTransform rectTransform = (RectTransform)this.dialogLine.transform;
		this.generationSettings = default(TextGenerationSettings);
		this.generationSettings.textAnchor = this.dialogLine.alignment;
		this.generationSettings.color = this.dialogLine.color;
		this.generationSettings.generationExtents = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
		this.generationSettings.pivot = Vector2.zero;
		this.generationSettings.richText = this.dialogLine.supportRichText;
		this.generationSettings.font = this.dialogLine.font;
		this.generationSettings.fontSize = this.dialogLine.fontSize;
		this.generationSettings.fontStyle = 0;
		this.generationSettings.verticalOverflow = this.dialogLine.verticalOverflow;
		this.backgorund.SetActive(false);
	}

	private void Update()
	{
		switch (this.state)
		{
		case DialogWidget.DialogState.FadeText:
		{
			bool flag = false;
			this.currentTextAlpha += this.fadeSpeed * Time.deltaTime;
			if (this.currentTextAlpha >= 1f)
			{
				this.currentTextAlpha = 1f;
				flag = true;
			}
			this.currentCanvas.alpha = this.currentTextAlpha;
			if (flag)
			{
				if (this.IsLongText)
				{
					this.timeToWait = this.scrollInitialTime;
					this.state = DialogWidget.DialogState.ScrollInitial;
					this.scrollbar.value = 1f;
				}
				else
				{
					this.EndTransitionText();
				}
			}
			break;
		}
		case DialogWidget.DialogState.WaitSound:
			this.timeToWait -= Time.deltaTime;
			if (this.timeToWait <= 0f)
			{
				Core.Dialog.UIEvent_LineEnded(-1);
			}
			break;
		case DialogWidget.DialogState.ScrollInitial:
			this.timeToWait -= Time.deltaTime;
			if (this.timeToWait <= 0f)
			{
				this.state = DialogWidget.DialogState.ScrollScrolling;
				float num = this.rtContentRect.rect.height - this.rtScrollViewRect.rect.height;
				float num2 = num / this.scrollSpeed;
				this.tweenFloat = 1f;
				this.scrollbarTween = TweenSettingsExtensions.SetId<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To(() => this.tweenFloat, delegate(float x)
				{
					this.tweenFloat = x;
				}, 0f, num2), 1), new TweenCallback(this.EndTransitionText)), "AutoScroll");
			}
			break;
		case DialogWidget.DialogState.ScrollScrolling:
			this.scrollbar.value = this.tweenFloat;
			break;
		}
	}

	private void EndTransitionText()
	{
		if (this.hasResponses)
		{
			this.state = DialogWidget.DialogState.WaitResponse;
			this.responsesUI.SetActive(true);
			EventSystem.current.SetSelectedGameObject(this.dialogResponse[0].gameObject);
			base.StartCoroutine(this.ShowFirstSecure());
		}
		else
		{
			DialogManager.ModalMode modalMode = this.currentModal;
			if (modalMode != DialogManager.ModalMode.Modal)
			{
				if (modalMode == DialogManager.ModalMode.NoModal || modalMode == DialogManager.ModalMode.Boss)
				{
					this.state = DialogWidget.DialogState.WaitSound;
					this.timeToWait = ((this.currentSoundLenght != 0f) ? this.currentSoundLenght : this.noModalNextLine);
				}
			}
			else
			{
				this.state = DialogWidget.DialogState.WaitPress;
			}
		}
	}

	public void DialogButtonPressed()
	{
		if (this.currentModal == DialogManager.ModalMode.NoModal)
		{
			return;
		}
		switch (this.state)
		{
		case DialogWidget.DialogState.FadeText:
			this.currentTextAlpha = 1f;
			break;
		case DialogWidget.DialogState.WaitPress:
			Core.Dialog.UIEvent_LineEnded(-1);
			break;
		case DialogWidget.DialogState.WaitSound:
			if (this.currentModal == DialogManager.ModalMode.Boss)
			{
				Core.Dialog.UIEvent_LineEnded(-1);
			}
			break;
		case DialogWidget.DialogState.ScrollInitial:
			this.timeToWait = 0f;
			break;
		case DialogWidget.DialogState.ScrollScrolling:
			TweenExtensions.Complete(this.scrollbarTween);
			this.scrollbar.value = 0f;
			break;
		}
	}

	public void SetOptionSelected(int response)
	{
		for (int i = 0; i < 2; i++)
		{
			this.dialogResponseSelection[i].SetActive(i == response);
			Text componentInChildren = this.dialogResponseSelection[i].transform.parent.GetComponentInChildren<Text>();
			componentInChildren.color = ((i != response) ? this.optionNormalColor : this.optionHighligterColor);
		}
	}

	public void ResponsePressed(int response)
	{
		if (this.state == DialogWidget.DialogState.WaitResponse)
		{
			Core.Dialog.UIEvent_LineEnded(response);
		}
	}

	public bool IsShowingDialog()
	{
		return this.state != DialogWidget.DialogState.Off;
	}

	public int GetNumberOfLines(string cad)
	{
		bool activeSelf = this.linesRoot.activeSelf;
		this.linesRoot.SetActive(true);
		this.dialogLine.text = cad;
		Canvas.ForceUpdateCanvases();
		int lineCount = this.dialogLine.cachedTextGenerator.lineCount;
		this.linesRoot.SetActive(activeSelf);
		return lineCount;
	}

	public void OnProgrammerSoundSeted(float time)
	{
		if (this.state == DialogWidget.DialogState.Off || this.currentModal == DialogManager.ModalMode.Modal)
		{
			return;
		}
		this.currentSoundLenght = time;
	}

	public void SetBackgound(bool enabled)
	{
		this.backgorund.SetActive(enabled);
	}

	public void ShowText(string line, List<string> responses, DialogManager.ModalMode modal)
	{
		this.InternalShow(responses, modal, this.textCanvas, false);
		this.linesRoot.SetActive(true);
		this.dialogLine.text = line;
	}

	public void ShowLongText(string line, List<string> responses, DialogManager.ModalMode modal)
	{
		this.InternalShow(responses, modal, this.longTextCanvas, false);
		this.longLinesRoot.SetActive(true);
		this.dialoglongLine.text = line;
		this.scrollbar.value = 1f;
		this.IsLongText = true;
	}

	public void ShowPurge(string purge, List<string> responses, DialogManager.ModalMode modal)
	{
		this.InternalShow(responses, modal, this.purgeCanvas, false);
		this.purgeRoot.SetActive(true);
		this.objectFirstText.text = ScriptLocalization.UI_Inventory.TEXT_QUESTION_GIVE_PURGE;
		this.purgueFirstText.text = ScriptLocalization.UI_Inventory.TEXT_QUESTION_GIVE_PURGE;
		this.purgueAmount.text = purge;
	}

	public void ShowPurgeGeneric(string text, List<string> responses, DialogManager.ModalMode modal)
	{
		this.InternalShow(responses, modal, this.purgeGenericCanvas, false);
		this.purgueGenericText.text = text;
		this.purgeGenericRoot.SetActive(true);
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.purgeGenericRoot.GetComponent<RectTransform>());
	}

	public void ShowItem(string caption, Sprite image, List<string> responses, DialogManager.ModalMode modal)
	{
		this.InternalShow(responses, modal, this.objectCanvas, false);
		this.objectRoot.SetActive(true);
		this.objectFirstText.text = ScriptLocalization.UI_Inventory.TEXT_QUESTION_GIVE_ITEM;
		this.objectSecondText.text = caption + "<color=#F8E4C6FF>?</color>";
		this.objectImage.sprite = image;
	}

	public void ShowBuy(string purge, string caption, string description, Sprite image, List<string> responses, DialogManager.ModalMode modal)
	{
		this.InternalShow(responses, modal, this.buyCanvas, true);
		this.buyRoot.SetActive(true);
		this.buyFirstText.text = ScriptLocalization.UI_Inventory.TEXT_QUESTION_BUY_ITEM;
		this.buyAmount.text = purge;
		this.buyCaption.text = caption;
		this.buyDescrption.text = description;
		this.buyImage.sprite = image;
		this.customScrollView.NewContentSetted();
	}

	public void Hide(bool hideWidget)
	{
		if (this.state != DialogWidget.DialogState.Off)
		{
			this.state = DialogWidget.DialogState.Off;
			if (hideWidget)
			{
				this.animator.SetBool("SHOW_DIALOG", false);
			}
			DOTween.Kill("AutoScroll", false);
			this.backgorund.SetActive(false);
		}
	}

	public IEnumerator ShowFirstSecure()
	{
		yield return new WaitForFixedUpdate();
		EventSystem.current.SetSelectedGameObject(this.dialogResponse[1].gameObject);
		EventSystem.current.SetSelectedGameObject(this.dialogResponse[0].gameObject);
		this.SetOptionSelected(0);
		yield break;
	}

	private void InternalShow(List<string> responses, DialogManager.ModalMode modal, CanvasGroup canvas, bool isBuyMenu)
	{
		this.linesRoot.SetActive(false);
		this.longLinesRoot.SetActive(false);
		this.objectRoot.SetActive(false);
		this.purgeRoot.SetActive(false);
		this.purgeGenericRoot.SetActive(false);
		this.buyRoot.SetActive(false);
		this.currentCanvas = canvas;
		this.IsLongText = false;
		this.currentSoundLenght = 0f;
		if (this.state == DialogWidget.DialogState.Off)
		{
			this.animator.SetBool("SHOW_DIALOG", true);
		}
		this.state = DialogWidget.DialogState.FadeText;
		this.hasResponses = (responses.Count > 0);
		this.responsesUI.SetActive(false);
		for (int i = 0; i < 2; i++)
		{
			if (i < responses.Count)
			{
				this.dialogResponse[i].gameObject.SetActive(true);
				this.dialogResponse[i].text = responses[i];
			}
			else
			{
				this.dialogResponse[i].gameObject.SetActive(false);
			}
		}
		this.currentCanvas.alpha = 0f;
		this.currentTextAlpha = ((responses.Count <= 0) ? 0f : 1f);
		this.currentModal = modal;
		this.responseRoot.localPosition = ((!isBuyMenu) ? this.normalResponsePosition.localPosition : this.buyResponsePosition.localPosition);
		this.linesRoot.transform.localPosition = ((!this.hasResponses) ? this.linesWithOutResponsePosition.localPosition : this.linesWithResponsePosition.localPosition);
		this.longLinesRoot.transform.localPosition = ((!this.hasResponses) ? this.linesWithOutResponsePosition.localPosition : this.linesWithResponsePosition.localPosition);
		this.backgroundImage.SetActive(!this.hasResponses && !isBuyMenu);
		this.backgroundWithResponseImage.SetActive(this.hasResponses && !isBuyMenu);
	}

	private const string ANIMATOR_BOOL = "SHOW_DIALOG";

	private const int MAX_RESPONSES = 2;

	private Animator animator;

	[BoxGroup("Controls", true, false, 0)]
	public GameObject linesRoot;

	[BoxGroup("Controls", true, false, 0)]
	public GameObject longLinesRoot;

	[BoxGroup("Controls", true, false, 0)]
	public GameObject objectRoot;

	[BoxGroup("Controls", true, false, 0)]
	public GameObject purgeRoot;

	[BoxGroup("Controls", true, false, 0)]
	public GameObject purgeGenericRoot;

	[BoxGroup("Controls", true, false, 0)]
	public GameObject buyRoot;

	[BoxGroup("Controls", true, false, 0)]
	public RectTransform responseRoot;

	[BoxGroup("Controls", true, false, 0)]
	public GameObject backgroundImage;

	[BoxGroup("Controls", true, false, 0)]
	public GameObject backgroundWithResponseImage;

	[BoxGroup("Positions", true, false, 0)]
	public RectTransform normalResponsePosition;

	[BoxGroup("Positions", true, false, 0)]
	public RectTransform buyResponsePosition;

	[BoxGroup("Positions", true, false, 0)]
	public RectTransform linesWithResponsePosition;

	[BoxGroup("Positions", true, false, 0)]
	public RectTransform linesWithOutResponsePosition;

	[BoxGroup("Lines", true, false, 0)]
	public Text dialogLine;

	[BoxGroup("Lines", true, false, 0)]
	public Text dialoglongLine;

	[BoxGroup("Object", true, false, 0)]
	public Text objectFirstText;

	[BoxGroup("Object", true, false, 0)]
	public Text objectSecondText;

	[BoxGroup("Object", true, false, 0)]
	public Image objectImage;

	[BoxGroup("Object", true, false, 0)]
	public GameObject backgorund;

	[BoxGroup("Purge", true, false, 0)]
	public Text purgueFirstText;

	[BoxGroup("Purge", true, false, 0)]
	public Text purgueAmount;

	[BoxGroup("Purge Generic", true, false, 0)]
	public Text purgueGenericText;

	[BoxGroup("Buy", true, false, 0)]
	public Text buyFirstText;

	[BoxGroup("Buy", true, false, 0)]
	public Text buyAmount;

	[BoxGroup("Buy", true, false, 0)]
	public Text buyCaption;

	[BoxGroup("Buy", true, false, 0)]
	public Text buyDescrption;

	[BoxGroup("Buy", true, false, 0)]
	public Image buyImage;

	[BoxGroup("Buy", true, false, 0)]
	public CustomScrollView customScrollView;

	[BoxGroup("Scroll", true, false, 0)]
	public RectTransform rtContentRect;

	[BoxGroup("Scroll", true, false, 0)]
	public RectTransform rtScrollViewRect;

	[BoxGroup("Scroll", true, false, 0)]
	public Scrollbar scrollbar;

	[BoxGroup("Scroll", true, false, 0)]
	public float scrollInitialTime = 2f;

	[BoxGroup("Scroll", true, false, 0)]
	public float scrollSpeed = 10f;

	[BoxGroup("Response", true, false, 0)]
	public Text[] dialogResponse = new Text[2];

	[BoxGroup("Options", true, false, 0)]
	public float fadeSpeed = 2f;

	[BoxGroup("Options", true, false, 0)]
	public float noModalNextLine = 3f;

	[SerializeField]
	[BoxGroup("Options", true, false, 0)]
	private Color optionNormalColor = new Color(0.972549f, 0.89411765f, 0.78039217f);

	[SerializeField]
	[BoxGroup("Options", true, false, 0)]
	private Color optionHighligterColor = new Color(0.80784315f, 0.84705883f, 0.49803922f);

	private DialogWidget.DialogState state;

	private GameObject responsesUI;

	private GameObject[] dialogResponseSelection = new GameObject[2];

	private bool hasResponses;

	private float currentTextAlpha;

	private CanvasGroup longTextCanvas;

	private CanvasGroup textCanvas;

	private CanvasGroup objectCanvas;

	private CanvasGroup purgeCanvas;

	private CanvasGroup purgeGenericCanvas;

	private CanvasGroup buyCanvas;

	private CanvasGroup currentCanvas;

	private Tweener scrollbarTween;

	private TextGenerationSettings generationSettings;

	private float currentSoundLenght;

	private bool IsLongText;

	private float timeToWait;

	private float tweenFloat;

	private DialogManager.ModalMode currentModal;

	private const string ITEM_GIVE_QUESTION_MARK = "<color=#F8E4C6FF>?</color>";

	private enum DialogState
	{
		Off,
		FadeText,
		WaitPress,
		WaitResponse,
		WaitSound,
		ScrollInitial,
		ScrollScrolling
	}
}
